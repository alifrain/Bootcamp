using System;

// Original Book class
public class Book
{
    // Fields (private data)
    private string title;
    private string author;
    private int pages;
    
    // Constructor - runs when you create a new Book
    public Book(string bookTitle, string bookAuthor, int pageCount)
    {
        title = bookTitle;
        author = bookAuthor;
        pages = pageCount;
    }
    
    // Properties - controlled access to private fields
    public string Title
    {
        get { return title; }
        set 
        { 
            if (string.IsNullOrEmpty(value))
                Console.WriteLine("Title cannot be empty!");
            else
                title = value;
        }
    }
    
    public string Author
    {
        get { return author; }
        set 
        { 
            if (string.IsNullOrEmpty(value))
                Console.WriteLine("Author cannot be empty!");
            else
                author = value;
        }
    }
    
    public int Pages
    {
        get { return pages; }
        set 
        { 
            if (value > 0)
                pages = value;
            else
                Console.WriteLine("Pages must be positive!");
        }
    }
    
    // Methods - what the book can do
    public virtual void DisplayInfo()  // Made virtual so it can be overridden
    {
        Console.WriteLine($"Title: {title}");
        Console.WriteLine($"Author: {author}");
        Console.WriteLine($"Pages: {pages}");
    }
    
    public bool IsLongBook()
    {
        return pages > 300;
    }
    
    public string GetShortDescription()
    {
        return $"'{title}' by {author} ({pages} pages)";
    }
}

// PROBLEM: This won't work because Book doesn't have a parameterless constructor
/*
public class Price : Book 
{     
    private int prices; 
    
    // This will cause a compile error!
    // Because Book requires parameters in its constructor
}
*/

// SOLUTION 1: Add a constructor that calls the base constructor
public class BookWithPrice : Book
{
    private decimal price;  // Changed to decimal (better for money)
    
    // Constructor that calls the base Book constructor
    public BookWithPrice(string title, string author, int pages, decimal bookPrice) 
        : base(title, author, pages)  // This calls Book's constructor
    {
        price = bookPrice;
    }
    
    // Property for price
    public decimal Price
    {
        get { return price; }
        set 
        { 
            if (value >= 0)
                price = value;
            else
                Console.WriteLine("Price cannot be negative!");
        }
    }
    
    // Override the DisplayInfo method to include price
    public override void DisplayInfo()
    {
        base.DisplayInfo();  // Call the parent's DisplayInfo first
        Console.WriteLine($"Price: ${price:F2}");
    }
    
    // New method specific to BookWithPrice
    public bool IsExpensive()
    {
        return price > 50;
    }
    
    // Override GetShortDescription to include price
    public new string GetShortDescription()
    {
        return $"'{Title}' by {Author} ({Pages} pages) - ${price:F2}";
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Testing BookWithPrice (Solution 1) ===");
        
        // Create a BookWithPrice object
        BookWithPrice expensiveBook = new BookWithPrice("Clean Code", "Robert Martin", 464, 45.99m);
        
        expensiveBook.DisplayInfo();
        Console.WriteLine($"Is expensive? {expensiveBook.IsExpensive()}");
        Console.WriteLine($"Short description: {expensiveBook.GetShortDescription()}");
        
    }
}

/*
KEY POINTS ABOUT INHERITANCE:

1. CONSTRUCTOR PROBLEM: 
   - If base class has only parameterized constructors, derived class must call one
   - Use ": base(parameters)" to call parent constructor

2. INHERITANCE SYNTAX:
   - "public class Child : Parent" is correct
   - Child inherits all public/protected members from Parent

3. METHOD OVERRIDING:
   - Use "virtual" in base class, "override" in derived class
   - Use "base.MethodName()" to call parent's version

4. ACCESS MODIFIERS:
   - private: Only accessible within the same class
   - protected: Accessible in the class and its derived classes
   - public: Accessible everywhere

5. "IS-A" RELATIONSHIP:
   - BookWithPrice IS a Book (can be used anywhere Book is expected)
   - Inheritance represents "is-a" relationship, not "has-a"

Try these exercises:
1. Add a "discount" field to BookWithPrice
2. Create a method "GetDiscountedPrice()" 
3. Make some Book fields "protected" instead of "private"
4. Create another class that inherits from Book (like "EBook")
*/