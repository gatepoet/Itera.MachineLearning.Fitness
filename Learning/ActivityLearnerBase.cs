using Itera.MachineLearning.Fitness.Services;
using Itera.MachineLearning.Fitness.Services.WeatherHistory;
using Newtonsoft.Json;
using numl;
using numl.Model;
using numl.Supervised.DecisionTree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Itera.MachineLearning.Fitness.Learning
{
    public abstract class ActivityLearnerBase<T>
    {
        protected List<T> activities;
        protected double trainingPercentage;
        protected int trainingRepeatCount;

        public ActivityLearnerBase(
            IEnumerable<HistoricalWeatherData> weatherHistory,
            IEnumerable<Activity> activityHistory,
            double trainingPercentage = 0.80,
            int trainingRepeatCount = 1000)
        {
            this.trainingPercentage = trainingPercentage;
            this.trainingRepeatCount = trainingRepeatCount;

            this.activities = weatherHistory
                .Join(activityHistory,
                    wd => wd.Date.Date,
                    a => a.Date.Date,
                    CreateDescriptor)
                .ToList();

        }

        protected abstract T CreateDescriptor(HistoricalWeatherData weatherData, Activity activity);


        protected LearningModel Learn(Descriptor descriptor, List<T> activities = null, [CallerMemberName] string caller = "")
        {
            var generator = new DecisionTreeGenerator(descriptor);
            generator.SetHint(false);

            var model = Learner.Learn(
                (activities ?? this.activities).Cast<object>(),
                trainingPercentage,
                trainingRepeatCount,
                generator);
            model.Model.Save(Path.Combine(
                Config.BasePath,
                caller.Substring(5) + ".xml"));

            return model;
        }

    }
}
