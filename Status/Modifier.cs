using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NipaGameKit.Statuses
{
    public class ContextCheckerBase<T> where T : Context
    {
        public string ConditionDescription { get; protected set; }

        public virtual bool Check(T context)
        {
            return true;
        }
    }

    public class AndChecker<T> : ContextCheckerBase<T> where T : Context
    {
        private readonly List<ContextCheckerBase<T>> checkers;

        public AndChecker(params ContextCheckerBase<T>[] checkers)
        {
            this.checkers = new List<ContextCheckerBase<T>>(checkers);
            // Auto-generate description text
            string fullDescription = string.Join(" and ", this.checkers.Select(c => c.ConditionDescription));

            // Truncate description if too long
            const int maxDescriptionLength = 100;
            if (fullDescription.Length > maxDescriptionLength)
            {
                this.ConditionDescription = fullDescription.Substring(0, maxDescriptionLength - 3) + "...";
            }
            else
            {
                this.ConditionDescription = fullDescription;
            }
        }

        public override bool Check(T context)
        {
            return this.checkers.All(c => c.Check(context));
        }
    }

    public interface IModifier
    {
        ModifierType Type { get; }
        float Value { get; }

        // Check if modifier is applicable using Context base class, with internal type checking
        bool IsApplicable(Context context);

        // Get modifier information
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

        public bool IsApplicable(Context context)
        {
            if(context is T specificContext)
            {
                return this.contextChecker.Check(specificContext);
            }

            return false;
        }

        public string GetModifyInfo()
        {
            return this.ModifyInfo;
        }
    }
}
