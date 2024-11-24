using System.Text;
using ConsoleAppFramework;
using Cysharp.Diagnostics;
using Zx;

var app = ConsoleApp.Create();
app.Add<UnityHubCli>();
app.Run(args);


public class UnityHubCli
{
    /// <summary>
    /// インストールされているかチェック
    /// 実行失敗かインストールされていない場合は exitCode = 1 。
    /// </summary>
    /// <param name="targetVersion">-v, チェック対象の UnityEditor のバージョン</param>
    public async Task<int> Check(string targetVersion)
    {
        try
        {
            var unityHubPath = GetUnityHubPath();
            
            // 無いならなにもできないのでおしまい
            // (これもWinGetとかでインストールしてしまえばいいが）
            if (!File.Exists(unityHubPath))
            {
                throw new ProcessErrorException(1, ["Unity Hub not found."]);
            }
            
            // コマンドを構築
            var commandStringBuilder = new StringBuilder();
            commandStringBuilder
                .Append(EncloseInQuotes(unityHubPath))
                .Append(" -- --headless editors --installed");
            Console.WriteLine($"Check command: {commandStringBuilder}");

            var result = await commandStringBuilder.ToString();

            var installedEditorVersionList = result.Split(Environment.NewLine).Select(x => x.Split(',')[0].Trim());
            if (installedEditorVersionList.Any(ver => string.Equals(targetVersion, ver)))
            {
                Console.WriteLine("Installed");
                return 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        Console.WriteLine("Not installed");
        return 1;
    }

    /// <summary>
    /// インストール
    /// </summary>
    /// <param name="targetVersion">-v, インストールする UnityEditor のバージョン</param>
    public async Task<int> Install(string targetVersion)
    {
        try
        {
            var unityHubPath = GetUnityHubPath();

            // 無いならなにもできないのでおしまい
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
            commandStringBuilder
                .Append(EncloseInQuotes(unityHubPath))
                .Append(" -- --headless install")
                .Append($" --version {targetVersion}")
                .Append($" --changeset `unity-changeset` {targetVersion}");
            Console.WriteLine($"Install command: {commandStringBuilder}");

            // 実行すると通常はUACでダイアログ操作が必要だが、
            // CI用のVMなどUACが無効化されて管理者として動作するのであれば不要なはず
            await commandStringBuilder.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return 0;
    }

    private static string GetUnityHubPath()
    {
        var os = Environment.OSVersion;
        var pid = os.Platform;

        return pid switch 
        {
            PlatformID.Win32NT => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Unity Hub/Unity Hub.exe"),
            PlatformID.MacOSX => @"/Applications/Unity\ Hub.app/Contents/MacOS/Unity\ Hub",
            PlatformID.Unix => "~/Applications/Unity\\ Hub.AppImage",
            _ => throw new ProcessErrorException(1, ["Platform not support."])
        } ;
    }

    /// <summary>
    /// ダブルコーテーションで包むだけ。
    /// 特定の実行ファイルパスを想定。
    /// </summary>
    private static string EncloseInQuotes(string filePath)
    {
        return $"\"{filePath}\"";
    }

}
