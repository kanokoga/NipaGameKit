using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NipaGameKit.Statuses
{
    public class ContextCheckerBase<T> where T : Context
    {
        public string conditionDescription;

        public virtual bool Check(T context)
        {
            return true;
        }
    }

    public class AndChecker<T> : ContextCheckerBase<T> where T : Context
    {
        private readonly List<ContextCheckerBase<T>> _checkers;

        public AndChecker(params ContextCheckerBase<T>[] checkers)
        {
            _checkers = new List<ContextCheckerBase<T>>(checkers);
            // 説明文を自動合成する
            conditionDescription = string.Join(" かつ ", _checkers.Select(c => c.conditionDescription));
        }

        public override bool Check(T context)
        {
            return _checkers.All(c => c.Check(context));
        }
    }

    public interface IModifier
    {
        ModifierType Type { get; }
        float Value { get; }

        // 引数を共通の親クラス「Context」にして、内部で型判定させる
        bool IsApplicable(Context context);
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
        public string modifyInfo;
        private ContextCheckerBase<T> contextChecker;

        public Modifier(ModifierType type, float value, ContextCheckerBase<T> contextChecker)
        {
            this.Type = type;
            this.Value = value;
            this.contextChecker = contextChecker;
            switch(this.Type)
            {
                case ModifierType.Additive:
                    this.modifyInfo = $"{(value >= 0 ? "+" : "")}{value:0.#}({contextChecker.conditionDescription})";
                    break;
                case ModifierType.Multiplicative:
                    this.modifyInfo =
                        $"{(value >= 0 ? "+" : "")}{(value*100f):0.#}%({contextChecker.conditionDescription})";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
    }
}
