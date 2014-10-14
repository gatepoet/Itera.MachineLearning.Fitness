using System;
using System.Collections.Generic;

namespace Itera.MachineLearning.Fitness.Services
{
    public interface IWeatherDataCollection<out T> : IEnumerable<T>
    {
        DateTime Start { get; }
        DateTime End { get; }
    }
}