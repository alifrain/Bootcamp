class Program
{
    static void Main(string[] args)
    {
        Console.Write("Enter a number (n): ");
        int n = int.Parse(Console.ReadLine());

        for (int i = 1; i <= n; i++)
        {
            if (i % 3 == 0 && i % 5 == 0)
            {
                Console.Write("foobar");
            }
            else if (i % 3 == 0)
            {
                Console.Write("foo");
            }
            else if (i % 5 == 0)
            {
                Console.Write("bar");
            }
            else
            {
                Console.Write(i);
            }
        }

    }
}
