using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example1
{
    class Program
    {
        static int Prase(string str)
        {
            int n = -1;
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (c == '+' || c == '-' || c == '*' || c == '/')
                    n = n == -1 ? i : -1;
            }
            return n;
        }
        static void Main(string[] args)
        {
            string str;
            int a, b, num;
            char c;
            double ans;
            while (true)
            {
                str = Console.ReadLine();
                if (str == "")
                {
                    Console.WriteLine("Over");
                    Console.ReadKey();
                    return;
                }
                num = Prase(str);
                if (num == -1)
                {
                    Console.WriteLine("Error");
                    Console.ReadKey();
                    return;
                }
                c = str[num];
                a = Int32.Parse(str.Remove(num));
                b = Int32.Parse(str.Substring(num + 1, str.Length - num - 1));
                if (c == '+')
                    ans = a + b;
                else if (c == '-')
                    ans = a - b;
                else if (c == '*')
                    ans = a * b;
                else if (c == '/')
                    ans = (double)a / b;
                else
                {
                    Console.WriteLine("Error");
                    Console.ReadKey();
                    return;
                }
                Console.WriteLine($"ans = {ans}");
            }
        }
    }
}
