using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Console.Write("Masukkan nilai n: ");
        int n = int.Parse(Console.ReadLine() ?? "0");

        var rules = new Dictionary<int, string>
        {
            { 3, "foo" },
            { 4, "baz" },
            { 5, "bar" },
            { 7, "jazz" },
            { 9, "huzz" }
        };

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
            
            Console.Write(result);
            
            if (x < n)
                Console.Write(", ");
        }

        Console.WriteLine();
    }
}