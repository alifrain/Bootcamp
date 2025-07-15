using System;

class Program
{
    static void Main()
    {
        Students s1 = new Students("Alif", "13119", 3.5);

        s1.PrintInfo();
        if (s1.IsCumlaude())
        {
            Console.WriteLine("Status: Cumlaude");
        }
        else
        {
            Console.WriteLine("Status : Biasa");
        }

        Console.WriteLine(); 

        Students s2 = new Students("Raka", "13120", 3.2);

        s2.PrintInfo();
        if (s2.IsCumlaude())
        {
            Console.WriteLine("Status: Cumlaude");
        }
        else
        {
            Console.WriteLine("Status : Biasa");
        }
    }
}


public class Students
{
    public string Name;
    public string NIM;
    public double GPA;

    public Students(string name, string nim, double gpa)
    {
        Name = name;
        NIM = nim;
        GPA = gpa;
    }

    public void PrintInfo()
    {
        Console.WriteLine($"Nama: {Name}");
        Console.WriteLine($"NIM:{NIM}");
        Console.WriteLine($"IPK: {GPA}");
    }
    public bool IsCumlaude()
    {
        return GPA >= 3.5;
    }
}