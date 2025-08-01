using System;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        Console.WriteLine("Fetching user data");

        Task<string> userTask = GetUserDataAsync();

        while (!userTask.IsCompleted)
        {
            Console.Write("....");
            Task.Delay(500).Wait(); // Simulate waiting
        }

        Console.WriteLine("Please wait while we load your info...");

        string userData = userTask.Result;
        Console.WriteLine($"User Data: {userData}");
    }
    static Task<string> GetUserDataAsync()
    {
        return Task.Run(() =>
        {
            return "Name: Alif Rahman, Age: 22, Status: Active";
        });
    }
}
