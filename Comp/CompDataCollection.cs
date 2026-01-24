using System;
using System.Collections.Generic;
using UnityEngine;

namespace NipaGameKit
{
    /// <summary>
    /// refパラメータ付きデリゲート（ジェネリック型パラメータ用）
    /// </summary>
    public delegate void UpdateDataDelegate<TData>(int monoId, ref TData data) where TData : struct, ICompData;

    /// <summary>
    /// SoA（Structure of Arrays）構造でデータを保持
    /// 連続メモリに配置され、キャッシュフレンドリーなアクセスを実現
    /// </summary>
    public static class CompDataCollection<TData> where TData : struct, ICompData
    {
        public static event Action<int> OnDataAdded = delegate { };
        public static event Action<int> OnDataRemoving = delegate { };
        public static event Action<int> OnDataRemoved = delegate { };

        // SoA構造：データを配列で保持（連続メモリ）
        private static TData[] _dataArray = new TData[256];
        private static int[] _monoIdToIndex = new int[1024]; // MonoId -> 配列インデックス
        private static int _count = 0;
        private static int _capacity = 256;

        // アクティブなデータのインデックスリスト（Update時にループする）
        private static int[] _activeIndices = new int[256];
        private static int _activeCount = 0;

        static CompDataCollection()
        {
            StaticResetter.OnResetStatic += Dispose;
            // 初期化時にmonoIdToIndexを-1で埋める（無効なインデックス）
            for(var i = 0; i < _monoIdToIndex.Length; i++)
            {
                _monoIdToIndex[i] = -1;
            }
        }

        /// <summary>
        /// データを追加（O(1)）
        /// </summary>
        public static void Add(int entityId, TData data)
        {
            // 容量チェック
            if(_count >= _capacity)
            {
                Resize();
            }

            var index = _count;
            _dataArray[index] = data;
            _dataArray[index].EntityId = entityId;
            _dataArray[index].IsActive = true;

            _monoIdToIndex[entityId] = index;
            _count++;

            // アクティブリストに追加
            if(_activeCount >= _activeIndices.Length)
            {
                Array.Resize(ref _activeIndices, _activeIndices.Length * 2);
            }

            _activeIndices[_activeCount] = index;
            _activeCount++;

            OnDataAdded.Invoke(entityId);
        }

        /// <summary>
        /// データを削除（O(1) - 最後の要素とスワップ）
        /// </summary>
        public static void Remove(int entityId)
        {
            var index = _monoIdToIndex[entityId];
            if(index < 0 || index >= _count)
            {
                return;
            }

            OnDataRemoving.Invoke(entityId);

            // 最後の要素とスワップして削除をO(1)に
            var lastIndex = _count - 1;
            if(index != lastIndex)
            {
                var lastMonoId = _dataArray[lastIndex].EntityId;
                _dataArray[index] = _dataArray[lastIndex];
                _monoIdToIndex[lastMonoId] = index;

                // アクティブリストも更新
                for(var i = 0; i < _activeCount; i++)
                {
                    if(_activeIndices[i] == lastIndex)
                    {
                        _activeIndices[i] = index;
                        break;
                    }
                }
            }

            _monoIdToIndex[entityId] = -1;
            _count--;

            // アクティブリストから削除
            for(var i = 0; i < _activeCount; i++)
            {
                if(_activeIndices[i] == index || _activeIndices[i] == lastIndex)
                {
                    _activeIndices[i] = _activeIndices[_activeCount - 1];
                    _activeCount--;
                    break;
                }
            }

            OnDataRemoved.Invoke(entityId);
        }

        /// <summary>
        /// データを取得（O(1)）
        /// </summary>
        public static ref TData GetData(int monoId)
        {
            var index = _monoIdToIndex[monoId];
            if(index < 0 || index >= _count)
            {
                throw new ArgumentException($"Data not found for MonoId: {monoId}");
            }

            return ref _dataArray[index];
        }

        /// <summary>
        /// データを取得（安全版）
        /// </summary>
        public static bool TryGetData(int monoId, out TData data)
        {
            var index = _monoIdToIndex[monoId];
            if(index >= 0 && index < _count)
            {
                data = _dataArray[index];
                return true;
            }

            data = default;
            return false;
        }

        /// <summary>
        /// データが存在するかチェック
        /// </summary>
        public static bool HasData(int monoId)
        {
            var index = _monoIdToIndex[monoId];
            return index >= 0 && index < _count;
        }

        /// <summary>
        /// アクティブなデータを更新（キャッシュフレンドリーなループ）
        /// </summary>
        public static void UpdateActiveData(UpdateDataDelegate<TData> updateAction)
        {
            // アクティブなインデックスのみをループ（連続メモリアクセス）
            for(var i = 0; i < _activeCount; i++)
            {
                var index = _activeIndices[i];
                if(index < _count && _dataArray[index].IsActive)
                {
                    updateAction(_dataArray[index].EntityId, ref _dataArray[index]);
                }
            }
        }

        /// <summary>
        /// Enumerates only active data for foreach usage.
        /// </summary>
        public static IEnumerable<TData> GetAllDataReadOnly()
        {
            for(var i = 0; i < _activeCount; i++)
            {
                var index = _activeIndices[i];
                if(index < _count && _dataArray[index].IsActive)
                {
                    yield return _dataArray[index];
                }
            }
        }

        /// <summary>
        /// すべてのデータを取得（読み取り専用）
        /// </summary>
        public static TData[] GetAllDataAsCopied()
        {
            var result = new TData[_count];
            Array.Copy(_dataArray, result, _count);
            return result;
        }

        /// <summary>
        /// アクティブなデータの数を取得
        /// </summary>
        public static int ActiveCount => _activeCount;

        /// <summary>
        /// データの総数を取得
        /// </summary>
        public static int Count => _count;

        /// <summary>
        /// IsActiveフラグを設定
        /// </summary>
        public static void SetActive(int monoId, bool isActive)
        {
            if(TryGetData(monoId, out var data))
            {
                data.IsActive = isActive;
                var index = _monoIdToIndex[monoId];
                _dataArray[index] = data;

                // アクティブリストの更新
                if(isActive)
                {
                    // 追加（重複チェック）
                    var exists = false;
                    for(var i = 0; i < _activeCount; i++)
                    {
                        if(_activeIndices[i] == index)
                        {
                            exists = true;
                            break;
                        }
                    }

                    if(!exists)
                    {
                        if(_activeCount >= _activeIndices.Length)
                        {
                            Array.Resize(ref _activeIndices, _activeIndices.Length * 2);
                        }

                        _activeIndices[_activeCount] = index;
                        _activeCount++;
                    }
                }
                else
                {
                    // 削除
                    for(var i = 0; i < _activeCount; i++)
                    {
                        if(_activeIndices[i] == index)
                        {
                            _activeIndices[i] = _activeIndices[_activeCount - 1];
                            _activeCount--;
                            break;
                        }
                    }
                }
            }
        }

        private static void Resize()
        {
            var newCapacity = _capacity * 2;
            Array.Resize(ref _dataArray, newCapacity);
            _capacity = newCapacity;
        }

        private static void Dispose()
        {
            OnDataAdded = delegate { };
            OnDataRemoved = delegate { };
            OnDataRemoving = delegate { };

            _count = 0;
            _activeCount = 0;

            // 配列をクリア
            Array.Clear(_dataArray, 0, _dataArray.Length);
            for(var i = 0; i < _monoIdToIndex.Length; i++)
            {
                _monoIdToIndex[i] = -1;
            }

            Debug.Log($"CompGroupData: {typeof(TData).Name} Cleared");
        }
    }
}
