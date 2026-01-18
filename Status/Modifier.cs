using System;
using UnityEngine;

namespace NipaGameKit.Statuses
{
    public interface IModifier
    {
        ModifierType Type { get; }
        float Value { get; }

        float Evaluate(Context context);

        string GetModifyInfo();
    }

    public enum ModifierType
    {
        Additive,
        AddictiveMultiplication,
        Multiplicative,
    }

    public class Modifier<T> : IModifier where T : Context
    {
        public ModifierType Type { get; }
        public float Value { get; }
        public string ModifyInfo;
        private ContextEvaluatorBase<T> contextEvaluator;

        public Modifier(ModifierType type, float value, ContextEvaluatorBase<T> contextEvaluator,
            bool valueAsPercentage = false)
        {
            this.Type = type;
            this.Value = value;
            this.contextEvaluator = contextEvaluator;
            this.ModifyInfo = GenerateModifyInfo(type,
                value,
                contextEvaluator.EvaluationDescription,
                valueAsPercentage);
        }

        private static string GenerateModifyInfo(ModifierType type,
            float value,
            string conditionDescription,
            bool valueAsPercentage)
        {
            string signPrefix = value >= 0 ? "+" : "-";
            string conditionInfo = $"({conditionDescription})";

            switch(type)
            {
                case ModifierType.Additive:
                    return valueAsPercentage == false
                        ? $"{signPrefix}{value:0.#}{conditionInfo}"
                        : $"{signPrefix}{Math.Abs(value) * 100f:0.#}%{conditionInfo}";
                case ModifierType.AddictiveMultiplication:
                    var percentageValue = Mathf.Abs(value * 100f);
                    return $"{signPrefix}x{percentageValue:0.#}%{conditionInfo}";
                case ModifierType.Multiplicative:
                    var percentageValue2 = (value + 1f) * 100f;
                    return $"x{percentageValue2:0.#}%{conditionInfo}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported modifier type");
            }
        }

        public float Evaluate(Context context)
        {
            if(context is T specificContext)
            {
                return this.contextEvaluator.Evaluate(specificContext);
            }

            return 0f;
        }

        public string GetModifyInfo()
        {
            return this.ModifyInfo;
        }
    }
}
