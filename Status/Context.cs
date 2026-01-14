using UnityEngine;

namespace NipaGameKit.Statuses
{
    /// <summary>
    /// Base class representing the application conditions for status modifiers
    ///
    /// This class itself has no implementation and is designed as an empty base class for extensibility.
    /// In actual games, implement specific context classes that inherit from this class
    /// and contain information that becomes the application conditions for modifiers,
    /// such as weather, time, character status, etc.
    ///
    /// Example usage:
    /// <code>
    /// public class WeatherContext : Context
    /// {
    ///     public enum WeatherType { Sunny, Rainy, Snowy }
    ///     public WeatherType CurrentWeather { get; private set; }
    /// }
    /// </code>
    /// </summary>
    public class Context
    {

    }
}
