using System;

namespace NipaGameKit.Statuses
{
    public interface IModifier
    {
        ModifierType Type { get; }
        float Value { get; }

        bool IsValid(Context context);

        string GetModifyInfo();
    }

    public enum ModifierType
    {
        Additive,
        Multiplicative
    }

    public class Modifier<T> : IModifier where T : Context
    {
        public ModifierType Type { get; }
        public float Value { get; }
        public string ModifyInfo;
        private ContextCheckerBase<T> contextChecker;

        public Modifier(ModifierType type, float value, ContextCheckerBase<T> contextChecker)
        {
            this.Type = type;
            this.Value = value;
            this.contextChecker = contextChecker;
            this.ModifyInfo = GenerateModifyInfo(type, value, contextChecker.ConditionDescription);
        }

        private static string GenerateModifyInfo(ModifierType type, float value, string conditionDescription)
        {
            string signPrefix = value >= 0 ? "+" : "";
            string conditionInfo = $"({conditionDescription})";

            switch(type)
            {
                case ModifierType.Additive:
                    return $"{signPrefix}{value:0.#}{conditionInfo}";
                case ModifierType.Multiplicative:
                    float percentageValue = value * 100f;
                    return $"{signPrefix}{percentageValue:0.#}%{conditionInfo}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported modifier type");
            }
        }

        public bool IsValid(Context context)
        {
            if(context is T specificContext)
            {
                return this.contextChecker.IsValid(specificContext);
            }

            return false;
        }

        public string GetModifyInfo()
        {
            return this.ModifyInfo;
        }
    }
}
