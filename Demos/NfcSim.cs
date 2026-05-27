using System.Numerics;
using System.Security.Cryptography;
using CryptoSandbox.Engine;
using Spectre.Console;
using static CryptoSandbox.Courses.Utils;

namespace CryptoSandbox.Courses
{
    public class NfcSim
    {
        private static void introduction() =>
            AnsiConsole.MarkupLine(
                "[cyan bold]Explicație[/]:"
                    + "\n\t[blue]• Ce avem[/]:"
                    + " un [magenta]telefon[/] și un [magenta]POS[/]"
                    + "\n\t[blue]• Ce facem[/]:"
                    + " vom simula efectuarea unei plăți prin [magenta]NFC (Near Field Communication)[/]"
            );

        private static void keypoints() =>
            AnsiConsole.MarkupLine(
                "[cyan bold]Puncte cheie[/]:"
                    + "\n\t[blue]•[/] [white bold]Cum cele 2 dispozitive comunică între ele[/]"
                    + "\n\t[blue]•[/] [white bold]Cum „cunosc” datele cardului fără să le comunice direct[/]"
                    + "\n\t[blue]•[/] [white bold]De ce este dificilă intervenția din exterior în timpul acestor schimburi de date[/]"
            );

        public static void Run()
        {
            int minPrime = 2,
                maxPrime = 20;
            introduction();
            keypoints();
            Pause();
            AnsiConsole.Clear();
            Markup contents = new Markup(
                "1) Vei observa [magenta]handshake[/]-ul care stă în spatele tuturor plăților prin [magenta]NFC (telefon-POS)[/]."
                    + $"\n2) [blue]Deciziile vor fi simulate individual[/] pentru ambele dispozitive."
                    + $"\n3) [red]Aceasta este doar o simulare simplificată[/] a procesului real, având doar scop educațional."
                    + $"\n*) Un [magenta]token[/] reprezintă un identificator temporar al cardului folosit în acest tip de tranzacții, pentru a ascunde datele reale."
                    + $"\n**) Token-ul va fi generat prin [blue]interschimbarea aleatorie a cifrelor[/]."
                    + $"\n***) Un [magenta]Shared Secret[/] este de fapt valoarea pe care ambele dispozitive o calculează înainte de a comunica datele cardului."
            );
            var panel = new Panel(contents)
                .Header("Handshake Securizat (Simulare Plată NFC)")
                .RoundedBorder()
                .BorderColor(Color.Aqua)
                .Expand();
            AnsiConsole.Write(panel);
            Pause();

            var phone = new Panel("Inițializare ...")
                .Header("[bold]TELEFON[/]")
                .Expand()
                .BorderColor(Color.Aquamarine1);
            var pos = new Panel("Inițializare ...")
                .Header("[bold]POS[/]")
                .Expand()
                .BorderColor(Color.CadetBlue);
            ;
            var columns = new Columns(phone, pos);
            string phoneHistory = new(""),
                posHistory = new("");
            int cardNumber = RandomNumberGenerator.GetInt32(10000000, 99999999);

            AnsiConsole.Write(
                new Panel("[yellow]Numărul cardului (exemplu):[/] " + cardNumber)
                    .Header("[red bold]Detalii Card (pe telefon)[/]")
                    .Border(BoxBorder.Rounded)
                    .BorderColor(Color.Yellow)
            );

            AnsiConsole
                .Live(columns)
                .AutoClear(false)
                .Start(ctx =>
                {
                    void UpdateUI(string leftNew, string rightNew)
                    {
                        phoneHistory += leftNew + "\n\n";
                        posHistory += rightNew + "\n\n";

                        phone = new Panel(phoneHistory.TrimEnd())
                            .Header("[bold]TELEFON[/]")
                            .Expand()
                            .BorderColor(Color.Aquamarine1);

                        pos = new Panel(posHistory.TrimEnd())
                            .Header("[bold]POS[/]")
                            .Expand()
                            .BorderColor(Color.CadetBlue);

                        ctx.UpdateTarget(new Columns(phone, pos));
                    }

                    // --- STEP 1 ---
                    UpdateUI("[grey]Căutare dispozitive...[/]", "[grey]Ascultare NFC...[/]");
                    Thread.Sleep(2000);
                    UpdateUI("[green]POS găsit![/]", "[green]Dispozitiv NFC detectat![/]");
                    Thread.Sleep(1000);

                    // --- STEP 2 ---
                    int powerBase = RandomNumberGenerator.GetInt32(2, 20);

                    UpdateUI(
                        $"Propun baza: [magenta italic]{powerBase}[/]",
                        "[green]Accept baza aleasă de telefon[/]"
                    );
                    Thread.Sleep(1000);

                    // --- STEP 3 ---
                    long prime = PrimeMathEngine.GetRandomPrime(minPrime, maxPrime);
                    while (prime == powerBase)
                        prime = PrimeMathEngine.GetRandomPrime(minPrime, maxPrime);
                    UpdateUI(
                        $"Aleg din lista proprie și propune nr. prim: [magenta italic]{prime}[/]",
                        "[green]Accept nr. prim ales de telefon[/]"
                    );
                    Thread.Sleep(1000);

                    // --- STEP 4 ---
                    long phonePrivKey = PrimeMathEngine.GetRandomPrime(minPrime, maxPrime),
                        posPrivKey = PrimeMathEngine.GetRandomPrime(minPrime, maxPrime);
                    while (phonePrivKey == posPrivKey)
                        posPrivKey = PrimeMathEngine.GetRandomPrime(minPrime, maxPrime);

                    UpdateUI(
                        $"[magenta]Cheie privată generată:[/][blue italic] {phonePrivKey}[/]",
                        $"[magenta]Cheie privată generată:[/][blue italic] {posPrivKey}[/]"
                    );
                    Thread.Sleep(1000);

                    // --- STEP 5 ---
                    UpdateUI(
                        $"Efectuez [cyan]{powerBase}^{phonePrivKey} mod {prime}[/]",
                        $"Efectuez [cyan]{powerBase}^{posPrivKey} mod {prime}[/]"
                    );
                    Thread.Sleep(1000);

                    // --- STEP 6 ---
                    long phonePublicKey = (long)BigInteger.ModPow(powerBase, phonePrivKey, prime);
                    long posPublicKey = (long)BigInteger.ModPow(powerBase, posPrivKey, prime);
                    UpdateUI(
                        $"[magenta]Cheie publică generată[/]: [blue italic]{phonePublicKey}[/]",
                        $"[magenta]Cheie publică generată[/]: [blue italic]{posPublicKey}[/]"
                    );
                    Thread.Sleep(1000);

                    // --- STEP 7a ---
                    UpdateUI(
                        $"Cheie publică [blue]trimisă spre POS[/]: [magenta italic]{phonePublicKey}[/]",
                        $"Cheie publică [blue]trimisă spre telefon[/]: [magenta italic]{posPublicKey}[/]"
                    );
                    UpdateUI(
                        $"Cheie publică primită: [magenta italic]{posPublicKey}[/]",
                        $"Cheie publică primită: [magenta italic]{phonePublicKey}[/]"
                    );
                    Thread.Sleep(1000);
                    // --- STEP 7b ---
                    ;
                    UpdateUI(
                        $"Calculez cheia de criptare: [cyan]{posPublicKey}^{phonePrivKey} mod {prime}[/]",
                        $"Calculez cheia de criptare: [cyan]{phonePublicKey}^{posPrivKey} mod {prime}[/]"
                    );
                    Thread.Sleep(1000);

                    // --- STEP 8 ---
                    long phoneSeed = (long)BigInteger.ModPow(posPublicKey, phonePrivKey, prime);
                    long posSeed = (long)BigInteger.ModPow(phonePublicKey, posPrivKey, prime);

                    UpdateUI(
                        $"[blue]Cheia de criptare calculată[/]: [magenta italic]{phoneSeed}[/]",
                        $"[blue]Cheia de criptare calculată[/]: [magenta italic]{posSeed}[/]"
                    );
                    Thread.Sleep(1000);

                    // --- STEP 9 ---
                    long token = CryptoEngine.ShuffleDigits(cardNumber);
                    UpdateUI(
                        $"[blue]Generez token-ul[/] cardului: [magenta italic]{token}[/]",
                        $"[grey]Aștept datele cardului...[/]"
                    );
                    Thread.Sleep(1000);

                    // --- STEP 10 ---
                    long obfuscator = phoneSeed * 12345678; // small secret -> big secret
                    long encryptedToken = token ^ obfuscator;
                    UpdateUI(
                        $"[blue]Criptez token-ul[/] cu [underline]Shared Secret[/] [cyan]({phoneSeed})[/]: [magenta italic]{token} [blue]XOR[/] {phoneSeed}[/]",
                        "[grey]Se recepționează pachetul criptat...[/]"
                    );
                    Thread.Sleep(1000);

                    // --- STEP 11 ---
                    UpdateUI(
                        $"[green]Token trimis către POS[/]: [magenta]{encryptedToken}[/]",
                        $"[green]Pachet primit cu succes[/]: [magenta]{encryptedToken}[/]"
                    );
                    Thread.Sleep(1000);

                    // --- STEP 12 ---
                    long decryptedToken = encryptedToken ^ obfuscator;
                    UpdateUI(
                        "[red]Închei conexiunea NFC ...[/]",
                        $"Decriptez pachetul: [magenta italic]{encryptedToken} [blue]XOR[/] {posSeed} = {decryptedToken}[/]"
                    );
                    Thread.Sleep(1000);

                    // --- STEP 13 ---
                    UpdateUI(
                        "[grey]...[/]",
                        $"[yellow]Trimit token-ul {decryptedToken} către bancă pentru aprobare ...[/]"
                    );

                    // --- STEP 14 ---
                    UpdateUI(
                        "[grey]...\n...\n...\n...\n...\n...\n...\n...[/]",
                        $"[red]Aștept confirmarea băncii ...[/]"
                            + "\n[blue bold underline]BANCA:[/]"
                            + "\n   • [yellow]A primit token-ul[/]"
                            + "\n   • [yellow]A verificat autenticitatea token-ului[/]"
                            + "\n   • [yellow]A identificat datele cardului[/]"
                            + "\n   • [yellow]A verificat soldul cardului[/]"
                            + "\n   [green]✓ Tranzacție aprobată[/]"
                            + "\n   • [green]Trimite codul de autorizare al tranzacției[/]"
                    );
                    Thread.Sleep(1000);

                    // --- STEP 15 ---
                    UpdateUI(
                        "[grey]...\n...\n...\n...[/]",
                        $"[green]✓ Marchez tranzacția ca efectuată ...[/]"
                            + "\n[blue bold underline]BANCA:[/]"
                            + "\n   • [yellow]Efectuează tranzacția ...[/]"
                            + "\n   [green]✓ Tranzacție efectuată[/]"
                    );
                    Thread.Sleep(1000);

                    // --- FINALIZE ---
                    UpdateUI("[blue underline]✓ NOTIFICARE: PLATĂ EFECTUATĂ[/]", $"[grey]...[/]");
                    Thread.Sleep(1000);
                });

            AnsiConsole.MarkupLine(
                "\n[white bold]> Acum imaginează-ți același proces, dar cu numere mult mai mari.[/]"
                    + "\n[white bold]> Pentru a intercepta și descifra [red]token-ul[/], o persoană rău intenționată ar avea nevoie de:[/]"
                    + "\n\t• [white]cheile private[/]"
                    + "\n\t• [white]acces la algoritmul de generare a token-ului pentru a putea deduce datele reale ale cardului.[/]"
                    + "\n[white bold]> Dar, deoarece datele sensibile nu circulă în clar între telefon și POS, interceptarea sau modificarea lor este practic imposibilă.[/]"
            );
            AnsiConsole.MarkupLine(
                "\n[green underline]AICI SE ÎNCHEIE CEL DE-AL TREILEA SET DE DEMONSTRAȚII[/]"
            );
            AnsiConsole.MarkupLine("[blue underline]Ce ai experimentat:[/]");
            AnsiConsole.MarkupLine(
                "[blue bold]•[/] [white bold]Cum funcționează o tranzacție prin NFC[/]."
                    + "\n[blue bold]•[/] [white bold]De ce aceste tranzacții sunt sigure din punct de vedere matematic[/]."
                    + "\n[blue bold]•[/] [white bold]De ce este imposibilă, în cazul ideal, modificarea și preluarea datelor sensibile despre tranzacție[/]."
            );

            Pause();
            return;
        }
    }
}
