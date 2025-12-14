# コマンドキューシステムの使用方法

## 概要

コマンドキューシステムにより、複数の移動命令を順番に実行できます。命令は上書きされず、キューに蓄積されて順番に処理されます。

## 基本的な使い方

### 1. 移動命令を出す

```csharp
// 基本的な移動命令
MoveCommandQueue.EnqueueMove(monoId, new Vector3(10, 0, 10));

// 速度を指定した移動命令
MoveCommandQueue.EnqueueMove(monoId, new Vector3(10, 0, 10), speed: 8.0f);

// 複数の移動命令を順番に実行
MoveCommandQueue.EnqueueMove(monoId, new Vector3(5, 0, 5));
MoveCommandQueue.EnqueueMove(monoId, new Vector3(10, 0, 10));
MoveCommandQueue.EnqueueMove(monoId, new Vector3(15, 0, 15));
// → 順番に実行される
```

### 2. 停止命令を出す

```csharp
MoveCommandQueue.EnqueueStop(monoId);
```

### 3. MoveCompProviderから使用

```csharp
var moveComp = GetComponent<MoveCompProvider>();

// 移動命令（コマンドキューに追加）
moveComp.SetDestination(new Vector3(10, 0, 10));

// 速度指定付き
moveComp.SetDestination(new Vector3(10, 0, 10), speed: 8.0f);

// 停止命令
moveComp.Stop();
```

## コマンドキューの動作

1. **コマンドの追加**: `EnqueueMove()`や`EnqueueStop()`でコマンドをキューに追加
2. **コマンドの処理**: `MoveCompSystem`の`Update()`で自動的に処理される
3. **順次実行**: キューに入った順番に1つずつ処理される
4. **到着判定**: 1つのコマンドが完了（到着）すると、次のコマンドが自動的に開始される

## コマンドキューの状態確認

```csharp
// キューサイズを確認
int queueSize = CommandQueue<CommandQueue.MoveCommand>.Count;

// キューをクリア（全コマンドを破棄）
CommandQueue<CommandQueue.MoveCommand>.Clear();
```

## 使用例

### パトロールルートの設定

```csharp
// パトロールポイントを順番に設定
int guardMonoId = 123;
MoveCommandQueue.EnqueueMove(guardMonoId, new Vector3(0, 0, 0));
MoveCommandQueue.EnqueueMove(guardMonoId, new Vector3(10, 0, 0));
MoveCommandQueue.EnqueueMove(guardMonoId, new Vector3(10, 0, 10));
MoveCommandQueue.EnqueueMove(guardMonoId, new Vector3(0, 0, 10));
MoveCommandQueue.EnqueueMove(guardMonoId, new Vector3(0, 0, 0));  // 開始地点に戻る
```

### 緊急停止

```csharp
// 現在のコマンドをキャンセルして停止
CommandQueue<CommandQueue.MoveCommand>.Clear();  // キューをクリア
MoveCommandQueue.EnqueueStop(monoId);  // 停止命令
```

### 速度変更付き移動

```csharp
// 通常速度で移動
MoveCommandQueue.EnqueueMove(monoId, new Vector3(5, 0, 5));

// 高速で移動
MoveCommandQueue.EnqueueMove(monoId, new Vector3(10, 0, 10), speed: 15.0f);

// 低速で移動
MoveCommandQueue.EnqueueMove(monoId, new Vector3(15, 0, 15), speed: 2.0f);
```

## メリット

✅ **コマンドの上書きなし**: 新しい命令を出しても、前の命令が失われない  
✅ **順次実行**: 複数の命令を順番に実行可能  
✅ **キューイング**: 命令を蓄積して処理可能  
✅ **柔軟性**: 速度やその他のパラメータを命令ごとに指定可能

## 注意点

- コマンドは`MoveCompSystem`の`Update()`で処理されるため、1フレームに1つずつ処理されます
- キューサイズの上限は100（`CommandQueue<TCommand>.MaxQueueSize`で変更可能）
- キューが満杯の場合、新しいコマンドは破棄されます

