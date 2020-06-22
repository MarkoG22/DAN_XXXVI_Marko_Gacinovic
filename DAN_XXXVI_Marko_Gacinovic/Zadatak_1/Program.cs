using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Zadatak_1
{
    class Program
    {
        // object for locking threads
        static readonly object locker = new object();

        // object for getting random numbers
        static Random rnd = new Random();

        // matrix and queue for filling the matrix with random numbers
        static int[,] matrix;
        static Queue<int> queue = new Queue<int>();

        static void Main(string[] args)
        {
            // creating two threads for methods for filling the matrix with random numbers
            Thread t1 = new Thread(() => Matrix());
            Thread t2 = new Thread(() => RandomNumbers());

            // starting the threads
            t1.Start();
            t2.Start();

            // creating two threads for writing and reading from the file
            Thread t3 = new Thread(() => WriteToTheFile());
            Thread t4 = new Thread(() => ReadFromTheFile());

            // starting and joining te third thread
            t3.Start();
            t3.Join();

            // starting the fourth thread
            t4.Start();

            Console.ReadLine();
        }

        /// <summary>
        /// method for filling the matrix
        /// </summary>
        static void Matrix()
        {
            matrix = new int[100, 100];

            // locking the thread until queue fills with 10000 random numbers
            lock (locker)
            {
                while (queue.Count < 10000)
                {
                    Monitor.Wait(locker);
                }
            }

            // filling the matrix
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    matrix[i, j] = queue.Dequeue();                   
                }                
            }            
        }

        /// <summary>
        /// method for getting the random numbers
        /// </summary>
        static void RandomNumbers()
        {
            // lock for filling the queue and notifying the locker with Monitor.Pulse
            lock (locker)
            {
                for (int i = 0; i < 10000; i++)
                {
                    queue.Enqueue(rnd.Next(10, 100));
                }

                // notifying the locker
                Monitor.Pulse(locker);
            }
        }

        /// <summary>
        /// method for writing array elements to the file
        /// </summary>
        static void WriteToTheFile()
        {
            try
            {
                // streamwriter for writing to the file
                using (StreamWriter sw = new StreamWriter("../../OddNumbers.txt"))
                {
                    // backup list for odd numbers from the matrix
                    List<int> backup = new List<int>();

                    // loop for filling the list with odd numbers from the matrix
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

                    // array for odd numbers
                    int[] array = new int[backup.Count];

                    // filling the array and writing the elements to the file
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

        /// <summary>
        /// method for reading from the file
        /// </summary>
        static void ReadFromTheFile()
        {
            try
            {
                // streamreader for reading lines from the files
                using (StreamReader sr = new StreamReader("../../OddNumbers.txt"))
                {
                    string line;

                    // reading and writing lines to the console
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
