using System;
using System.Collections.Generic;
using System.Linq;
using Itera.MachineLearning.Fitness.Learning;
using Itera.MachineLearning.Fitness.Services;

namespace Itera.MachineLearning.Fitness
{
    public class ExercisePlanner
    {
        public Calendar<DailyExercisePlan> CreatePlan(string place)
        {
            var weatherService = new WeatherService();
            var factory = new LearnerFactory(
                weatherService,
                new FitnessService(),
                0.8,
                1000
            );

            var learner = factory.CreateActivityLearner();
            var typedLearner = factory.CreateTypedActivityLearner();

            var list = (from forecast in weatherService.NextWeek(place)
                let date = forecast.Date
                let distance1 = learner.PredictDistanceByWeekday(date)
                let distance2 = learner.PredictDistanceByWeather(forecast)
                let distance = (distance1 + distance2)/2
                let duration1 = learner.PredictDurationByWeekday(date)
                let duration2 = learner.PredictDurationByWeather(forecast)
                let duration = TimeSpan.FromMinutes((duration1.Minutes + duration2.Minutes)/1.5)
                let type1 = typedLearner.PredictActivityTypeByWeather(forecast)
                let types = GetHourlyExercisePlans(typedLearner, date, distance, duration)
                select new DailyExercisePlan
                {
                    Date = date,
                    TypeByWeather = type1,
                    TypeByWeekdayAndHour = types,
                    Duration = duration,
                    Percipitation = forecast.Percipitation,
                    Wind = forecast.Wind,
                    Temperature = forecast.Temperature
                }).ToList();

            var calendar = new Calendar<DailyExercisePlan>(list, l => l.Date);
            
            return calendar;
        }

        private static IEnumerable<HourlyExercisePlan> GetHourlyExercisePlans(TypedActivityLearner typedLearner, DateTime date, double distance,
            TimeSpan duration)
        {
            var types = Enumerable.Range(6, 16)
                .ToList()
                .Select(i =>
                {
                    var activity = typedLearner.PredictActivityTypeByWeekdayAndHour(date.AddHours(i));
                    return new
                    {
                        Hour = i,
                        Activity = activity,
                        Distance = typedLearner.PredictDistanceByActivityType(activity)
                    };
                })
                .Where(a => a.Distance > 0)
                .GroupBy(a => a.Activity)
                .Select(g =>
                {
                    var dist = (0.8*distance) + (g.First().Distance);
                    return new HourlyExercisePlan()
                    {
                        Activity = g.Key,
                        Hours = g.Select(t => t.Hour),
                        Distance = dist,
                        Speed = typedLearner.PredictAverageSpeedByDurationAndType(g.Key, duration)
                    };
                });
            return types;
        }
    }

    public class DailyExercisePlan : ICalendarData
    {
        public DateTime Date { get; set; }
        public ActivityType TypeByWeather { get; set; }
        public IEnumerable<HourlyExercisePlan> TypeByWeekdayAndHour { get; set; }
        public TimeSpan Duration { get; set; }
        public double Percipitation  { get; set; }
        public double Wind { get; set; }
        public double Temperature { get; set; }
    }

    public class HourlyExercisePlan
    {
        public ActivityType Activity { get; set; }
        public IEnumerable<int> Hours { get; set; }
        public double Distance { get; set; }
        public double AverageSpeed { get; set; }
        public ActivitySpeed Speed { get; set; }
    }
}
