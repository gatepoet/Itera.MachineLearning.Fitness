<Query Kind="Statements">
  <Reference>C:\Workbench\Itera.MachineLearning.Fitness\Program\bin\Debug\Itera.MachineLearning.Fitness.dll</Reference>
  <Reference>C:\Workbench\Itera.MachineLearning.Fitness\Program\bin\Debug\Newtonsoft.Json.dll</Reference>
  <Reference>C:\Workbench\Itera.MachineLearning.Fitness\Program\bin\Debug\numl.dll</Reference>
  <Reference>C:\Workbench\Itera.MachineLearning.Fitness\Program\bin\Debug\Program.exe</Reference>
  <Namespace>Itera.MachineLearning.Fitness.Services</Namespace>
  <Namespace>Itera.MachineLearning.Fitness.Services.WeatherHistory</Namespace>
  <Namespace>numl</Namespace>
  <Namespace>numl.Model</Namespace>
  <Namespace>numl.Supervised.DecisionTree</Namespace>
  <Namespace>Program</Namespace>
</Query>

var path = Path.Combine(
	Environment.GetFolderPath(
		Environment.SpecialFolder.Desktop),
			"results.xml");

var weatherDataService = new WeatherService();
//TODO: 18700 = OSLO station
var weatherData = weatherDataService.GetHistoricalMetDataDaily<HistoricalWeatherData>(
	Station.Oslo,
	new DateTime(2011, 01, 01),
	DateTime.Now)
	.ToList();
var fitnessService = new FitnessService();
var allActivities = fitnessService.GetAllActivities().Dump();
var activities = fitnessService.GetTypedActivities().ToList().OrderBy(a => a.AverageSpeed).Dump();

var activitiesWithWeatherData = from weatherObject in weatherData
                             join a in allActivities on weatherObject.Date.Date equals a.Date.Date
                             select new TypedActivityDescriptor()
                             {
                                 AverageSpeed = a.AverageSpeed,
                                 Date = a.Date,
                                 Distance = a.Distance,
                                 Duration = a.Duration,
                                 //Type = a.Type,
                                 Wind = weatherObject.Wind,
								 Percipitation = weatherObject.Percipitation,
								 Temperature = weatherObject.Temperature
                             };
							 
var activitiesWithWeatherDataList = activitiesWithWeatherData.ToList().Dump();
var descriptor = Descriptor.For<TypedActivityDescriptor>()
	.WithDateTime(a => a.Date, DateTimeFeature.DayOfWeek)
	.WithDateTime(a => a.Date, DateTimeFeature.Hour)
	.Learn(a => a.Type);
var generator = new DecisionTreeGenerator(descriptor);
generator.SetHint(true);
var model = Learner.Learn(activitiesWithWeatherDataList, 0.80, 10000, generator);
model.ToString().Dump();
model.Model.Save(path);
for (int j = 0; j < 7; j++)
{
	for (int i = 8; i < 24; i++)
	{
		var result = model.Model.Predict(
			new TypedActivityDescriptor() {
				Date = DateTime.Today.AddDays(j).AddHours(i),
				Wind = i % 3});
		(result.Date.DayOfWeek + " " + result.Date.Hour + " " + result.Type).Dump();
	}
}