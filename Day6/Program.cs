using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        // Demonstrasi penggunaan enumerator dengan yield return
        // Menggunakan enumerator untuk mendapatkan bilangan genap dari 0 hingga n

        Console.WriteLine("n = ");
        int n = int.Parse(Console.ReadLine());

        foreach (int angka in GetEvenNumbers(n))
        {
            Console.WriteLine(angka);
        }

        // Demonstrasi penggunaan operator overloading pada struct Fraction

        Fraction a = new Fraction(1, 2);
        Fraction b = new Fraction(1, 3);

        Fraction resultAdd = a + b; 
        Fraction resultMultiply = a * b; 

        Console.WriteLine($"Hasil penjumlahan {a} + {b} = {resultAdd}");
        Console.WriteLine($"Hasil perkalian {a} * {b} = {resultMultiply}");

        // Prakter seputar null type
        int? usia = 20; // Nullable int, bisa null atau memiliki nilai

        if (usia.HasValue)
        {
            Console.WriteLine($"Usia kamu: {usia.Value}");
        }
        else
        {
            Console.WriteLine("Usia belum diisi.");
        }

        int usiaFinal = usia ?? 18;
        Console.WriteLine($"Usia default: {usiaFinal}");

        
    }

    // iterator method dengan yield return untuk demonstrasi enumerator
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