// See https://aka.ms/new-console-template for more information
using Sha256Prog;
using System.Collections;
using System.Text;

Console.WriteLine("Hello, World!");

//ConstantGenerator.DevelopmentInit();


RandomBenchmark.StartBenchmark();
//Init();
static void Init()
{
    string[] hashValues = ConstantGenerator.generateConstants(8, RootMode.Square);
    string[] roundConstants = ConstantGenerator.generateConstants(64, RootMode.Cube);

    while (true)
    {
        Console.WriteLine("Please input string to hash");
        string tohash = Console.ReadLine();
        SHA256Encrypt(tohash, hashValues, roundConstants);
        //SHA256Encrypt("hello world", hashValues, roundConstants);
    }
}

//Console.WriteLine(StringToBinary(tohash));

static void SHA256Encrypt(string input, string[] hashValues, string[] roundConstants)
{
    // initialize hash values in binary
    BitArray[] bitHashValues = new BitArray[hashValues.Length];
    BitArray[] bitHashValuesClean = new BitArray[hashValues.Length]; // this will not be modified in the code
    BitArray[] bitRoundConstants = new BitArray[roundConstants.Length];

    for (int i = 0; i < hashValues.Length; i++)
    {
        //bitHashValues.SetValue(HexToBits(hashValues[i]), i);
        bitHashValuesClean.SetValue(HexToBits(hashValues[i]), i);
        //Console.WriteLine("index " + i + " bin: " + BitsToString(bitHashValues[i]));
    }
    for (int i = 0; i < roundConstants.Length; i++)
    {
        bitRoundConstants.SetValue(HexToBits(roundConstants[i]), i);
        //Console.WriteLine("index " + i + " bin: " + BitsToString(bitHashValues[i]));
    }


    Byte[] bytes = Encoding.UTF8.GetBytes(input);

    long bitlength = bytes.Length * 8;
    //Console.WriteLine(bitlength);

    bytes = bytes.Append((byte)0x80).ToArray(); // append 1 (and 7x 0)

    while ((bytes.Length + (64 / 8)) % (512 / 8) > 0)
    {
        bytes = bytes.Append((byte)0x00).ToArray();
    }

    Byte[] byteLength = BitConverter.GetBytes(bitlength); // check here if shit dont work, ved ikke om jeg laver big endian rigtigt.
    //Console.WriteLine(BinaryToString(byteLength));
    for (int i = 0; i < (64 / 8); i++)
    {
        bytes = bytes.Append(byteLength[byteLength.Length - 1 - i]).ToArray();
    }

    //Console.WriteLine(BinaryToString(bytes));

    // skal lave ting så den looper for hver 512 bit chunk

    //Console.WriteLine("loops: " + bytes.Length/64);
    for (int chunkIndex = 0; chunkIndex < bytes.Length / 64; chunkIndex++)
    {
       

        BitArray[] Array32 = new BitArray[64];
        for (int i = 0; i < (512 / 32); i++)
        {
            BitArray bit32 = new BitArray(32);
            for (int a = 0; a < 4; a++)
            {
                byte curbyte = bytes[(chunkIndex * 64) + i * 4 + a];
                //Console.WriteLine("Index: " + ((chunkIndex * 64) + i * 4 + a));
                BitArray bit8 = new BitArray(new byte[] { curbyte });

                for (int b = 0; b < 8; b++)
                {
                    bit32.Set(a * 8 + b, bit8[8 - b - 1]);
                }
            }
            Array32[i] = bit32;
        }
        BitArray blankbit32 = new BitArray(32);
        for (int i = 0; i < 32; i++)
        {
            blankbit32[i] = false; // false = 0, true = 1
        }
        for (int i = 0; i < 64 - 16; i++) // create 48 blank 32x 0s
        {
            Array32[i + 16] = blankbit32;
        }


        for (int i = 16; i < 64; i++)
        {
            //Console.WriteLine("i is " + i);
            var s0_1 = rightrotate(Array32[i - 15], 7);
            var s0_2 = rightrotate(Array32[i - 15], 18);
            var s0_3 = rightshift(Array32[i - 15], 3);

            var s0 = XOR(XOR(s0_1, s0_2), s0_3);
            //Console.WriteLine(BitsToString(s0));


            var s1_1 = rightrotate(Array32[i - 2], 17);
            var s1_2 = rightrotate(Array32[i - 2], 19);
            var s1_3 = rightshift(Array32[i - 2], 10);

            var s1 = XOR(XOR(s1_1, s1_2), s1_3);
            //Console.WriteLine(BitsToString(s1));

            BitArray added = AddBits(AddBits(AddBits(Array32[i - 16], s0), Array32[i - 7]), s1);

            //Console.WriteLine("Setting array index of " + i + " to: " + BitsToString(added));
            Array32.SetValue(added, i);

            //Console.WriteLine(BitArrayToString(Array32));
            //Console.ReadLine();
        }

        //Console.WriteLine(BitArrayToString(Array32));



        // S1 = (e rightrotate 6) xor (e rightrotate 11) xor (e rightrotate 25)


        // 0..63 for loop goes here
        //var iloop = 0; // temporary before for loop

        bitHashValues = new BitArray[8];
        for (int i = 0; i < 8; i++)
        {
            bitHashValues[i] = new BitArray(bitHashValuesClean[i]);
        }

        for (int iloop = 0; iloop < 64; iloop++)
        { // maybe 64 is not hardcoded

            var S1_1 = rightrotate(bitHashValues[4], 6);
            var S1_2 = rightrotate(bitHashValues[4], 11);
            var S1_3 = rightrotate(bitHashValues[4], 25);

            var S1 = XOR(XOR(S1_1, S1_2), S1_3);
            //Console.WriteLine("S1: " + BitsToString(S1));

            var EAF = AndBits(bitHashValues[4], bitHashValues[5]);
            //Console.WriteLine("EAF: " + BitsToString(EAF));

            var NotE = NotBits(bitHashValues[4]);
            //Console.WriteLine("NotE: " + BitsToString(NotE));

            var NotEAndG = AndBits(NotE, bitHashValues[6]);
            //Console.WriteLine("NotEAngG: " + BitsToString(NotEAndG));

            var ch = XOR(EAF, NotEAndG);
            //Console.WriteLine("ch: " + BitsToString(ch));

            var temp1 = AddBits(bitHashValues[7], AddBits(S1, AddBits(ch, AddBits(bitRoundConstants[iloop], Array32[iloop]))));
            //Console.WriteLine("temp1: " + BitsToString(temp1));

            var S0_1 = rightrotate(bitHashValues[0], 2);
            var S0_2 = rightrotate(bitHashValues[0], 13);
            var S0_3 = rightrotate(bitHashValues[0], 22);

            var S0 = XOR(XOR(S0_1, S0_2), S0_3);
            //Console.WriteLine("S0: " + BitsToString(S0));

            var AandB = AndBits(bitHashValues[0], bitHashValues[1]);
            var AandC = AndBits(bitHashValues[0], bitHashValues[2]);
            var BandC = AndBits(bitHashValues[1], bitHashValues[2]);

            var maj = XOR(AandB, XOR(AandC, BandC));
            //Console.WriteLine("maj: " + BitsToString(maj));

            var temp2 = AddBits(S0, maj);

            bitHashValues[7] = bitHashValues[6];   //h = g
            bitHashValues[6] = bitHashValues[5];    //g = f
            bitHashValues[5] = bitHashValues[4];    //f = e
            bitHashValues[4] = AddBits(bitHashValues[3], temp1);    //e = d + temp1
            bitHashValues[3] = bitHashValues[2];    //d = c
            bitHashValues[2] = bitHashValues[1];    //c = b
            bitHashValues[1] = bitHashValues[0];   //b = a
            bitHashValues[0] = AddBits(temp1, temp2);    //a = temp1 + temp2
        }
        //after for loop

        // need new loop to remake hashes here !!
        for (int i = 0; i < bitHashValues.Length; i++)
        {
            bitHashValuesClean[i] = AddBits(bitHashValuesClean[i], bitHashValues[i]);
        }
    }
    //throw new Exception();

    /*BitArray[] bitHashValuesClean = new BitArray[hashValues.Length]; // DEV
    for (int i = 0; i < hashValues.Length; i++)
    {
        bitHashValuesClean.SetValue(HexToBits(hashValues[i]), i);
        //Console.WriteLine("index " + i + " bin: " + BitsToString(bitHashValues[i]));
    }
    for (int i = 0; i < bitHashValuesClean.Length; i++)
    {
        Console.WriteLine(i + " : " + BitsToString(bitHashValuesClean[i]));
    }
    Console.WriteLine("----------------"); // DEV


    for (int i = 0; i < bitHashValues.Length; i++)
    {
        Console.WriteLine(i + " : " + BitsToString(bitHashValues[i]));
    }*/

    string finalSha256Hash = "";

    for (int i = 0; i < bitHashValues.Length; i++)
    {
        //var hashPlusClean = AddBits(bitHashValues[i], bitHashValuesClean[i]);
        //finalSha256Hash += BitsToHex(hashPlusClean);
        finalSha256Hash += BitsToHex(bitHashValuesClean[i]);
    }

    Console.WriteLine("Final SHA256 Hash:");
    Console.WriteLine(finalSha256Hash);

    

}



