using System.Collections.Generic;
using System.Linq;

namespace NipaGameKit.Statuses
{
    public interface IContextChecker
    {
        string ConditionDescription { get; }
        bool IsValid(Context context);
    }

    public abstract class ContextCheckerBase<T> : IContextChecker where T : Context
    {
        public string ConditionDescription { get; set; }

        // インターフェースの実装
        public bool IsValid(Context context)
        {
            // 引数の context 自体、あるいはその中身から
            // このチェッカーが期待する型「T」を探し出す
            T target = context.GetContext<T>();
            //必ず見つかると仮定
            return this._IsValid(target);
        }

        protected abstract bool _IsValid(T context);
    }

    public static class ContextCheckerExtensions
    {
        // メソッドの引数に <T> が必要
        public static void SetDescription<T>(this ContextCheckerBase<T> checker, string description) where T : Context
        {
            checker.ConditionDescription = description;
        }

        public static ContextCheckerBase<T> SetDescriptionNew<T>(this ContextCheckerBase<T> checker, string description)
            where T : Context
        {
            checker.ConditionDescription = description;
            return checker;
        }
    }

    public class AndChecker<T> : ContextCheckerBase<T> where T : Context
    {
        private readonly List<IContextChecker> checkers;

        public AndChecker(params IContextChecker[] checkers)
        {
            this.checkers = new List<IContextChecker>(checkers);
            this.ConditionDescription = string.Join(" and ", this.checkers.Select(c => c.ConditionDescription));
        }

        protected override bool _IsValid(T context)
        {
            return this.__IsValid(context) == true
                   && this.checkers.All(c => c.IsValid(context));
        }

        protected virtual bool __IsValid(T context)
        {
            return true;
        }
    }

    public enum ConditionalBool
    {
        RequireFalse = 0,
        RequireTrue = 1,
        None = 99,
    }

    public static class CxtCheckerUtil
    {
        public static bool IsTrue(bool value, ConditionalBool condition)
        {
            return condition == ConditionalBool.None
                   || (condition == ConditionalBool.RequireTrue && value)
                   || (condition == ConditionalBool.RequireFalse && value == false);
        }

        public static bool IsTrue(int value, int condition)
        {
            return condition < 0
                   || value == condition;
        }
    }
}
