using Itera.MachineLearning.Fitness.Services;
using System;
namespace Itera.MachineLearning.Fitness.Learning
{
    interface IActivityLearner
    {
        double PredictAverageSpeedByDuration(TimeSpan duration);
        double PredictDistanceByWeekday(DateTime date);
        TimeSpan PredictDurationByWeekday(DateTime date);
        double PredictDistanceByWeather(WeatherData weatherData);
        TimeSpan PredictDurationByWeather(WeatherData weatherData);
    }
}
