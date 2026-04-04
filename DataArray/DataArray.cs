using UnityEngine;
using System;
using System.Collections.Generic;

namespace NipaGameKit
{
    /// <summary>
    /// IDを持つ構造体の制約
    /// </summary>
    public interface IId
    {
        int Id { get; }
    }

    /// <summary>
    /// データ指向のメモリ効率に優れた構造体専用コレクション
    /// </summary>
    public static class DataArray<T> where T : struct, IId
    {
        private const int InvalidIndex = -1;

        public static int Count { get; private set; }

        private static T[] Datas;
        private static int[] ArrayToId;
        private static int[] IdToArray;


        static DataArray()
        {
            StaticResetter.OnResetStatic += Clear;
            var capacity = 256; // 初期容量
            var maxIdRange = 1024; // IDの最大範囲（Sparse配列のサイズ）
            Datas = new T[capacity];
            ArrayToId = new int[capacity];
            IdToArray = new int[maxIdRange];
            Array.Fill(IdToArray, InvalidIndex);
            Count = 0;
        }

        /// <summary>
        /// データの追加（オブジェクトプールからの取得に相当）
        /// </summary>
        public static void Add(T item)
        {
            if(Count >= Datas.Length)
            {
                throw new Exception("Capacity exceeded");
            }

            if(item.Id >= IdToArray.Length)
            {
                throw new Exception("ID range exceeded");
            }

            var index = Count;
            Datas[index] = item;
            ArrayToId[index] = item.Id;
            IdToArray[item.Id] = index;
            Count++;
        }

        /// <summary>
        /// IDを指定してデータを削除（返却）
        /// 最後の要素を削除箇所に上書きすることで「隙間」を埋める
        /// </summary>
        public static bool Remove(int id)
        {
            var indexToRemove = IdToArray[id];
            if(indexToRemove == InvalidIndex)
            {
                return false;
            }

            // 最後の要素の情報を取得
            var lastIndex = Count - 1;
            var lastItem = Datas[lastIndex];
            var lastItemId = ArrayToId[lastIndex];

            // 削除したい場所に最後の要素を移動（Swap and Pop）
            Datas[indexToRemove] = lastItem;
            ArrayToId[indexToRemove] = lastItemId;
            IdToArray[lastItemId] = indexToRemove;

            // 削除されたIDの情報をクリア
            IdToArray[id] = InvalidIndex;
            Count--;

            return true;
        }

        /// <summary>
        /// IDからデータを取得（修正を可能にするため ref 戻り値）
        /// </summary>
        public static ref T GetRef(int id)
        {
            var index = IdToArray[id];
            if(index == InvalidIndex)
            {
                throw new KeyNotFoundException();
            }

            return ref Datas[index];
        }

        public static T Get(int id)
        {
            var index = IdToArray[id];
            if(index == InvalidIndex)
            {
                throw new KeyNotFoundException();
            }
            return Datas[index];
        }

        /// <summary>
        /// 使用中の全データに対する高速なループ用
        /// </summary>
        public static Span<T> AsSpan() => Datas.AsSpan(0, Count);

        private static void Clear()
        {
            Array.Fill(IdToArray, InvalidIndex);
            Count = 0;
        }
    }
}
