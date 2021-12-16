using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sha256Prog
{
    internal class RandomBenchmark
    {
        public static void StartBenchmark()
        {
            static void Init()
            {
                string[] hashValues = ConstantGenerator.generateConstants(8, RootMode.Square);
                string[] roundConstants = ConstantGenerator.generateConstants(64, RootMode.Cube);

                Program prog = new Program();
                while (true)
                {
                    Console.WriteLine("Please input string to hash");
                    string tohash = Console.ReadLine();
                    SHA256Encrypt(tohash, hashValues, roundConstants);
                    //SHA256Encrypt("hello world", hashValues, roundConstants);
                }
            }
        }
    }
}
