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

    public class Modifier : IModifier
    {
        public ModifierType Type { get; }
        public float Value { get; }
        public string ModifyInfo;
        private IContextEvaluator contextEvaluator;

        public Modifier(ModifierType type, float value, IContextEvaluator contextEvaluator,
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
            var signPrefix = value >= 0 ? "+" : "-";
            var conditionInfo = $"({conditionDescription})";

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
            return this.contextEvaluator.Evaluate(context);
        }

        public string GetModifyInfo()
        {
            return this.ModifyInfo;
        }
    }
}
