namespace MyApp
{
    public delegate int Operation(int x);

    public class MathOperations
    {
        public static int Add(int x)
        {
            return x + 10;
        }
        public static int Subtract(int x)
        {
            return x - 5;
        }

        public static int Multiply(int x)
        {
            return x * 2;
        }
    }

}

