using System;
using System.Buffers;
using System.Configuration.Assemblies;
using MyApp;


class Program
{
    static void RespondToClick(string msg)
    {
        Console.WriteLine($"[Display] message: {msg}");
    }

    static void ShowTime(string msg)
    {
        Console.WriteLine($"[Timestamp] {DateTime.Now} | {msg}");
    }

    static void ErrorChecker(string msg)
    {
        if (msg.Contains("error"))
        {
            Console.WriteLine($"[Message Invalid Error :] {msg}");
        }
        else
        {
            Console.WriteLine($"[Message valid] {msg}");
        }
    }

    static void Main()
    {
        Operation op = MathOperations.Add;
        Console.WriteLine(op(5));

        Operation op2 = MathOperations.Subtract;
        Console.WriteLine(op2(5));

        Operation op3 = MathOperations.Multiply;
        Console.WriteLine(op3(5));

        Button btn = new Button();

        btn.Clicked += RespondToClick;
        btn.Clicked += ShowTime;
        btn.Clicked += ErrorChecker;

        btn.Press("error");
        Console.WriteLine("");

        btn.Press("Display this message");

        try
        {
            int a = 10;
            int b = 0;

            int hasil = a / b;

            Console.WriteLine($"Hasil penjumlahan {a} / {b} = {hasil}");
        }

        catch (DivideByZeroException)
        {
            Console.WriteLine("Error: Division by zero is not allowed.");
        }
        catch (FormatException)
        {
            Console.WriteLine("Error: Masukkan harus berupa angka.");
        }
        finally
        {
            Console.WriteLine("Program selesai dijalankan.");
        }

    }

}