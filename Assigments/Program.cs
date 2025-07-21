using System;

class Program
{
    static void Main()
    {

        // Masukkan input dulu
        Console.Write("Masukkan nilai n: ");
        int n = int.Parse(Console.ReadLine() ?? "0");

        // Looping
        for (int x = 1; x <= n; x++)
        {
            if (x % 3 == 0 && x % 5 == 0 && x % 7 == 0)
            {
                Console.Write("foobarjazz");
            }
            else if (x % 3 == 0 && x % 5 == 0)
            {
                Console.Write("foobar");
            }
            else if (x % 3 == 0 && x % 7 == 0)
            {
                Console.Write("foojazz");
            }
            else if (x % 5 == 0 && x % 7 == 0)
            {
                Console.Write("barjazz");
            }
            else if (x % 3 == 0)
            {
                Console.Write("foo");
            }
            else if (x % 5 == 0)
            {
                Console.Write("bar");
            }
            else if (x % 7 == 0)
            {
                Console.Write("jazz");
            }
            else
            {
                Console.Write(x);
            }
            if (x < n)
                Console.Write(", ");
        }

        Console.WriteLine();
    }
}
