using System.Text;
using ConsoleAppFramework;
using Cysharp.Diagnostics;
using Zx;

await ConsoleApp.RunAsync(args, MainAsync);


/// <summary>
/// Main
/// </summary>
/// <param name="targetVersion">-v, 使用する予定のUnityEditorのバージョン</param>
async Task<int> MainAsync(string targetVersion)
{
    var programFilesDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
    var unityHubPath = Path.Combine(programFilesDir, "Unity Hub/Unity Hub.exe");

    try
    {
        // 無いならそもそもインストールできないのでおしまい
        // (これもWinGetとかでインストールしてしまえばいいが）
        if (!File.Exists(unityHubPath))
        {
            throw new ProcessErrorException(1, ["Unity Hub not found."]);
        }

        // 基本 Unity Hub の Releases にない Editor バージョンなので、ChangeSetを取得する必要があるため
        // https://github.com/mob-sakai/unity-changeset を利用して取得する
        await "npm install unity-changeset";
        
        // インストール用のコマンドを構築
        var commandStringBuilder = new StringBuilder();
        commandStringBuilder.Append(EncloseInQuotes(unityHubPath));
        commandStringBuilder.Append(" -ArgumentList \"");
        commandStringBuilder.Append(" -- --headless install");
        commandStringBuilder.Append($" --version {targetVersion}");
        commandStringBuilder.Append($" --changeset `unity-changeset` {targetVersion}");
        commandStringBuilder.Append(" --silent\" -Wait -PassThru");
        Console.WriteLine($"Install command: {commandStringBuilder}");
        
        // 実行すると通常はUACでダイアログ操作が必要だが、
        // CI用のVMなどUACが無効化されて管理者として動作するのであれば不要なはず
        await commandStringBuilder.ToString();
    }
    catch (ProcessErrorException ex)
    {
        Console.WriteLine(ex.ToString());
    }

    return 0;
}


/// <summary>
/// ダブルコーテーションで包むだけ。
/// 特定のパスの実行ファイルパスを想定しています。
/// </summary>
string EncloseInQuotes(string filePath)
{
    return $"\"{filePath}\"";
}
