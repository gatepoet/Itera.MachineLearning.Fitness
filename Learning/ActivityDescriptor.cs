using System;
using Itera.MachineLearning.Fitness.Services;
using numl.Model;

namespace Itera.MachineLearning.Fitness.Learning
{
    public class ActivityDescriptor
    {
        public DateTime Time { get; set; }
        public double Distance { get; set; }
        public TimeSpan Duration { get; set; }
        public double AverageSpeed { get; set; }
        public double Wind { get; set; }
        public double Percipitation { get; set; }
        public double Temperature { get; set; }
    }
}
