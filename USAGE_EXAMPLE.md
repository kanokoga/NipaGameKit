# 新しいシステムの使用方法

## MoveCompの使い方

### 1. GameObjectのセットアップ

1. **GameObjectを作成**
2. **`NipaMono`コンポーネントを追加**
   - これがエントリーポイントで、すべての`CompDataProvider`を自動検出・初期化します
3. **`MoveCompProvider`コンポーネントを追加**
   - Inspectorで`Speed`を設定可能
   - 実際の移動処理は`MoveCompSystem`が行います

### 2. Systemの登録（シーンに1回だけ）

シーン内のどこかに`ComponentUpdater`を持つGameObjectを作成します：

```csharp
// ComponentUpdaterは既に実装済み
// シーンにGameObjectを作成し、ComponentUpdaterをアタッチ
// Awakeで自動的にMoveCompSystemが登録されます
```

または、手動で登録する場合：

```csharp
// どこかで1回だけ実行（例：GameManagerなど）
var moveCompSystem = new MoveCompSystem();
CompSystemManager.Instance.RegisterSystem<MoveCompData>(moveCompSystem);
```

### 3. コードから使用する

#### 目的地を設定して移動

```csharp
// MoveCompProviderを取得
var moveComp = GetComponent<MoveCompProvider>();

// 目的地を設定（自動的に移動開始）
moveComp.SetDestination(new Vector3(10, 0, 10));

// 停止
moveComp.Stop();
```

#### データに直接アクセス

```csharp
var moveComp = GetComponent<MoveCompProvider>();

// データを取得（参照で変更可能）
ref var data = ref moveComp.GetData();

// 直接データを変更
data.Destination = new Vector3(5, 0, 5);
data.Speed = 10.0f;
data.HasArrived = false;
```

#### MonoIdからデータにアクセス

```csharp
// 他のスクリプトから、MonoIdでデータにアクセス
int targetMonoId = 123;

if (CompGroupData<MoveCompData>.TryGetData(targetMonoId, out MoveCompData data))
{
    // データを読み取り
    Vector3 destination = data.Destination;
    bool hasArrived = data.HasArrived;
}

// または、参照で変更
if (CompGroupData<MoveCompData>.HasData(targetMonoId))
{
    ref var data = ref CompGroupData<MoveCompData>.GetData(targetMonoId);
    data.Destination = new Vector3(10, 0, 10);
}
```

#### すべてのMoveCompを処理

```csharp
// すべてのMoveCompDataを取得
MoveCompData[] allMoveComps = CompGroupData<MoveCompData>.GetAllData();

foreach (var data in allMoveComps)
{
    if (data.IsActive && !data.HasArrived)
    {
        // 処理
    }
}
```

### 4. Systemのカスタマイズ

`MoveCompSystem`を継承してカスタムロジックを追加：

```csharp
public class CustomMoveCompSystem : MoveCompSystem
{
    protected override void UpdateData(int monoId, ref MoveCompData data, float time, float deltaTime)
    {
        // 基底クラスの処理を実行
        base.UpdateData(monoId, ref data, time, deltaTime);
        
        // カスタム処理を追加
        if (data.Velocity.magnitude > 10.0f)
        {
            // 速度制限など
            data.Velocity = data.Velocity.normalized * 10.0f;
        }
    }
}
```

### 5. 新しいコンポーネントの作成例

#### ステップ1: データ構造を定義

```csharp
public struct HealthCompData : ICompData
{
    public int MonoId { get; set; }
    public bool IsActive { get; set; }
    
    public float MaxHealth;
    public float CurrentHealth;
    public bool IsDead;
}
```

#### ステップ2: Systemを実装

```csharp
public class HealthCompSystem : CompSystem<HealthCompData>
{
    protected override void UpdateData(int monoId, ref HealthCompData data, float time, float deltaTime)
    {
        if (data.CurrentHealth <= 0 && !data.IsDead)
        {
            data.IsDead = true;
            // 死亡処理など
        }
    }
}
```

#### ステップ3: データプロバイダーを実装

```csharp
public class HealthCompProvider : CompDataProvider<HealthCompData>
{
    [SerializeField] private float maxHealth = 100.0f;

    protected override HealthCompData CreateData(int monoId)
    {
        return new HealthCompData
        {
            MonoId = monoId,
            IsActive = enabled,
            MaxHealth = maxHealth,
            CurrentHealth = maxHealth,
            IsDead = false
        };
    }

    public void TakeDamage(float damage)
    {
        ref var data = ref GetData();
        data.CurrentHealth = Mathf.Max(0, data.CurrentHealth - damage);
    }
}
```

#### ステップ4: Systemを登録

```csharp
// ComponentUpdaterのAwakeで
var healthCompSystem = new HealthCompSystem();
CompSystemManager.Instance.RegisterSystem<HealthCompData>(healthCompSystem);
```

### 6. パフォーマンスのメリット

- **連続メモリアクセス**: データが配列で保持されるため、キャッシュ効率が良い
- **バッチ処理**: Systemがすべてのデータを一括処理
- **O(1)アクセス**: MonoIdから直接データにアクセス可能

### 7. 注意点

- `NipaMono`は必ずGameObjectにアタッチする必要があります
- `CompDataProvider`は`NipaMono`と同じGameObjectにアタッチする必要があります
- Systemはシーンに1回だけ登録してください（`ComponentUpdater`で自動管理）

