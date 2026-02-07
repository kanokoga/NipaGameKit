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

    public class TerrainContext : Context
    {
        public enum TerrainType
        {
            Plains,
            Forest,
            Mountain
        }

        public TerrainType CurrentTerrain { get; private set; }

        public TerrainContext(TerrainType terrain)
        {
            this.CurrentTerrain = terrain;
        }
    }

    public class EnvironmentContext : Context
    {
        public TerrainContext terrainContext;
        public WeatherContext weatherContext;

        protected override T SearchContext<T>()
        {
            if(this is T result)
            {
                return result;
            }

            if(typeof(T) == typeof(WeatherContext))
            {
                return (T)(object)this.weatherContext;
            }

            if(typeof(T) == typeof(TerrainContext))
            {
                return (T)(object)this.terrainContext;
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

    public class TerrainCxtEvaluator : ContextEvaluatorBase<TerrainContext>
    {
        private TerrainContext.TerrainType requiredTerrain;

        public TerrainCxtEvaluator(TerrainContext.TerrainType terrain)
        {
            this.requiredTerrain = terrain;
        }

        protected override float _Evaluate(TerrainContext context)
        {
            return CEE.Evaluate(context.CurrentTerrain == this.requiredTerrain);
        }
    }

    public class Sample : MonoBehaviour
    {
        public WeatherContext.WeatherType weatherType = WeatherContext.WeatherType.Sunny;
        public TerrainContext.TerrainType terrainType = TerrainContext.TerrainType.Plains;
        private StatusUpdater statusModifier = new StatusUpdater();

        private void Start()
        {
            var sunnyAndPlainSpeedup = new Modifier<EnvironmentContext>
            (ModifierType.AddictiveMultiplication, 0.5f,
                new AndEvaluator<EnvironmentContext>(
                    new WeatherCxtEvaluator(WeatherContext.WeatherType.Sunny),
                    new TerrainCxtEvaluator(TerrainContext.TerrainType.Plains)
                ).SetDescriptionNew("Sunny weather and plains, the best condition to move"));

            var forestSpeedDown = new Modifier<EnvironmentContext>
            (ModifierType.AddictiveMultiplication, -0.1f,
                new AndEvaluator<EnvironmentContext>(
                new TerrainCxtEvaluator(TerrainContext.TerrainType.Forest)
                    .SetDescriptionNew("Forests are hard to walk")));

            var mountainSpeedDown = new Modifier<EnvironmentContext>
            (ModifierType.AddictiveMultiplication, -0.2f,
                new AndEvaluator<EnvironmentContext>(
                new TerrainCxtEvaluator(TerrainContext.TerrainType.Mountain)
                    .SetDescriptionNew("Mountains are hard to walk")));

            var environmentContext = new EnvironmentContext
            {
                weatherContext = new WeatherContext(WeatherContext.WeatherType.Sunny),
                terrainContext = new TerrainContext(TerrainContext.TerrainType.Plains)
            };

            var speedStatusType = "UnitSpeed";
            var speedStat = new Status(speedStatusType, 99, 10f);

            this.statusModifier.AddModifier(speedStatusType, sunnyAndPlainSpeedup);
            this.statusModifier.AddModifier(speedStatusType, forestSpeedDown);
            this.statusModifier.AddModifier(speedStatusType, mountainSpeedDown);
            this.statusModifier.UpdateStatus(speedStat, environmentContext);
            Debug.Log(speedStat);
        }

        // onguiwindows to change weather and terrain,then update status
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 200, 500));
            GUILayout.Label("Weather:" + this.weatherType.ToString());
            if(GUILayout.Button("Sunny"))
            {
                this.weatherType = WeatherContext.WeatherType.Sunny;
            }

            if(GUILayout.Button("Rainy"))
            {
                this.weatherType = WeatherContext.WeatherType.Rainy;
            }

            if(GUILayout.Button("Snowy"))
            {
                this.weatherType = WeatherContext.WeatherType.Snowy;
            }

            GUILayout.Space(20);
            GUILayout.Label("Terrain:" + this.terrainType.ToString());
            if(GUILayout.Button("Plains"))
            {
                this.terrainType = TerrainContext.TerrainType.Plains;
            }

            if(GUILayout.Button("Forest"))
            {
                this.terrainType = TerrainContext.TerrainType.Forest;
            }

            if(GUILayout.Button("Mountain"))
            {
                this.terrainType = TerrainContext.TerrainType.Mountain;
            }

            GUILayout.Space(20);

            if(GUILayout.Button("Update Status"))
            {
                var environmentContext = new EnvironmentContext
                {
                    weatherContext = new WeatherContext(this.weatherType),
                    terrainContext = new TerrainContext(this.terrainType)
                };

                var speedStatusType = "UnitSpeed";
                var speedStat = new Status(speedStatusType, 99, 10f);

                this.statusModifier.UpdateStatus(speedStat, environmentContext);
                Debug.Log(speedStat);
            }

            GUILayout.EndArea();
        }
    }
}
