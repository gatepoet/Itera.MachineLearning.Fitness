using Itera.MachineLearning.Fitness.Services;
using Itera.MachineLearning.Fitness.Services.WeatherHistory;
using numl;
using numl.Model;
using numl.Supervised.DecisionTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Itera.MachineLearning.Fitness.Learning
{
    public class TypedActivityLearner : ActivityLearnerBase<TypedActivityDescriptor>, Itera.MachineLearning.Fitness.Learning.ITypedActivityLearner
    {
        public TypedActivityLearner(
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
            
            activityTypeByWeekdayAndHour = new Lazy<LearningModel>(LearnActivityTypeByWeekdayAndHour);
            activityTypeByWeather = new Lazy<LearningModel>(LearnActivityTypeByWeather);
            distanceByActivityType = new Lazy<LearningModel>(LearnDistanceByActivityType);
            averageSpeedByDurationAndType = new Lazy<LearningModel>(LearnAverageSpeedByDurationAndType);
        }

        protected override TypedActivityDescriptor CreateDescriptor(
            HistoricalWeatherData weatherData,
            Activity activity)
        {
            return new TypedActivityDescriptor()
                {
                    AverageSpeed = activity.AverageSpeed,
                    Time = activity.Date,
                    Distance = activity.Distance,
                    Duration = activity.Duration,
                    Type = activity.Type,
                    Wind = weatherData.Wind,
                    Percipitation = weatherData.Percipitation,
                    Temperature = weatherData.Temperature
                };
        }

        public ActivityType PredictActivityTypeByWeekdayAndHour(DateTime time)
        {
            var prediction = activityTypeByWeekdayAndHour.Value.Model.Predict(
                new TypedActivityDescriptor
                {
                    Time = time
                });

            return prediction.Type;
        }
        private Lazy<LearningModel> activityTypeByWeekdayAndHour;
        private LearningModel LearnActivityTypeByWeekdayAndHour()
        {
            var descriptor = Descriptor.For<TypedActivityDescriptor>()
                .WithDateTime(a => a.Time, DateTimeFeature.DayOfWeek)
                .WithDateTime(a => a.Time, DateTimeFeature.Hour)
                .Learn(a => a.Type);

            return Learn(descriptor);
        }

        public ActivityType PredictActivityTypeByWeather(WeatherData weatherData)
        {
            var prediction = activityTypeByWeather.Value.Model.Predict(
                new TypedActivityDescriptor
                {
                    Wind = weatherData.Wind,
                    Temperature = weatherData.Temperature,
                    Percipitation = weatherData.Percipitation
                });

            return prediction.Type;
        }
        private Lazy<LearningModel> activityTypeByWeather;
        private LearningModel LearnActivityTypeByWeather()
        {
            var descriptor = Descriptor.For<TypedActivityDescriptor>()
                .With(a => a.Wind)
                .With(a => a.Temperature)
                .With(a => a.Percipitation)
                .Learn(a => a.Type);

            return Learn(descriptor);
        }

        public double PredictDistanceByActivityType(ActivityType type)
        {
            var prediction = distanceByActivityType.Value.Model.Predict(
                new TypedActivityDescriptor
                {
                    Type = type
                });

            return prediction.Distance;
        }
        private Lazy<LearningModel> distanceByActivityType;
        private LearningModel LearnDistanceByActivityType()
        {
            var descriptor = Descriptor.For<TypedActivityDescriptor>()
                .With(a => a.Type)
                .Learn(a => a.Distance);

            return Learn(
                descriptor,
                activities.Where(a => a.Distance > 0).ToList());
        }


        public double PredictAverageSpeedByDurationAndType(ActivityType type, TimeSpan duration)
        {
            var prediction = averageSpeedByDurationAndType.Value.Model.Predict(
                new TypedActivityDescriptor
                {
                    Type = type,
                    Duration = duration
                });

            return prediction.AverageSpeed;
        }
        private Lazy<LearningModel> averageSpeedByDurationAndType;
        private LearningModel LearnAverageSpeedByDurationAndType()
        {
            var descriptor = Descriptor.For<TypedActivityDescriptor>()
                .With(a => a.Type)
                .With(a => a.Duration)
                .Learn(a => a.AverageSpeed);

            return Learn(
                descriptor,
                activities.Where(a => a.AverageSpeed > 0).ToList());
        }

    }
}
