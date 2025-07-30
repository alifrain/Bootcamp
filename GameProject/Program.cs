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
    void SetPiece(int row, int col, IPiece piece);
    IPiece GetPiece(int row, int col);
}

// New interface for user interaction (Separation of Concerns)
public interface IGameView
{
    void DisplayBoard(IBoard board);
    void DisplayScore(Dictionary<IPlayer, IPiece> players);
    void ShowMessage(string message);
    void ShowValidMoves(List<Position> validMoves);
    Position GetPlayerMove(List<Position> validMoves, string playerName);
    string GetPlayerName(string prompt);
}

// Concrete Classes
public class Piece : IPiece
{
    public ColorType Type { get; }

    public Piece(ColorType type)
    {
        Type = type;
    }

    public override bool Equals(object? obj)
    {
        return obj is Piece piece && Type == piece.Type;
    }

    public override int GetHashCode()
    {
        return Type.GetHashCode();
    }
}

public class Player : IPlayer
{
    public string Username { get; }
    public int Score { get; set; } = 2;

    public Player(string name)
    {
        Username = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Player name cannot be empty", nameof(name)) : name;
    }
}

public class Board : IBoard
{
    private readonly IPiece[,] _grid;
    public IPiece[,] Grid => (IPiece[,])_grid.Clone(); // Return copy to maintain encapsulation

    public Board()
    {
        _grid = new IPiece[8, 8];
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        // Initialize all positions with None
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                _grid[row, col] = new Piece(ColorType.None);
            }
        }

        // Set initial Othello positions
        _grid[3, 3] = new Piece(ColorType.White);
        _grid[3, 4] = new Piece(ColorType.Black);
        _grid[4, 3] = new Piece(ColorType.Black);
        _grid[4, 4] = new Piece(ColorType.White);
    }

    public void SetPiece(int row, int col, IPiece piece)
    {
        if (!IsValidPosition(row, col))
            throw new ArgumentOutOfRangeException("Invalid board position");
        
        _grid[row, col] = piece ?? throw new ArgumentNullException(nameof(piece));
    }

    public IPiece GetPiece(int row, int col)
    {
        if (!IsValidPosition(row, col))
            throw new ArgumentOutOfRangeException("Invalid board position");
        
        return _grid[row, col];
    }

    private static bool IsValidPosition(int row, int col)
    {
        return row >= 0 && row < 8 && col >= 0 && col < 8;
    }
}

