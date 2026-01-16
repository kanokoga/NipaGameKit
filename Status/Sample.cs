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
            this.CurrentWeather = weather;
        }
    }

    public class WeatherCxtChecker : ContextCheckerBase<WeatherContext>
    {
        private WeatherContext.WeatherType requiredWeather;

        public WeatherCxtChecker(WeatherContext.WeatherType weather)
        {
            this.requiredWeather = weather;
        }

        public override bool Check(WeatherContext context)
        {
            return context.CurrentWeather == this.requiredWeather;
        }
    }

    public class Sample : MonoBehaviour
    {
        void Start()
        {
            var unitSpeedModify = new Modifier<WeatherContext>
                (ModifierType.Multiplicative, 1.2f, new WeatherCxtChecker(WeatherContext.WeatherType.Sunny));

            var weatherContext = new WeatherContext(WeatherContext.WeatherType.Sunny);

            var speedStatusType = "UnitSpeed";
            var unit99Speed = new Status(speedStatusType, 99, 10f);

            var statusModifier = new StatusUpdater();
            statusModifier.AddModifier(speedStatusType, unitSpeedModify);

            statusModifier.UpdateStatus(unit99Speed, weatherContext);
        }
    }
}
