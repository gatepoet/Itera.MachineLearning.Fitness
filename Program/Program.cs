using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Itera.MachineLearning.Fitness.Services;
using Itera.MachineLearning.Fitness.Services.WeatherHistory;
using numl;
using numl.Model;
using numl.Supervised.DecisionTree;

namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            var weatherDataService = new WeatherDataService();
            var fitnessService = new FitnessService();
            //TODO: 18700 = OSLO station
            var weatherData = weatherDataService.GetHistoricalMetDataDaily<HistoricalWeatherData>(18700, new DateTime(2010, 01, 01), DateTime.Now).ToList();
            var activities = fitnessService.GetActivities().ToList();
            var activitiesWithWeatherData = from weatherObject in weatherData
                                            join a in activities on weatherObject.Date.Date equals a.Date.Date
                                            select new Activity()
                                            {
                                                AverageSpeed = a.AverageSpeed,
                                                Date = a.Date,
                                                Distance = a.Distance,
                                                Duration = a.Duration,
                                                Type = a.Type,
                                                Wind = weatherObject.Wind
                                            };
            var activitiesWithWeatherDataList = activitiesWithWeatherData.ToList();
            var descriptor = Descriptor.Create<Activity>();
            var generator = new DecisionTreeGenerator(descriptor);
            generator.SetHint(false);
            var model = Learner.Learn(activitiesWithWeatherDataList, 0.80, 1000, generator);
            var result = model.Model.Predict(new Activity() { Type = ActivityType.Cycling });
            //model.Model.Save(@"C:\Users\Sirar\Documents\Visual Studio 2013\Projects\Program\results.xml");
            Console.WriteLine(model);
            Console.ReadLine();
        }
    }
}
