using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NipaGameKit.Statuses
{
    public interface IContextEvaluator
    {
        string EvaluationDescription { get; }
        float Evaluate(Context context);
    }

    public abstract class ContextEvaluatorBase<T> : IContextEvaluator where T : Context
    {
        public string EvaluationDescription { get; set; }


        // インターフェースの実装
        public float Evaluate(Context context)
        {
            // 引数の context 自体、あるいはその中身から
            // このチェッカーが期待する型「T」を探し出す
            T target = context.GetContext<T>();
            //必ず見つかると仮定
            return this._Evaluate(target);
        }

        protected abstract float _Evaluate(T context);
    }

    public class AndEvaluator<T> : ContextEvaluatorBase<T> where T : Context
    {
        private readonly List<IContextEvaluator> checkers;

        public AndEvaluator(params IContextEvaluator[] checkers)
        {
            this.checkers = new List<IContextEvaluator>(checkers);
            this.EvaluationDescription = string.Join(" and ", this.checkers.Select(c => c.EvaluationDescription));
        }

        protected override float _Evaluate(T context)
        {
            var weight = this.SelfEvaluate(context);

            foreach(var checker in this.checkers)
            {
                var cWeight = checker.Evaluate(context);
                weight *= cWeight;
                if(CEE.IsValidWeight(weight) == false)
                {
                    break;
                }
            }

            return weight;
        }

        protected virtual float SelfEvaluate(T context)
        {
            return CEE.WeightBase;
        }
    }

    /// <summary>
    /// ContextEvaluator Extensions
    /// </summary>
    public static class CEE
    {
        public const float WeightBase = 1f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidWeight(float weight)
        {
            return weight > 0f;
        }

        public static float Evaluate(ConditionalBool condition, bool value)
        {
            return Evaluate(condition == ConditionalBool.None
                            || (condition == ConditionalBool.RequireTrue && value)
                            || (condition == ConditionalBool.RequireFalse && value == false));
        }

        public static float AndEvaluate(float weight, float val)
        {
            return weight < 0f ? 1f : val * weight;
        }

        // メソッドの引数に <T> が必要
        public static void SetDescription<T>(this ContextEvaluatorBase<T> evaluator, string description)
            where T : Context
        {
            evaluator.EvaluationDescription = description;
        }

        public static ContextEvaluatorBase<T> SetDescriptionNew<T>(this ContextEvaluatorBase<T> evaluator,
            string description)
            where T : Context
        {
            evaluator.EvaluationDescription = description;
            return evaluator;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Evaluate(bool value)
        {
            return value ? 1f : 0f;
        }
    }

    public enum ConditionalBool
    {
        RequireFalse = 0,
        RequireTrue = 1,
        None = 99,
    }
}
