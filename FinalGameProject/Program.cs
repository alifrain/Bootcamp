using System;
using System.Dynamic;
using Microsoft.VisualBasic;
namespace OthelloGameProject;


public record struct Position(int Row, int Col);

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Welcome to OTHELLO");
        Console.WriteLine("═══════════════════════════");
        Console.WriteLine("Rules:");
        Console.WriteLine("• Players take turns placing pieces");
        Console.WriteLine("• You must capture opponent pieces by flanking them");
        Console.WriteLine("• Enter coordinates as: row,col (example: 3,4 or 34)");
        Console.WriteLine("• Coordinates range from 0-7");
        Console.WriteLine("• B = Black pieces, W = White pieces");
        Console.WriteLine();

        Console.Write("Enter Player 1 name (Black B): ");
        string player1Name = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(player1Name)) player1Name = "Player 1";

        Console.Write("Enter Player 2 name (White W): ");
        string player2Name = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(player2Name)) player2Name = "Player 2";

        var player1 = new Player(player1Name);
        var player2 = new Player(player2Name);

        var blackPiece = new Piece(ColorType.Black);
        var whitePiece = new Piece(ColorType.White);

        var game = new GameController(player1, player2, blackPiece, whitePiece);

        game.OnBoardUpdated += () => { /* Board display is handled in game loop */ };
        game.OnGameEnded += (message) => Console.WriteLine($"\n🎊 {message}");

        Console.WriteLine("\n🚀 Starting game...");

        try
        {
            game.StartGame();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        Console.WriteLine("\nThanks for playing! Press any key to exit...");
        Console.ReadKey();
    }

}
