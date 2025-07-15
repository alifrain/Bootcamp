using System;

class Program
{
    static void Main()
    {
        Book b1 = new Book("C Mastery", "Alif", 2002);

        b1.PrintDetail();
        if (b1.IsNew())
        {
            Console.WriteLine("New Books");
        }
        else
        {
            Console.WriteLine("Old Books");
        }
    }
}

public class Book
{
    // Property
    public string Name;
    public string Author;
    public int Release;

    // Constructor
    public Book(string name, string author, int release)
    {
        Name = name;
        Author = author;
        Release = release;
    }

    // method 1
    public void PrintDetail()
    {
        Console.WriteLine($"Nama : {Name}");
        Console.WriteLine($"Author : {Author}");
        Console.WriteLine($"Release : {Release}");
    }

    // method 2
    public bool IsNew()
    {
        return Release >= 2000;
    }

}