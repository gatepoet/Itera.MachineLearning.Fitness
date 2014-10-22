using System;
using Itera.MachineLearning.Fitness.Services;
using numl.Model;

namespace Program
{
    public class Activity
    {
        [Feature]
        public ActivityType Type { get; set; }
        public DateTime Date { get; set; }
        //[Feature]
        public double Distance { get; set; }
        //[Feature]
        public TimeSpan Duration { get; set; }
        //[Feature]
        public double AverageSpeed { get; set; }
        [Feature]
        public string Day { get { return Date.DayOfWeek.ToString(); } set{} }
        [Label]
        public double Wind { get; set; }
    }
}
