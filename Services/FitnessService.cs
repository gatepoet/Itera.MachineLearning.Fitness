using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Itera.MachineLearning.Fitness.Services
{
    public class FitnessService
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        public IEnumerable<Activity> GetActivities()
        {
            return GetRunkeeperActivities();
        }

        private IEnumerable<Activity> GetRunkeeperActivities()
        {
            var path = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.MyDocuments),
                @"MachineLearning\Runkeeper\cardioActivities.csv");
                
            
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

    }
}
