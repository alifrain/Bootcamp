using System;
using System.Collections.Generic;
using System.Linq;

// Enums
public enum PieceType
{
    NORMAL,
    KING
}

public enum PieceColor
{
    RED,
    BLACK
}

// Position record struct
public record struct Position(int X, int Y);

// Interfaces
public interface IPiece
{
    PieceColor Color { get; }
    Position Position { get; }
    PieceType Type { get; }
}

public interface IPlayer
{
    PieceColor Color { get; }
    string Name { get; }
}

public interface IBoard
{
    IPiece GetPiece(int x, int y);
    void SetPiece(int x, int y, IPiece piece);
}

// Concrete Classes
public class Piece : IPiece
{
    private PieceColor color;
    private Position position;
    private PieceType type;

    public PieceColor Color => color;
    public Position Position => position;
    public PieceType Type => type;

    public Piece(PieceColor color, Position position, PieceType type = PieceType.NORMAL)
    {
        this.color = color;
        this.position = position;
        this.type = type;
    }

    public void SetPosition(Position newPosition)
    {
        position = newPosition;
    }

    public void SetType(PieceType newType)
    {
        type = newType;
    }
}

public class Player : IPlayer
{
    private string name;
    private PieceColor color;

    public PieceColor Color => color;
    public string Name => name;

    public Player(string name, PieceColor color)
    {
        this.name = name;
        this.color = color;
    }
}

public class Board : IBoard
{
    private IPiece[,] grid;
    private const int BOARD_SIZE = 8;

    public Board()
    {
        grid = new IPiece[BOARD_SIZE, BOARD_SIZE];
        InitializeBoard();
    }

    public IPiece GetPiece(int x, int y)
    {
        if (x < 0 || x >= BOARD_SIZE || y < 0 || y >= BOARD_SIZE)
            return null;
        return grid[x, y];
    }

    public void SetPiece(int x, int y, IPiece piece)
    {
        if (x >= 0 && x < BOARD_SIZE && y >= 0 && y < BOARD_SIZE)
        {
            grid[x, y] = piece;
        }
    }

