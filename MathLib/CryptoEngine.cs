using System.Security.Cryptography;
using System.Text;

namespace CryptoSandbox.Engine
{
    public class CryptoEngine
    {
        // --- 1. Symmetric ENcryption (AES) ---
        public static (string CipherText, string Key, string IV) EncryptSymmetric(string plainText)
        {
            using Aes aes = Aes.Create();
            // generate keys
            aes.GenerateKey();
            aes.GenerateIV();

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using MemoryStream ms = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }
            }

            return (
                Convert.ToBase64String(ms.ToArray()),
                Convert.ToBase64String(aes.Key),
                Convert.ToBase64String(aes.IV)
            );
        }

        // --- 2. Symmetric DEcryption (AES) ---
        public static (string msg, bool error) DecryptSymmetric(
            string cipherText,
            string keyBase64,
            string ivBase64
        )
        {
            try
            {
                // check if it's valid Base64
                byte[] keyBytes;
                try
                {
                    keyBytes = Convert.FromBase64String(keyBase64);
                }
                catch
                {
                    return ("[red]EROARE: Cheia nu este în format valid (Base64)![/]", true);
                }

                using Aes aes = Aes.Create();
                aes.Key = keyBytes;
                aes.IV = Convert.FromBase64String(ivBase64);

                using MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText));
                using CryptoStream cs = new CryptoStream(
                    ms,
                    aes.CreateDecryptor(),
                    CryptoStreamMode.Read
                );
                using StreamReader sr = new StreamReader(cs);

                return (sr.ReadToEnd(), false);
            }
            catch (CryptographicException)
            {
                return (
                    "[red]EROARE: Cheia este validă ca format, dar este GREȘITĂ! (Mesajul a rămas sigilat)[/]",
                    true
                );
            }
            catch (Exception ex)
            {
                return ($"[red]EROARE CRITICĂ: {ex.Message}[/]", true);
            }
        }

        // --- 3. Aymmetric Encryption (Signing) (RSA) ---
        public static (string Signature, string PrivateKey, string PublicKey) SignMessage(
            string message
        )
        {
            using RSA rsa = RSA.Create(2048);
            //generate XML and remove new lines
            string privateKey = rsa.ToXmlString(true).Replace("\r", "").Replace("\n", "");
            string publicKey = rsa.ToXmlString(false).Replace("\r", "").Replace("\n", "");

            byte[] dataToSign = Encoding.UTF8.GetBytes(message);
            // in real RSA we sign the hash, not the data itself
            // here we sign the data directly, for the demo
            byte[] signature = rsa.SignData(
                dataToSign,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );

            return (Convert.ToHexString(signature), privateKey, publicKey);
        }

        // --- 4. Symmetric DEcryption (Check Signature) (RSA) ---
        public static bool VerifySignature(string message, string signatureHex, string publicKeyXml)
        {
            try
            {
                using RSA rsa = RSA.Create();
                rsa.FromXmlString(publicKeyXml);

                byte[] data = Encoding.UTF8.GetBytes(message);
                byte[] signature = Convert.FromHexString(signatureHex);

                return rsa.VerifyData(
                    data,
                    signature,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1
                );
            }
            catch
            {
                return false;
            }
        }

        // --- 5. HASH ---
        public enum HashType
        {
            MD5,
            SHA256,
            SHA512,
        }

        public static string GetTextHash(string input, HashType type)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes;

            switch (type)
            {
                case HashType.MD5:
                    using (var algo = MD5.Create())
                        hashBytes = algo.ComputeHash(inputBytes);
                    break;
                case HashType.SHA512:
                    using (var algo = SHA512.Create())
                        hashBytes = algo.ComputeHash(inputBytes);
                    break;
                case HashType.SHA256:
                default:
                    using (var algo = SHA256.Create())
                        hashBytes = algo.ComputeHash(inputBytes);
                    break;
            }

            return Convert.ToHexString(hashBytes);
        }

        public static string GetFileHash(string filePath, HashType type)
        {
            try
            {
                using (FileStream stream = File.OpenRead(filePath))
                {
                    byte[] hashBytes;

                    switch (type)
                    {
                        case HashType.MD5:
                            using (var algo = MD5.Create())
                                hashBytes = algo.ComputeHash(stream);
                            break;
                        case HashType.SHA512:
                            using (var algo = SHA512.Create())
                                hashBytes = algo.ComputeHash(stream);
                            break;
                        case HashType.SHA256:
                        default:
                            using (var algo = SHA256.Create())
                                hashBytes = algo.ComputeHash(stream);
                            break;
                    }
                    return Convert.ToHexString(hashBytes);
                }
            }
            catch (IOException ex)
            {
                return $"EROARE de acces (fișier deschis în altă parte?): {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"EROARE neașteptată: {ex.Message}";
            }
        }

        // ----- HELPER METHODS -----
        public static string GenerateSampleBase64Key()
        {
            byte[] fakeBytes = new byte[32]; // 256 bits
            RandomNumberGenerator.Fill(fakeBytes);
            return Convert.ToBase64String(fakeBytes);
        }

        public static long ShuffleDigits(long number)
        {
            // convert the number to an array of characters
            char[] digits = number.ToString().ToCharArray();
            Random rng = new Random();

            // Fisher-Yates algorithm
            int n = digits.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = digits[k];
                digits[k] = digits[n];
                digits[n] = value;
            }

            // convert back to long
            string shuffledString = new string(digits);
            return long.Parse(shuffledString);
        }
    }
}
