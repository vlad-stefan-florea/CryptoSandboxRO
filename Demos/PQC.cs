using Spectre.Console;
using static CryptoSandbox.Courses.Utils;

namespace CryptoSandbox.Courses
{
    public class PQC
    {
        private static void introduction() =>
            AnsiConsole.MarkupLine(
                "[cyan bold]Explicație[/]:"
                    + "\n\t[blue]• Ce avem[/]:"
                    + " o rețea de puncte în [magenta italic]N[/] dimensiuni"
                    + "\n\t[blue]• Ce facem[/]:"
                    + " vom alege un punct, la care vom adăuga [magenta]mici erori[/] pentru fiecare coordonată"
                    + "\n\t[red]• Problema[/]:"
                    + " Cu cât lucrăm în mai multe dimensiuni, metoda [magenta italic]LWE (Learning With Errors)[/] va fi mai greu de spart:\n   pentru a ghici punctul original, calculatorul va trebui să verifice extrem de multe puncte"
            );

        private static void keypoints() =>
            AnsiConsole.MarkupLine(
                "[cyan bold]Puncte cheie[/]:"
                    + "\n\t[blue]•[/] [white bold]Exemplu de LWE în 2 dimensiuni[/]"
                    + "\n\t[blue]•[/] [white bold]Cum poate ghici un calculator punctul original dacă îl știe pe cel cu erori[/]"
                    + "\n\t[blue]•[/] [white bold]De ce crește dificultatea odată cu numărul de dimensiuni[/]"
            );

        private static void FirstTest()
        {
            int width = 20;
            int height = 20;
            AnsiConsole.Clear();
            Markup contents = new Markup(
                "1) Acum vei avea ocazia să experimentezi [magenta]criptografia PQC[/] în cadrul unui experiment [magenta]Lattice-based[/]."
                    + $"\n2) Vei primi un [magenta]tabel de {width}×{height} căsuțe[/], colorate în diverse nuanțe."
                    + "\n3) Vei primi un punct ce are [red]coordonatele greșite[/] și va trebui să găsești [green]punctul original[/], prin încercări."
                    + $"\n4) Coorodonatele pot avea valorile 0-{width - 1} în lungime și 0-{height - 1} în lățime."
                    + "\n5) Codul de culori utilizat este:\n   [blue]Date publice[/]\n   [red]Încercare eșuată[/]\n   [green]Cheia originală[/]"
                    + "\n[cyan bold]SUCCES![/]"
            );
            var panel = new Panel(contents)
                .Header("TESTUL 1 - LWE Hands-on Experiment")
                .RoundedBorder()
                .BorderColor(Color.Aquamarine1)
                .Expand();
            AnsiConsole.Write(panel);
            Pause();
            var rand = Random.Shared;

            // the secret
            int secretX = rand.Next(2, width - 2);
            int secretY = rand.Next(2, height - 2);

            // public coords with LWE
            int publicX = secretX + rand.Next(-2, 3);
            int publicY = secretY + rand.Next(-2, 3);

            // remember tries
            var attempts = new List<(int X, int Y, bool IsCorrect)>();
            // generating the canvas
            Canvas GenerateCanvas(List<(int X, int Y, bool IsCorrect)> history)
            {
                var c = new Canvas(width, height);

                // background with NOISE
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int b = rand.Next(50, 150);
                        c.SetPixel(x, y, new Color(0, (byte)(b / 4), (byte)(b / 2))); // darker shade of blue so red is seen easily
                    }
                }

                // render all previously tried points
                foreach (var attempt in history)
                {
                    c.SetPixel(attempt.X, attempt.Y, attempt.IsCorrect ? Color.Green : Color.Red);
                }

                return c;
            }
            // --- loop logic ---
            bool found = false;

