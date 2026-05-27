using System.Diagnostics;
using CryptoSandbox.Engine;
using Spectre.Console;
using static CryptoSandbox.Courses.Utils;
using static CryptoSandbox.Engine.CryptoEngine;

namespace CryptoSandbox.Courses
{
    public class CryptoPlayground
    {
        private static void introduction() =>
            AnsiConsole.MarkupLine(
                "[cyan bold]Explicație[/]:"
                    + "\n\t[blue]• Ce avem[/]:"
                    + " un mesaj (text) și [magenta]chei de criptare[/] (publice și private)"
                    + "\n\t[blue]• Ce facem[/]:"
                    + " vom cripta/altera mesajul folosind criptarea [magenta]simetrică[/] și cea [magenta]asimetrică[/]"
                    + "\n\t[red]• Problema[/]:"
                    + " pentru că vom folosi diverse metode de criptare, vom vedea că rolul unei chei\n\t            poate varia în funcție de situație"
            );

        private static void keypoints() =>
            AnsiConsole.MarkupLine(
                "[cyan bold]Puncte cheie[/]:"
                    + "\n\t[blue]•[/] [white bold]Criptarea simetrică (AES)[/]"
                    + "\n\t[blue]•[/] [white bold]Criptarea Asimetrică & Semnătura (RSA)[/]"
                    + "\n\t[blue]•[/] [white bold]Hashing - Amprenta Digitală (SHA-256)[/]"
            );

