using Heracles.Application.Weather;

namespace Heracles.Web.Components.Theme;

public static class HeraclesImages
{
    public static class Weather
    {
        public const string ClearDay = "images/weather/clear-day.png";
        public const string ClearNight = "images/weather/clear-night.png";
        public const string Cloudy = "images/weather/cloudy.png";
        public const string Fog = "images/weather/fog.png";
        public const string Rain = "images/weather/heavy-rain.png";
        public const string PartlyCloudyDay = "images/weather/partly-cloudy.png";
        public const string PartlyCloudyNight = "images/weather/partly-cloudy-night.png";
        public const string ShowersDay = "images/weather/rain.png";
        public const string ShowersNight = "images/weather/rain-night.png";
        public const string Snow = "images/weather/heavy-snow.png";
        public const string SnowShowersDay = "images/weather/snow.png";
        public const string SnowShowersNight = "images/weather/snow.png";
        public const string ThunderRain = "images/weather/thunder-storm.png";
        public const string ThunderShowersDay = "images/weather/thunder-storm.png";
        public const string ThunderShowersNight = "images/weather/thunder-storm.png";
        public const string Wind = "images/weather/wind.png";
        public const string Unknown = "images/weather/cloud.png";
    }
    
    internal static string GetWeatherImage(WeatherCode? code) => code switch {
        WeatherCode.ClearDay => Weather.ClearDay,
        WeatherCode.ClearNight => Weather.ClearNight,
        WeatherCode.Cloudy => Weather.Cloudy,
        WeatherCode.Fog => Weather.Fog,
        WeatherCode.Rain => Weather.Rain,
        WeatherCode.PartlyCloudyDay => Weather.PartlyCloudyDay,
        WeatherCode.PartlyCloudyNight => Weather.PartlyCloudyNight,
        WeatherCode.ShowersDay => Weather.ShowersDay,
        WeatherCode.ShowersNight => Weather.ShowersNight,
        WeatherCode.Snow => Weather.Snow,
        WeatherCode.SnowShowersDay => Weather.SnowShowersDay,
        WeatherCode.SnowShowersNight => Weather.SnowShowersNight,
        WeatherCode.ThunderRain => Weather.ThunderRain,
        WeatherCode.ThunderShowersDay => Weather.ThunderShowersDay,
        WeatherCode.ThunderShowersNight => Weather.ThunderShowersNight,
        WeatherCode.Wind => Weather.Wind,
        _ => Weather.Unknown
    };
}
