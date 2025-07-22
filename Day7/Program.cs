using System;

class Program
{
    static void Main()
    {
        // Menampilkan waktu dan tanggal saat ini
        DateTime Now = DateTime.Now;

        string tanggal = DateTime.Now.ToString("dd/MM/yyyy");
        string waktu = DateTime.Now.ToString("HH:mm:ss");

        Console.WriteLine($"Waktu sekarang: {waktu}");
        Console.WriteLine($"Tanggal hari ini: {tanggal}");

        // Contoh parsing tanggal dari string
        string input = "2025-01-01";
        DateTime tanggalNow = DateTime.Parse(input);
        Console.WriteLine(tanggalNow.ToString("dd/MM/yyyy"));

        
    }
}