using UnityEngine;
using System.Collections.Generic;

namespace NipaGameKit.Statuses.Samples
{
    public class WeatherContext : Context
    {
        public enum WeatherType
        {
            Sunny,
            Rainy,
            Snowy
        }

        public WeatherType CurrentWeather { get; private set; }

        public WeatherContext(WeatherType weather)
        {
            CurrentWeather = weather;
        }
    }

    public class WeatherCxtChecker : ContextCheckerBase<WeatherContext>
    {
        private WeatherContext.WeatherType requiredWeather;

        public WeatherCxtChecker(WeatherContext.WeatherType weather)
        {
            requiredWeather = weather;
        }

        public override bool Check(WeatherContext context)
        {
            return context.CurrentWeather == requiredWeather;
        }
    }

    public class Sample : MonoBehaviour
    {
        void Start()
        {
            var unitSpeedModify = new Modifier<WeatherContext>
                (ModifierType.Multiplicative, 1.2f, new WeatherCxtChecker(WeatherContext.WeatherType.Sunny));

            var currentContext = new WeatherContext(WeatherContext.WeatherType.Sunny);
            bool isApplicable = unitSpeedModify.IsApplicable(currentContext);
            Debug.Log($"Is the modifier applicable? {isApplicable}");
        }
    }
}
