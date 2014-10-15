using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Itera.MachineLearning.Fitness.Services
{
    public enum ActivityType
    {
        Other,
        Running,
        Cycling,
        Kitesurfing,
        Walking,
        Skiing
    }
    public class Activity
    {
        public ActivityType Type { get; set; }
        public DateTime Date { get; set; }
        public double Distance { get; set; }
        public TimeSpan Duration { get; set; }
        public double AverageSpeed { get; set; }
    }
}
