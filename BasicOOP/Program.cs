using System;
using Students;

class Program
{
    static void Main()
    {
        Console.Write("Masukkan angka pertama: ");
        int a = int.Parse(Console.ReadLine());

        Console.Write("Masukkan angka kedua: ");
        int b = int.Parse(Console.ReadLine() ?? "0");

        int hasil = a + b;
        Console.WriteLine($"Hasil penjumlahan {a} + {b} = {hasil}");

        int min = a - b;
        Console.WriteLine($"Hasil pengurangan {a} - {b} = {min}");

        int kali = a * b;
        Console.WriteLine($"Hasil perkalian {a} * {b} = {kali}");

        int bagi = a / b;
        if (b == 0)
        {
            Console.WriteLine("Pembagian dengan nol tidak diperbolehkan.");
            return;
        }
        Console.WriteLine($"Hasil pembagian {a} / {b} = {bagi}");

        Student s1 = new Student("Budi", "123456", 2.5);
        s1.DisplayInfo();
        s1.CheckIpk();
    }
         
}