using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
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

        public static List<IdCode> GenerateRandomSisu()
        {
            Random rand = new Random();
            List<IdCode> ID = new List<IdCode>();           

            int inimestearv = rand.Next(20, 145);
            Console.WriteLine(inimestearv);
            Console.WriteLine();

            for (int i = 0; i < inimestearv; i++)
            {

                string[] names = { "Avrora", "Valeriya", "Marta", "Makar", "Varlam", "Nata", "Anzhelika", "Alevtina", "Inga", "Alexsandr", "Aristarkh", "Varlaam", "Liliya", "Alik" };
                string[] surenames = { "Liubov", "Faina", "Anastasiy", "Radmir", "Kirill", "Gleb", "Ilariy", "Zoya", "Vlas", "Gennadi", "Irinei", "Yaropolk", "Kuzma", "Albert" };
                
                // random index name & surename
                int randomNameIndex = rand.Next(0, names.Length);
                int randomSurnameIndex = rand.Next(0, surenames.Length);

                // saame nimi ja perenimi
                string nimi = names[randomNameIndex];
                string perenimi = surenames[randomSurnameIndex];

                IdCode.GetBirthDate;

                int intValue = rand.Next(300, 501);
                double keskH = intValue / 100.0;

                Liik liik = new Liik(nimi, perenimi, synnipaev, keskH);
                Console.WriteLine(liik.Nimi);
                Console.WriteLine(liik.Perenimi);
                Console.WriteLine(liik.Sündimispäev);
                Console.WriteLine(liik.Keskhinne);


                Grupp.Add(liik);
                Console.WriteLine();
            }

            return Grupp;
        }
    }

    public class Program
    {
        public static void Main()
        {
            Console.WriteLine(new IdCode("27605030298").GetFullYear());  // 1876
            Console.WriteLine(new IdCode("37605030299").GetFullYear());  // 1976
            Console.WriteLine(new IdCode("50005200009").GetFullYear());  // 2000
            Console.WriteLine(new IdCode("27605030298").GetBirthDate());  // 03.05.1876
            Console.WriteLine(new IdCode("37605030299").GetBirthDate());  // 03.05.1976
            Console.WriteLine(new IdCode("50005200009").GetBirthDate());  // 20.05.2000

            Console.WriteLine(new IdCode("a").IsValid());  // False
            Console.WriteLine(new IdCode("123").IsValid());  // False
            Console.WriteLine(new IdCode("37605030299").IsValid());  // True
                                                                     // 30th February
            Console.WriteLine(new IdCode("37602300299").IsValid());  // False
            Console.WriteLine(new IdCode("52002290299").IsValid());  // False
            Console.WriteLine(new IdCode("50002290231").IsValid());  // True
            Console.WriteLine(new IdCode("30002290231").IsValid());  // False

            // control number 2nd round
            Console.WriteLine(new IdCode("51809170123").IsValid());  // True
            Console.WriteLine(new IdCode("39806302730").IsValid());  // True

            // control number 3rd round
            Console.WriteLine(new IdCode("60102031670").IsValid());  // True
            Console.WriteLine(new IdCode("39106060750").IsValid());  // True
        }
    }
}
