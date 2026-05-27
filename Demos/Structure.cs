using Spectre.Console;
using static CryptoSandbox.Courses.Utils;

namespace CryptoSandbox.Courses
{
    public class Structure
    {
        public static void Run()
        {
            var app = new Tree("[bold underline blue]CryptoSandbox[/]")
                .Guide(TreeGuide.BoldLine)
                .Style(Style.Parse("dim"));

            var primes = app.AddNode("[cyan underline]Factorizarea numerelor prime[/]");
            var cryptography = app.AddNode("[cyan underline]Criptarea mesajelor[/]");
            var nfc = app.AddNode("[cyan underline]Handshake Securizat (Simulare Plată NFC)[/]");
            var pqc = app.AddNode("[cyan underline]PQC Demo[/]");

            primes.AddNodes("Factorizarea - experiment", "Spargerea numerelor", "RSA");
            cryptography.AddNodes("Criptarea simetrică", "Criptarea asimetrică", "Algoritmii HASH");
            pqc.AddNodes("LWE - experiment", "LWE Brute-Force");
            AnsiConsole.Write(app);
            Pause();
        }
    }
}
