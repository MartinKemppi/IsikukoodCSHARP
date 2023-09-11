using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IsikukoodCSHARP
{
    public class IdCode
    {
        private readonly string _nimi;
        private readonly string _perenimi;
        private readonly string _idCode;
        private readonly DateTime _sündimispäev;


        public IdCode(string nimi, string perenimi, string idCode, DateTime sündimispäev)
        {
            this._nimi = nimi;
            this._perenimi = perenimi;
            this._idCode = idCode;
            this._sündimispäev = sündimispäev;
        }

        public string IDcode { get => _idCode; }
        public DateTime Sündimispäev { get => _sündimispäev; }
        public string Nimi { get => _nimi; }
        public string Perenimi { get => _perenimi; }

        private bool IsValidLength()
        {
            return _idCode.Length == 11;
        }

        private bool ContainsOnlyNumbers()
        {
            // return _idCode.All(Char.IsDigit);

            for (int i = 0; i < _idCode.Length; i++)
            {
                if (!Char.IsDigit(_idCode[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private int GetGenderNumber()
        {
            return Convert.ToInt32(_idCode.Substring(0, 1));
        }

        private bool IsValidGenderNumber()
        {
            int genderNumber = GetGenderNumber();
            return genderNumber > 0 && genderNumber < 7;
        }

        private int Get2DigitYear()
        {
            return Convert.ToInt32(_idCode.Substring(1, 2));
        }

        public int GetFullYear()
        {
            int genderNumber = GetGenderNumber();
            // 1, 2 => 18xx
            // 3, 4 => 19xx
            // 5, 6 => 20xx
            // 1, 3, 5 => mees
            // 2, 4, 6 => naine
            return 1800 + (genderNumber - 1) / 2 * 100 + Get2DigitYear();
        }

        private int GetMonth()
        {
            return Convert.ToInt32(_idCode.Substring(3, 2));
        }

        private bool IsValidMonth()
        {
            int month = GetMonth();
            return month > 0 && month < 13;
        }

        private static bool IsLeapYear(int year)
        {
            return year % 4 == 0 && year % 100 != 0 || year % 400 == 0;
        }
        private int GetDay()
        {
            return Convert.ToInt32(_idCode.Substring(5, 2));
        }

        private bool IsValidDay()
        {
            int day = GetDay();
            int month = GetMonth();
            int maxDays = 31;
            if (new List<int> { 4, 6, 9, 11 }.Contains(month))
            {
                maxDays = 30;
            }
            if (month == 2)
            {
                if (IsLeapYear(GetFullYear()))
                {
                    maxDays = 29;
                }
                else
                {
                    maxDays = 28;
                }
            }
            return 0 < day && day <= maxDays;
        }

        private int CalculateControlNumberWithWeights(int[] weights)
        {
            int total = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                total += Convert.ToInt32(_idCode.Substring(i, 1)) * weights[i];
            }
            return total;
        }

        private bool IsValidControlNumber()
        {
            int controlNumber = Convert.ToInt32(_idCode[^1..]);
            int[] weights = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1 };
            int total = CalculateControlNumberWithWeights(weights);
            if (total % 11 < 10)
            {
                return total % 11 == controlNumber;
            }
            // second round
            int[] weights2 = { 3, 4, 5, 6, 7, 8, 9, 1, 2, 3 };
            total = CalculateControlNumberWithWeights(weights2);
            if (total % 11 < 10)
            {
                return total % 11 == controlNumber;
            }
            // third round, control number has to be 0
            return controlNumber == 0;
        }

        public bool IsValid()
        {
            return IsValidLength() && ContainsOnlyNumbers()
                && IsValidGenderNumber() && IsValidMonth()
                && IsValidDay()
                && IsValidControlNumber();
        }

        public DateOnly GetBirthDate()
        {
            int day = GetDay();
            int month = GetMonth();
            int year = GetFullYear();
            return new DateOnly(year, month, day);
        }

        public static List<IdCode> GenerateRandomSisu(int inimestearv)
        {
            Random rand = new Random();
            List<IdCode> ID = new List<IdCode>();          
            Console.WriteLine();

            string[] names = { "Avrora", "Valeriya", "Marta", "Makar", "Varlam", "Nata", "Anzhelika", "Alevtina", "Inga", "Alexsandr", "Aristarkh", "Varlaam", "Liliya", "Alik" };
            string[] surenames = { "Liubov", "Faina", "Anastasiy", "Radmir", "Kirill", "Gleb", "Ilariy", "Zoya", "Vlas", "Gennadi", "Irinei", "Yaropolk", "Kuzma", "Albert" };

            for (int i = 0; i < inimestearv; i++)
            {
                string nimi = names[rand.Next(names.Length)];
                string perenimi = surenames[rand.Next(surenames.Length)];

                DateTime birthDate = GenerateRandomBirthDate(rand);
                string idCode = GenerateRandomIdCode(rand, birthDate);
               
                int genderNumber = GetGenderNumbers(idCode);
                string sugu;

                if (genderNumber == 1 || genderNumber == 3 || genderNumber == 5)
                {
                    sugu = "Mees";
                }
                else
                {
                    sugu = "Naine";
                }

                IdCode inimene = new IdCode(nimi, perenimi, idCode, birthDate);
                Console.WriteLine(inimene.Nimi);
                Console.WriteLine(inimene.Perenimi);
                Console.WriteLine(inimene.Sündimispäev);
                Console.WriteLine(sugu);
                Console.WriteLine(inimene.IDcode);
                

                string haigla = IdCode.Haiglakoht(inimene);
                Console.WriteLine($"Haigla: {haigla}");               

                if (inimene.IsValid())
                {
                    ID.Add(inimene);
                    Console.WriteLine("Kehtiv Isikukood.");
                }
                else
                {
                    ID.Add(inimene);
                    Console.WriteLine("Mitte kehtiv Isikukood.");
                }

                Console.WriteLine();
            }

            return ID;
        }

        private static int GetGenderNumbers(string idCode)
        {
            int firstDigit = int.Parse(idCode.Substring(0, 1));

            if (firstDigit == 1 || firstDigit == 3 || firstDigit == 5)
            {
                return firstDigit;
            }
            else if (firstDigit == 2 || firstDigit == 4 || firstDigit == 6)
            {
                return firstDigit;
            }
            else
            {                
                throw new ArgumentException("Vale sugu number isikukoodis.");
            }
        }

        public static string GenerateRandomIdCode(Random rand, DateTime birthDate)
        {           
            int year = birthDate.Year % 100;
            int month = birthDate.Month;
            int day = birthDate.Day;            

            int genderNumber;
            if (birthDate.Year > 1800 && birthDate.Year < 1900)
            {
                genderNumber = rand.Next(1, 3);                
            }
            else if (birthDate.Year > 1900 && birthDate.Year < 2000)
            {
                genderNumber = rand.Next(3, 5);               
            }
            else
            {
                genderNumber = rand.Next(5, 7);                
            }

            int hospitalID = rand.Next(0, 1000);

            string idCodeWithoutControl = $"{genderNumber}{year:D2}{month:D2}{day:D2}{hospitalID:D3}";

            int controlNumber = CalculateControlNumberWithWeights(idCodeWithoutControl, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1 });
            string idCode = $"{idCodeWithoutControl}{controlNumber:D1}";

            return idCode;
        }       

        private static int CalculateControlNumberWithWeights(string idCodeWithoutControl, int[] weights)
        {
            int total = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                total += int.Parse(idCodeWithoutControl[i].ToString()) * weights[i];
            }
            int controlNumber = total % 11;
            if (controlNumber == 10)
            {
                controlNumber = 0;
            }
            return controlNumber;
        }

        private static DateTime GenerateRandomBirthDate(Random rand)
        {           
            int year = rand.Next(1800, 2024);
            int month = rand.Next(1, 13);
            int maxDay = DateTime.DaysInMonth(year, month);
            int day = rand.Next(1, maxDay + 1);

            return new DateTime(year, month, day);
        }

        public static string Haiglakoht(IdCode idCodeObject)
        {
            string idCode = idCodeObject.IDcode;

            if (idCode.Length >= 10)
            {
                string tahed8910 = idCode.Substring(7, 3); // Get the 8th, 9th, and 10th digits

                if (int.TryParse(tahed8910, out int t))
                {
                    if (1 <= t && t <= 10)
                    {
                        return "Kuressaare Haigla";
                    }
                    else if (11 <= t && t <= 18)
                    {
                        return "Tartu Ülikooli Naistekliinik";
                    }
                    else if (21 <= t && t <= 220)
                    {
                        return "Ida-Tallinna Keskhaigla";
                    }
                    else if (221 <= t && t <= 270)
                    {
                        return "Ida-Viru Keskhaigla";
                    }
                    else if (271 <= t && t <= 370)
                    {
                        return "Maarjamõisa Kliinikum";
                    }
                    else if (371 <= t && t <= 420)
                    {
                        return "Narva Haigla";
                    }
                    else if (421 <= t && t <= 470)
                    {
                        return "Pärnu Haigla";
                    }
                    else if (471 <= t && t <= 490)
                    {
                        return "Pelgulinna Sünnitusmaja";
                    }
                    else if (491 <= t && t <= 520)
                    {
                        return "Järvamaa Haigla";
                    }
                    else if (521 <= t && t <= 570)
                    {
                        return "Rakvere, Tapa haigla";
                    }
                    else if (571 <= t && t <= 600)
                    {
                        return "Valga Haigla";
                    }
                    else if (601 <= t && t <= 650)
                    {
                        return "Viljandi Haigla";
                    }
                    else if (651 <= t && t <= 700)
                    {
                        return "Lõuna-Eesti Haigla";
                    }
                }
            }
            return "Välismaa Haigla";
        }

        public static void CheckAndDisplayMembers(List<IdCode> members)
        {
            foreach (var member in members)
            {
                bool isValid = member.IsValid();
                string fullName = $"{member.Nimi} {member.Perenimi}";

                if (isValid)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{fullName}, {member.IDcode} - Kehtiv Isikukood.");                    
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{fullName}, {member.IDcode} - Mitte kehtiv Isikukood.");
                }

                Console.ResetColor();
            }
        }
    }
}