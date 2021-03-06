﻿using System;
using Itera.MachineLearning.Fitness.Learning;

namespace Itera.MachineLearning.Fitness.Services
{
    public class Activity
    {
        public ActivityType Type { get; set; }
        public DateTime Date { get; set; }
        public double Distance { get; set; }
        public TimeSpan Duration { get; set; }
        public double AverageSpeed { get; set; }
        public ActivitySpeed Speed { get; set; }
    }
}
