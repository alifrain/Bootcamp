using System;
using Animal;

class Program
{
    static void Main()
    {
        Library library = new Library();
        library.AddSampleBooks();
        library.ShowBooks();

        IAnimal myDog = new Dog();
        IAnimal myCat = new Cat();

        myDog.Speak(); // Output: Woof!
        myCat.Speak(); // Output: Meow!
    }


}
