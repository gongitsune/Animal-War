﻿using System.Collections.Generic;
using UnityEngine;

namespace MyPackages.StateMachine
{
    /// <summary>
    ///     ステートマシンクラス
    ///     状態遷移の定義版
    /// </summary>
    public class StateMachine<TOwner, TMemory> where TMemory : Memory
    {
        private readonly LinkedList<StateBase> _states = new(); // 全てのステート定義
        private StateBase _currentState; // 現在のステート

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        /// <param name="owner">StateMachineを使用するOwner</param>
        /// <param name="initMemory"></param>
        public StateMachine(TOwner owner, TMemory initMemory)
        {
            Owner = owner;
            Memory = initMemory;
        }

        private TOwner Owner { get; }
        private TMemory Memory { get; }

        /// <summary>
        ///     ステート追加
        /// </summary>
        private T Add<T>() where T : StateBase, new()
        {
            // ステートを追加
            var newState = new T
            {
                StateMachine = this
            };
            _states.AddLast(newState);
            return newState;
        }

        /// <summary>
        ///     ステート取得、無ければ追加
        /// </summary>
        private T GetOrAdd<T>() where T : StateBase, new()
        {
            // 追加されていれば返却
            foreach (var state in _states)
                if (state is T result)
                    return result;
            // 無ければ追加
            return Add<T>();
        }

        /// <summary>
        ///     イベントIDに対応した遷移情報を登録
        /// </summary>
        /// <param name="eventId">イベントID</param>
        /// <typeparam name="TFrom">遷移元ステート</typeparam>
        /// <typeparam name="TTo">遷移先ステート</typeparam>
        public void AddTransition<TFrom, TTo>(int eventId)
            where TFrom : StateBase, new()
            where TTo : StateBase, new()
        {
            // 既にイベントIDが登録済ならエラー
            var from = GetOrAdd<TFrom>();
            if (from.Transitions.ContainsKey(eventId))
            {
                Debug.LogError("already register eventId!! : " + eventId);
                return;
            }

            // 指定のイベントIDで追加する
            var to = GetOrAdd<TTo>();
            from.Transitions.Add(eventId, to);
        }

        /// <summary>
        ///     ステート開始処理
        /// </summary>
        /// <typeparam name="T">開始するステート</typeparam>
        public void OnStart<T>() where T : StateBase, new()
        {
            _currentState = GetOrAdd<T>();
            _currentState.OnStart();
        }

        /// <summary>
        ///     ステート更新処理
        /// </summary>
        public void OnUpdate()
        {
            _currentState.OnUpdate();
        }

        /// <summary>
        ///     イベント発行
        ///     指定されたIDのステートに切り替える
        /// </summary>
        /// <param name="eventId">イベントID</param>
        public void DispatchEvent(int eventId)
        {
            // イベントIDからステート取得
            if (!_currentState.Transitions.TryGetValue(eventId, out var nextState))
            {
                Debug.LogError("not found eventId!! : " + eventId);
                return;
            }

            // ステートを切り替える
            _currentState.OnEnd();
            nextState.OnStart();
            _currentState = nextState;
        }

        /// <summary>
        ///     ステート基底クラス
        ///     各ステートクラスはこのクラスを継承する
        /// </summary>
        public abstract class StateBase
        {
            public readonly Dictionary<int, StateBase> Transitions = new(); // ステート遷移情報
            public StateMachine<TOwner, TMemory> StateMachine;
            protected TOwner Owner => StateMachine.Owner;
            protected TMemory Memory => StateMachine.Memory;

            public virtual void OnStart()
            {
            }

            public virtual void OnUpdate()
            {
            }

            public virtual void OnEnd()
            {
            }
        }
    }
}