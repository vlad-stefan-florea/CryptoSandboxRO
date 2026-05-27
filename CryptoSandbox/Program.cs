using System.Runtime.InteropServices;
using System.Text;
using CryptoSandbox.Demos;
using Spectre.Console;
using static CryptoSandbox.Demos.Utils;

namespace CryptoSandbox
{
    internal class Program
    {
        static void Titles()
        {
            var figlet = new FigletText("CryptoSandbox")
            {
                Color = Color.GreenYellow,
                Justification = Justify.Left,
            };
            var rule = new Rule()
            {
                Title = "Meniu de selecție demonstrații",
                Style = Style.Parse("#00ff22"),
                Justification = Justify.Left,
            };
            AnsiConsole.Write(figlet);

            string githubLink = "https://github.com/vlad-stefan-florea";
            AnsiConsole.MarkupLine($"Creat de: [link={githubLink}]@vlad-stefan-florea[/]");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                AnsiConsole.MarkupLine(
                    "[grey]Pentru utilizatorii WINDOWS: pentru o experiență optimă, alege din setări „Windows Terminal” ca terminal implicit.[/]"
                );
            AnsiConsole.Write(rule);
        }

        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            bool exit = false;
            try
            {
                while (!exit)
                {
                    AnsiConsole.Clear();
                    Titles();
                    var demo = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[yellow bold]Selectează un scenariu demo:[/]")
                            .PageSize(6)
                            .HighlightStyle(Color.Magenta)
                            .MoreChoicesText(
                                "[grey](Folosește tastele-săgeți pentru a naviga în meniu)[/]"
                            )
                            .AddChoices(
                                "Factorizarea numerelor prime",
                                "Criptarea mesajelor",
                                "Handshake Securizat (Simulare Plată NFC)",
                                "PQC Demo",
                                "Structura testelor",
                                "Închide aplicația"
                            )
                            .AddCancelResult("none")
                    );
                    switch (demo)
                    {
                        case "Factorizarea numerelor prime":
                            Primes.Run();
                            break;
                        case "Criptarea mesajelor":
                            CryptoPlayground.Run();
                            break;
                        case "Handshake Securizat (Simulare Plată NFC)":
                            NfcSim.Run();
                            break;
                        case "PQC Demo":
                            PQC.Run();
                            break;
                        case "Structura testelor":
                            Structure.Run();
                            break;
                        case "Închide aplicația":
                            exit = true;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[red bold]HOPA! Se pare că aplicația nu a funcționat corect:[/]")
                        .PageSize(3)
                        .HighlightStyle(Color.Red)
                        .MoreChoicesText(
                            "[grey](Folosește tastele-săgeți pentru a naviga în meniu)[/]"
                        )
                        .AddChoices("Afișează mesajul de eroare", "Închide aplicația")
                        .AddCancelResult("none")
                );
                switch (choice)
                {
                    case "Afișează mesajul de eroare":
                        AnsiConsole.WriteException(ex);
                        Pause();
                        break;
                    case "Închide aplicația":
                        exit = true;
                        break;
                    default:
                        exit = true;
                        break;
                }
            }
            finally
            {
                AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.BluePulse)
                    .Start(
                        "[blue bold]Se închide...[/]",
                        ctx =>
                        {
                            Thread.Sleep(1000);
                        }
                    );
            }
        }
    }
}
