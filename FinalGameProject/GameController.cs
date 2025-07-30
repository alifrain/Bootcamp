using System.Drawing;
using System.Net.Http.Headers;
using OthelloGameProject;

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

        _directions = new int[,]
        {
            { -1, -1 }, { -1, 0 }, { -1, 1 },
            { 0, -1 },           { 0, 1 },
            { 1, -1 }, { 1, 0 }, { 1, 1 }
        };

        _currentPlayer = player1;
    }

    public void StartGame()
    {
        UpdateScore();
        DisplayBoard();
    }

    public void UpdateScore()
    {
        foreach (var player in _players.Keys)
        {
            int count = 0;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (_board.Grid[row, col].Color == _players[player].Color)
                    {
                        count++;
                    }
                }
            }
            player.Score = count;
        }
    }

    public Position MakeMove(IBoard board, List<Position> validMoves, Dictionary<IPlayer, IPiece> player)
    {
        return validMoves.First();
    }

    private Position GetPlayerMove(List<Position> validMoves)
    {
        while (true)
        {
            Console.Write("Input your move (row,col);");
            var input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Invalid input. Please try again.");
                continue;
            }

            var parts = input.Replace(",", " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1 && parts[0].Length == 2)
            {

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
    
    
}
