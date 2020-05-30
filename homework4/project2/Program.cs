using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alarm
{
    class Program
    {
        public delegate void InformHandler(Clock clock);

        static void Tick(Clock clock)
        {
            Console.WriteLine($"{clock.now_time_h}:{clock.now_time_m}:{clock.now_time_s}   ");
        }

        static void Alarm(Clock clock)
        {
            Console.WriteLine($"{clock.now_time_h}:{clock.now_time_m}:{clock.now_time_s}  alarm!!!");
        }

        public class Clock
        {
            public InformHandler tick, alarm;
            public int now_time_h, now_time_m, now_time_s;
            public int alarm_time_h, alarm_time_m, alarm_time_s;
            public Clock(int alarm_time_h, int alarm_time_m, int alarm_time_s)
            {
                now_time_h = DateTime.Now.Hour;
                now_time_m = DateTime.Now.Minute;
                now_time_s = DateTime.Now.Second;
                this.alarm_time_h = alarm_time_h;
                this.alarm_time_m = alarm_time_m;
                this.alarm_time_s = alarm_time_s;
            }
            public void Strat()
            {
                int position = 7;
                Console.WriteLine("Strat!");
                while (true)
                {
                    Console.SetCursorPosition(0, position);
                    if (now_time_h == alarm_time_h && now_time_m == alarm_time_m && now_time_s == alarm_time_s)
                    {
                        alarm(this);
                        position++;
                    }
                    else
                    {
                        tick(this);
                    }
                    System.Threading.Thread.Sleep(1000);
                    now_time_s++;
                    if (now_time_s == 60)
                    {
                        now_time_s = 0;
                        now_time_m++;
                    }
                    if(now_time_m == 60)
                    {
                        now_time_m = 0;
                        now_time_h++;
                    }
                    if(now_time_h == 24)
                    {
                        now_time_h = 0;
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            Clock clock;
            Console.WriteLine("Alarm time:(hour)");
            string str1 = Console.ReadLine();
            Console.WriteLine("Alarm time:(minute)");
            string str2 = Console.ReadLine();
            Console.WriteLine("Alarm time:(second)");
            string str3 = Console.ReadLine();
            try {
                int hour = Int32.Parse(str1);
                int minute = Int32.Parse(str2);
                int second = Int32.Parse(str3);
                if (hour < 0 || hour >= 24 || minute < 0 || minute >= 60 || second < 0 || second >= 60)
                {
                    Console.WriteLine("Time Error!");
                    Console.ReadKey();
                    return;
                }
                clock = new Clock(hour, minute, second);
            }
            catch (FormatException)
            {
                Console.WriteLine("Input Error!");
                Console.ReadKey();
                return;
            }
            clock.tick += Tick;
            clock.alarm += Alarm;
            clock.Strat();
        }
    }
}
