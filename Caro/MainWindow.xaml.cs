using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Win32;
namespace Caro
{
    struct SizeInt
    {
        public SizeInt(int width,int height)
        {
            Width = width;
            Height = height;
        }
        public  int Width { get; set; }
        public  int Height { get; set; }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameData game = new GameData();
        public MainWindow()
        {
            InitializeComponent();
        }

        /* 
         * Helpers
         */

        private Point GetClickedGrid()
        {
            Size gridSize = new Size(GameField.ActualWidth / game.MapSize, GameField.ActualHeight / game.MapSize);
            Point grid = new Point(Math.Floor(MousePosition.X / gridSize.Width), Math.Floor(MousePosition.Y / gridSize.Height));
            Debug.WriteLine(string.Format("grid [{0}, {1}] is clicked", grid.X, grid.Y));
            return grid;
        }
        private Path CreatePath(int player)
        {
            Debug.Assert(player == game.OPlayer.ID || player == game.XPlayer.ID, "invalid path number");
            if (player == game.OPlayer.ID) return FindResource("O") as Path;
            else if(player == game.XPlayer.ID) return FindResource("X") as Path;
            return null;
        }
        private void InitializeGame()
        {
            // setting up game
            if (game.PlayerToMove == null) game.PlayerToMove = game.OPlayer;
            Trace.Assert(game.Initialize() == 0, "Unable to initialize! Check output");

            // setting up UI
            for (int x = 0; x < game.MapSize; ++x)
            {
                for (int y = 0; y < game.MapSize; ++y)
                {
                    Button button = new Button
                    {
                        Style = FindResource("General") as Style
                    };
                    if (game.Map[x, y] != 0) button.Content = CreatePath(game.Map[x, y]);
                    Grid.SetRow(button, y);
                    Grid.SetColumn(button, x);
                    GameField.Children.Add(button);
                }
            }
        }

        /*
         * Game Events
         */


        private void OnSizeChanging(object sender, MouseWheelEventArgs e)
        {
            double factor = 50.0 / 120.0;
            this.Width += e.Delta * factor;
            this.Height += e.Delta * factor;
        }

        private void OnPlayerClick(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            if (bt.Content == null && GameStatus == GAME_RUNNING)
            {
                // Update game state
                bt.Content = CreatePath(game.PlayerToMove.ID);
                Point clickedGrid = GetClickedGrid();
                game.Update(clickedGrid);
                GameMove? winMove = game.IsOver();

                if(winMove.HasValue)
                {
                    Player winner = winMove.Value.Player;
                    Label annoucement = new Label()
                    {
                        Style = FindResource("Announcement") as Style,
                        Content = $"winner: {winner.Name}".ToUpper()
                    };
                    Label commands = new Label()
                    {
                        Style = FindResource("GameCommand") as Style,
                        Margin = new Thickness(0.0, 30.0, 0.0, 0.0),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Content = "Press R to restart game"
                    };

                    // Update UI
                    NotifyPanel.Children.Clear();
                    NotifyPanel.Children.Add(annoucement);
                    NotifyPanel.Children.Add(commands);
                    GameNotifyOverlay.Visibility = Visibility.Visible;
                    GameStatus = GAME_ENDED;
                }
                else if (game.HasEmptyGrid() == false)
                {
                    // Update UI
                    Label annoucement = new Label()
                    {
                        Style = FindResource("Announcement") as Style,
                        Content = "game draw".ToUpper()
                    };
                    Label commands = new Label()
                    {
                        Style = FindResource("GameCommand") as Style,
                        Margin = new Thickness(0.0, 30.0, 0.0, 0.0),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Content = "Press R to restart game"
                    };

                    // Update UI
                    NotifyPanel.Children.Clear();
                    NotifyPanel.Children.Add(annoucement);
                    NotifyPanel.Children.Add(commands);
                    GameNotifyOverlay.Visibility = Visibility.Visible;
                    GameStatus = GAME_ENDED;
                }
            }
        }
        private void MouseTracker(object sender, MouseEventArgs e)
        {
            MousePosition = e.GetPosition(this);
        }
        private void DisableKeyboardInput(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (GameStatus == 0)
                {
                    MenuOverlay.Visibility = Visibility.Hidden;
                    GameField.Visibility = Visibility.Visible;
                    InitializeGame();
                    GameStatus = GAME_RUNNING;
                }
                else if (GameStatus == GAME_RUNNING)
                {
                    Label pause = new Label()
                    {
                        Style = FindResource("Announcement") as Style,
                        Content = "GAME PAUSED"
                    };
                    NotifyPanel.Children.Clear();
                    NotifyPanel.Children.Add(pause);
                    GameNotifyOverlay.Visibility = Visibility.Visible;
                    GameStatus = GAME_PAUSE;
                }
                else if (GameStatus == GAME_PAUSE)
                {
                    GameNotifyOverlay.Visibility = Visibility.Hidden;
                    GameStatus = GAME_RUNNING;
                }
                else if (GameStatus == GAME_ENDED)
                {
                    if (GameNotifyOverlay.Visibility == Visibility.Visible) GameNotifyOverlay.Visibility = Visibility.Hidden;
                    else GameNotifyOverlay.Visibility = Visibility.Visible;
                }
            }
            else if (e.Key == Key.Escape)
            {
                GameField.Visibility = Visibility.Hidden;
                GameNotifyOverlay.Visibility = Visibility.Hidden;
                MenuOverlay.Visibility = Visibility.Visible;
                game.Reset();
                GameStatus = 0;
            }
            else if (e.Key == Key.R)
            {
                game.Reset();
                GameStatus = GAME_RUNNING;
                foreach (Button button in GameField.Children)
                {
                    button.Content = null;
                }
                GameNotifyOverlay.Visibility = Visibility.Hidden;
                MenuOverlay.Visibility = Visibility.Hidden;
            }
            else if (e.Key == Key.F1)
            {
                SaveFileDialog dialog = new SaveFileDialog()
                {
                    DefaultExt = "caro",
                    CreatePrompt = true
                };

                if(dialog.ShowDialog() == true)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(GameData));
                    using XmlWriter writer = XmlWriter.Create(dialog.FileName);
                    serializer.Serialize(writer, game);
                }
            }
            else if (e.Key == Key.F2)
            {
                OpenFileDialog dialog = new OpenFileDialog()
                {
                    DefaultExt = "caro",
                    CheckFileExists = true,
                    Multiselect = false
                };

                if(dialog.ShowDialog() == true)
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(GameData));
                    using XmlReader reader = XmlReader.Create(dialog.FileName);
                    game = deserializer.Deserialize(reader) as GameData;
                }
            }
        }
        private Point MousePosition;
        private int GameStatus;
        private const int GAME_RUNNING = 1;
        private const int GAME_PAUSE = 2;
        private const int GAME_ENDED = 3;
    }
}