    private void InitializeBoard()
    {
        // Place black pieces (top 3 rows)
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                if ((row + col) % 2 == 1) // Only on dark squares
                {
                    SetPiece(col, row, new Piece(PieceColor.BLACK, new Position(col, row)));
                }
            }
        }

        // Place red pieces (bottom 3 rows)
        for (int row = 5; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                if ((row + col) % 2 == 1) // Only on dark squares
                {
                    SetPiece(col, row, new Piece(PieceColor.RED, new Position(col, row)));
                }
            }
        }
    }

    public void DisplayBoard()
    {
        Console.WriteLine("  0 1 2 3 4 5 6 7");
        for (int row = 0; row < BOARD_SIZE; row++)
        {
            Console.Write(row + " ");
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                var piece = GetPiece(col, row);
                if (piece == null)
                {
                    Console.Write((row + col) % 2 == 0 ? "O " : "V ");
                }
                else
                {
                    string symbol = piece.Color == PieceColor.RED ? "R" : "B";
                    if (piece.Type == PieceType.KING)
                        symbol = piece.Color == PieceColor.RED ? "♔" : "♚";
                    Console.Write(symbol + " ");
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}

public class GameController
{
    private IBoard board;
    private IPlayer[] players;
    private int currentPlayerIndex;
    private Dictionary<PieceColor, IPlayer> playersByColor;

    public Action<Position, Position> OnMoveExecuted;

    public GameController()
    {
        board = new Board();
        players = new IPlayer[]
        {
            new Player("Player 1", PieceColor.RED),
            new Player("Player 2", PieceColor.BLACK)
        };
        currentPlayerIndex = 0;
        playersByColor = new Dictionary<PieceColor, IPlayer>
        {
            { PieceColor.RED, players[0] },
            { PieceColor.BLACK, players[1] }
        };
    }

    public void StartGame()
    {
        Console.WriteLine("Welcome to Checkers!");
        Console.WriteLine($"{players[0].Name} (RED) vs {players[1].Name} (BLACK)");
        Console.WriteLine("Enter moves as: fromX fromY toX toY (e.g., '1 5 2 4')");
        Console.WriteLine();

        ((Board)board).DisplayBoard();

        while (!IsGameOver())
        {
            var currentPlayer = GetCurrentPlayer();
            Console.WriteLine($"{currentPlayer.Name}'s turn ({currentPlayer.Color})");

            // Check for forced captures
            if (HasForcedCaptures(currentPlayer))
            {
                Console.WriteLine("You must make a capture move!");
            }

            Console.Write("Enter your move: ");
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            string[] parts = input.Split(' ');
            if (parts.Length != 4 || 
                !int.TryParse(parts[0], out int fromX) ||
                !int.TryParse(parts[1], out int fromY) ||
                !int.TryParse(parts[2], out int toX) ||
                !int.TryParse(parts[3], out int toY))
            {
                Console.WriteLine("Invalid input format!");
                continue;
            }

            Position from = new Position(fromX, fromY);
            Position to = new Position(toX, toY);

            if (HandleMove(from, to))
            {
                ((Board)board).DisplayBoard();
                OnMoveExecuted?.Invoke(from, to);
            }
            else
            {
                Console.WriteLine("Invalid move! Try again.");
            }
        }

        EndGame();
    }

    public bool HandleMove(Position from, Position to)
    {
        var piece = board.GetPiece(from.X, from.Y);
        if (piece == null || piece.Color != GetCurrentPlayer().Color)
        {
            Console.WriteLine("No piece at source position or not your piece!");
            return false;
        }

        if (!EvaluateMove(from, to))
        {
            return false;
        }

        // Check if this is a forced capture situation
        if (HasForcedCaptures(GetCurrentPlayer()) && !IsCapture(from, to))
        {
            Console.WriteLine("You must make a capture move!");
            return false;
        }

        MovePiece(piece, to);

        if (IsCapture(from, to))
        {
            CapturePiece(from, to);
            
            // Check for additional captures with the same piece
            var captureChain = GetCaptureChain(from, to);
            if (captureChain.Count > 0)
            {
                Console.WriteLine("Additional captures available! Continue with the same piece.");
                return true; // Don't switch turns yet
            }
        }

        PromoteIfNeeded(board.GetPiece(to.X, to.Y));
        SwitchTurn();
        return true;
    }

    public bool CanMoveTo(Position to, IBoard board)
    {
        if (to.X < 0 || to.X >= 8 || to.Y < 0 || to.Y >= 8)
            return false;
        return board.GetPiece(to.X, to.Y) == null;
    }

    public List<Position> GetPossibleMoves(IBoard board)
    {
        var moves = new List<Position>();
        var currentPlayer = GetCurrentPlayer();
        
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                var piece = board.GetPiece(x, y);
                if (piece != null && piece.Color == currentPlayer.Color)
                {
                    var from = new Position(x, y);
                    for (int tx = 0; tx < 8; tx++)
                    {
                        for (int ty = 0; ty < 8; ty++)
                        {
                            var to = new Position(tx, ty);
                            if (EvaluateMove(from, to))
                            {
                                moves.Add(to);
                            }
                        }
                    }
                }
            }
        }
        return moves;
    }

    public bool EvaluateMove(Position from, Position to)
    {
        var piece = board.GetPiece(from.X, from.Y);
        if (piece == null || !CanMoveTo(to, board))
            return false;

        int deltaX = to.X - from.X;
        int deltaY = to.Y - from.Y;

        // Check if it's a diagonal move
        if (Math.Abs(deltaX) != Math.Abs(deltaY))
            return false;

        // Normal piece movement rules
        if (piece.Type == PieceType.NORMAL)
        {
            int direction = piece.Color == PieceColor.RED ? -1 : 1;
            
            // Simple move (one square diagonally)
            if (Math.Abs(deltaX) == 1 && deltaY == direction)
                return true;

            // Capture move (two squares diagonally)
            if (Math.Abs(deltaX) == 2 && deltaY == 2 * direction)
            {
                int midX = from.X + deltaX / 2;
                int midY = from.Y + deltaY / 2;
                var capturedPiece = board.GetPiece(midX, midY);
                return capturedPiece != null && capturedPiece.Color != piece.Color;
            }
        }
        // King movement rules
        else if (piece.Type == PieceType.KING)
        {
            // Kings can move diagonally in any direction
            if (Math.Abs(deltaX) == 1)
                return true;

            // King capture moves
            if (Math.Abs(deltaX) == 2)
            {
                int midX = from.X + deltaX / 2;
                int midY = from.Y + deltaY / 2;
                var capturedPiece = board.GetPiece(midX, midY);
                return capturedPiece != null && capturedPiece.Color != piece.Color;
            }
        }

        return false;
    }

    public List<Position> GetCapturedPositions(Position from, Position to)
    {
        var captured = new List<Position>();
        if (IsCapture(from, to))
        {
            int midX = from.X + (to.X - from.X) / 2;
            int midY = from.Y + (to.Y - from.Y) / 2;
            captured.Add(new Position(midX, midY));
        }
        return captured;
    }

    public bool IsCapture(Position from, Position to)
    {
        return Math.Abs(to.X - from.X) == 2 && Math.Abs(to.Y - from.Y) == 2;
    }

    public void MovePiece(IPiece piece, Position to)
    {
        board.SetPiece(piece.Position.X, piece.Position.Y, null);
        ((Piece)piece).SetPosition(to);
        board.SetPiece(to.X, to.Y, piece);
    }

    public void CapturePiece(Position from, Position to)
    {
        var capturedPositions = GetCapturedPositions(from, to);
        foreach (var pos in capturedPositions)
        {
            board.SetPiece(pos.X, pos.Y, null);
        }
    }

    public bool HasForcedCaptures(IPlayer player)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                var piece = board.GetPiece(x, y);
                if (piece != null && piece.Color == player.Color)
                {
                    var from = new Position(x, y);
                    // Check all possible capture moves
                    int[] directions = { -2, 2 };
                    foreach (int dx in directions)
                    {
                        foreach (int dy in directions)
                        {
                            var to = new Position(x + dx, y + dy);
                            if (EvaluateMove(from, to) && IsCapture(from, to))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public List<Position> GetCaptureChain(Position from, Position to)
    {
        var chain = new List<Position>();
        var piece = board.GetPiece(to.X, to.Y);
        
        if (piece != null)
        {
            // Check for additional captures from the new position
            int[] directions = { -2, 2 };
            foreach (int dx in directions)
            {
                foreach (int dy in directions)
                {
                    var nextTo = new Position(to.X + dx, to.Y + dy);
                    if (EvaluateMove(to, nextTo) && IsCapture(to, nextTo))
                    {
                        chain.Add(nextTo);
                    }
                }
            }
        }
        
        return chain;
    }

    public void PromoteIfNeeded(IPiece piece)
    {
        if (piece == null || piece.Type == PieceType.KING)
            return;

        bool shouldPromote = false;
        if (piece.Color == PieceColor.RED && piece.Position.Y == 0)
            shouldPromote = true;
        else if (piece.Color == PieceColor.BLACK && piece.Position.Y == 7)
            shouldPromote = true;

        if (shouldPromote)
        {
            ((Piece)piece).SetType(PieceType.KING);
            Console.WriteLine($"Piece promoted to King at ({piece.Position.X}, {piece.Position.Y})!");
        }
    }

    public void SwitchTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
    }

    public IPlayer GetCurrentPlayer()
    {
        return players[currentPlayerIndex];
    }

    public List<Position[]> GetAllValidMoves(IPlayer player)
    {
        var validMoves = new List<Position[]>();
        
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                var piece = board.GetPiece(x, y);
                if (piece != null && piece.Color == player.Color)
                {
                    var from = new Position(x, y);
                    for (int tx = 0; tx < 8; tx++)
                    {
                        for (int ty = 0; ty < 8; ty++)
                        {
                            var to = new Position(tx, ty);
                            if (EvaluateMove(from, to))
                            {
                                validMoves.Add(new Position[] { from, to });
                            }
                        }
                    }
                }
            }
        }
        
        return validMoves;
    }

    public bool IsGameOver()
    {
        // Check if current player has any valid moves
        var currentPlayer = GetCurrentPlayer();
        var validMoves = GetAllValidMoves(currentPlayer);
        
        if (validMoves.Count == 0)
        {
            return true;
        }

        // Check if either player has no pieces left
        bool redHasPieces = false;
        bool blackHasPieces = false;
        
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                var piece = board.GetPiece(x, y);
                if (piece != null)
                {
                    if (piece.Color == PieceColor.RED)
                        redHasPieces = true;
                    else if (piece.Color == PieceColor.BLACK)
                        blackHasPieces = true;
                }
            }
        }

        return !redHasPieces || !blackHasPieces;
    }

    public void EndGame()
    {
        Console.WriteLine("Game Over!");
        
        // Determine winner
        var currentPlayer = GetCurrentPlayer();
        var validMoves = GetAllValidMoves(currentPlayer);
        
        if (validMoves.Count == 0)
        {
            var otherPlayer = players[(currentPlayerIndex + 1) % players.Length];
            Console.WriteLine($"{otherPlayer.Name} wins!");
        }
        else
        {
            // Check piece count
            int redCount = 0, blackCount = 0;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    var piece = board.GetPiece(x, y);
                    if (piece != null)
                    {
                        if (piece.Color == PieceColor.RED) redCount++;
                        else blackCount++;
                    }
                }
            }
            
            if (redCount == 0)
                Console.WriteLine($"{playersByColor[PieceColor.BLACK].Name} wins!");
            else if (blackCount == 0)
                Console.WriteLine($"{playersByColor[PieceColor.RED].Name} wins!");
            else
                Console.WriteLine("It's a draw!");
        }
    }
}

// Program entry point
public class Program
{
    public static void Main(string[] args)
    {
        var game = new GameController();
        game.OnMoveExecuted += (from, to) => {
            Console.WriteLine($"Move executed: ({from.X},{from.Y}) -> ({to.X},{to.Y})");
        };
        
        game.StartGame();
        
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}