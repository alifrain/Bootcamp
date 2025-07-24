using System;
using System.Collections.Generic;
using System.Linq;

namespace CheckersGame
{
    // Position class
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Position other)
        {
            return other != null && X == other.X && Y == other.Y;
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }

    // Abstract Piece class
    public abstract class Piece
    {
        protected Player owner;
        protected Position position;

        public Piece(Player owner, Position position)
        {
            this.owner = owner;
            this.position = position;
        }

        public Player GetOwner() => owner;
        public Position GetPosition() => position;
        public void SetPosition(Position newPosition) => position = newPosition;
        public abstract bool IsKing();
        public abstract char GetSymbol();
    }

    // Normal Piece class
    public class NormalPiece : Piece
    {
        public NormalPiece(Player owner, Position position) : base(owner, position) { }

        public override bool IsKing() => false;

        public override char GetSymbol()
        {
            return owner.IsBottom() ? 'O' : 'X';
        }
    }

    // King Piece class
    public class KingPiece : Piece
    {
        public KingPiece(Player owner, Position position) : base(owner, position) { }

        public override bool IsKing() => true;

        public override char GetSymbol()
        {
            return owner.IsBottom() ? 'K' : 'Q';
        }
    }

    // Player class
    public class Player
    {
        private string name;
        private bool isBottomPlayer;
        private List<Piece> pieces;

        public Player(string name, bool isBottomPlayer)
        {
            this.name = name;
            this.isBottomPlayer = isBottomPlayer;
            this.pieces = new List<Piece>();
        }

        public List<Piece> GetAllPieces() => pieces;
        public string Name => name;
        public bool IsBottom() => isBottomPlayer;

        public void AddPiece(Piece piece) => pieces.Add(piece);
        public void RemovePiece(Piece piece) => pieces.Remove(piece);
    }

    // Cell class
    public class Cell
    {
        private Position position;
        private Piece piece;

        public Cell(Position position)
        {
            this.position = position;
            this.piece = null;
        }

        public bool IsOccupied() => piece != null;
        public Piece GetPiece() => piece;
        public void SetPiece(Piece piece) => this.piece = piece;
        public void Clear() => piece = null;
        public Position GetPosition() => position;
    }

    // Move class
    public class Move
    {
        public Position From { get; set; }
        public Position To { get; set; }
        public List<Position> CapturedPositions { get; set; }

        public Move(Position from, Position to)
        {
            From = from;
            To = to;
            CapturedPositions = new List<Position>();
        }
    }

    // MoveHandler class
    public class MoveHandler
    {
        public Action<Move> OnMoveExecuted;

        public void ExecuteMove(Board board, Move move)
        {
            Cell fromCell = board.GetCell(move.From.X, move.From.Y);
            Cell toCell = board.GetCell(move.To.X, move.To.Y);
            
            Piece piece = fromCell.GetPiece();
            
            // Move piece
            toCell.SetPiece(piece);
            fromCell.Clear();
            piece.SetPosition(move.To);

            // Handle captures
            HandleCapture(board, move);

            OnMoveExecuted?.Invoke(move);
        }

        public void HandleCapture(Board board, Move move)
        {
            foreach (Position capturedPos in move.CapturedPositions)
            {
                Cell capturedCell = board.GetCell(capturedPos.X, capturedPos.Y);
                Piece capturedPiece = capturedCell.GetPiece();
                if (capturedPiece != null)
                {
                    capturedPiece.GetOwner().RemovePiece(capturedPiece);
                    capturedCell.Clear();
                }
            }
        }
    }

    // Board class
    public class Board
    {
        private Cell[,] grid;
        private const int BOARD_SIZE = 8;

        public Board()
        {
            grid = new Cell[BOARD_SIZE, BOARD_SIZE];
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    grid[i, j] = new Cell(new Position(i, j));
                }
            }
        }

        public void InitializeBoard(List<Player> players)
        {
            Player bottomPlayer = players.Find(p => p.IsBottom());
            Player topPlayer = players.Find(p => !p.IsBottom());

            // Place bottom player pieces (rows 0, 1, 2)
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    if ((row + col) % 2 == 1) // Dark squares only
                    {
                        NormalPiece piece = new NormalPiece(bottomPlayer, new Position(row, col));
                        PlacePiece(piece);
                        bottomPlayer.AddPiece(piece);
                    }
                }
            }

            // Place top player pieces (rows 5, 6, 7)
            for (int row = 5; row < BOARD_SIZE; row++)
            {
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    if ((row + col) % 2 == 1) // Dark squares only
                    {
                        NormalPiece piece = new NormalPiece(topPlayer, new Position(row, col));
                        PlacePiece(piece);
                        topPlayer.AddPiece(piece);
                    }
                }
            }
        }

        public Cell GetCell(int x, int y)
        {
            if (x >= 0 && x < BOARD_SIZE && y >= 0 && y < BOARD_SIZE)
                return grid[x, y];
            return null;
        }

        public void PlacePiece(Piece piece)
        {
            Position pos = piece.GetPosition();
            grid[pos.X, pos.Y].SetPiece(piece);
        }

        public List<Cell> GetAllCells()
        {
            List<Cell> cells = new List<Cell>();
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    cells.Add(grid[i, j]);
                }
            }
            return cells;
        }

        public void DisplayBoard()
        {
            Console.WriteLine("  0 1 2 3 4 5 6 7");
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                Console.Write(i + " ");
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    Cell cell = grid[i, j];
                    if (cell.IsOccupied())
                    {
                        Console.Write(cell.GetPiece().GetSymbol() + " ");
                    }
                    else if ((i + j) % 2 == 1)
                    {
                        Console.Write("- ");
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }
                Console.WriteLine();
            }
        }
    }

    // GameController class
    public class GameController
    {
        private Board board;
        private List<Player> players;
        private Player currentPlayer;
        private MoveHandler moveHandler;

        public GameController()
        {
            board = new Board();
            players = new List<Player>();
            moveHandler = new MoveHandler();
        }

        public void StartGame()
        {
            // Initialize players
            players.Add(new Player("Player 1 (Bottom)", true));
            players.Add(new Player("Player 2 (Top)", false));
            
            currentPlayer = players[0]; // Bottom player starts
            
            board.InitializeBoard(players);
            
            Console.WriteLine("Checkers Game Started!");
            Console.WriteLine("Player 1 (O/K) vs Player 2 (X/Q)");
            Console.WriteLine("Use coordinates like: 2,1 to 3,2");
            
            GameLoop();
        }

        private void GameLoop()
        {
            while (!IsGameOver())
            {
                board.DisplayBoard();
                Console.WriteLine($"\n{currentPlayer.Name}'s turn");
                
                if (HandlePlayerMove())
                {
                    SwitchTurn();
                }
            }
            
            EndGame();
        }

        private bool HandlePlayerMove()
        {
            try
            {
                Console.Write("Enter move (from_x,from_y to_x,to_y): ");
                string input = Console.ReadLine();
                
                if (string.IsNullOrEmpty(input))
                    return false;
                
                string[] parts = input.Split(' ');
                if (parts.Length != 3 || parts[1].ToLower() != "to")
                {
                    Console.WriteLine("Invalid format. Use: x,y to x,y");
                    return false;
                }
                
                string[] fromCoords = parts[0].Split(',');
                string[] toCoords = parts[2].Split(',');
                
                Position from = new Position(int.Parse(fromCoords[0]), int.Parse(fromCoords[1]));
                Position to = new Position(int.Parse(toCoords[0]), int.Parse(toCoords[1]));
                
                Move move = new Move(from, to);
                
                if (EvaluateMove(move))
                {
                    HandleMove(move);
                    return true;
                }
                else
                {
                    Console.WriteLine("Invalid move!");
                    return false;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid input format!");
                return false;
            }
        }

        public void HandleMove(Move move)
        {
            Cell fromCell = board.GetCell(move.From.X, move.From.Y);
            Piece piece = fromCell.GetPiece();
            
            // Check for captures
            int deltaX = move.To.X - move.From.X;
            int deltaY = move.To.Y - move.From.Y;
            
            if (Math.Abs(deltaX) == 2 && Math.Abs(deltaY) == 2)
            {
                int captureX = move.From.X + deltaX / 2;
                int captureY = move.From.Y + deltaY / 2;
                move.CapturedPositions.Add(new Position(captureX, captureY));
            }
            
            moveHandler.ExecuteMove(board, move);
            PromoteIfNeeded(piece);
        }

        public bool EvaluateMove(Move move)
        {
            Cell fromCell = board.GetCell(move.From.X, move.From.Y);
            Cell toCell = board.GetCell(move.To.X, move.To.Y);
            
            if (fromCell == null || toCell == null || !fromCell.IsOccupied() || toCell.IsOccupied())
                return false;
            
            Piece piece = fromCell.GetPiece();
            if (piece.GetOwner() != currentPlayer)
                return false;
            
            List<Move> validMoves = GetValidMoves(piece);
            return validMoves.Any(m => m.From.Equals(move.From) && m.To.Equals(move.To));
        }

        public void PromoteIfNeeded(Piece piece)
        {
            Position pos = piece.GetPosition();
            
            if (!piece.IsKing())
            {
                bool shouldPromote = (piece.GetOwner().IsBottom() && pos.X == 7) ||
                                   (!piece.GetOwner().IsBottom() && pos.X == 0);
                
                if (shouldPromote)
                {
                    // Replace with king
                    KingPiece king = new KingPiece(piece.GetOwner(), pos);
                    piece.GetOwner().RemovePiece(piece);
                    piece.GetOwner().AddPiece(king);
                    board.GetCell(pos.X, pos.Y).SetPiece(king);
                    Console.WriteLine("Piece promoted to King!");
                }
            }
        }

        public void SwitchTurn()
        {
            currentPlayer = (currentPlayer == players[0]) ? players[1] : players[0];
        }

        public List<Move> GetValidMoves(Piece piece)
        {
            List<Move> moves = new List<Move>();
            Position pos = piece.GetPosition();
            
            int[] directions;
            if (piece.IsKing())
            {
                directions = new int[] { -1, 1 }; // Kings can move in both directions
            }
            else
            {
                directions = piece.GetOwner().IsBottom() ? new int[] { 1 } : new int[] { -1 };
            }
            
            foreach (int dirX in directions)
            {
                foreach (int dirY in new int[] { -1, 1 })
                {
                    // Normal move
                    Position newPos = new Position(pos.X + dirX, pos.Y + dirY);
                    Cell newCell = board.GetCell(newPos.X, newPos.Y);
                    
                    if (newCell != null && !newCell.IsOccupied())
                    {
                        moves.Add(new Move(pos, newPos));
                    }
                    
                    // Capture move
                    Position capturePos = new Position(pos.X + dirX * 2, pos.Y + dirY * 2);
                    Cell captureCell = board.GetCell(capturePos.X, capturePos.Y);
                    Cell middleCell = board.GetCell(pos.X + dirX, pos.Y + dirY);
                    
                    if (captureCell != null && !captureCell.IsOccupied() && 
                        middleCell != null && middleCell.IsOccupied() &&
                        middleCell.GetPiece().GetOwner() != piece.GetOwner())
                    {
                        Move captureMove = new Move(pos, capturePos);
                        captureMove.CapturedPositions.Add(new Position(pos.X + dirX, pos.Y + dirY));
                        moves.Add(captureMove);
                    }
                }
            }
            
            return moves;
        }

        public List<Move> GetAllValidMoves(Player player)
        {
            List<Move> allMoves = new List<Move>();
            foreach (Piece piece in player.GetAllPieces())
            {
                allMoves.AddRange(GetValidMoves(piece));
            }
            return allMoves;
        }

        public bool IsGameOver()
        {
            foreach (Player player in players)
            {
                if (player.GetAllPieces().Count == 0 || GetAllValidMoves(player).Count == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public void EndGame()
        {
            Console.WriteLine("\nGame Over!");
            
            foreach (Player player in players)
            {
                if (player.GetAllPieces().Count > 0 && GetAllValidMoves(player).Count > 0)
                {
                    Console.WriteLine($"{player.Name} wins!");
                    return;
                }
            }
            
            Console.WriteLine("It's a draw!");
        }
    }

    // Game class
    public class Game
    {
        private GameController controller;

        public Game()
        {
            controller = new GameController();
        }

        public void Start()
        {
            controller.StartGame();
        }
    }

    // Main Program
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Start();
        }
    }
}