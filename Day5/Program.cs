using System;
using System.Buffers;
using MyApp;


class Program
{

    static void RespondToClick(string msg)
    {
        Console.WriteLine($"[Subscriber] Received message: {msg}");
    }
    static void Main()
    {
        Operation op = MathOperations.Add;
        Console.WriteLine(op(5));

        Operation op2 = MathOperations.Subtract;
        Console.WriteLine(op2(5));

        Button btn = new Button();

        btn.Clicked += RespondToClick;

        btn.Press();
    }

}