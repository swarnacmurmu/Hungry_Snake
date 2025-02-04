using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.Food, Images.Food }

        };
        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            { Direction.Up, 0 },
            {Direction.Right,90 },
            {Direction.Down,180 },
            {Direction.Left,270 }
        };


        private readonly int rows=15, cols=15;
        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRunning;

        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            gameState=new GameState(rows, cols);
            this.Loaded += Window_Loaded;
            this.KeyDown += Window_KeyDown;
            

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async Task RunGame()
        { 
        
        
            Draw();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await ShowGameOver();
            gameState= new GameState(rows, cols);


        }
        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }
            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.Up:
                    gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Direction.Down);
                    break;
            }

        }
        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(150);
                gameState.Move();
                Draw();

            }
        }
        
        private Image[,] SetupGrid()
        {
            Image[,] images=new Image[rows,cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Border border = new Border
                    {
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1)
                    };
                    Image image = new Image
                    {
                        Source = Images.Empty
                    };
                    border.Child= image;
                    images[r,c] = image;
                    GameGrid.Children.Add(border);   
                }
            }
            return images;

        }
        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();

            ScoreText.Text=$"SCORE {gameState.Score}";


        }
        private void DrawGrid()
        {
            for (int r = 0;r < rows; r++)
            {
                for (int c = 0;c < cols; c++)
                {
                    GridValue gridVal = gameState.Grid[r, c];
                    Border border = (Border)GameGrid.Children[r * cols + c];
                    Image image = (Image)border.Child;
                    image.Source = gridValToImage[gridVal];
                    
                }
            }
        }

        private void DrawSnakeHead()
        {
            Position headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = Images.Head;
            int rotation = dirToRotation[gameState.Dir];
            image.RenderTransform=new RotateTransform(rotation);
                

        }

        private async Task ShowCountDown()
        {
            for (int i=3;i>=1;i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(1000);
            }
        }
        private async Task ShowGameOver()
        {
            await Task.Delay(1000);
            Overlay.Visibility=Visibility.Visible;
            OverlayText.Text = "PRESS ANY KEY TO START";
        }
    }
}