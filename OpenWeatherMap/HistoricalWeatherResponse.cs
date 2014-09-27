using System.Collections.Generic;
using Newtonsoft.Json;

namespace Itera.MachineLearning.Fitness.OpenWeatherMap
{
    public class HistoricalWeatherResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("cod")]
        public string Cod { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("station_id")]
        public int StationId { get; set; }

        [JsonProperty("calctime")]
        public double Calctime { get; set; }

        [JsonProperty("cnt")]
        public int Count { get; set; }

        [JsonProperty("list")]
        public List<HistoricalWeatherData> Data { get; set; }
    }
}