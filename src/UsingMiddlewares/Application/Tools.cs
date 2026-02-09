using System.ComponentModel;

namespace UsingMiddlewares.Application;

public static class Tools
{
    [Description("Get the weather for a given location.")]
    public static string GetWeather(
        [Description("The location to get the weather for.")] string location)
    {
        return $"The weather in {location} is cloudy with a high of 15°C.";
    }
    
    [Description("The current datetime offset.")]
    public static string GetDateTime()
        => DateTimeOffset.Now.ToString();

}