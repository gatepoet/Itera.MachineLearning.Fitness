<Query Kind="Statements">
  <Reference>C:\Workbench\Itera.MachineLearning.Fitness\bin\Debug\GPXLib.dll</Reference>
  <Reference>C:\Workbench\Itera.MachineLearning.Fitness\bin\Debug\Itera.MachineLearning.Fitness.dll</Reference>
  <Reference>C:\Workbench\Itera.MachineLearning.Fitness\bin\Debug\Newtonsoft.Json.dll</Reference>
  <Reference>C:\Workbench\Itera.MachineLearning.Fitness\bin\Debug\numl.dll</Reference>
  <Namespace>Itera.MachineLearning.Fitness.Learning</Namespace>
  <Namespace>Itera.MachineLearning.Fitness.Services</Namespace>
  <Namespace>Itera.MachineLearning.Fitness.Services.WeatherHistory</Namespace>
  <Namespace>numl</Namespace>
  <Namespace>numl.Model</Namespace>
  <Namespace>numl.Supervised.DecisionTree</Namespace>
</Query>

var weatherService = new WeatherService();
var factory = new LearnerFactory(
	weatherService,
	new FitnessService()
);

var learner = factory.CreateActivityLearner();
var typedLearner = factory.CreateTypedActivityLearner();

//Debugger.Launch();

var list = new List<dynamic>();
foreach (var forecast in weatherService.NextWeek(Place.Oslo))
{
	var date = forecast.Date;
	var distance = learner.PredictDistanceByWeekday(date);
	var duration1 = learner.PredictDurationByWeekday(date);
	var duration2 = learner.PredictDurationByWeather(forecast);
	var duration = TimeSpan.FromMinutes((duration1.Minutes + duration2.Minutes) / 1.5);
	var type1 = typedLearner.PredictActivityTypeByWeather(forecast);
	var types = Enumerable.Range(6,16)
		.ToList()
		.Select(i => { 
			var activity = typedLearner.PredictActivityTypeByWeekdayAndHour(date.AddHours(i));
			return new {
				Hour = i,
				Activity = activity,
				Distance = typedLearner.PredictDistanceByActivityType(activity)
			};
		})
		.Where(a => a.Distance > 0)
		.GroupBy(a => a.Activity)
		.Select(g => {
			var dist = (0.8 * distance) + (g.First().Distance);
			return new {
				Activity = g.Key,
				Hours = string.Join(", ", g.Select(t => t.Hour.ToString()).ToArray()),
				Distance = dist,
				AverageSpeed = dist / duration.TotalHours
			};
		});

	list.Add(new {
		Day = date.DayOfWeek,
		TypeByWeather = type1,
		TypeByWeekdayAndHour = types,
		Duration = duration,
	});
}
list.Dump(true);