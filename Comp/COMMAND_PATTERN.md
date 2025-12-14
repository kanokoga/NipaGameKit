# コマンドパターンとSOA構造

## 現在の実装の問題点

現在の`MoveCompData`では、`Destination`が実質的に「移動命令」として機能しています：

```csharp
public struct MoveCompData : ICompData
{
    public Vector3 Destination;  // これが実質的に「命令」
    public bool HasArrived;       // 状態
    public Vector3 Velocity;     // 状態
    public float Speed;          // 設定値
}
```

### 問題点

1. **コマンドの上書き**: 新しい命令を出すと、前の命令が失われる
2. **コマンドのキューイング不可**: 複数の命令を順番に実行できない
3. **コマンドの取り消し不可**: 一度出した命令を取り消せない
4. **状態と命令の混在**: データ構造が「現在の状態」と「実行したい命令」を混在させている

## 理想的なアプローチ

### アプローチ1: コマンドキュー（推奨）

状態とコマンドを分離し、コマンドをキューで管理：

```csharp
// 状態データ（現在の状態のみ）
public struct MoveCompData : ICompData
{
    public int MonoId { get; set; }
    public bool IsActive { get; set; }
    
    // 状態のみ
    public Vector3 CurrentDestination;
    public bool HasArrived;
    public Vector3 Velocity;
    public float Speed;
}

// コマンドデータ（実行したい命令）
public struct MoveCommand
{
    public int MonoId;
    public Vector3 Destination;
    public CommandPriority Priority;
    public float Timestamp;
}

// コマンドキュー（別のデータ構造で管理）
public static class MoveCommandQueue
{
    private static Queue<MoveCommand> _commandQueue = new Queue<MoveCommand>();
    
    public static void Enqueue(MoveCommand command)
    {
        _commandQueue.Enqueue(command);
    }
    
    public static bool TryDequeue(out MoveCommand command)
    {
        return _commandQueue.TryDequeue(out command);
    }
}
```

### アプローチ2: コマンドフラグ（シンプル）

状態データにコマンドフラグを追加：

```csharp
public struct MoveCompData : ICompData
{
    public int MonoId { get; set; }
    public bool IsActive { get; set; }
    
    // 状態
    public Vector3 CurrentDestination;
    public bool HasArrived;
    public Vector3 Velocity;
    public float Speed;
    
    // コマンド（Systemが処理したらクリア）
    public bool HasMoveCommand;
    public Vector3 MoveCommandDestination;
}

// Systemで処理
protected override void UpdateData(int monoId, ref MoveCompData data, float time, float deltaTime)
{
    // コマンドを処理
    if (data.HasMoveCommand)
    {
        data.CurrentDestination = data.MoveCommandDestination;
        data.HasArrived = false;
        data.HasMoveCommand = false;  // コマンドをクリア
    }
    
    // 通常の更新処理...
}
```

### アプローチ3: イベントベース（柔軟）

コマンドをイベントとして発行：

```csharp
// コマンドイベント
public struct MoveCommandEvent
{
    public int MonoId;
    public Vector3 Destination;
    public CommandSource Source;
}

// イベントシステム
public static class CommandEventBus
{
    private static List<MoveCommandEvent> _pendingCommands = new List<MoveCommandEvent>();
    
    public static void Publish(MoveCommandEvent command)
    {
        _pendingCommands.Add(command);
    }
    
    public static void ProcessCommands()
    {
        foreach (var command in _pendingCommands)
        {
            // コマンドを処理
            if (CompGroupData<MoveCompData>.HasData(command.MonoId))
            {
                ref var data = ref CompGroupData<MoveCompData>.GetData(command.MonoId);
                data.Destination = command.Destination;
                data.HasArrived = false;
            }
        }
        _pendingCommands.Clear();
    }
}
```

## 推奨アプローチ

**用途に応じて選択**：

1. **シンプルなケース**: アプローチ2（コマンドフラグ）
   - 1つのコマンドのみで十分
   - 実装が簡単
   - パフォーマンスが良い

2. **複雑なケース**: アプローチ1（コマンドキュー）
   - 複数のコマンドを順番に実行したい
   - コマンドの優先順位が必要
   - コマンドの履歴が必要

3. **柔軟性が必要**: アプローチ3（イベントベース）
   - 複数のSystemがコマンドを処理する必要がある
   - コマンドの監視・ログが必要

## 現在の実装の評価

現在の実装（`Destination`に直接書き込む）は：

✅ **メリット**:
- シンプルで理解しやすい
- パフォーマンスが良い（直接書き込み）
- 実装が簡単

❌ **デメリット**:
- コマンドの上書きが発生
- コマンドの履歴が残らない
- 複雑なコマンド処理が難しい

**結論**: シンプルな移動システムであれば現在の実装で十分ですが、より複雑な要件がある場合はコマンドキューやイベントシステムを検討すべきです。