static BitArray HexToBits(string hex)
{
    Dictionary<char, bool[]> hexDictionary = new Dictionary<char, bool[]>()
    {
        { "0"[0], new bool[4] { false, false, false, false } },
        { "1"[0], new bool[4] { false, false, false, true } },
        { "2"[0], new bool[4] { false, false, true, false } },
        { "3"[0], new bool[4] { false, false, true, true } },
        { "4"[0], new bool[4] { false, true, false, false } },
        { "5"[0], new bool[4] { false, true, false, true } },
        { "6"[0], new bool[4] { false, true, true, false } },
        { "7"[0], new bool[4] { false, true, true, true } },
        { "8"[0], new bool[4] { true, false, false, false } },
        { "9"[0], new bool[4] { true, false, false, true } },
        { "A"[0], new bool[4] { true, false, true, false } },
        { "B"[0], new bool[4] { true, false, true, true } },
        { "C"[0], new bool[4] { true, true, false, false } },
        { "D"[0], new bool[4] { true, true, false, true } },
        { "E"[0], new bool[4] { true, true, true, false } },
        { "F"[0], new bool[4] { true, true, true, true } }
    };

    var boollist = new bool[0];

    for (int a = 0; a < hex.Length; a++)
    {

        for (int i = 0; i < 4; i++)
        {
            boollist = boollist.Append(hexDictionary[hex[a]][i]).ToArray();
        }
    }

    return new BitArray(boollist);
}

