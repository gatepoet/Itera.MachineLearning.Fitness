using System;
using Itera.MachineLearning.Fitness.Services;
using numl.Model;

namespace Itera.MachineLearning.Fitness.Learning
{
    public class TypedActivityDescriptor : ActivityDescriptor
    {
        public ActivityType Type { get; set; }
    }
}
