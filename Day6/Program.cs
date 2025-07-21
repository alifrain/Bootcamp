using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Console.WriteLine("n = ");
        int n = int.Parse(Console.ReadLine());

        foreach (int angka in GetEvenNumbers(n))
        {
            Console.WriteLine(angka);
        }

        
    }

    // TODO: Buat iterator method dengan yield return
    static IEnumerable<int> GetEvenNumbers(int n)
    {
        for (int i = 0; i <= n; i++)
        {
            if (i % 3 == 0)
            {
                yield return i;
            }
        }
    }
}
