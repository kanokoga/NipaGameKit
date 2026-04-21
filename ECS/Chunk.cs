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
            this.Capacity = capacity;
            this.Count = 0;
            foreach(var type in types)
            {
                // 各コンポーネントの配列を事前確保
                this._componentArrays[type] = Array.CreateInstance(type, capacity);
            }
        }

        // 特定のコンポーネント配列を型安全に取得
        public T[] GetArray<T>() => (T[])this._componentArrays[typeof(T)];

        public bool HasComponent<T>() => this._componentArrays.ContainsKey(typeof(T));

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
