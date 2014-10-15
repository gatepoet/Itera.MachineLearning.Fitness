using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Itera.MachineLearning.Fitness.Services.WeatherForecast
{
    public class ForecastService
    {
        public async Task<IEnumerable<ForecastWeatherData>> GetForecast(string place)
        {
            var url = Path.Combine(
                "http://www.yr.no/place",
                place,
                "varsel.xml")
                .Replace(@"\", @"/");

            var xml = await new HttpClient()
                .GetStringAsync(url);

            var weatherData = XDocument.Parse(xml)
                .Descendants("weatherdata").Single()
                .Descendants("forecast").Single()
                .Descendants("tabular").Single()
                .Descendants("time")
                .Select(time => new
                {
                    From = DateTime.Parse(time.Attribute("from").Value),
                    To = DateTime.Parse(time.Attribute("to").Value),
                    Symbol = time.Descendants("symbol")
                        .Select(s => new
                        {
                            Number = int.Parse(s.Attribute("number").Value),
                            Name = s.Attribute("name").Value,
                            Var = s.Attribute("var").Value
                        })
                        .Single(),
                    Precipitation = double.Parse(
                        time.Descendants("precipitation")
                            .Single()
                            .Attribute("value").Value,
                        CultureInfo.InvariantCulture),
                    Wind = time.Descendants("windSpeed")
                        .Select(s => new
                        {
                            Mps = double.Parse(
                                s.Attribute("mps").Value,
                                CultureInfo.InvariantCulture),
                            Name = s.Attribute("name").Value
                        })
                        .Single(),
                    Temperature = time.Descendants("temperature")
                        .Select(s => new
                        {
                            Unit = s.Attribute("unit").Value,
                            Value = double.Parse(
                                s.Attribute("value").Value,
                                CultureInfo.InvariantCulture)
                        })
                        .Single()
                })
                .Where(m => m.From.Hour >= 18)
                .Select(m => new ForecastWeatherData()
                {
                    Date = m.From.Date,
                    Temperature = m.Temperature.Value,
                    Percipitation = m.Precipitation,
                    Wind = m.Wind.Mps
                });

            return weatherData;
        }
    }
}
