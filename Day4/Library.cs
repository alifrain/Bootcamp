public class Library
{
    public Book[] books = new Book[5];

    public void AddSampleBooks()
    {
        books[0] = new Book("The Great Gatsby", "F. Scott Fitzgerald", 1925, BookGenre.Fiction);
        books[1] = new Book("To Kill a Mockingbird", "Harper Lee", 1960, BookGenre.Fiction);
        books[2] = new Book("1984", "George Orwell", 1949, BookGenre.Mystery);
        books[3] = new Book("The Catcher in the Rye", "J.D. Salinger", 1951, BookGenre.Fiction);
        books[4] = new Book("The Hobbit", "J.R.R. Tolkien", 1937, BookGenre.Fantasy);
    }

    public void ShowBooks()
    {
        foreach (var book in books)
        {
            if (book != null)
            {
                book.PrintDetails();
                Console.WriteLine(book.IsNew() ? "Status: New Book" : "Status: Old Book");
                Console.WriteLine();
            }
        }
    }

    // Nested class
    public class Book
    {
        public string Title;
        public string Author;
        public int Year;
        public BookGenre Genre;

        public Book(string title, string author, int year, BookGenre genre)
        {
            Title = title;
            Author = author;
            Year = year;
            Genre = genre;
        }

        public void PrintDetails()
        {
            Console.WriteLine($"Title : {Title}");
            Console.WriteLine($"Author: {Author}");
            Console.WriteLine($"Year  : {Year}");
            Console.WriteLine($"Genre : {Genre}");
        }

        public bool IsNew()
        {
            return Year >= 2010;
        }
    }
}

public enum BookGenre
{
    Fiction,
    NonFiction,
    Mystery,
    Fantasy,
    ScienceFiction,
    Biography
}