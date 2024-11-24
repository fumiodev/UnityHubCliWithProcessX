# UnityHubCliWithProcessX

## 使用方法

> [!NOTE] 
> UnityHub 本体のインストールは未実装

```bash
# インストール済みかチェック
dotnet run check --target-version  6000.0.23f1
```

```bash
# 指定バージョンのインストール
dotnet run install -v 6000.0.23f1
```

```bash
# ヘルプ表示
dotnet run --
````

## Dependencies

- [Cysharp/ConsoleAppFramework](https://github.com/Cysharp/ConsoleAppFramework) 
- [Cysharp/ProcessX](https://github.com/Cysharp/ProcessX)
- [mob-sakai/unity-changeset](https://github.com/mob-sakai/unity-changeset)

## 参考

- [UnityHub CLI](https://docs.unity3d.com/hub/manual/HubCLI.html)