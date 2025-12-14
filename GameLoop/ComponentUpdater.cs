using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NipaGameKit
{
    /// <summary>
    /// コンポーネントの更新を管理
    /// Systemベースのアプローチで、すべてのSystemを登録・管理
    /// </summary>
    public class ComponentUpdater : GameLoopBase
    {
        private void Awake()
        {
            // すべてのSystemを初期化・登録
            var moveCompSystem = new MoveCompSystem();
            CompSystemManager.Instance.RegisterSystem<MoveCompData>(moveCompSystem);
            
            // 他のSystemもここに追加
            // var coreCompSystem = new CoreCompSystem();
            // CompSystemManager.Instance.RegisterSystem<CoreCompData>(coreCompSystem);
        }

        public override void UpdateGameloop(float time, float deltaTime)
        {
            // CompSystemManagerが自動的にすべてのSystemを更新
            // このメソッドは空でも良いが、カスタム更新が必要な場合はここに追加
        }
    }
}
