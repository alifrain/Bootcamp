using System;

class Program
{
    static void Main()
    {
        // Declaration statement
        int angka = 7;

        // Selection statement (if-else)
        if (angka % 2 == 0)
        {
            Console.WriteLine("Angka genap");
        }
        else
        {
            Console.WriteLine("Angka ganjil");
        }

        // Switch statement
        switch (angka)
        {
            case 1:
                Console.WriteLine("Satu");
                break;
            case 7:
                Console.WriteLine("Tujuh");
                break;
            default:
                Console.WriteLine("Angka lain");
                break;
        }

        // Iteration statement (for loop)
        for (int i = 1; i <= 5; i++)
        {
            if (i == 3)
                continue; // lompat ke iterasi berikutnya

            Console.WriteLine("Loop ke-" + i);

            if (i == 4)
                break; // keluar dari loop
        }

        // Return statement
        int hasil = Tambah(5, 3);
        Console.WriteLine("Hasil tambah: " + hasil);
    }

    // Method dengan return statement
    static int Tambah(int a, int b)
    {
        return a + b;
    }
}
