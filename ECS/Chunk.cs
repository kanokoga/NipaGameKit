using System;
using System.Collections.Generic;

namespace NipaGameKit.ECS
{
    public class Chunk
    {
        // コンポーネントの型をキーに、配列の実体を保持する
        private readonly Dictionary<Type, Array> _componentArrays = new Dictionary<Type, Array>();
        public int Capacity { get; private set; }
        public int Count { get; private set; }

        public Chunk(int capacity, params Type[] types)
        {
            if(capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Chunk capacity must be greater than 0.");
            }

            if(types == null || types.Length == 0)
            {
                throw new ArgumentException("Chunk must have at least one component type.", nameof(types));
            }

            this.Capacity = capacity;
            this.Count = 0;
            foreach(var type in types)
            {
                if(type == null)
                {
                    throw new ArgumentException("Component type cannot be null.", nameof(types));
                }

                if(type.IsValueType == false)
                {
                    throw new ArgumentException($"Component type must be struct: {type.FullName}", nameof(types));
                }

                if(this._componentArrays.ContainsKey(type))
                {
                    throw new ArgumentException($"Duplicate component type in chunk: {type.FullName}", nameof(types));
                }

                // 各コンポーネントの配列を事前確保
                this._componentArrays[type] = Array.CreateInstance(type, capacity);
            }
        }

        // 特定のコンポーネント配列を型安全に取得
        public T[] GetArray<T>() where T : struct
        {
            if(this._componentArrays.TryGetValue(typeof(T), out var array) == false)
            {
                throw new KeyNotFoundException($"Chunk does not contain component: {typeof(T).FullName}");
            }

            return (T[])array;
        }

        public bool HasComponent<T>() where T : struct => this._componentArrays.ContainsKey(typeof(T));

        public bool TryGetIndexById<T>(int targetId, out int index) where T : struct, IId
        {
            var array = this.GetArray<T>();
            for(var i = 0; i < this.Count; i++)
            {
                if(array[i].Id == targetId)
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }

        // エンティティの追加（初期値の設定など）
        public int AddEntity()
        {
            if(this.Count >= this.Capacity)
            {
                throw new Exception("Chunk is full");
            }

            return this.Count++;
        }

        // 削除（末尾の要素を空いた場所に移動して連続性を維持）
        public void RemoveAt(int index)
        {
            if(index < 0 || index >= this.Count)
            {
                return;
            }

            var lastIndex = this.Count - 1;
            if(index < lastIndex)
            {
                foreach(var array in this._componentArrays.Values)
                {
                    // 末尾のデータを削除位置にコピー（Swap-back）
                    Array.Copy(array, lastIndex, array, index, 1);
                }
            }

            this.Count--;
        }
    }
}
