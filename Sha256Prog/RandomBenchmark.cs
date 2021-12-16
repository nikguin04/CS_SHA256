using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sha256Prog
{
    internal class RandomBenchmark
    {
        //public static List<string> toHash = new List<string>();
        public static int running = 0;
        public static int maxthreads = 0;
        public static long startms = 0;
        public static int hashes = 10000;
        public static int hashesleft = 0;
        public static void StartBenchmark(int threads)
        {
            Console.WriteLine("press enter to benchmark");
            Console.ReadLine();
            Console.WriteLine("Running benchmark with " + hashes + " hashes and " + threads + " threads");
            /*for (int i = 0; i < hashes; i++)
            {
                toHash.Add(i.ToString());
            }*/

            maxthreads = threads;
            startms = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            hashesleft = hashes;


            /*for (int i = 0; i < toHash.Count; i++)
            {
                string enc = Sha256Hasher.SHA256Encrypt(toHash[i]);

            }
            long totalms =  DateTimeOffset.Now.ToUnixTimeMilliseconds() - startms;

            Console.WriteLine("Took " + totalms + " ms, to hash " + toHash.Count + " Hashes");*/

            for (int i = 0; i < maxthreads; i++) {
                //running++;
                if (hashesleft > 0)
                {
                    hashesleft--;
                    startThread(hashesleft+1.ToString());
                    
                }
            }
            Console.ReadLine();
        }

        public static void startThread(string newHash)
        {
            running++;
            doWorkCallback callback = new doWorkCallback(startThread);
            Thread workThread = new Thread(() => HashCallback(newHash, callback));
            workThread.Start();

            
        }

        static void HashCallback(string tohash, doWorkCallback callback)
        {
            string hash = Sha256Hasher.SHA256Encrypt(tohash);
            if (hashesleft > 0)
            {
                
                running--;
                string newh = hashesleft.ToString();
                hashesleft--;
                //throw new Exception();
                //Console.WriteLine(toHash.Count);
                callback(newh);
            } else
            {
                running--;
                if (running <= 0)
                {
                    long totalms = DateTimeOffset.Now.ToUnixTimeMilliseconds() - startms;

                    Console.WriteLine("Took " + totalms + " ms, to hash " + hashes + " Hashes");
                }
            }
            

        }

        public static int[] testint = new int[8] { 1,1,1,1,1,1,1,1 };


        public delegate void doWorkCallback(string result);

    }
}
