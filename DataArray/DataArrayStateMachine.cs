using System;
using System.Collections.Generic;

namespace NipaGameKit
{
    public interface IFsmContext<TStateEnum> where TStateEnum : struct
    {
        TStateEnum CurrentState { get; set; }
    }

    /// <summary>
    /// IId を持つコンテキスト向けのステートマシン。
    /// wire 制約は持たず、任意の状態へ遷移できる。
    /// ステート処理内では context.Id を使って任意型の DataArray を参照可能。
    /// </summary>
    public class DataStateMachineLogic<TStateEnum, TContext>
        where TStateEnum : struct
        where TContext : struct, IFsmContext<TStateEnum>, IId
    {
        private readonly Dictionary<TStateEnum, IDataState<TStateEnum, TContext>> _stateMap = new();

        public event Action<Exception> Unhandled;
        public event Action<TStateEnum, TStateEnum> Changed;

        public DataStateMachineLogic()
        {
        }

        #region Configuration

        public void AddState(TStateEnum state, Action<IDataState<TStateEnum, TContext>> config)
        {
            var s = new CDataState<TStateEnum, TContext>(state);
            config(s);
            this._stateMap[state] = s;
        }

        #endregion

        #region Execution Logic

        public void Update(ref TContext context)
        {
            if(this._stateMap.TryGetValue(context.CurrentState, out var state))
            {
                try
                {
                    state.Update?.Invoke(ref context);
                }
                catch(Exception e)
                {
                    this.HandleException(context.CurrentState, ref context, e);
                }
            }
        }

        public bool Transit(ref TContext context, TStateEnum next)
        {
            var prev = context.CurrentState;
            if(EqualityComparer<TStateEnum>.Default.Equals(prev, next))
            {
                return false;
            }

            if(!this._stateMap.TryGetValue(next, out var nextState))
            {
                return false;
            }

            this._stateMap.TryGetValue(prev, out var prevState);

            try
            {
                prevState?.Exit?.Invoke(ref context);
                context.CurrentState = next;
                nextState.Enter?.Invoke(ref context);

                this.Changed?.Invoke(prev, next);
                return true;
            }
            catch(Exception e)
            {
                this.HandleException(context.CurrentState, ref context, e);
                return false;
            }
        }

        private void HandleException(TStateEnum currentState, ref TContext context, Exception e)
        {
            if(this._stateMap.TryGetValue(currentState, out var state) && state.Handle != null)
            {
                try
                {
                    state.Handle.Invoke(ref context, e);
                }
                catch(Exception inner)
                {
                    this.Unhandled?.Invoke(inner);
                }
            }
            else
            {
                this.Unhandled?.Invoke(e);
            }
        }

        public static ref TData GetDataRef<TData>(ref TContext context) where TData : struct, IId
        {
            return ref DataArray<TData>.GetRef(context.Id);
        }

        #endregion
    }

    public delegate void DataContextAction<TContext>(ref TContext context);
    public delegate void DataContextExceptionAction<TContext>(ref TContext context, Exception exception);

    public interface IDataState<TStateEnum, TContext>
    {
        TStateEnum Target { get; }
        DataContextAction<TContext> Enter { get; set; }
        DataContextAction<TContext> Update { get; set; }
        DataContextAction<TContext> Exit { get; set; }
        DataContextExceptionAction<TContext> Handle { get; set; }
    }

    public class CDataState<TStateEnum, TContext> : IDataState<TStateEnum, TContext>
    {
        public TStateEnum Target { get; }
        public DataContextAction<TContext> Enter { get; set; }
        public DataContextAction<TContext> Update { get; set; }
        public DataContextAction<TContext> Exit { get; set; }
        public DataContextExceptionAction<TContext> Handle { get; set; }

        public CDataState(TStateEnum target)
        {
            this.Target = target;
        }
    }
}
