# ECS Design Guide

このドキュメントは `Assets/Packages/NipaGameKit/ECS` の設計方針と実装ルールをまとめたものです。  
目的は「少ない抽象で高速に回るECS」を維持しながら、運用時の事故を減らすことです。

## 1. コアコンセプト

- **Data Oriented**: データは連続メモリに寄せ、`for` ループで順次走査する
- **SoA (Structure of Arrays)**: `Chunk` 内でコンポーネントを型別配列として保持する
- **System Driven**: 振る舞いはすべて `ComponentSystem` 側で更新する
- **Order Matters**: `World` に登録したシステム順に実行される

## 2. モデル構造

### Entity
- Entity は明示クラスを持たない
- 「同じインデックスにある複数コンポーネントの集合」を Entity とみなす

### Component
- コンポーネントは `struct` を前提とする
- 参照型フィールドは必要最小限にする（GC負荷と追跡コスト増加を招くため）

### Chunk
- `Chunk(capacity, types...)` でアーキタイプを定義する
- 追加時は `AddEntity()` でインデックスを確保し、各配列の同一インデックスへ書き込む
- 削除は `RemoveAt(index)` の swap-back を使い、配列を常に密に保つ

### System
- `Filter(Chunk)` で対象アーキタイプを選定する
- `Update(deltaTime)` は対象チャンクを順次走査する
- 処理は「読むデータ」「書くデータ」を意識して責務分割する

### World
- `AddSystem` でシステムを登録する
- `CreateChunk` でチャンクを生成し、既存システムへ自動登録する
- `Update(deltaTime)` は登録順で各システムを実行する

## 3. 実装ルール

1. **データとロジックの分離**
   - データは `struct`
   - ロジックは `ComponentSystem` 派生クラス

2. **Chunkアクセス最適化**
   - `GetArray<T>()` はループ前で1回だけ取得する
   - ループ中に Dictionary 参照や型解決を繰り返さない

3. **削除時の走査方向**
   - `RemoveAt` があり得るループは `for (i = Count - 1; i >= 0; i--)`
   - 削除しないループは前進ループでよい

4. **ランダムアクセス最小化**
   - Entity ID 検索を多用せず、同一インデックスで同時更新する
   - 必要なら別途インデックステーブルを設計する

5. **システム責務の分離**
   - 例: `MovementSystem` / `ScanSystem` / `WeaponSystem` / `AISystem`
   - 1システムに複数責務を詰め込みすぎない

6. **更新順の明示**
   - システム間依存は登録順で表現する
   - 推奨例: Input -> AI -> Movement -> Scan -> Weapon -> Presentation

## 4. 現実装の安全ガード

現在のECS実装には次の防御ロジックを入れている。

- `Chunk`:
  - `capacity <= 0` を拒否
  - コンポーネント型未指定を拒否
  - `null` 型・重複型・`struct` 以外の型を拒否
  - 存在しない型に対する `GetArray<T>()` を明確な例外で通知

- `ComponentSystem`:
  - `null` チャンク登録を拒否
  - 同一チャンクの重複登録を抑止

- `World`:
  - `null` システム登録を拒否
  - `Chunks` と `Systems` の読み取り専用ビューを公開

## 5. 運用上の注意

- `Chunk.RemoveAt` は swap-back のため、削除された要素の順序は維持されない
- 見た目やイベントの順序が必要な処理は別のキーで管理する
- 長時間実行でGCを抑えるには、参照型コンポーネントの利用を減らす
- `Filter` が広すぎると不要チャンクまで対象になるため、必要コンポーネントを明示する

## 6. 最小サンプル

```csharp
public class MoveSystem : ComponentSystem
{
    protected override bool Filter(Chunk chunk)
    {
        return chunk.HasComponent<BodyData>() && chunk.HasComponent<MoveData>();
    }

    public override void Update(float deltaTime)
    {
        foreach(var chunk in this.TargetChunks)
        {
            var bodies = chunk.GetArray<BodyData>();
            var moves = chunk.GetArray<MoveData>();
            for(var i = 0; i < chunk.Count; i++)
            {
                ref var body = ref bodies[i];
                ref var move = ref moves[i];
                if(move.IsMoving == false)
                {
                    continue;
                }

                var dir = (move.Destination - body.Position).normalized;
                body.Position += dir * (deltaTime * 2f);
            }
        }
    }
}
```

## 7. 今後の拡張候補

- システムの有効/無効切替
- システムグループ（Simulation / Presentation）の導入
- Chunkの型マスク化による `Filter` 高速化
- Entity生成/破棄のコマンドバッファ化