using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using Itera.MachineLearning.Fitness.Learning;
using numl.Math.LinearAlgebra;
using GPXLib;

namespace Itera.MachineLearning.Fitness.Services
{
    public class FitnessService
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        public IEnumerable<Activity> GetAllActivities()
        {
            var trails = GetTrailSummary();
            var activities = trails
                .Select(t =>
                {
                    var date = t.TrackPointArray.First().time;
                    var duration = t.TrackPointArray.Last().time
                        .Subtract(date);
                    return new Activity
                        {
                            Distance = t.TotalDistance,
                            Date = date,
                            Duration = duration,
                            AverageSpeed = t.TotalDistance / duration.TotalHours,
                        };
                })
                .OrderBy(a => a.Date);

            return activities;
        }

        private IEnumerable<TrailSummary> GetTrailSummary()
        {
            var trails = Directory
                .EnumerateFiles(
                    Config.BasePath,
                    "*.gpx",
                    SearchOption.AllDirectories)
                .Select(LoadTrail)
                .Where(trail => trail != null && trail.TotalDistance > 0);

            return trails;
        }

        private TrailSummary LoadTrail(string filename)
        {
            try
            {
                return new TrailSummary(filename);
            }
            catch (Exception ex)
            {
                return null;
            }
        } 

        public IEnumerable<Activity> GetTypedActivities()
        {
            return GetRunkeeperActivities().Union(
                GetPolarPersonalTrainerActivities());
        }

        public IEnumerable<Activity> GetRunkeeperActivities()
        {
            var path = Path.Combine(
                Config.BasePath,
                @"Runkeeper\cardioActivities.csv");
            
            return File.ReadAllLines(path)
                .Skip(1)
                .Select(CreateRunkeeperActivity);
        }

        private Activity CreateRunkeeperActivity(string line)
        {
            var arr = line.Split(',');
            var distance = GetDouble(arr[3]);
            ActivityType type;
            if (!Enum.TryParse(arr[1], out type))
            {
                type = ParseActivityType(arr[1], distance > 0);
            }
            var avgSpeed = GetAverageSpeed(
                                GetDouble(
                                    arr[6]));
            return new Activity
            {
                Date = DateTime.Parse(arr[0], Culture),
                Type = type,
                Distance = distance,
                Duration = GetDuration(arr[4]),
                AverageSpeed = avgSpeed,
                Speed = GetSpeed(avgSpeed)
            };
        }

        private ActivitySpeed GetSpeed(double averageSpeed)
        {
            if (averageSpeed < 10 && averageSpeed > 0)
                return ActivitySpeed.Comfortable;
            if (averageSpeed > 10 && averageSpeed < 18)
                return ActivitySpeed.Medium;
            if (averageSpeed > 18)
                return ActivitySpeed.Intense;

            return ActivitySpeed.None;
        }
        private static double GetAverageSpeed(double a)
        {
            var avgSpeed = a;
            if (avgSpeed > 1000)
                avgSpeed = avgSpeed / 1000;
            if (avgSpeed > 100)
                avgSpeed = avgSpeed / 100;
            if (avgSpeed > 60)
                avgSpeed = avgSpeed / 10;
            return avgSpeed;
        }

        private ActivityType ParseActivityType(string text, bool hasDistance)
        {
            ActivityType type;
            if (Enum.TryParse(text, out type))
            {
                return type;
            }
            switch (text)
            {
                case "Cross-country":
                case "Cross-Country Skiing":
                    type = ActivityType.Skiing;
                    break;
                case "Mountain Biking":
                    type = ActivityType.Cycling;
                    break;
                case "Hiking":
                    type = ActivityType.Walking;
                    break;
                case "Other sport":
                    type = hasDistance
                        ? ActivityType.Other
                        : ActivityType.Gym;
                    break;
                default:
                    Console.WriteLine("Unknown activity: " + text);
                    type = ActivityType.Other;
                    break;
            }
            return type;
        }

        private double GetDouble(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return 0;

            return double.Parse(s, Culture);
        }
        private TimeSpan GetDuration(string value)
        {
            return (value.Split(':').Length == 2)
                ? TimeSpan.ParseExact(value, "m\\:ss", Culture)
                : TimeSpan.ParseExact(value, "h\\:m\\:ss", Culture);
        }

        XNamespace stNs = "http://www.polarpersonaltrainer.com";

        public IEnumerable<Activity> GetPolarPersonalTrainerActivities()
        {

            var document = GetPolarXML();
            return document
                .Descendants(stNs + "polar-exercise-data")
                .Descendants(stNs + "calendar-items")
                .Descendants(stNs + "exercise")
                .Select(PolarPersonalTrainerActivity);
        }

        private Activity PolarPersonalTrainerActivity(XElement xml)
        {
            ActivityType type;
            var typeString = xml.Element(stNs + "sport").Value;
            if (!Enum.TryParse(typeString, out type))
            {
                type = ParseActivityType(typeString, true);
            }
            var result = xml.Element(stNs + "result");
            var distance = GetDouble(GetValue(result.Element(stNs + "distance")));
            var duration = TimeSpan.Parse(result.Element(stNs + "duration").Value);
            var avgSpeed = GetAverageSpeed(
                distance / duration.TotalHours * 1.6);

            return new Activity
            {
                Date = DateTime.Parse(xml.Element(stNs + "time").Value),
                Type = type,
                Distance = distance/1000.0,
                Duration = duration,
                AverageSpeed = avgSpeed,
                Speed = GetSpeed(avgSpeed)
            };
        }

        private string GetValue(XElement element)
        {
            return element == null ? "" : element.Value;
        }

        public static XDocument GetPolarXML()
        {
            var path = Path.Combine(
                Config.BasePath,
                @"PolarPersonalTrainer\Travelisio_15.10.2014_export.xml");

            var document = XDocument.Load(path);
            return document;
        }
    }
}
