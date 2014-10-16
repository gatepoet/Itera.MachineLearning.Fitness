using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using numl.Math.LinearAlgebra;

namespace Itera.MachineLearning.Fitness.Services
{
    public class FitnessService
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        private static readonly string BasePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "MachineLearning");

        public IEnumerable<Activity> GetActivities()
        {
            return GetRunkeeperActivities().Union(
                GetSportsTrackerActivities());
        }

        public IEnumerable<Activity> GetRunkeeperActivities()
        {
            var path = Path.Combine(
                BasePath,
                @"Runkeeper\cardioActivities.csv");
            
            return File.ReadAllLines(path)
                .Skip(1)
                .Select(CreateRunkeeperActivity);
        }

        private Activity CreateRunkeeperActivity(string line)
        {
            var arr = line.Split(',');
            ActivityType type;
            if (!Enum.TryParse(arr[1], out type))
            {
                switch (arr[1])
                {
                    case "MountinBiking":
                        type = ActivityType.Cycling;
                        break;
                    default:
                        type = ActivityType.Other;
                        break;
                }
            }
            return new Activity
            {
                Date = DateTime.Parse(arr[0], Culture),
                Type = type,
                Distance = GetDouble(arr[3]),
                Duration = GetDuration(arr[4]),
                AverageSpeed = GetDouble(arr[6])

            };
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

        public IEnumerable<Activity> GetSportsTrackerActivities()
        {

            var document = GetPolarXML();
            return document
                .Descendants(stNs + "polar-exercise-data")
                .Descendants(stNs + "calendar-items")
                .Descendants(stNs + "exercise")
                .Select(CreateSportsTrackerActivity);
        }

        private Activity CreateSportsTrackerActivity(XElement xml)
        {
            ActivityType type;
            var typeString = xml.Element(stNs + "sport").Value;
            if (!Enum.TryParse(typeString, out type))
            {
                switch (typeString)
                {
                    case "Cross-country":
                        type = ActivityType.Skiing;
                        break;
                    default:
                        type = ActivityType.Other;
                        break;
                }
            }
            var result = xml.Element(stNs + "result");
            var distance = GetDouble(GetValue(result.Element(stNs + "distance")));
            var duration = TimeSpan.Parse(result.Element(stNs + "duration").Value);
            
            return new Activity
            {
                Date = DateTime.Parse(xml.Element(stNs + "time").Value),
                Type = type,
                Distance = distance/1000.0,
                Duration = duration,
                AverageSpeed = distance/duration.TotalHours * 1.6
            };
        }

        private string GetValue(XElement element)
        {
            return element == null ? "" : element.Value;
        }

        public static XDocument GetPolarXML()
        {
            var path = Path.Combine(
                BasePath,
                @"PolarPersonalTrainer\Travelisio_15.10.2014_export.xml");

            var document = XDocument.Load(path);
            return document;
        }
    }
}