        private static void FirstTest()
        {
            AnsiConsole.Clear();
            int minChars = 10,
                maxChars = 500;
            Markup contents = new Markup(
                "1) Ți se va cere un mesaj de [magenta]tip text[/] [red]între 10 și 500 de caractere[/] (litere, cifre, simboluri)."
                    + "\n2) Aplicația [magenta]va genera o cheie[/] de criptare simetrică, pe care o va folosi să modifice mesajul."
                    + "\n3) Folosind aceeași cheie, vei putea [magenta]să descifrezi mesajul[/]."
                    + "\n4) Orice altă cheie nu va funcționa și va rezulta un mesaj la fel de indescifrabil."
                    + "\n[cyan bold]SUCCES![/]"
            );
            var panel = new Panel(contents)
                .Header("TESTUL 1 - Criptarea Simetrică (AES)")
                .RoundedBorder()
                .BorderColor(Color.Aquamarine1)
                .Expand();
            AnsiConsole.Write(panel);
            Pause();

            // read msg
            string input = string.Empty,
                initialmsg = string.Empty;
            while (true)
            {
                input = AnsiConsole.Ask<string>($"> [bold white]Mesajul tău:[/]");
                initialmsg = input;
                if (string.IsNullOrEmpty(input))
                    AnsiConsole.MarkupLine("[red]Mesajul nu poate fi gol.[/]");
                else if (input.Length < minChars || input.Length > maxChars)
                    AnsiConsole.MarkupLine(
                        $"[red]Mesajul trebuie să aibă între {minChars} și {maxChars} caractere.[/]"
                    );
                else
                    break;
            }

            // generate and display keys and encrypted msg
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var output = EncryptSymmetric(input);
            string cipherText = output.CipherText,
                key = output.Key,
                iv = output.IV;
            sw.Stop();
            AnsiConsole.MarkupLine($"[yellow]Cheia simetrică:[/] [blue bold]{key}[/]");
            AnsiConsole.MarkupLine(
                "[green bold]✓ Cheie copiată automat în clipboard! Apasă[/] [yellow]CTRL+V[/] [green bold]pentru a insera.[/]"
            );
            CopyToClipboard(key);
            AnsiConsole.MarkupLine($"[yellow]Mesajul criptat:[/]\n\t[magenta]{cipherText}[/]");
            AnsiConsole.MarkupLine(
                $"[yellow]Criptat în:[/] [green]{sw.Elapsed.TotalMilliseconds:F4} ms[/]"
            );
            AnsiConsole.MarkupLine(
                "> [cyan bold]TASK-ul tău:[/] Încearcă să decriptezi mesajul introducând o cheie la întâmplare."
            );
            AnsiConsole.MarkupLine(
                "> [cyan bold]Există 2 cazuri posibile aici[/]:"
                    + "\n\t[magenta]1)[/] Cheia nu este într-un format valid"
                    + "\n\t[magenta]2)[/] Cheia nu este cea corectă"
            );
            AnsiConsole.MarkupLine(
                "> [cyan bold]SFAT[/]: Inserează cheia și modifică [magenta]o singură[/] [red bold]LITERĂ[/] pentru a evita erorile de format al cheii."
            );
            Pause();

            // manual test
            while (true)
            {
                input = AnsiConsole.Ask<string>($"> [bold white]Cheia aleatoare:[/] ");
                if (input == key)
                    AnsiConsole.MarkupLine(
                        "[red]Se pare că ai introdus aceeași cheie. Încearcă oricare altă combinație de caractere.[/]"
                    );
                else if (string.IsNullOrEmpty(input))
                    AnsiConsole.MarkupLine("[red]Cheia nu poate fi goală.[/]");
                else if (input.Length > maxChars)
                    AnsiConsole.MarkupLine(
                        $"[red]Cheia ar trebui să cel mult {maxChars} caractere.[/]"
                    );
                else
                    break;
            }
            var decrypted = DecryptSymmetric(cipherText, input.Replace(" ", ""), iv);
            AnsiConsole.MarkupLine(decrypted.msg);
            AnsiConsole.MarkupLine(
                "\n> Chiar și așa, probabilitatea ca o cheie simetrică să fie ghicită este de: "
                    + "[magenta bold]1[/][white bold]/[/][magenta bold](2^256)[/]"
            );
            AnsiConsole.MarkupLine(
                "> [blue]Acum vei asista la un test automat în care aplicația va încerca mai multe chei valide și la final pe cea corectă.[/]"
            );
            Pause();

            // auto test
            int tests = 10;

            var table = new Table()
                .Border(TableBorder.Rounded)
                .Expand()
                .AddColumn("[yellow]Nr.[/]")
                .AddColumn("[magenta]Cheia încercată[/]")
                .AddColumn("[cyan]Succes[/]");
            AnsiConsole
                .Live(table)
                .Start(ctx =>
                {
                    for (int c = 0; c < tests; c++)
                    {
                        string newkey = GenerateSampleBase64Key();
                        decrypted = DecryptSymmetric(cipherText, newkey, iv);
                        table.AddRow(
                            "[yellow]" + (c + 1).ToString() + "[/]",
                            newkey,
                            decrypted.error ? "❌" : "✅"
                        );
                        ctx.Refresh();
                    }
                    decrypted = DecryptSymmetric(cipherText, key, iv);
                    table.AddRow(
                        "[yellow]" + (tests + 1).ToString() + "[/]",
                        "[green]" + key + "[/]",
                        decrypted.error ? "❌" : "✅"
                    );
                    ctx.Refresh();
                });
            AnsiConsole.Write(
                new Panel($"[blue]{decrypted.msg}[/]")
                    .Header("[magenta]Mesajul decriptat[/]")
                    .Expand()
            );

            AnsiConsole.Write(
                new Panel($"[white]{initialmsg}[/]").Header("[magenta]Mesajul inițial[/]").Expand()
            );
            Pause();
        }

