class Program
{
    static void Main()
    {   
        // Create object
        BookWithPrice expensiveBook = new BookWithPrice("Clean Code", "Robert Martin", 464, 45.99m,50);
        
        expensiveBook.DisplayInfo();
        Console.WriteLine($"Is expensive? {expensiveBook.IsExpensive()}");
        Console.WriteLine($"Short description: {expensiveBook.GetShortDescription()}");
        
    }
}