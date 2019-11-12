using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
namespace Caro
{
    [Serializable]
    [XmlRoot()]
    public class GameData : ISerializable
    {
        public GameData() { }
        /*
         * Helpers
         */
        private void Log(string message = "", [CallerMemberName] string tag = "")
        {
            Debug.WriteLine($"@{tag}: {message}");
        }
        private void LogIf(bool condition, string message = "", [CallerMemberName] string tag = "")
        {
            Debug.WriteLineIf(condition, $"@{tag}: {message}");
        }

        private Player CheckHorizon(Point grid)
        {
            Log($"checking move [{grid.X}, {grid.Y}]");

            /*
             note: OPlayerMove = 1, XPlayerMove = -1
             imagine if a grid has a value of (1,-1) will mean that it is owned by O-Player,
             and a grid with a value of (1,-1) is owned by X-Player
             winning move is decided by calculating the accumulated sum of all horizontal grids,
             if the sum is (WinPoint, -WinPoint), O-Player wins, and with (-WinPoint, WinPoint) means X-Player wins
            */

            Point sum = new Point();
            for (int x = 0; x < MapSize; ++x)
            {
                int playerID = Map[x, (int)grid.Y];
                int n = 0;
                if (playerID == OPlayer.ID) n = 1;
                if (playerID == XPlayer.ID) n = -1;
                sum.X += n;
                sum.Y += -n;
            }
            double absO = Math.Abs(sum.X);
            double absX = Math.Abs(sum.Y);
            LogIf(absX == WinPoint && absO == WinPoint, $"[{grid.X}, {grid.Y}] is winning move");
            if (Math.Abs(sum.X) == WinPoint) return OPlayer;
            if (Math.Abs(sum.Y) == -WinPoint) return XPlayer;
            return null;
        }
        private Player CheckVertical(Point grid)
        {
            Log($"checking move [{grid.X}, {grid.Y}]");

            /*
             note: OPlayerMove = 1, XPlayerMove = -1
             imagine if a grid has a value of (1,-1) will mean that it is owned by O-Player,
             and a grid with a value of (1,-1) is owned by X-Player
             winning move is decided by calculating the accumulated sum of all horizontal grids,
             if the sum is (WinPoint, -WinPoint), O-Player wins, and with (-WinPoint, WinPoint) means X-Player wins
            */

            Point sum = new Point();
            for (int y = 0; y < MapSize; ++y)
            {
                int playerID = Map[(int)grid.X, y];
                int n = 0;
                if (playerID == OPlayer.ID) n = 1;
                if (playerID == XPlayer.ID) n = -1;
                sum.X += n;
                sum.Y += -n;
            }
            double absO = Math.Abs(sum.X);
            double absX = Math.Abs(sum.Y);
            LogIf(absX == WinPoint && absO == WinPoint, $"[{grid.X}, {grid.Y}] is winning move");
            if (Math.Abs(sum.X) == WinPoint) return OPlayer;
            if (Math.Abs(sum.Y) == -WinPoint) return XPlayer;
            return null;
        }
        private Player CheckDiagonal(Point grid)
        {
            Log($"checking move [{grid.X}, {grid.Y}]");

            /*
             note: OPlayerMove = 1, XPlayerMove = -1
             imagine if a grid has a value of (1,-1) will mean that it is owned by O-Player,
             and a grid with a value of (1,-1) is owned by X-Player
             winning move is decided by calculating the accumulated sum of all horizontal grids,
             if the sum is (WinPoint, -WinPoint), O-Player wins, and with (-WinPoint, WinPoint) means X-Player wins
            */

            Point sum = new Point();
            double diagPos = grid.X - grid.Y;
            if (diagPos < 0)
            {
                for (int y = (int)-diagPos; y < MapSize; ++y)
                {
                    int playerID = Map[(int)(y + diagPos), y];
                    int n = 0;
                    if (playerID == OPlayer.ID) n = 1;
                    if (playerID == XPlayer.ID) n = -1;
                    sum.X += n;
                    sum.Y += -n;
                }
            }
            else
            {
                for (int x = (int)diagPos; x < MapSize; ++x)
                {
                    int playerID = Map[x, (int)(x - diagPos)];
                    int n = 0;
                    if (playerID == OPlayer.ID) n = 1;
                    if (playerID == XPlayer.ID) n = -1;
                    sum.X += n;
                    sum.Y += -n;
                }
            }

            double absO = Math.Abs(sum.X);
            double absX = Math.Abs(sum.Y);
            LogIf(absX == WinPoint && absO == WinPoint, $"[{grid.X}, {grid.Y}] is winning move");
            if (Math.Abs(sum.X) == WinPoint) return OPlayer;
            if (Math.Abs(sum.Y) == -WinPoint) return XPlayer;
            return null;
        }
        private Player CheckAntiDiagonal(Point grid)
        {
            Log($"checking move [{grid.X}, {grid.Y}]");

            /*
             note: OPlayerMove = 1, XPlayerMove = -1
             imagine if a grid has a value of (1,-1) will mean that it is owned by O-Player,
             and a grid with a value of (1,-1) is owned by X-Player
             winning move is decided by calculating the accumulated sum of all horizontal grids,
             if the sum is (WinPoint, -WinPoint), O-Player wins, and with (-WinPoint, WinPoint) means X-Player wins
            */

            Point sum = new Point();
            double antiDiagPos = grid.X + grid.Y;
            if (antiDiagPos < MapSize)
            {
                for (int x = 0; x <= antiDiagPos; ++x)
                {
                    int playerID = Map[x, (int)(antiDiagPos - x)];
                    int n = 0;
                    if (playerID == OPlayer.ID) n = 1;
                    if (playerID == XPlayer.ID) n = -1;
                    sum.X += n;
                    sum.Y += -n;
                }
            }
            else
            {
                for (int y = (int)antiDiagPos - (MapSize - 1); y < MapSize; ++y)
                {
                    int playerID = Map[(int)(antiDiagPos - y), y];
                    int n = 0;
                    if (playerID == OPlayer.ID) n = 1;
                    if (playerID == XPlayer.ID) n = -1;
                    sum.X += n;
                    sum.Y += -n;
                }
            }

            double absO = Math.Abs(sum.X);
            double absX = Math.Abs(sum.Y);
            LogIf(absX == WinPoint && absO == WinPoint, $"[{grid.X}, {grid.Y}] is winning move");
            if (Math.Abs(sum.X) == WinPoint) return OPlayer;
            if (Math.Abs(sum.Y) == -WinPoint) return XPlayer;
            return null;
        }