        private static void SecondTest()
        {
            AnsiConsole.Clear();
            int minChars = 10,
                maxChars = 500;
            Markup contents = new Markup(
                "1) Aplicația va alege un mesaj la întâmplare."
                    + "\n2) Aplicația va „semna” mesajul folosind o cheie privată."
                    + "\n3) Vei primi mesajul codificat și cheia publică."
                    + "\n4) Vei vedea că dacă modifici chiar și o singură literă din mesajul codificat, semnătura va fi invalidă."
                    + "\n[cyan bold]Așa poți afla dacă un mesaj a fost modificat de altă persoană înainte să ajungă la tine.[/]"
            );
            var panel = new Panel(contents)
                .Header("TESTUL 2 - Criptarea Asimetrică & Semnătura (RSA)")
                .RoundedBorder()
                .BorderColor(Color.Aquamarine1)
                .Expand();
            AnsiConsole.Write(panel);
            Pause();

            // read msg
            string msg = string.Empty;
            Random rand = new Random();
            string[] Messages =
            {
                "Certific faptul că acest mesaj vine de la aplicație și nu a fost modificat.",
                "Acest mesaj este unul autentic.",
                "Nimeni nu poate modifica acest mesaj fără cheia privată.",
                "Acesta este un mesaj care nu poate fi falsificat.",
            };
            msg = Messages[rand.NextInt64(0, Messages.Length)];
            AnsiConsole.MarkupLine("[green bold]✓ Mesajul a fost ales![/]");

            // generate & display keys and signature
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var output = SignMessage(msg);
            string sig = output.Signature,
                privateKey = output.PrivateKey,
                publicKey = output.PublicKey;
            sw.Stop();
            AnsiConsole.MarkupLine($"\n[yellow]Cheia publică:[/] [blue bold]{publicKey}[/]");
            CopyToClipboard(publicKey);
            AnsiConsole.MarkupLine(
                "[green bold]✓ Cheie copiată automat în clipboard! Apasă[/] [yellow]CTRL+V[/] [green bold]pentru a insera.[/]"
            );
            AnsiConsole.MarkupLine($"\n[yellow]Mesajul semnat:[/]\n[magenta]{sig}[/]");
            AnsiConsole.MarkupLine(
                $"[yellow]Criptat în:[/] [green]{sw.Elapsed.TotalMilliseconds:F4} ms[/]"
            );
            AnsiConsole.MarkupLine(
                "\n> [cyan bold]TASK-ul tău:[/] Încearcă să decriptezi mesajul introducând cheia publică."
            );
            Pause();

            string input = string.Empty;
            while (true)
            {
                while (true)
                {
                    input = AnsiConsole.Ask<string>($"> [bold white]Cheia publică:[/]");
                    if (string.IsNullOrEmpty(input))
                        AnsiConsole.MarkupLine("[red]Cheia publică nu poate fi goală.[/]");
                    else
                        break;
                }
                var isValid = VerifySignature(msg, sig, input);
                if (isValid)
                {
                    AnsiConsole.MarkupLine("[green bold]✓ Decodificat cu succes![/]");
                    AnsiConsole.MarkupLine(
                        $"\n[yellow]Mesajul decodificat:[/]\n\t[magenta]{msg}[/]"
                    );
                    break;
                }
                else
                {
                    AnsiConsole.MarkupLine("[red bold]✓ Decodificare eșuată![/]");
                    if (input != publicKey)
                        AnsiConsole.MarkupLine(
                            $"\t[red]Cheia introdusă nu se potrivește cu cheia publică. Copiaz-o de mai sus și insereaz-o aici.[/]"
                        );
                }
            }
            Pause();

            AnsiConsole.MarkupLine(
                "> [blue]Acum încearcă să modifici mesajul: fie doar o literă, un cuvânt, sau mai mult.[/]"
            );
            CopyToClipboard(sig);
            AnsiConsole.MarkupLine(
                "[green bold]✓ Semnătură copiată automat în clipboard! Apasă[/] [yellow]CTRL+V[/] [green bold]pentru a insera.[/]"
            );
            while (true)
            {
                input = AnsiConsole.Ask<string>($"> [bold white]Semnătura modificată:[/]");
                if (string.IsNullOrEmpty(input))
                    AnsiConsole.MarkupLine("[red]Semnătura nu poate fi goală.[/]");
                else if (input == sig)
                    AnsiConsole.MarkupLine("[red]Te rog să modifici semătura.[/]");
                else
                    break;
            }
            var valid = VerifySignature(msg, sig, input);
            if (!valid)
                AnsiConsole.MarkupLine(
                    "[green bold]✓ Semnătura a fost depistată cu succes ca fiind [yellow]modificată[/]![/]"
                );
            else
                AnsiConsole.MarkupLine("[red bold]Ups! Se pare că a intervenit o eroare![/]");

            Pause();
            AnsiConsole.MarkupLine(
                "\n> [yellow]În cele din urmă, pentru a putea falsifica un mesaj vei avea nevoie de cheia privată:[/]"
            );
            AnsiConsole.Write(
                new Panel(privateKey)
                    .Expand()
                    .Header("[magenta]Cheia privată[/]")
                    .BorderStyle(Color.Purple)
            );
            AnsiConsole.MarkupLine(
                "\n> [yellow]Mult mai mare decât te-ai fi aștepat, nu-i așa?[/]"
            );
        }

