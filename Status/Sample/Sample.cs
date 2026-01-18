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

    public class WeatherCxtEvaluator : ContextEvaluatorBase<WeatherContext>
    {
        private WeatherContext.WeatherType requiredWeather;

        public WeatherCxtEvaluator(WeatherContext.WeatherType weather)
        {
            this.requiredWeather = weather;
        }

        protected override float _Evaluate(WeatherContext context)
        {
            return CEE.Evaluate(context.CurrentWeather == this.requiredWeather);
        }
    }

    public class Sample : MonoBehaviour
    {
        private void Start()
        {
            var sunnyUnitSpeedUp = new Modifier<SampleContext>
            (ModifierType.AddictiveMultiplication, 0.2f,
                new AndEvaluator<SampleContext>(
                    new WeatherCxtEvaluator(WeatherContext.WeatherType.Sunny)
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