// Console-based implementation of IGameView
public class ConsoleGameView : IGameView
{
    public void DisplayBoard(IBoard board)
    {
        Console.WriteLine("\n    0 1 2 3 4 5 6 7");
        Console.WriteLine("  ┌─────────────────┐");
        
        for (int row = 0; row < 8; row++)
        {
            Console.Write($"{row} │ ");
            for (int col = 0; col < 8; col++)
            {
                char symbol = board.GetPiece(row, col).Type switch
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

    public void DisplayScore(Dictionary<IPlayer, IPiece> players)
    {
        Console.WriteLine("\n📊 CURRENT SCORE:");
        foreach (var playerPair in players)
        {
            char symbol = playerPair.Value.Type == ColorType.Black ? 'B' : 'W';
            Console.WriteLine($"   {playerPair.Key.Username} ({symbol}): {playerPair.Key.Score}");
        }
    }

    public void ShowMessage(string message)
    {
        Console.WriteLine(message);
    }

    public void ShowValidMoves(List<Position> validMoves)
    {
        Console.WriteLine($"Valid moves: {string.Join(", ", validMoves.Select(p => $"({p.Row},{p.Col})"))}");
    }

    public Position GetPlayerMove(List<Position> validMoves, string playerName)
    {
        while (true)
        {
            Console.Write($"{playerName}, enter your move (row,col) [0-7]: ");
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
                ShowValidMoves(validMoves);
                continue;
            }

            return position;
        }
    }

    public string GetPlayerName(string prompt)
    {
        Console.Write(prompt);
        var name = Console.ReadLine()?.Trim();
        return string.IsNullOrEmpty(name) ? "Player" : name;
    }
}

public class GameController
{
    private readonly IBoard _board;
    private readonly Dictionary<IPlayer, IPiece> _players;
    private readonly int[,] _directions;
    private readonly IGameView _gameView;
    private IPlayer _currentPlayer;

    public event Action? OnBoardUpdated;
    public event Action<string>? OnGameEnded;

    // Fixed constructor - now follows Dependency Inversion Principle
    public GameController(IPlayer player1, IPlayer player2, IPiece piece1, IPiece piece2, IBoard board, IGameView gameView)
    {
        _board = board ?? throw new ArgumentNullException(nameof(board));
        _gameView = gameView ?? throw new ArgumentNullException(nameof(gameView));
        
        if (player1 == null) throw new ArgumentNullException(nameof(player1));
        if (player2 == null) throw new ArgumentNullException(nameof(player2));
        if (piece1 == null) throw new ArgumentNullException(nameof(piece1));
        if (piece2 == null) throw new ArgumentNullException(nameof(piece2));

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
        _gameView.DisplayBoard(_board);
        _gameView.ShowMessage($"Game started! {_currentPlayer.Username}'s turn ({_players[_currentPlayer].Type}).");
        
        while (!IsGameOver())
        {
            var validMoves = GetValidMoves(_board, _currentPlayer);
            
            if (validMoves.Count == 0)
            {
                _gameView.ShowMessage($"No valid moves for {_currentPlayer.Username}. Skipping turn.");
                SwitchTurn();
                _gameView.ShowMessage($"\n{_currentPlayer.Username}'s turn ({_players[_currentPlayer].Type}).");
                continue;
            }

            _gameView.ShowValidMoves(validMoves);
            
            var move = _gameView.GetPlayerMove(validMoves, _currentPlayer.Username);
            ApplyMove(move, _currentPlayer);
            
            UpdateScore();
            _gameView.DisplayBoard(_board);
            _gameView.DisplayScore(_players);
            OnBoardUpdated?.Invoke();
            
            if (IsGameOver())
            {
                EndGame();
                break;
            }
            
            SwitchTurn();
            _gameView.ShowMessage($"\n{_currentPlayer.Username}'s turn ({_players[_currentPlayer].Type}).");
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
                    if (_board.GetPiece(row, col).Type == playerPair.Value.Type)
                    {
                        count++;
                    }
                }
            }
            playerPair.Key.Score = count;
        }
    }

    // Kept for interface compatibility but simplified
    public Position MakeMove(IBoard board, List<Position> validMove, Dictionary<IPlayer, IPiece> player)
    {
        return validMove.FirstOrDefault();
    }

    // Improved - no longer creates unnecessary dictionaries
    public List<Position> GetValidMoves(IBoard board, IPlayer player)
    {
        var validMoves = new List<Position>();
        var playerPiece = _players[player];

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                if (board.GetPiece(row, col).Type == ColorType.None)
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

    // Improved - no longer creates unnecessary dictionaries
    public List<Position> GetFlippedPositions(IBoard board, int row, int col, IPlayer player)
    {
        var flippedPositions = new List<Position>();
        var playerColor = _players[player].Type;
        var opponentColor = GetOpponentColor(playerColor);

        // Check all 8 directions
        for (int dir = 0; dir < 8; dir++)
        {
            var tempFlipped = new List<Position>();
            int currentRow = row + _directions[dir, 0];
            int currentCol = col + _directions[dir, 1];

            // Look for opponent pieces in this direction
            while (IsValidPosition(currentRow, currentCol) && 
                   board.GetPiece(currentRow, currentCol).Type == opponentColor)
            {
                tempFlipped.Add(new Position(currentRow, currentCol));
                currentRow += _directions[dir, 0];
                currentCol += _directions[dir, 1];
            }

            // If we found opponent pieces and ended with our piece, these are valid flips
            if (tempFlipped.Count > 0 && 
                IsValidPosition(currentRow, currentCol) && 
                board.GetPiece(currentRow, currentCol).Type == playerColor)
            {
                flippedPositions.AddRange(tempFlipped);
            }
        }

        return flippedPositions;
    }

    // Simplified - returns the actual opponent player
    public IPlayer GetOpponent(IPlayer player)
    {
        return _players.Keys.First(p => p != player);
    }

    // Improved - simplified parameter
    public void ApplyMove(Position pos, IPlayer player)
    {
        var playerPiece = _players[player];
        var flippedPositions = GetFlippedPositions(_board, pos.Row, pos.Col, player);

        // Place the new piece
        _board.SetPiece(pos.Row, pos.Col, new Piece(playerPiece.Type));

        // Flip the captured pieces
        foreach (var flipPos in flippedPositions)
        {
            _board.SetPiece(flipPos.Row, flipPos.Col, new Piece(playerPiece.Type));
        }
    }

    // Fixed method name (was "GetOpponentType" but should match diagram)
    public Dictionary<IPlayer, IPiece> GetOpponentType(Dictionary<IPlayer, IPiece> player)
    {
        var currentPlayerKey = player.Keys.First();
        var opponent = GetOpponent(currentPlayerKey);
        return new Dictionary<IPlayer, IPiece> { { opponent, _players[opponent] } };
    }

    public void SwitchTurn()
    {
        _currentPlayer = GetOpponent(_currentPlayer);
    }

    public IBoard GetBoard()
    {
        return _board;
    }

    public void ResetBoard()
    {
        // Create new board instead of modifying existing one
        var newBoard = new Board();
        
        // Copy pieces from new board to current board
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                _board.SetPiece(row, col, newBoard.GetPiece(row, col));
            }
        }
        
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
        
