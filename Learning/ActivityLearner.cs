using Itera.MachineLearning.Fitness.Services;
using Itera.MachineLearning.Fitness.Services.WeatherHistory;
using numl;
using numl.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Itera.MachineLearning.Fitness.Learning
{
    public class ActivityLearner : ActivityLearnerBase<ActivityDescriptor>, Itera.MachineLearning.Fitness.Learning.IActivityLearner
    {
        public ActivityLearner(
            IEnumerable<HistoricalWeatherData> weatherHistory,
            IEnumerable<Activity> activityHistory,
            double trainingPercentage = 0.80,
            int trainingRepeatCount = 1000)
            : base(
                weatherHistory,
                activityHistory,
                trainingPercentage,
                trainingRepeatCount)
        {
            distanceByWeekday = new Lazy<LearningModel>(LearnDistanceByWeekday);
            distanceByWeather = new Lazy<LearningModel>(LearnDistanceByWeather);
            durationByWeekday = new Lazy<LearningModel>(LearnDurationByWeekday);
            averageSpeedByDuration = new Lazy<LearningModel>(LearnAverageSpeedByDuration);
            durationByWeather = new Lazy<LearningModel>(LearnDurationByWeather);
        }

        protected override ActivityDescriptor CreateDescriptor(HistoricalWeatherData weatherData, Activity activity)
        {
            return new ActivityDescriptor()
            {
                AverageSpeed = activity.AverageSpeed,
                Time = activity.Date,
                Distance = activity.Distance,
                Duration = activity.Duration,
                Wind = weatherData.Wind,
                Percipitation = weatherData.Percipitation,
                Temperature = weatherData.Temperature
            };
        }


        public double PredictDistanceByWeekday(DateTime date)
        {
            var prediction = distanceByWeekday.Value.Model.Predict(
                new TypedActivityDescriptor
                {
                    Time = date
                });

            return prediction.Distance;
        }
        private Lazy<LearningModel> distanceByWeekday;
        private LearningModel LearnDistanceByWeekday()
        {
            var descriptor = Descriptor.For<ActivityDescriptor>()
                .WithDateTime(a => a.Time, DateTimeFeature.DayOfWeek)
                .Learn(a => a.Distance);

            return Learn(
                descriptor,
                activities.Where(a => a.Distance > 0).ToList());
        }

        public double PredictDistanceByWeather(WeatherData weatherData)
        {
            var prediction = distanceByWeather.Value.Model.Predict(
                new TypedActivityDescriptor
                {
                    Wind = weatherData.Wind,
                    Temperature = weatherData.Temperature,
                    Percipitation = weatherData.Percipitation
                });

            return prediction.Distance;
        }
        private Lazy<LearningModel> distanceByWeather;
        private LearningModel LearnDistanceByWeather()
        {
            var descriptor = Descriptor.For<ActivityDescriptor>()
                .With(a => a.Wind)
                .With(a => a.Temperature)
                .With(a => a.Percipitation)
                .Learn(a => a.Distance);

            return Learn(
                descriptor,
                activities.Where(a => a.Distance > 0).ToList());
        }

        public TimeSpan PredictDurationByWeekday(DateTime date)
        {
            var prediction = durationByWeekday.Value.Model.Predict(
                new TypedActivityDescriptor
                {
                    Time = date
                });

            return prediction.Duration;
        }
        private Lazy<LearningModel> durationByWeekday;
        private LearningModel LearnDurationByWeekday()
        {
            var descriptor = Descriptor.For<ActivityDescriptor>()
                .WithDateTime(a => a.Time, DateTimeFeature.DayOfWeek)
                .Learn(a => a.Duration);

            return Learn(descriptor);
        }

        public double PredictAverageSpeedByDuration(TimeSpan duration)
        {
            var prediction = averageSpeedByDuration.Value.Model.Predict(
                new TypedActivityDescriptor
                {
                    Duration = duration
                });

            return prediction.AverageSpeed;
        }
        private Lazy<LearningModel> averageSpeedByDuration;
        private LearningModel LearnAverageSpeedByDuration()
        {
            var descriptor = Descriptor.For<ActivityDescriptor>()
                .With(a => a.Duration)
                .Learn(a => a.AverageSpeed);

            return Learn(
                descriptor,
                activities.Where(a => a.AverageSpeed > 0).ToList());
        }


        public TimeSpan PredictDurationByWeather(WeatherData weatherData)
        {
            var prediction = durationByWeather.Value.Model.Predict(
                new ActivityDescriptor
                {
                    Wind = weatherData.Wind,
                    Temperature = weatherData.Temperature,
                    Percipitation = weatherData.Percipitation
                });

            return prediction.Duration;
        }
        private Lazy<LearningModel> durationByWeather;
        private LearningModel LearnDurationByWeather()
        {
            var descriptor = Descriptor.For<ActivityDescriptor>()
                .With(a => a.Wind)
                .With(a => a.Temperature)
                .With(a => a.Percipitation)
                .Learn(a => a.Duration);

            return Learn(descriptor);
        }
    }
}
