using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sha256Prog
{
    public class ConstantGenerator
    {
        public static void DevelopmentInit()
        {
            Console.WriteLine("Square Hash Values:");
            string[] HashValues = generateConstants(8, RootMode.Square);
            for (int i = 0; i < HashValues.Length; i++)
            {
                Console.WriteLine(HashValues[i]);
            }

            Console.WriteLine("");
            Console.WriteLine("Cube Root Constants:");
            string[] RoundConstants = generateConstants(64, RootMode.Cube);
            for (int i = 0; i < RoundConstants.Length; i++)
            {
                Console.WriteLine(RoundConstants[i]);
            }
        }

        public static string[] generateConstants(int length, RootMode rootMode)
        {
            var constantArray = new string[length];
            int arrayIndex = 0;
            var number = 2;

            while (arrayIndex < length)
            {
                int i, bottom, flag = 0;
                bottom = number / 2;
                for (i = 2; i <= bottom; i++)
                {
                    if (number % i == 0)
                    {
                        //Console.Write("Number is not Prime.");
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    //Console.Write("Number is Prime.");
                    constantArray[arrayIndex++] = primeCubeDecimal(number, rootMode);
                }
                number++;
            }
            return constantArray;
        }

        static string primeCubeDecimal(int prime, RootMode rootMode)
        {
            double cube = 0;
            if (rootMode == RootMode.Cube)
            {
                cube = Math.Cbrt(prime);
            }
            else if (rootMode == RootMode.Square)
            {
                cube = Math.Sqrt(prime);
            }
            var cubedecimals = cube - Math.Floor(cube);

            //int testi = Convert.ToInt32("F", 16);
            //Console.WriteLine(cubedecimals);

            return doubleToHex(cubedecimals);
        }

        static string doubleToHex(double decimals)
        {
            var hexLength = 8;
            var returnString = "";

            for (int i = 0; i < hexLength; i++)
            {
                double modulusDecimal = 1 / Math.Pow(16, i + 1);
                //Console.WriteLine(modulusDecimal);

                int hexInt = 0;
                while (decimals > modulusDecimal)
                {
                    hexInt++;
                    decimals -= modulusDecimal;
                }
                returnString += hexInt.ToString("X");
            }

            return returnString;
        }
    }




    public enum RootMode
    {
        Square = 0,
        Cube = 1
    }
}
