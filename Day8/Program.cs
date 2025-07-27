using System;
using System.Collections.Generic;
using System.Linq;

// Position record struct
public record struct Position(int X, int Y);

// Enums
public enum PieceType { NORMAL, KING }
public enum PieceColor { RED, BLACK }

// Interfaces
public interface ICell
{
    Position GetPosition();
    void SetPosition(Position position);
    Piece GetPiece();
    void SetPiece(Piece piece);
}

public interface IPlayer
{
    PieceColor GetColor();
    string GetName();
    bool IsBottom();
}

public interface IBoard
{
    void InitializeBoard(List<IPlayer> players);
    ICell GetCell(int x, int y);
    void PlacePiece(Piece piece);
    List<ICell> GetAllCells();
}

// Concrete Classes
public class Cell : ICell
{
    private Position position;
    private Piece piece;

    public Cell(Position pos)
    {
        position = pos;
        piece = null;
    }

    public Position GetPosition() => position;
    public void SetPosition(Position pos) => position = pos;
    public Piece GetPiece() => piece;
    public void SetPiece(Piece p) => piece = p;
}

public class Player : IPlayer
{
    private string name;
    private PieceColor color;
    private bool isBottomPlayer;

    public Player(string playerName, PieceColor playerColor, bool bottom)
    {
        name = playerName;
        color = playerColor;
        isBottomPlayer = bottom;
    }

    public PieceColor GetColor() => color;
    public string GetName() => name;
    public bool IsBottom() => isBottomPlayer;
}

public class Piece
{
    private PieceColor color;
    private Position position;
    private PieceType type;

    public Piece(PieceColor pieceColor, Position pos)
    {
        color = pieceColor;
        position = pos;
        type = PieceType.NORMAL;
    }

    public PieceColor GetColor() => color;
    public Position GetPosition() => position;
    public PieceType GetType() => type;
    public void SetType(PieceType newType) => type = newType;

    public bool CanMoveTo(Position targetPos, IBoard board)
    {
        var targetCell = board.GetCell(targetPos.X, targetPos.Y);
        if (targetCell.GetPiece() != null) return false;

        int deltaX = Math.Abs(targetPos.X - position.X);
        int deltaY = Math.Abs(targetPos.Y - position.Y);

        // Must move diagonally
        if (deltaX != deltaY) return false;

        // Normal pieces can only move forward, kings can move any direction
        if (type == PieceType.NORMAL)
        {
            int direction = color == PieceColor.RED ? 1 : -1;
            if ((targetPos.Y - position.Y) * direction <= 0) return false;
        }

        return deltaX == 1; // Simple move (captures handled separately)
    }

    public List<Move> GetPossibleMoves(IBoard board)
    {
        var moves = new List<Move>();
        int[] directions = type == PieceType.KING ? new[] { -1, 1 } : 
                          color == PieceColor.RED ? new[] { 1 } : new[] { -1 };

        foreach (int dy in directions)
        {
            foreach (int dx in new[] { -1, 1 })
            {
                var newPos = new Position(position.X + dx, position.Y + dy);
                if (newPos.X >= 0 && newPos.X < 8 && newPos.Y >= 0 && newPos.Y < 8)
                {
                    if (CanMoveTo(newPos, board))
                    {
                        moves.Add(new Move { From = position, To = newPos });
                    }
                }
            }
        }
        return moves;
    }
}

public class Move
{
    public Position From { get; set; }
    public Position To { get; set; }
    public List<Position> CapturedPositions { get; set; } = new List<Position>();

    public bool IsValid(IBoard board)
    {
        var fromCell = board.GetCell(From.X, From.Y);
        var toCell = board.GetCell(To.X, To.Y);

        if (fromCell.GetPiece() == null) return false;
        if (toCell.GetPiece() != null) return false;

        return fromCell.GetPiece().CanMoveTo(To, board);
    }

    public bool IsCapture() => CapturedPositions.Count > 0;
}

public class Board : IBoard
{
    private ICell[][] grid;

    public Board()
    {
        grid = new ICell[8][];
        for (int i = 0; i < 8; i++)
        {
            grid[i] = new ICell[8];
            for (int j = 0; j < 8; j++)
            {
                grid[i][j] = new Cell(new Position(i, j));
            }
        }
    }

    public void InitializeBoard(List<IPlayer> players)
    {
        // Clear board first
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                grid[x][y].SetPiece(null);
            }
        }

        // Place red pieces (bottom rows 0, 1, 2)
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if ((x + y) % 2 == 1) // Only on dark squares
                {
                    var piece = new Piece(PieceColor.RED, new Position(x, y));
                    grid[x][y].SetPiece(piece);
                }
            }
        }

        // Place black pieces (top rows 5, 6, 7)
        for (int y = 5; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if ((x + y) % 2 == 1) // Only on dark squares
                {
                    var piece = new Piece(PieceColor.BLACK, new Position(x, y));
                    grid[x][y].SetPiece(piece);
                }
            }
        }
    }

    public ICell GetCell(int x, int y)
    {
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
            return grid[x][y];
        return null;
    }

    public void PlacePiece(Piece piece)
    {
        var pos = piece.GetPosition();
        GetCell(pos.X, pos.Y)?.SetPiece(piece);
    }

    public List<ICell> GetAllCells()
    {
        var cells = new List<ICell>();
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                cells.Add(grid[i][j]);
        return cells;
    }
}

public class GameController
{
    private IBoard board;
    private List<IPlayer> players;
    private int currentPlayerIndex;
    public Action<Move> OnMoveExecuted;

