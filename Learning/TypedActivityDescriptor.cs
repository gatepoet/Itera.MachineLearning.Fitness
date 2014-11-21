using System;
using Itera.MachineLearning.Fitness.Services;
using numl.Model;

namespace Itera.MachineLearning.Fitness.Learning
{
    public class TypedActivityDescriptor : ActivityDescriptor
    {
        public ActivityType Type { get; set; }

        public ActivitySpeed Speed { get; set; }
    }

    public enum ActivitySpeed
    {
        None,
        Comfortable,
        Medium,
        Intense
    }
}
