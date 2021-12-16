// See https://aka.ms/new-console-template for more information
using Sha256Prog;
using System.Collections;
using System.Text;

//Console.WriteLine("Hello, World!");



//ConstantGenerator.DevelopmentInit();

Sha256Hasher.Init();
//RandomBenchmark.StartBenchmark(100);
Init();

static void Init()
{
    while (true)
    {
        Console.WriteLine("Please input string to hash");
        string tohash = Console.ReadLine();
        string hash = Sha256Hasher.SHA256Encrypt(tohash);
        Console.WriteLine("SHA256 hash: " + hash);
        Console.WriteLine();
    }
}

//Console.WriteLine(StringToBinary(tohash));

