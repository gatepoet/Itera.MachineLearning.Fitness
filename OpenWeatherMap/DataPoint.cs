using Newtonsoft.Json;

namespace Itera.MachineLearning.Fitness.OpenWeatherMap
{
    public class DataPoint
    {
        [JsonProperty("v")]
        public double Value { get; set; }

        [JsonProperty("c")]
        public int Index { get; set; }

        [JsonProperty("mi")]
        public double Minimum { get; set; }

        [JsonProperty("ma")]
        public double Maximum { get; set; }
    }
}
