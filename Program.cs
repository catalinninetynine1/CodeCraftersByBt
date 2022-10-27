using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;

namespace SolutionCodeCraftersByBT
{

    public class Generator
    {
        public string GenerateFormat(int id)
        {

            string? userName = Console.ReadLine();
            DateTime date = DateTime.Now;
            date.ToString();
            string generatedPassword = $"{id}|{date}|{userName}";
            id++;//cum incrementez eu id-ul asta de fiecare data cand fac un call la aceasta metoda.
            return generatedPassword;
        }

    }


    public class Validator
    {
        private List<string> goodPasswords = new List<string>();
        public bool ValidateInput(string input)
        {

            string[] splittedInput = input.Split('|');

            string totallyFullDate = splittedInput[1];

            string yearMmDd = totallyFullDate.Split(" ")[0];
            string hourMinSec = totallyFullDate.Split(" ")[1];
            // Comparat date1 cu date2 ca sa vad daca au trecut 30 de secunde de cand ai facut contul pana la momentul actual.
            var year = yearMmDd.Split("/")[2];
            int.TryParse(year, out int numberForYear);
            var month = yearMmDd.Split("/")[1];
            int.TryParse(month, out int numberForMonth);
            var day = yearMmDd.Split("/")[0];
            int.TryParse(day, out int numberForDay);
            var hour = hourMinSec.Split(":")[0];
            int.TryParse(hour, out int numberForHour);
            var minute = hourMinSec.Split(":")[1];
            int.TryParse(minute, out int numberForMinute);
            var second = hourMinSec.Split(":")[2];
            int.TryParse(second, out int numberForSecond);
            var date1 = new DateTime(numberForYear, numberForMonth, numberForDay, numberForHour, numberForMinute, numberForSecond);

            Console.WriteLine("Parola dumneavoastră este: " + input);
            Console.WriteLine("Aveti 30 de secunde la dispozitie pentru a introduce parola de mai sus \n" +
            "folositi Ctrl C -> Ctrl V");

            string? parolaIntrodusa = Console.ReadLine();

            DateTime date2 = DateTime.Now;
            var difference = date2 - date1;
            int result = difference.Seconds;
            if (result <= 30 && !string.IsNullOrEmpty(parolaIntrodusa) && goodPasswords.Contains(parolaIntrodusa) == false)
            {
                goodPasswords.Add(parolaIntrodusa);
                Console.WriteLine("Parola ta a fost adăugată la lista de parole, felicitari!\n");
                return true;
            }
            else
            {
                Console.WriteLine("Imi pare rau! \n Întâmpini una din urmatoarele probleme: \n -> timpul de incarcare al parolei a expirat \n -> nu ai introdus parola \n -> parola introdusa deja exista \n");
                return false;
            }

        }
    }
    class Solution
    {
        static void Main(string[] args)
        {
            //programul a fost gandit pe happy flow doar,
            //am incercat sa acopar unele din potentialele erori, dar mai trebuie rezolvate anumite flow-uri.
            Validator validatorGlobal = new Validator();

            bool more = true;

            while (more)
            {
                Console.WriteLine("Te salut, o intrebare simpla: \n" +
                    "Împarti zâmBT zilnic? " + '\u263B' + "\n" +
                    "Dacă vei scrie 'nu' programul se va închide.");

                string? words = Console.ReadLine();
                string result = words.ToLower();

                if (result != "nu")
                {
                    Console.WriteLine("Fain vibe! introdu numele tau mai jos: ");
                    int id = 0; 
                    Generator generator = new Generator();
                    string generated = generator.GenerateFormat(id);
                    
                    Console.WriteLine("Dupa ce ai introdus numele mai ai 30 de secunde sa introduci prola generata!\n");
                    validatorGlobal.ValidateInput(generated);
                    Console.WriteLine("Pot sa iti arat parola criptata daca doresti, precum si decriptata, scrie 'da' si incepem");

                    string? showEncryptDecrypt = Console.ReadLine();
                    string checkEncryptDecrypy = showEncryptDecrypt.ToLower();

                    if (checkEncryptDecrypy == "da")
                    {
                        using (Aes myEncryption = Aes.Create())
                        {
                            Console.WriteLine("Mai jos ai parola criptata.\n");

                            byte[] encrypted = EncryptStringToBytes_Aes(generated, myEncryption.Key, myEncryption.IV);
                            string resultToBase64 = System.Convert.ToBase64String(encrypted);//convertToBase64
                            Console.WriteLine(resultToBase64 + "\n");//pot adauga aceasta valoare la lista pentru a nu oferi adminului parola utilizatorilor


                            Console.WriteLine("Urmează Decryption, practic originalul.\n");
                            string decryption = DecryptStringFromBytes_Aes(encrypted, myEncryption.Key, myEncryption.IV);
                            Console.WriteLine(decryption + "\n");


                        }
                    }
                }
                else
                {
                    Console.WriteLine("Încearcă sa pui mai des un zâmBT pe buze!");
                    more = false;
                    //Opresc aplicatia.
                }
            }
        }

        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }

                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string? plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
