using System;
using System.Collections.Generic;
using System.ComponentModel;

public class Fizzbuzz
{
    private Dictionary<int, string> rules;

    public Fizzbuzz()
    {
        rules = new Dictionary<int, string>();
    }

    public void AddRule(int input, string output)
    {
        rules[input] = output;
    }

    public void Generate(int n)
    {
        for (int x = 1; x <= n; x++)
        {
            string result = "";

            foreach (var rule in rules)
            {
                if (x % rule.Key == 0)
                {
                    result += rule.Value;
                }
            }
            if (string.IsNullOrEmpty(result))
            {
                result = x.ToString();
            }

            Console.WriteLine(result);

            if (x < n)
            {
                Console.WriteLine(", ");
            }
        }
    }
    
    public void RemoveRule(int input)
    {
        rules.Remove(input);
    }
    public void ClearRules()
    {
        rules.Clear();
    }
    public bool HasRule(int input)
    {
        return rules.ContainsKey(input);
    }
}

class Program {
    static void Main()
    {
        Console.WriteLine("Masukkan angka :");
        int n = int.Parse(Console.ReadLine() ?? "0");

        Fizzbuzz myClass = new Fizzbuzz();

        // menambahkan aturan
        myClass.AddRule(3, "Foo");
        myClass.AddRule(4, "Baz");
        myClass.AddRule(5, "Bar");
        myClass.AddRule(7, "Jazz");
        myClass.AddRule(9, "Huzz");

        Console.WriteLine("Program :");
        myClass.Generate(n);

        // Custom rules
        Console.WriteLine("Custom Rules");

        Console.WriteLine("Masukkan angka :");
        int x = int.Parse(Console.ReadLine() ?? "0");

        Fizzbuzz customFizzbuzz = new Fizzbuzz();
        customFizzbuzz.AddRule(2, "Gozz");
        customFizzbuzz.AddRule(6, "Sizz");

        customFizzbuzz.Generate(x);
    }
}