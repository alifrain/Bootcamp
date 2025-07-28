using System;
using System.Collections.Generic;
using System.Linq;

// Enums and Records
public enum ColorType
{
    None,
    Black,
    White
}

public record struct Position(int Row, int Col);

// Interfaces
public interface IPiece
{
    ColorType Type { get; }
}

public interface IPlayer
{
    string Username { get; }
    int Score { get; set; }
}

public interface IBoard
{
    IPiece[,] Grid { get; }
}

// Concrete Classes
public class Piece : IPiece
{
    public ColorType Type { get; }

    public Piece(ColorType type)
    {
        Type = type;
    }
}

public class Player : IPlayer
{
    public string Username { get; }
    public int Score { get; set; } = 2;

    public Player(string name)
    {
        Username = name;
    }
}

public class Board : IBoard
{
    public IPiece[,] Grid { get; }

    public Board()
    {
        Grid = new IPiece[8, 8];
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        // Initialize all positions with empty pieces
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                Grid[row, col] = new Piece(ColorType.None);
            }
        }

        // Set initial Othello positions
        Grid[3, 3] = new Piece(ColorType.White);
        Grid[3, 4] = new Piece(ColorType.Black);
        Grid[4, 3] = new Piece(ColorType.Black);
        Grid[4, 4] = new Piece(ColorType.White);
    }
}

public class GameController
{
    private IBoard _board;
    private Dictionary<IPlayer, IPiece> _players;
    private int[,] _directions;
    private IPlayer _currentPlayer;

    public event Action OnBoardUpdated;
    public event Action<string> OnGameEnded;

    public GameController(IPlayer player1, IPlayer player2, IPiece piece1, IPiece piece2)
    {
        _board = new Board();
        _players = new Dictionary<IPlayer, IPiece>
        {
            { player1, piece1 },
            { player2, piece2 }
        };
        
        // 8 directions: N, NE, E, SE, S, SW, W, NW
        _directions = new int[8, 2]
        {
            {-1, 0}, {-1, 1}, {0, 1}, {1, 1},
            {1, 0}, {1, -1}, {0, -1}, {-1, -1}
        };

        _currentPlayer = player1;
    }

    public void StartGame()
    {
        UpdateScore();
        DisplayBoard();
        
        Console.WriteLine($"Game started! {_currentPlayer.Username}'s turn ({_players[_currentPlayer].Type}).");
        
        while (!IsGameOver())
        {
            var validMoves = GetValidMoves(_board, new Dictionary<IPlayer, IPiece> { { _currentPlayer, _players[_currentPlayer] } });
            
            if (validMoves.Count == 0)
            {
                Console.WriteLine($"No valid moves for {_currentPlayer.Username}. Skipping turn.");
                SwitchTurn();
                Console.WriteLine($"\n{_currentPlayer.Username}'s turn ({_players[_currentPlayer].Type}).");
                continue;
            }

            Console.WriteLine($"\nValid moves: {string.Join(", ", validMoves.Select(p => $"({p.Row},{p.Col})"))}");
            
            var move = GetPlayerMove(validMoves);
            ApplyMove(move, new Dictionary<IPlayer, IPiece> { { _currentPlayer, _players[_currentPlayer] } });
            
            UpdateScore();
            DisplayBoard();
            DisplayScore();
            
            if (IsGameOver())
            {
                EndGame();
                break;
            }
            
            SwitchTurn();
            Console.WriteLine($"\n{_currentPlayer.Username}'s turn ({_players[_currentPlayer].Type}).");
        }
    }