            while (!found)
            {
                AnsiConsole.Clear(); // clean the console

                AnsiConsole.MarkupLine(
                    $"[bold yellow]LWE Prompt:[/] Cheia publică interceptată: [white]({publicX}, {publicY})[/]."
                );
                AnsiConsole.MarkupLine("[cyan]Marjă de eroare: ±2 unități.[/]\n");

                // display the canvas with full history
                AnsiConsole.Write(
                    new Panel(GenerateCanvas(attempts))
                        .Header("Matricea de Încercări")
                        .BorderColor(Color.Blue)
                );

                var input = AnsiConsole.Ask<string>("[white]Introdu (X,Y):[/]");

                try
                {
                    var parts = input.Split(
                        new[] { ',', ' ', ';' },
                        StringSplitOptions.RemoveEmptyEntries
                    );
                    int gX = int.Parse(parts[0]);
                    int gY = int.Parse(parts[1]);
                    if (gX < 0 || gX >= width || gY < 0 || gY >= height)
                    {
                        AnsiConsole.MarkupLine("[red]Punctul nu se află în rețea![/]");
                        Pause();
                    }
                    else
                    {
                        bool isCorrect = (gX == secretX && gY == secretY);

                        // add the attempt in the history
                        attempts.Add((gX, gY, isCorrect));
                        if (isCorrect)
                        {
                            found = true;
                            AnsiConsole.Clear();
                            AnsiConsole.Write(
                                new Panel(GenerateCanvas(attempts))
                                    .Header("DECRIPTAT")
                                    .BorderColor(Color.Green)
                            );
                            AnsiConsole.MarkupLine(
                                "[green bold]✓ EXCELENT! Ai eliminat eroarea și ai găsit punctul secret![/]"
                            );
                        }
                    }
                }
                catch
                {
                    AnsiConsole.MarkupLine("[red]Format invalid![/]");
                    Thread.Sleep(2000); // let the user see the error message
                }
            }
        }

        private static void SecondTest()
        {
            int bfWidth = 100;
            int bfHeight = 50;
            AnsiConsole.Clear();
            Markup contents = new Markup(
                "1) Acum vei [magenta]observa[/] cum un calculator va face exact ceea ce ai experimentat și la [blue]TESTUL 1[/]."
                    + $"\n2) PC-ul va primi o rețea de {bfWidth}×{bfHeight} căsuțe, în care va căuta [magenta]una câte una[/] până va ajunge la cea corectă."
                    + "\n3) Spre deosebire de tine, [magenta]calculatorul nu va ști marja de eroare[/], exact ca într-o situație reală."
                    + "\n4) Tocmai de aceea, [magenta]brute-force[/] rămâne singura soluție."
                    + "\n[cyan bold]Privește cu atenție procesul vizual![/]"
            );
            var panel = new Panel(contents)
                .Header("TESTUL 2 - LWE Brute-Force")
                .RoundedBorder()
                .BorderColor(Color.Aquamarine1)
                .Expand();
            AnsiConsole.Write(panel);
            Pause();
            int tries = 0;
            var rand = Random.Shared;
            // the secret
            int secretX = rand.Next(2, bfWidth - 2);
            int secretY = rand.Next(2, bfHeight - 2);
            // public coords with LWE
            int publicX = secretX + rand.Next(-2, 3);
            int publicY = secretY + rand.Next(-2, 3);
            var bfCanvas = new Canvas(bfWidth, bfHeight);

            // fill the canvas with noise
            for (int x = 0; x < bfWidth; x++)
            {
                for (int y = 0; y < bfHeight; y++)
                {
                    int b = Random.Shared.Next(30, 80);
                    bfCanvas.SetPixel(x, y, new Color(0, 0, (byte)b));
                }
            }

            AnsiConsole
                .Live(
                    new Panel(bfCanvas)
                        .Header("Simulare de Atac Brute Force")
                        .BorderColor(Color.Red)
                )
                .Start(ctx =>
                {
                    bool broken = false;
                    // computer doesn't know the error size, tries all points
                    for (int y = 0; y < bfHeight && !broken; y++)
                    {
                        for (int x = 0; x < bfWidth; x++)
                        {
                            tries++;
                            // check if found the og coords
                            if (x == secretX && y == secretY)
                            {
                                bfCanvas.SetPixel(x, y, Color.Green);
                                broken = true;
                                ctx.Refresh();
                                break;
                            }
                            // flag attempt as failed
                            bfCanvas.SetPixel(x, y, Color.Red);
                            // refresh the console and canvas each N points/pixels checked to reduce CPU usage
                            if (x % 20 == 0)
                                ctx.Refresh();
                        }
                    }
                });

            AnsiConsole.MarkupLine(
                $"[green bold]✓ Calculatorul a găsit cheia după {tries} încercări![/]"
            );
            AnsiConsole.MarkupLine(
                "> [white bold]Dacă singura soluție eficientă în cazul în care nu se cunoaște procesul de atribuire a zgomotului (cum a fost atribuită eroare la coordonate) atunci imaginează-ți același proces dar în peste 100 de dimensiuni.\n> Greu de crezut că un calculatorul ar putea găsi punctul corect încercând zeci de milioane de puncte, nu-i așa?\n> Tocmai de aceea LWE este un standard bun în PQC.[/]"
            );
            AnsiConsole.MarkupLine(
                "\n[green underline]AICI SE ÎNCHEIE CEL DE-AL PATRULEA ȘI ULTIM SET DE DEMONSTRAȚII[/]"
            );
            AnsiConsole.MarkupLine("[blue underline]Ce ai experimentat:[/]");
            AnsiConsole.MarkupLine(
                "[blue bold]•[/] [white bold]Cum funcționează criptarea Lattice-Based și LWE[/]."
                    + "\n[blue bold]•[/] [white bold]Cât de mult durează găsirea soluției[/]."
                    + "\n[blue bold]•[/] [white bold]De ce este un standard bun pentru PQC (Post-Quantum-Cryptography)[/]."
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
                        .PageSize(3)
                        .HighlightStyle(Color.Magenta)
                        .MoreChoicesText(
                            "[grey](Folosește tastele-săgeți pentru a naviga în meniu)[/]"
                        )
                        .AddChoices("LWE - experiment", "LWE Brute-Force", "Înapoi")
                        .AddCancelResult("none")
                );
                switch (test)
                {
                    case "LWE - experiment":
                        FirstTest();
                        break;
                    case "LWE Brute-Force":
                        SecondTest();
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
