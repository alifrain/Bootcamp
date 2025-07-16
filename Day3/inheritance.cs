using System;

// Class Book
public class Book
{
    // Fields tempat untuk store data
    private string title;
    private string author;
    private int pages;
    
    // Constructor for when we create new data
    public Book(string bookTitle, string bookAuthor, int pageCount)
    {
        title = bookTitle;
        author = bookAuthor;
        pages = pageCount;
    }
    
    // Properties untuk mengkontrol akses ke private
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
    
    // Methods untuk outputnya
    public virtual void DisplayInfo()  // dengan virtual karena mau di override nanti saat ingin menambahkan inheritance
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


// Inheritance basic
public class BookWithPrice : Book
{
    private decimal price;
    private decimal discount;

    // Constructor untuk memanggil constructor base
    public BookWithPrice(string title, string author, int pages, decimal bookPrice, decimal bookDiscount)
        : base(title, author, pages)
    {
        price = bookPrice;
        discount = bookDiscount;
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
    
    public decimal Discount
    {
        get { return discount; }
        set 
        { 
            if (value >= 0)
                discount = value;
            else
                Console.WriteLine("Discount cannot be negative!");
        }
    }

    // Override display info agar include fields dari inheritance
    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"Price: ${price:F2}");
        Console.WriteLine($"Discount : ${discount:F2}");
    }
    
    // Method untuk price
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