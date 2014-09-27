using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Itera.MachineLearning.Fitness.OpenWeatherMap
{
    public class DateTimeConverter : DateTimeConverterBase
    {
        private const string Format = "{{date:new Date({0}000)}}";
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
        };

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = string.Format(
                Format,
                reader.Value);

            var obj = (JObject) JsonConvert.DeserializeObject(
                value,
                SerializerSettings);
            
            return (DateTime) obj.GetValue("date");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).ToString("dd/MM/yyyy"));
        }
    }
}
