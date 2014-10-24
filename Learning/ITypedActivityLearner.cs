using Itera.MachineLearning.Fitness.Services;
using System;
namespace Itera.MachineLearning.Fitness.Learning
{
    interface ITypedActivityLearner
    {
        ActivityType PredictActivityTypeByWeather(WeatherData weatherData);
        ActivityType PredictActivityTypeByWeekdayAndHour(DateTime time);
        double PredictAverageSpeedByDurationAndType(ActivityType type, TimeSpan duration);
        double PredictDistanceByActivityType(ActivityType type);
    }
}