    public void UpdateScore()
    {
        foreach (var playerPair in _players)
        {
            int count = 0;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (_board.Grid[row, col].Type == playerPair.Value.Type)
                    {
                        count++;
                    }
                }
            }
            playerPair.Key.Score = count;
        }
    }

    public Position MakeMove(IBoard board, List<Position> validMoves, Dictionary<IPlayer, IPiece> player)
    {
        // This method is kept for interface compatibility
        // Actual player input is handled by GetPlayerMove
        return validMoves.First();
    }

    private Position GetPlayerMove(List<Position> validMoves)
    {
        while (true)
        {
            Console.Write($"Enter your move (row,col) [0-7]: ");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Invalid input. Please try again.");
                continue;
            }

            // Handle different input formats: "3,4" or "3 4" or "34"
            var parts = input.Replace(",", " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 1 && parts[0].Length == 2)
            {
                // Handle format like "34" (row 3, col 4)
                if (int.TryParse(parts[0][0].ToString(), out int row1) && 
                    int.TryParse(parts[0][1].ToString(), out int col1))
                {
                    parts = new[] { row1.ToString(), col1.ToString() };
                }
            }

            if (parts.Length != 2)
            {
                Console.WriteLine("Invalid format. Use format like: 3,4 or 3 4");
                continue;
            }

            if (!int.TryParse(parts[0], out int row) || !int.TryParse(parts[1], out int col))
            {
                Console.WriteLine("Invalid numbers. Please enter valid coordinates.");
                continue;
            }

            if (row < 0 || row > 7 || col < 0 || col > 7)
            {
                Console.WriteLine("Coordinates must be between 0 and 7.");
                continue;
            }

            var position = new Position(row, col);
            if (!validMoves.Contains(position))
            {
                Console.WriteLine($"Invalid move! Position ({row},{col}) is not a valid move.");
                Console.WriteLine($"Valid moves are: {string.Join(", ", validMoves.Select(p => $"({p.Row},{p.Col})"))}");
                continue;
            }

            return position;
        }
    }

    public List<Position> GetValidMoves(IBoard board, Dictionary<IPlayer, IPiece> player)
    {
        var validMoves = new List<Position>();
        var playerPiece = player.Values.First();

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                if (board.Grid[row, col].Type == ColorType.None)
                {
                    var flippedPositions = GetFlippedPositions(board, row, col, player);
                    if (flippedPositions.Count > 0)
                    {
                        validMoves.Add(new Position(row, col));
                    }
                }
            }
        }

        return validMoves;
    }

    public List<Position> GetFlippedPositions(IBoard board, int row, int col, Dictionary<IPlayer, IPiece> player)
    {
        var flippedPositions = new List<Position>();
        var playerColor = player.Values.First().Type;
        var opponentColor = GetOpponentColor(playerColor);

        // Check all 8 directions
        for (int dir = 0; dir < 8; dir++)
        {
            var tempFlipped = new List<Position>();
            int currentRow = row + _directions[dir, 0];
            int currentCol = col + _directions[dir, 1];

            // Look for opponent pieces in this direction
            while (IsValidPosition(currentRow, currentCol) && 
                   board.Grid[currentRow, currentCol].Type == opponentColor)
            {
                tempFlipped.Add(new Position(currentRow, currentCol));
                currentRow += _directions[dir, 0];
                currentCol += _directions[dir, 1];
            }

            // If we found opponent pieces and ended with our piece, these are valid flips
            if (tempFlipped.Count > 0 && 
                IsValidPosition(currentRow, currentCol) && 
                board.Grid[currentRow, currentCol].Type == playerColor)
            {
                flippedPositions.AddRange(tempFlipped);
            }
        }

        return flippedPositions;
    }

    public Dictionary<IPlayer, IPiece> GetOpponent(Dictionary<IPlayer, IPiece> player)
    {
        var currentPlayerKey = player.Keys.First();
        var opponent = _players.Keys.First(p => p != currentPlayerKey);
        return new Dictionary<IPlayer, IPiece> { { opponent, _players[opponent] } };
    }

    public void ApplyMove(Position pos, Dictionary<IPlayer, IPiece> player)
    {
        var playerPiece = player.Values.First();
        var flippedPositions = GetFlippedPositions(_board, pos.Row, pos.Col, player);

        // Place the new piece
        ((Board)_board).Grid[pos.Row, pos.Col] = new Piece(playerPiece.Type);

        // Flip the captured pieces
        foreach (var flipPos in flippedPositions)
        {
            ((Board)_board).Grid[flipPos.Row, flipPos.Col] = new Piece(playerPiece.Type);
        }
    }

    public Dictionary<IPlayer, IPiece> GetOpponentType(Dictionary<IPlayer, IPiece> player)
    {
        return GetOpponent(player);
    }

    public void SwitchTurn()
    {
        _currentPlayer = _players.Keys.First(p => p != _currentPlayer);
    }

    public IBoard GetBoard()
    {
        return _board;
    }

    public void ResetBoard()
    {
        _board = new Board();
        foreach (var player in _players.Keys)
        {
            player.Score = 2;
        }
        _currentPlayer = _players.Keys.First();
    }

    public void EndGame()
    {
        UpdateScore();
        var winner = _players.Keys.OrderByDescending(p => p.Score).First();
        var loser = _players.Keys.OrderBy(p => p.Score).First();
        
        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine("                 GAME OVER!");
        Console.WriteLine(new string('=', 50));
        
        if (winner.Score == loser.Score)
        {
            Console.WriteLine("It's a TIE!");
        }
        else
        {
            Console.WriteLine($"🎉 WINNER: {winner.Username}!");
            Console.WriteLine($"Final Score: {winner.Username} {winner.Score} - {loser.Score} {loser.Username}");
        }
        
        var message = winner.Score == loser.Score ? "Game ended in a tie!" : 
                     $"Game Over! Winner: {winner.Username} with {winner.Score} pieces!";
        
        OnGameEnded?.Invoke(message);
    }

    // Helper methods
    private void DisplayBoard()
    {
        Console.WriteLine("\n    0 1 2 3 4 5 6 7");
        Console.WriteLine("  ┌─────────────────┐");
        
        for (int row = 0; row < 8; row++)
        {
            Console.Write($"{row} │ ");
            for (int col = 0; col < 8; col++)
            {
                char symbol = _board.Grid[row, col].Type switch
                {
                    ColorType.Black => 'B',
                    ColorType.White => 'W',
                    ColorType.None => '.'
                };
                Console.Write(symbol + " ");
            }
            Console.WriteLine("│");
        }
        Console.WriteLine("  └─────────────────┘");
    }

    private void DisplayScore()
    {
        Console.WriteLine("\n📊 CURRENT SCORE:");
        foreach (var playerPair in _players)
        {
            char symbol = playerPair.Value.Type == ColorType.Black ? 'B' : 'W';
            Console.WriteLine($"   {playerPair.Key.Username} ({symbol}): {playerPair.Key.Score}");
        }
    }
    private bool IsValidPosition(int row, int col)
    {
        return row >= 0 && row < 8 && col >= 0 && col < 8;
    }

    private ColorType GetOpponentColor(ColorType color)
    {
        return color == ColorType.Black ? ColorType.White : ColorType.Black;
    }

    private bool IsGameOver()
    {
        // Game is over if no valid moves for both players or board is full
        foreach (var playerPair in _players)
        {
            var validMoves = GetValidMoves(_board, new Dictionary<IPlayer, IPiece> { { playerPair.Key, playerPair.Value } });
            if (validMoves.Count > 0)
            {
                return false;
            }
        }
        return true;
    }
}