        var gameOverMessage = "\n" + new string('=', 50) + "\n" +
                             "                 GAME OVER!\n" +
                             new string('=', 50);
        
        _gameView.ShowMessage(gameOverMessage);
        
        if (winner.Score == loser.Score)
        {
            _gameView.ShowMessage("It's a TIE!");
        }
        else
        {
            _gameView.ShowMessage($"🎉 WINNER: {winner.Username}!");
            _gameView.ShowMessage($"Final Score: {winner.Username} {winner.Score} - {loser.Score} {loser.Username}");
        }
        
        var message = winner.Score == loser.Score ? "Game ended in a tie!" : 
                     $"Game Over! Winner: {winner.Username} with {winner.Score} pieces!";
        
        OnGameEnded?.Invoke(message);
    }

    // Helper methods
    private static bool IsValidPosition(int row, int col)
    {
        return row >= 0 && row < 8 && col >= 0 && col < 8;
    }

    private static ColorType GetOpponentColor(ColorType color)
    {
        return color == ColorType.Black ? ColorType.White : ColorType.Black;
    }

    private bool IsGameOver()
    {
        // Game is over if no valid moves for both players
        return _players.Keys.All(player => GetValidMoves(_board, player).Count == 0);
    }
}

// Improved Program class
public class Program
{
    public static void Main()
    {
        try
        {
            var gameView = new ConsoleGameView();
            
            gameView.ShowMessage("🎮 Welcome to OTHELLO! 🎮");
            gameView.ShowMessage("═══════════════════════════");
            gameView.ShowMessage("Rules:");
            gameView.ShowMessage("• Players take turns placing pieces");
            gameView.ShowMessage("• You must capture opponent pieces by flanking them");
            gameView.ShowMessage("• Enter coordinates as: row,col (example: 3,4 or 34)");
            gameView.ShowMessage("• Coordinates range from 0-7");
            gameView.ShowMessage("• B = Black pieces, W = White pieces");
            gameView.ShowMessage("");

            // Get player names
            string player1Name = gameView.GetPlayerName("Enter Player 1 name (Black B): ");
            string player2Name = gameView.GetPlayerName("Enter Player 2 name (White W): ");

            // Create players
            var player1 = new Player(player1Name);
            var player2 = new Player(player2Name);

            // Create pieces
            var blackPiece = new Piece(ColorType.Black);
            var whitePiece = new Piece(ColorType.White);

            // Create board
            var board = new Board();

            // Create game controller with proper dependency injection
            var game = new GameController(player1, player2, blackPiece, whitePiece, board, gameView);

            // Subscribe to events
            game.OnGameEnded += (message) => gameView.ShowMessage($"\n🎊 {message}");

            gameView.ShowMessage("\n🚀 Starting game...");
            
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