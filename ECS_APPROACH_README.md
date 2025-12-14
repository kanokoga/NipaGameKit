# ECS的アプローチの実装（破壊的変更版）

このドキュメントでは、Unity ECSパッケージを使わずに、ECSの設計思想を取り入れた**純粋なデータ構造ベース**のコンポーネントシステムについて説明します。

**注意**: この実装は既存の`CompMonoBase`や`CompGroup`を完全に置き換える破壊的変更です。

## 設計思想

### 1. データとロジックの分離
- **データ**: 構造体（struct）で定義し、連続メモリに配置
- **ロジック**: Systemクラスで一括処理

### 2. SoA（Structure of Arrays）構造
- データを配列で保持することで、キャッシュフレンドリーなアクセスを実現
- メモリアクセスの局所性を向上

### 3. パフォーマンス最適化
- 連続メモリアクセスによるキャッシュミスの削減
- アクティブなデータのみをループ
- O(1)の追加・削除・取得

## アーキテクチャ

### コアクラス

#### `ICompData`
コンポーネントデータの基底インターフェース
```csharp
public interface ICompData
{
    int MonoId { get; set; }
    bool IsActive { get; set; }
}
```

#### `CompGroupData<TData>`
SoA構造でデータを保持する静的クラス
- データを配列で管理（連続メモリ）
- MonoIdからO(1)でデータにアクセス
- アクティブなデータのみを効率的にループ

#### `CompSystem<TData>`
データを処理するSystem基底クラス
- データの更新ロジックを実装
- バッチ処理でパフォーマンスを最適化

#### `CompDataBridge<TData, TComp>`
既存のCompMonoBaseと新しいデータ構造を統合するブリッジ
- 段階的な移行を可能にする
- 既存コードとの互換性を維持

## アーキテクチャの変更点

### 削除されたクラス
- `CompMonoBase` - データ構造ベースに完全移行
- `CompGroup<T>` - `CompGroupData<TData>`に置き換え
- `CompDataBridge` - 不要（直接`CompDataProvider`を使用）

### 新しく追加されたクラス
- `CompDataProvider<TData>` - データ構造を提供するMonoBehaviour基底クラス
- `CompRegistry` - MonoIdとUnityオブジェクト（Transform）のマッピング
- `CompGroupData<TData>` - SoA構造でデータを保持
- `CompSystem<TData>` - データを処理するSystem基底クラス
- `CompSystemManager` - Systemの管理と自動更新

## 使用例

### 1. データ構造の定義

```csharp
public struct MoveCompData : ICompData
{
    public int MonoId { get; set; }
    public bool IsActive { get; set; }
    
    public Vector3 Destination;
    public bool HasArrived;
    public Vector3 Velocity;
    public float Speed;
}
```

### 2. Systemの実装

```csharp
public class MoveCompSystem : CompSystem<MoveCompData>
{
    protected override void UpdateData(int monoId, ref MoveCompData data, float time, float deltaTime)
    {
        if (data.HasArrived || !data.IsActive) return;
        
        Vector3 currentPos = CompRegistry.GetPosition(monoId);
        Vector3 direction = (data.Destination - currentPos).normalized;
        data.Velocity = direction * data.Speed;
        
        // 位置を更新
        CompRegistry.SetPosition(monoId, currentPos + data.Velocity * deltaTime);
    }
}
```

### 3. データプロバイダーの実装

```csharp
public class MoveCompProvider : CompDataProvider<MoveCompData>
{
    [SerializeField] private float speed = 5.0f;

    protected override MoveCompData CreateData(int monoId)
    {
        return new MoveCompData
        {
            MonoId = monoId,
            IsActive = enabled,
            Destination = transform.position,
            HasArrived = true,
            Velocity = Vector3.zero,
            Speed = speed
        };
    }

    public void SetDestination(Vector3 destination)
    {
        ref var data = ref GetData();
        data.Destination = destination;
        data.HasArrived = false;
    }
}
```

### 4. Systemの登録

```csharp
// ComponentUpdaterのAwakeで登録
private void Awake()
{
    var moveCompSystem = new MoveCompSystem();
    CompSystemManager.Instance.RegisterSystem<MoveCompData>(moveCompSystem);
}
```

### 5. GameObjectのセットアップ

1. GameObjectに`NipaMono`コンポーネントを追加
2. 同じGameObjectに`MoveCompProvider`などのデータプロバイダーを追加
3. `NipaMono`が自動的にすべてのデータプロバイダーを検出して初期化

## パフォーマンス比較

### 従来のアプローチ（CompGroup）
- メモリアクセス: 散在（キャッシュミスが多い）
- 更新処理: O(n) × キャッシュミス
- メモリ使用量: Dictionary + List（2倍）

### 新しいアプローチ（CompGroupData）
- メモリアクセス: 連続（キャッシュフレンドリー）
- 更新処理: O(n) × キャッシュヒット
- メモリ使用量: 配列のみ（効率的）

## 移行ガイド（破壊的変更）

### 既存コードからの移行

1. **CompMonoBase → CompDataProvider**
   ```csharp
   // 旧
   public class MyComp : CompMonoBase { }
   
   // 新
   public class MyComp : CompDataProvider<MyCompData> { }
   ```

2. **CompGroup → CompGroupData**
   ```csharp
   // 旧
   CompGroup<MyComp>.GetComponent(monoId);
   
   // 新
   CompGroupData<MyCompData>.GetData(monoId);
   ```

3. **UpdateComponent → System**
   ```csharp
   // 旧: CompMonoBaseのUpdateComponent
   public override void UpdateComponent(float time, float deltaTime) { }
   
   // 新: CompSystemのUpdateData
   public class MyCompSystem : CompSystem<MyCompData>
   {
       protected override void UpdateData(int monoId, ref MyCompData data, float time, float deltaTime) { }
   }
   ```

4. **Transformアクセス**
   ```csharp
   // 旧
   this._transform.position
   
   // 新
   CompRegistry.GetPosition(monoId)
   CompRegistry.SetPosition(monoId, position)
   ```

## 注意事項

- データ構造は値型（struct）である必要がある
- 大きなデータ構造の場合は、参照型のフィールドを避ける
- Systemの更新順序に注意（依存関係がある場合）

## 今後の改善案

1. **SIMD命令の活用**
   - 大量のデータを並列処理

2. **Job Systemとの統合**
   - Unity Job Systemで並列処理

3. **メモリプール**
   - 頻繁な追加・削除を最適化

4. **プロファイリングツール**
   - パフォーマンス測定と可視化