static string BitsToHex(BitArray bits)
{
    Dictionary<bool[], char> bitDictionary = new Dictionary<bool[], char>()
    {
        { new bool[4] { false, false, false, false }, "0"[0] },
        { new bool[4] { false, false, false, true }, "1"[0] },
        { new bool[4] { false, false, true, false }, "2"[0] },
        { new bool[4] { false, false, true, true }, "3"[0] },
        { new bool[4] { false, true, false, false }, "4"[0] },
        { new bool[4] { false, true, false, true }, "5"[0] },
        { new bool[4] { false, true, true, false }, "6"[0] },
        { new bool[4] { false, true, true, true }, "7"[0] },
        { new bool[4] { true, false, false, false }, "8"[0] },
        { new bool[4] { true, false, false, true }, "9"[0] },
        { new bool[4] { true, false, true, false }, "A"[0] },
        { new bool[4] { true, false, true, true }, "B"[0] },
        { new bool[4] { true, true, false, false }, "C"[0] },
        { new bool[4] { true, true, false, true }, "D"[0] },
        { new bool[4] { true, true, true, false }, "E"[0] },
        { new bool[4] { true, true, true, true }, "F"[0] },
    };

    string finalString = "";

    for (int i = 0; i < bits.Length/4; i++)
    {
        bool[] tempbool = new bool[4];
        for (int a = 0; a < 4; a++)
        {
            tempbool[a] = (bool)bits[i * 4 + a];
        }
        //var thisbool = bitDictionary.Keys.ToList()[11];
        //Console.WriteLine(bitDictionary.Keys.ToList().Contains(tempbool));
        //tempbool = tempbool.ToArray();

        bool[] match = new bool[0];
        
        for (int j = 0; j < bitDictionary.Count; j++)
        {
            var keyarray = bitDictionary.Keys.ToArray();
            bool matched = true;
            for (int k = 0; k< 4; k++)
            {
                if (keyarray[j][k] != tempbool[k])
                {
                    matched = false;
                    break;
                }
            }
            if (matched)
            {
                match = keyarray[j];
            }
        }

        finalString += bitDictionary[match];
    }

    return finalString;
}

