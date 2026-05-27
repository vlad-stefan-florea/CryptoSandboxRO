using System.Diagnostics;
using System.Runtime.InteropServices;
using Spectre.Console;

namespace CryptoSandbox.Courses
{
    public class Utils
    {
        public static void Pause()
        {
            AnsiConsole.Markup("[bold green]APASĂ ORICE PENTRU A CONTINUA[/]");
            Console.ReadKey();
            AnsiConsole.Cursor.MoveUp(1);
            AnsiConsole.Write("\r" + new string(' ', Console.WindowWidth) + "\r\n");
        }

        public static void CopyToClipboard(string text)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // for Windows
                var powershell = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = $"-Command \"Set-Clipboard -Value '{text}'\"",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                };
                Process.Start(powershell);
            }
        }
    }
}