// Example usage
public class Program
{
    public static void Main()
    {
        Console.WriteLine("🎮 Welcome to OTHELLO! 🎮");
        Console.WriteLine("═══════════════════════════");
        Console.WriteLine("Rules:");
        Console.WriteLine("• Players take turns placing pieces");
        Console.WriteLine("• You must capture opponent pieces by flanking them");
        Console.WriteLine("• Enter coordinates as: row,col (example: 3,4 or 34)");
        Console.WriteLine("• Coordinates range from 0-7");
        Console.WriteLine("• B = Black pieces, W = White pieces");
        Console.WriteLine();

        // Get player names
        Console.Write("Enter Player 1 name (Black B): ");
        string player1Name = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(player1Name)) player1Name = "Player 1";

        Console.Write("Enter Player 2 name (White W): ");
        string player2Name = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(player2Name)) player2Name = "Player 2";

        // Create players
        var player1 = new Player(player1Name);
        var player2 = new Player(player2Name);

        // Create pieces
        var blackPiece = new Piece(ColorType.Black);
        var whitePiece = new Piece(ColorType.White);

        // Create game controller
        var game = new GameController(player1, player2, blackPiece, whitePiece);

        // Subscribe to events
        game.OnBoardUpdated += () => { /* Board display is handled in game loop */ };
        game.OnGameEnded += (message) => Console.WriteLine($"\n🎊 {message}");

        Console.WriteLine("\n🚀 Starting game...");
        
        try
        {
            // Start the game
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