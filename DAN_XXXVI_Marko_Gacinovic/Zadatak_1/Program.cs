using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak_1
{
    class Program
    {
        static readonly object locker = new object();
        static Random rnd = new Random();

        static int[,] matrix;
        static Queue<int> queue = new Queue<int>();

        static void Main(string[] args)
        {
            Thread t1 = new Thread(() => Matrix());
            Thread t2 = new Thread(() => RandomNumbers());

            t1.Start();
            t2.Start();

            Thread t3 = new Thread(() => WriteToTheFile());
            Thread t4 = new Thread(() => ReadFromTheFile());

            t3.Start();
            t3.Join();
            t4.Start();

            Console.ReadLine();
        }

        static void Matrix()
        {
            matrix = new int[100, 100];

            lock (locker)
            {
                while (queue.Count < 10000)
                {
                    Monitor.Wait(locker);
                }
            }

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    matrix[i, j] = queue.Dequeue();                   
                }                
            }            
        }

        static void RandomNumbers()
        {
            lock (locker)
            {
                for (int i = 0; i < 10000; i++)
                {
                    queue.Enqueue(rnd.Next(10, 100));
                }

                Monitor.Pulse(locker);
            }
        }

        static void WriteToTheFile()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("../../OddNumbers.txt"))
                {
                    List<int> backup = new List<int>();

                    for (int i = 0; i < 100; i++)
                    {
                        for (int j = 0; j < 100; j++)
                        {
                            if (matrix[i,j]%2!=0)
                            {
                                backup.Add(matrix[i, j]);
                            }
                        }
                    }

                    int[] array = new int[backup.Count];

                    for (int i = 0; i < backup.Count; i++)
                    {
                        array[i] = backup[i];

                        sw.WriteLine(array[i]);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void ReadFromTheFile()
        {
            try
            {
                using (StreamReader sr = new StreamReader("../../OddNumbers.txt"))
                {
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
