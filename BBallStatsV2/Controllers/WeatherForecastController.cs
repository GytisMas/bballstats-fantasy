using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices.Marshalling;

namespace BBallStatsV2.Controllers
{
    [ApiController]
    [Route("api/test/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        //[HttpGet(Name = "GetWeatherForecast")]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}

        [HttpGet(Name = "TestFetch")]
        public async Task<string> Get()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync("https://api-live.euroleague.net/v1/games?seasonCode=E2023&gameCode=234");
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
