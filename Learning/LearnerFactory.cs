using Itera.MachineLearning.Fitness.Services;
using Itera.MachineLearning.Fitness.Services.WeatherHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Itera.MachineLearning.Fitness.Learning
{
    public class LearnerFactory
    {
        private Lazy<IEnumerable<HistoricalWeatherData>> weatherHistory;
        private FitnessService fitnessService;
        private double trainingPercentage;
        private int trainingRepeatCount;

        public LearnerFactory(
            WeatherService weatherService,
            FitnessService fitnessService,
            double trainingPercentage = 0.8,
            int trainingRepeatCount = 1000)
        {
            this.trainingPercentage = trainingPercentage;
            this.trainingRepeatCount = trainingRepeatCount;
            this.fitnessService = fitnessService;

            this.weatherHistory = new Lazy<IEnumerable<HistoricalWeatherData>>(
                () => weatherService.GetHistoricalMetDataDaily<HistoricalWeatherData>(
                    Station.Oslo,
                    new DateTime(2011, 01, 01),
                    DateTime.Now));
        }

        public TypedActivityLearner CreateTypedActivityLearner()
        {
            var activities = fitnessService.GetTypedActivities();

            return new TypedActivityLearner(
                weatherHistory.Value,
                activities,
                trainingPercentage,
                trainingRepeatCount);
        }

        public ActivityLearner CreateActivityLearner()
        {
            var activities = fitnessService.GetAllActivities();

            return new ActivityLearner(
                weatherHistory.Value,
                activities,
                trainingPercentage,
                trainingRepeatCount);
        }
    }
}