        /*
         * Interface
         */

        public int Initialize()
        {
            Map = new int[MapSize, MapSize];
            Trace.WriteLine($"Initializing...");
            Trace.WriteLine($"Map's size: {MapSize}");
            Trace.WriteLine($"X-Player: id={XPlayer.ID}, name={XPlayer.Name}");
            Trace.WriteLine($"O-Player: id={OPlayer.ID}, name={OPlayer.Name}");

            Trace.WriteLineIf(MoveLogger.Count > 0, "Detected that game has been loaded");
            if (MoveLogger.Count > 0)
            {
                // get the first player to move
                PlayerToMove = MoveLogger[0].Player;
                int turn = 0;
                foreach (GameMove item in MoveLogger)
                {
                    Point move = item.Move;
                    int id = item.Player.ID;
                    Trace.WriteLine($"Turn {turn}: Player {item.Player.Name} has moved [{move.X}, {move.Y}]");
                    Map[(int)move.X, (int)move.Y] = id;

                    if (++PlayMovedCount >= 2)
                    {
                        PlayMovedCount = 0;
                        ++turn;
                    }

                    // switch player
                    if (PlayerToMove.ID == OPlayer.ID) PlayerToMove = XPlayer;
                    else PlayerToMove = OPlayer;
                }
                Trace.Assert(turn == Turn, $"Data mismatch detected: {nameof(Turn)}");
            }

            if (PlayerToMove == null)
            {
                Trace.WriteLine($"{nameof(PlayerToMove)} is not set");
                return -1;
            }
            else Trace.WriteLine($"First to move: id={PlayerToMove.ID}, name={PlayerToMove.Name}");
            return 0;
        }

        public int Reset()
        {
            MoveLogger.Clear();
            Map = new int[MapSize, MapSize];
            Turn = 0;
            return 0;
        }

        public int Update(Point move)
        {
            Map[(int)move.X, (int)move.Y] = PlayerToMove.ID;
            // Add to logger
            MoveLogger.Add(new GameMove(PlayerToMove, move, Turn));
            Trace.WriteLine($"Turn {Turn}: {PlayerToMove.Name} moved [{move.X}, {move.Y}]");
            if (++PlayMovedCount >= 2)
            {
                PlayMovedCount = 0;
                ++Turn;
            }
            // switch player
            if (PlayerToMove.ID == OPlayer.ID) PlayerToMove = XPlayer;
            else PlayerToMove = OPlayer;
            return 0;
        }

        public GameMove? IsOver()
        {
            Point move = MoveLogger[MoveLogger.Count - 1].Move;
            if (CheckHorizon(move) != null) return MoveLogger[MoveLogger.Count - 1];
            if (CheckVertical(move) != null) return MoveLogger[MoveLogger.Count - 1];
            if (CheckDiagonal(move) != null) return MoveLogger[MoveLogger.Count - 1];
            if (CheckAntiDiagonal(move) != null) return MoveLogger[MoveLogger.Count - 1];
            return null;
        }
        public bool HasEmptyGrid()
        {
            return MoveLogger.Count < MapSize * MapSize;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException(nameof(GetObjectData));
        }

        public Player XPlayer { get; set; } = new Player(1, "X-Player");
        public Player OPlayer { get; set; } = new Player(2, "O-Player");
        public int MapSize { get; set; } = 6;
        public int WinPoint { get; set; } = 5;
        public int Turn { get; set; } = 0;
        public List<GameMove> MoveLogger { get; set; } = new List<GameMove>();

        [XmlIgnore]
        public Player PlayerToMove { get; set; }
        [XmlIgnore]
        public int[,] Map { get; private set; }
        private int PlayMovedCount = 0;
    }

    [Serializable]
    public class Player
    {
        public Player()
        {
            ID = 0;
            Name = string.Empty;
        }
        public Player(int id, string name)
        {
            if (string.IsNullOrEmpty(name)) name = "Player";
            this.ID = id;
            this.Name = name;
        }
        public int ID { get; set; }
        public string Name { get; set; }
    }
    [Serializable]
    public struct GameMove
    {
        public GameMove(Player player, Point grid, int gameTurn)
        {
            this.Player = player;
            this.Move = grid;
            this.GameTurn = gameTurn;
        }
        public Player Player { get; set; }
        public Point Move { get; set; }
        public int GameTurn { get; set; }
    }
}