static BitArray NotBits(BitArray arr1) // if number overflows, remove last 1
{
    arr1 = new BitArray(arr1);
    for (int i = 0; i < arr1.Length; i++)
    {
        arr1[i] = !arr1[i];
    }
    return arr1;
}

    static BitArray AndBits(BitArray arr1, BitArray arr2) // if number overflows, remove last 1
{
    arr1 = new BitArray(arr1); // If this is not isolated, it will change the previous variable

    for (int i = 0; i < arr1.Length; i++)
    {
        int index = arr1.Length - i - 1;


        if (arr1[index] == true && arr2[index] == true)
        {
            arr1.Set(index, true);

        } else
        {
            arr1.Set(index, false);
        }
    }

    return arr1;
}

static BitArray AddBits(BitArray arr1, BitArray arr2) // if number overflows, remove last 1
{
    arr1 = new BitArray(arr1); // If this is not isolated, it will change the previous variable
    BitArray overlapbits = new BitArray(arr1.Length);
    overlapbits.Set(arr1.Length-1, false);
    bool overlap = false;

    for (int i = 0; i < arr1.Length; i++)
    {
        int index = arr1.Length - i - 1;
        
        
        if (arr1[index] == true && arr2[index] == true)
        {
            // overlap plus
            arr1.Set(index, false);
            if (index > 0)
            {
                overlapbits.Set(index - 1, true);

                overlap = true;
            }
            //Console.WriteLine("overlap1");
            
        } else if (arr1[index] == false && arr2[index] == false)
        {
            // dont plus
            arr1.Set(index, false);
            if (index > 0)
                overlapbits.Set(index - 1, false);

        } else if (arr1[index] == true && arr2[index] == false || arr1[index] == false && arr2[index] == true)
        {
            // plus
            arr1.Set(index, true);
            if (index > 0)
                overlapbits.Set(index - 1, false);
        } else
        {
            throw new Exception("How did be end up here?");
        }
    }

    if (overlap)
    {
        //Console.WriteLine("overlap: " + BitsToString(overlapbits));
        arr1 = AddBits(arr1, overlapbits);
    }

    return arr1;
}

static BitArray XOR(BitArray arr1, BitArray arr2)
{
    if (arr1.Length != arr2.Length)
    {
        throw new Exception("Lengths of array dont match");
    }
    BitArray tempbits = new BitArray(arr1.Length);

    for (int i = 0; i < arr1.Length; i++)
    {
        if (arr1[i] != arr2[i])
        {
            tempbits.Set(i, true);
        }
        else
        {
            tempbits.Set(i, false);
        }
    }
    return tempbits;
}

static BitArray rightshift(BitArray bits, int amount)
{
    BitArray tempbits = new BitArray(bits.Length);

    for (int i = 0; i < amount; i++)
    {
        tempbits.Set(i, false);
    }

    for (int i = 0; i < bits.Length - amount; i++)
    {
        tempbits.Set(i + amount, bits[i]);
    }

    return tempbits;
}

static BitArray rightrotate(BitArray bits, int amount)
{
    BitArray tempbits = new BitArray(bits.Length);

    for (int i = 0; i < amount; i++)
    {
        tempbits.Set(i, bits[bits.Length - amount + i]);
    }

    for (int i = 0; i < bits.Length - amount; i++)
    {
        tempbits.Set(i + amount, bits[i]);
    }

    return tempbits;
}

static string BitArrayToString(BitArray[] bits)
{
    string temp = "";
    for (int a = 0; a < bits.Length; a++)
    {
        for (int i = 0; i < bits[a].Length; i++)
        {
            temp += Convert.ToInt32(bits[a].Get(i));
        }
        temp += " ";
    }
    return temp;
}

static string BitsToString(BitArray bits)
{
    string temp = "";
    for (int i = 0; i < bits.Length; i++)
    {
        temp += Convert.ToInt32(bits.Get(i));
    }
    return temp;
}

static string StringToBinary(string textString)
{
    Byte[] data = Encoding.UTF8.GetBytes(textString);
    return string.Join(" ", data.Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0')));
}

static string BinaryToString(Byte[] input)
{
    return string.Join(" ", input.Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0')));
}