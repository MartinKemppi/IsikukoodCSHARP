using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsikukoodCSHARP
{
    class ID
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Sisesta palju inimesi tuli");
            int maxMembers = Convert.ToInt32(Console.ReadLine());
            List<IdCode> members = IdCode.GenerateRandomSisu(maxMembers);
            IdCode.CheckAndDisplayMembers(members);
        }
    }
}