        private static void ThirdTest()
        {
            AnsiConsole.Clear();
            int minChars = 5,
                maxChars = 500;
            Markup contents = new Markup(
                "1) Acum vei avea ocazia să experimentezi cu [magenta]algorimtii HASH[/] în cadrul a 3+2 mici experimente."
                    + $"\n2) Vei putea să introduci [magenta]un mesaj[/] [red]({minChars}-{maxChars} carctere)[/], apoi [magenta]un fișier[/], pentru a genera aceste [magenta]coduri de identificare[/]."
                    + "\n3) Pentru aceste teste, vom utiliza algoritmul [magenta]SHA-256, SHA-512 și MD5[/] (deja spart, dar folosit pentru amprente de fișiere)."
                    + "\n4) [yellow underline]Recomandare[/]: Încearcă, măcar o dată, să modifici [magenta]o singură literă[/] la 2 teste consectuive.\n   HASH-ul generat va fi unul total diferit datorită efectului de „avalanșă” al algoritmoilor HASH:\n   sunt foarte sensibili chiar și la cele mai mici modificări."
                    + "\n[cyan bold]În sine, HASH-ul reprezintă amprenta unei informații, de la care nu ar trebui să poți ajunge înapoi la cea inițială.[/]"
            );
            var panel = new Panel(contents)
                .Header("TESTUL 3 - Hashing - Amprenta Digitală (SHA-256)")
                .RoundedBorder()
                .BorderColor(Color.Aquamarine1)
                .Expand();
            AnsiConsole.Write(panel);
            Pause();

            // read msg
            AnsiConsole.Write(new Rule("[yellow]3 teste cu mesaj tip text[/]").RuleStyle("grey"));

            string input = string.Empty;
            int maxTests = 3;
            for (int test = 0; test < maxTests; test++)
            {
                while (true)
                {
                    input = AnsiConsole.Ask<string>(
                        $"> [bold white]Mesajul tău ({test + 1}/{maxTests}):[/]"
                    );
                    if (string.IsNullOrEmpty(input))
                        AnsiConsole.MarkupLine("[red]Mesajul nu poate fi gol.[/]");
                    else if (input.Length < minChars || input.Length > maxChars)
                        AnsiConsole.MarkupLine(
                            $"[red]Mesajul trebuie să aibă între {minChars} și {maxChars} caractere.[/]"
                        );
                    else
                        break;
                }
                var tabel = new Table().Border(TableBorder.Rounded).Expand().ShowRowSeparators();
                tabel.AddColumn("[yellow]Algoritm HASH[/]");
                tabel.AddColumn("[cyan]Amprentă (Hash)[/]");

                // loop through the enum
                foreach (HashType type in Enum.GetValues<HashType>())
                {
                    string hashResult = CryptoEngine.GetHash(input, type);
                    tabel.AddRow(type.ToString(), $"[green]{hashResult}[/]");
                }
                AnsiConsole.Write(tabel);
            }
            Pause();
            // read file
            Console.WriteLine();
            AnsiConsole.Write(new Rule("[yellow]2 teste cu fișiere[/]").RuleStyle("grey"));

            AnsiConsole.MarkupLine(
                "[green bold]SFAT:[/]Alege și trage fișierul în această fereastră pentru a-l copia aici.\n\t[yellow]⚠️ Fișierul nu va fi trimis nicăieri[/], va fi interpretat doar la tine, pe acest PC <3."
            );
            input = string.Empty;
            maxTests = 2;
            for (int test = 0; test < maxTests; test++)
            {
                while (true)
                {
                    input = AnsiConsole
                        .Ask<string>($"> [bold white]Fișierul tău ({test + 1}/{maxTests}):[/]")
                        .Replace("\"", "")
                        .TrimEnd();
                    if (string.IsNullOrEmpty(input))
                        AnsiConsole.MarkupLine("[red]Acest câmp nu poate fi gol.[/]");
                    else if (!File.Exists(input))
                        AnsiConsole.MarkupLine($"[red]Fișierul nu există sau nu a fost găsit.[/]");
                    else
                    {
                        FileInfo info = new FileInfo(input);
                        // limit (100 MB = 100 * 1024 * 1024 bytes)
                        long maxSizeBytes = 100 * 1024 * 1024;
                        if (info.Length > maxSizeBytes)
                        {
                            // compute size in MB for error msg
                            double sizeInMb = (double)info.Length / (1024 * 1024);
                            AnsiConsole.MarkupLine(
                                $"[red]EROARE: Fișierul este prea mare ({sizeInMb:F2} MB). Limita este de 100 MB.[/]"
                            );
                        }
                        else
                            break;
                    }
                }
                var tabel = new Table().Border(TableBorder.Rounded).Expand().ShowRowSeparators();
                tabel.AddColumn("[yellow]Algoritm HASH[/]");
                tabel.AddColumn("[cyan]Amprentă (Hash)[/]");

                // loop through the enum
                foreach (HashType type in Enum.GetValues<HashType>())
                {
                    string hashResult = CryptoEngine.GetFileHash(input, type);
                    tabel.AddRow(type.ToString(), $"[green]{hashResult}[/]");
                }
                AnsiConsole.Write(tabel);
            }

            AnsiConsole.MarkupLine(
                "\n[green underline]AICI SE ÎNCHEIE CEL DE-AL DOILEA SET DE DEMONSTRAȚII[/]"
            );
            AnsiConsole.MarkupLine("[blue underline]Ce ai experimentat:[/]");
            AnsiConsole.MarkupLine(
                "[blue bold]•[/] [white bold]Cum funcționează criptarea simetrică[/]."
                    + "\n[blue bold]•[/] [white bold]De ce criptarea asimetrică este mai sigură[/]."
                    + "\n[blue bold]•[/] [white bold]Principalele tipuri de algorimti HASH și cum funcționează aceștia[/]."
            );
        }

        public static void Run()
        {
            introduction();
            keypoints();
            bool back = false;
            while (!back)
            {
                Console.WriteLine();
                var test = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow bold]Alege un test dintre cele de mai jos:[/]")
                        .PageSize(4)
                        .HighlightStyle(Color.Magenta)
                        .MoreChoicesText(
                            "[grey](Folosește tastele-săgeți pentru a naviga în meniu)[/]"
                        )
                        .AddChoices(
                            "Criptarea simetrică",
                            "Criptarea asimetrică",
                            "Algoritmii HASH",
                            "Înapoi"
                        )
                        .AddCancelResult("none")
                );
                switch (test)
                {
                    case "Criptarea simetrică":
                        FirstTest();
                        break;
                    case "Criptarea asimetrică":
                        SecondTest();
                        break;
                    case "Algoritmii HASH":
                        ThirdTest();
                        break;
                    case "Înapoi":
                        back = true;
                        break;
                    default:
                        break;
                }
            }
            return;
        }
    }
}
