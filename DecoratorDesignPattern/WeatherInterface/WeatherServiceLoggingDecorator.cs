using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DecoratorDesignPattern.WeatherInterface
{
    public class WeatherServiceLoggingDecorator : IWeatherService
    {
        private IWeatherService _innerWeatherService;
        private ILogger<WeatherServiceLoggingDecorator> _logger;

        public WeatherServiceLoggingDecorator(IWeatherService weatherService, ILogger<WeatherServiceLoggingDecorator> logger)
        {
            _innerWeatherService = weatherService;
            _logger = logger;
        }

        public CurrentWeather GetCurrentWeather(string location)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var currentWeather = _innerWeatherService.GetCurrentWeather(location);
            sw.Stop();
            long elapsedMillis = sw.ElapsedMilliseconds;
            _logger.LogWarning("Retrieved weather data for {location} - Elapsed ms: {} {@currentWeather}", location, elapsedMillis, currentWeather);
            return currentWeather;
        }

        public LocationForecast GetForecast(string location)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var forecast = _innerWeatherService.GetForecast(location);
            sw.Stop();
            long elapsedMillis = sw.ElapsedMilliseconds;
            _logger.LogWarning("Retrieved forecast data for {location} - Elapsed ms: {} {@currentWeather}", location, elapsedMillis, forecast);
            return forecast;
        }
    }
}
