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

    public class SampleContext : Context
    {
        public Context dummyContext;
        public WeatherContext weatherContext;

        protected override T SearchComponent<T>()
        {
            if(this is T result)
            {
                return result;
            }

            if(typeof(T) == typeof(WeatherContext))
            {
                return (T)(object)this.weatherContext;
            }

            return null;
        }
    }

    public class WeatherCxtChecker : ContextCheckerBase<WeatherContext>
    {
        private WeatherContext.WeatherType requiredWeather;

        public WeatherCxtChecker(WeatherContext.WeatherType weather)
        {
            this.requiredWeather = weather;
        }

        protected override bool _IsValid(WeatherContext context)
        {
            return context.CurrentWeather == this.requiredWeather;
        }
    }

    public class Sample : MonoBehaviour
    {
        private void Start()
        {
            var sunnyUnitSpeedUp = new Modifier<SampleContext>
            (ModifierType.Multiplicative, 0.2f,
                new AndChecker<SampleContext>(
                    new WeatherCxtChecker(WeatherContext.WeatherType.Sunny)
                        .SetDescriptionNew("I like sunny weather")
                ));

            var weatherContext = new WeatherContext(WeatherContext.WeatherType.Sunny);
            var sampleContext = new SampleContext
            {
                weatherContext = weatherContext
            };

            var speedStatusType = "UnitSpeed";
            var speedStat = new Status(speedStatusType, 99, 10f);

            var statusModifier = new StatusUpdater();
            statusModifier.AddModifier(speedStatusType, sunnyUnitSpeedUp);

            statusModifier.UpdateStatus(speedStat, sampleContext);

            Debug.Log(speedStat);
        }
    }
}
