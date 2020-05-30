using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericsChainlist
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("请输入整数列表：");
            string[] tmp_arr= Console.ReadLine().Split(" ");
            List<int> ls = new List<int>();
            int len = tmp_arr.Length;
           foreach (string s in tmp_arr)
            {
                ls.Add(int.Parse(s));
            }
            int sum = 0, max = ls[0], min = ls[0];
            double avg = 0;
            Action<int> print = delegate (int i) {
                Console.Write(i+" ");
            };
            ls.ForEach(print);
            Console.WriteLine();
            ls.ForEach(i => { sum += i; max = max < i ? i : max; min = min > i ? i : min; });
            avg = sum *1.0/ len;
            Console.Write($"求和：{sum} 最大值：{max} 最小值: {min} 平均值：{avg}");
        }
    }
}