    public GameController()
    {
        board = new Board();
        players = new List<IPlayer>
        {
            new Player("Player 1", PieceColor.RED, true),
            new Player("Player 2", PieceColor.BLACK, false)
        };
        currentPlayerIndex = 0;
    }

    public void StartGame()
    {
        board.InitializeBoard(players);
        Console.WriteLine("Checkers Game Started!");
        DisplayBoard();
    }

    public bool HandleMove(Move move)
    {
        if (!EvaluateMove(move)) return false;

        if (move.IsCapture())
            CapturePiece(move);
        else
            MovePiece(board.GetCell(move.From.X, move.From.Y).GetPiece(), move.To);

        OnMoveExecuted?.Invoke(move);
        
        PromoteIfNeeded(board.GetCell(move.To.X, move.To.Y).GetPiece());
        
        if (!IsGameOver())
            SwitchTurn();

        return true;
    }

    public bool EvaluateMove(Move move)
    {
        if (!move.IsValid(board)) return false;
        
        var piece = board.GetCell(move.From.X, move.From.Y)?.GetPiece();
        if (piece == null) return false;
        
        return piece.GetColor() == GetCurrentPlayer().GetColor();
    }

    public void MovePiece(Piece piece, Position newPos)
    {
        var oldPos = piece.GetPosition();
        board.GetCell(oldPos.X, oldPos.Y).SetPiece(null);
        board.GetCell(newPos.X, newPos.Y).SetPiece(piece);
    }

    public void CapturePiece(Move move)
    {
        MovePiece(board.GetCell(move.From.X, move.From.Y).GetPiece(), move.To);
        
        foreach (var capturedPos in move.CapturedPositions)
        {
            board.GetCell(capturedPos.X, capturedPos.Y).SetPiece(null);
        }
    }

    public bool HasForcedCaptures(IPlayer player)
    {
        // Simplified - in real checkers, captures are mandatory
        return false;
    }

    public List<Move> GetCaptureChain(Move move)
    {
        // Simplified - would implement multi-jump logic
        return new List<Move> { move };
    }

    public void PromoteIfNeeded(Piece piece)
    {
        if (piece == null) return;
        
        var pos = piece.GetPosition();
        if ((piece.GetColor() == PieceColor.RED && pos.Y == 7) ||
            (piece.GetColor() == PieceColor.BLACK && pos.Y == 0))
        {
            piece.SetType(PieceType.KING);
            Console.WriteLine($"Piece promoted to KING at {pos.X},{pos.Y}!");
        }
    }

    public void SwitchTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        Console.WriteLine($"\n{GetCurrentPlayer().GetName()}'s turn ({GetCurrentPlayer().GetColor()})");
    }

    public IPlayer GetCurrentPlayer()
    {
        return players[currentPlayerIndex];
    }

    public List<Move> GetAllValidMoves(IPlayer player)
    {
        var moves = new List<Move>();
        foreach (var cell in board.GetAllCells())
        {
            var piece = cell.GetPiece();
            if (piece?.GetColor() == player.GetColor())
            {
                moves.AddRange(piece.GetPossibleMoves(board));
            }
        }
        return moves;
    }

    public bool IsGameOver()
    {
        var currentPlayerMoves = GetAllValidMoves(GetCurrentPlayer());
        return currentPlayerMoves.Count == 0;
    }

    public void EndGame()
    {
        var winner = players[(currentPlayerIndex + 1) % players.Count];
        Console.WriteLine($"\nGame Over! {winner.GetName()} ({winner.GetColor()}) wins!");
    }

    public void DisplayBoard()
    {
        Console.WriteLine("\n  0 1 2 3 4 5 6 7");
        for (int y = 7; y >= 0; y--) // Display from top to bottom
        {
            Console.Write($"{y} ");
            for (int x = 0; x < 8; x++)
            {
                var piece = board.GetCell(x, y)?.GetPiece();
                if (piece != null)
                {
                    // Show pieces
                    string symbol = piece.GetColor() == PieceColor.RED ? "R" : "B";
                    if (piece.GetType() == PieceType.KING) symbol = symbol.ToLower();
                    Console.Write($"{symbol} ");
                }
                else
                {
                    // Show empty squares - dark squares are playable
                    Console.Write((x + y) % 2 == 1 ? "■ " : "□ ");
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}

public class Game
{
    private GameController controller;

    public Game()
    {
        controller = new GameController();
        controller.OnMoveExecuted += (move) => 
        {
            Console.WriteLine($"Move executed: ({move.From.X},{move.From.Y}) → ({move.To.X},{move.To.Y})");
            controller.DisplayBoard();
        };
    }

    public void Start()
    {
        controller.StartGame();
        
        while (!controller.IsGameOver())
        {
            try
            {
                Console.WriteLine($"{controller.GetCurrentPlayer().GetName()}, enter move (format: x1,y1 x2,y2): ");
                var input = Console.ReadLine()?.Split(' ');
                
                if (input?.Length != 2) continue;
                
                var from = input[0].Split(',');
                var to = input[1].Split(',');
                
                var move = new Move
                {
                    From = new Position(int.Parse(from[0]), int.Parse(from[1])),
                    To = new Position(int.Parse(to[0]), int.Parse(to[1]))
                };
                
                if (!controller.HandleMove(move))
                    Console.WriteLine("Invalid move! Try again.");
            }
            catch
            {
                Console.WriteLine("Invalid input format! Use: x1,y1 x2,y2");
            }
        }
        
        controller.EndGame();
    }
}

// Program entry point
class Program
{
    static void Main(string[] args)
    {
        var game = new Game();
        game.Start();
    }
}