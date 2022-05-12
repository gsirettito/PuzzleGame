using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Puzzle {
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private bool[,] BitMap;
        private bool move;
        private Point origen;
        private Table pzaSelect;
        private Brush[] PColors ={
                              new SolidColorBrush(Colors.GreenYellow),
                              new SolidColorBrush(Colors.Wheat),
                              new SolidColorBrush(Colors.RosyBrown),
                              new SolidColorBrush(Colors.Purple),
                              new SolidColorBrush(Colors.LightYellow),
                              new SolidColorBrush(Colors.Wheat),
                              new SolidColorBrush(Colors.RosyBrown),
                              new SolidColorBrush(Colors.Purple),
                              new SolidColorBrush(Colors.LightYellow),
                              new SolidColorBrush(Colors.DeepSkyBlue),
                              new SolidColorBrush(Colors.DeepSkyBlue),
                              new SolidColorBrush(Colors.LawnGreen),
                              new SolidColorBrush(Colors.LawnGreen),
                              new SolidColorBrush(Colors.Moccasin),
                              new SolidColorBrush(Colors.Moccasin),
                              new SolidColorBrush(Colors.Moccasin),
                              new SolidColorBrush(Colors.Moccasin),
                              new SolidColorBrush(Colors.LimeGreen),
                              new SolidColorBrush(Colors.IndianRed),
                              new SolidColorBrush(Colors.PaleGreen),
                              new SolidColorBrush(Colors.PaleGreen),
                              new SolidColorBrush(Colors.PaleGreen),
                              new SolidColorBrush(Colors.PaleGreen),
                                };
        private int[][,] Piezas = { 
                                 new int[,]{ { 1 } }, 
                                 new int[,]{ { 1 ,1 } }, 
                                 new int[,]{ { 1, 1, 1 } },
                                 new int[,]{ { 1, 1, 1, 1 } },
                                 new int[,]{ { 1, 1, 1, 1, 1 } },
                                 new int[,]{ { 1 }, { 1 } },
                                 new int[,]{ { 1 }, { 1 }, { 1 } },
                                 new int[,]{ { 1 }, { 1 }, { 1 }, { 1 } },
                                 new int[,]{ { 1 }, { 1 }, { 1 }, { 1 }, { 1 } },
                                 new int[,]{ { 1, 0 }, { 1, 0 }, { 1, 1 } }, 
                                 new int[,]{ { 0, 1 }, { 0, 1 }, { 1, 1 } }, 
                                 new int[,]{ { 0, 1, 1 }, { 1, 1, 0 } },
                                 new int[,]{ { 1, 1, 0 }, { 0, 1, 1 } },
                                 new int[,]{ { 1, 1 }, { 1, 0 } },
                                 new int[,]{ { 1, 1 }, { 0, 1 } },
                                 new int[,]{ { 0, 1 }, { 1, 1 } },
                                 new int[,]{ { 1, 0 }, { 1, 1 } },
                                 new int[,]{ { 1, 1 }, { 1, 1 } },
                                 new int[,]{ { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } },
                                 new int[,]{ { 0, 1, 0 }, { 1, 1, 1 } },
                                 new int[,]{ { 1, 1, 1 }, { 0, 1, 0 } },
                                 new int[,]{ { 1, 0 }, { 1, 1 }, { 1, 0 } },
                                 new int[,]{ { 0, 1 }, { 1, 1 }, { 0, 1 } }};
        private Table newPza;
        private int points;
        private int maxPoints;
        private string LocalPath;
        private bool gameOver;

        public MainWindow() {
            InitializeComponent();
            LocalPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\" + "Puzzle";
            if (!System.IO.Directory.Exists(LocalPath))
                System.IO.Directory.CreateDirectory(LocalPath);
            BitMap = new bool[10, 10];
            this.MouseMove += Pz_MouseMove;
            this.MouseUp += Pz_MouseUp;
        }

        private Size SizeOf(Array array) {
            if (array.Rank == 2)
                return new Size(BitMap.GetUpperBound(0) + 1, BitMap.GetUpperBound(1) + 1);
            return new Size(-1, -1);
        }

        private void Path_MouseDown_1(object sender, MouseButtonEventArgs e) {
            move = true;
        }

        private void Path_MouseMove_1(object sender, MouseEventArgs e) {
            if (move) {
                ((Path)sender).Margin = new Thickness(e.GetPosition(null).X, e.GetPosition(null).Y, 0, 0);
            }
        }

        private void Path_MouseUp_1(object sender, MouseButtonEventArgs e) {
            move = false;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e) {
            if (System.IO.File.Exists(LocalPath + "\\" + "puzzle.dat"))
                using (System.IO.StreamReader sr = new System.IO.StreamReader(LocalPath + "\\" + "puzzle.dat")) {
                    var result = UnicodeEncoding.Unicode.GetString(Convert.FromBase64String(sr.ReadLine()));
                    maxPoints = Convert.ToInt32(result);
                    MaxPoints.Text = result;
                    int r = 0;
                    while (!sr.EndOfStream) {                        
                        result = UnicodeEncoding.Unicode.GetString(Convert.FromBase64String(sr.ReadLine()));
                        string bin = Convert.ToString(Convert.ToInt32(result), 2);
                        while (bin.Length < table.Columns) bin = "0" + bin;
                        if (r >= table.Rows) break;
                        for (int c = 0; c < table.Columns; c++) {
                            BitMap[c, r] = (bin[c] == '1');
                            table[r, c].IsActive = BitMap[c, r];
                        }
                        r++;
                    }
                }
            Produce();
        }

        void Produce() {
            if (PBox.Children.Count == 0)
                for (int i = 0; i < 3; i++) {
                    int rd = new Random().Next(Piezas.Length);
                    Thread.Sleep(9);
                    Table pza = new Table(Piezas[rd]);
                    pza.CellStyle = FindResource("ActiveRoundCells") as Style;
                    foreach (var j in pza.Cells)
                        j.Background = PColors[rd];
                    pza.CellWidth = table.CellWidth - 10;
                    pza.CellHeight = table.CellHeight - 10;
                    pza.MouseDown += Pz_MouseDown;
                    pza.MouseUp += Pz_MouseUp;
                    Grid.SetColumn(pza, i);
                    PBox.Children.Add(pza);
                }
        }

        void Pz_MouseDown(object sender, MouseButtonEventArgs e) {
            pzaSelect = sender as Table;
            Table pza = new Table(pzaSelect);
            newPza = pza;
            pza.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            pza.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            pzaSelect.Visibility = System.Windows.Visibility.Hidden;
            pza.CellWidth = table.CellWidth;
            pza.CellHeight = table.CellHeight;
            (this.Content as Grid).Children.Add(pza);
            pza.MouseLeave += Pz_MouseLeave;
            pza.MouseMove += Pz_MouseMove;
            pza.MouseUp += Pz_MouseUp;
            origen = e.GetPosition(pzaSelect);
            pza.Margin = new Thickness(
                e.GetPosition(null).X - origen.X,
                e.GetPosition(null).Y - origen.Y,
                0,
                0);
            move = true;
        }

        void Pz_MouseLeave(object sender, MouseEventArgs e) {
            Table pza = sender as Table;
            //EndDrag(pza);
        }

        private void Pz_MouseUp(object sender, MouseButtonEventArgs e) {
            Table pza = newPza;
            EndDrag(pza);
        }

        private void EndDrag(Table pza) {
            if (move) {
                move = false;
                pza.IsMove = false;
                double tMLeft = ((this.Content as Grid).ActualWidth / 2) - (table.ActualWidth / 2) - table.Margin.Left;
                double tMTop = ((this.Content as Grid).ActualHeight / 2) - (table.ActualHeight / 2) - table.Margin.Top;
                int x = (int)((pza.Margin.Left - tMLeft) / table.CellWidth);
                int y = (int)((pza.Margin.Top - tMTop) / table.CellHeight);
                if (pza.Margin.Left >= tMLeft - table.CellWidth / 4 &&
                    pza.Margin.Left + pza.ActualWidth <= tMLeft + table.ActualWidth + table.CellWidth / 4 &&
                    pza.Margin.Top >= tMTop - table.CellHeight / 4 &&
                    pza.Margin.Top + pza.ActualHeight <= tMTop + table.ActualHeight + table.CellHeight / 4 &&
                    EmptySpace(x, y, pza)) {
                    PBox.Children.Remove(pzaSelect);
                    Build(x, y, pza);
                    points += Cost(pza);
                    Points.Text = points.ToString();
                    (this.Content as Grid).Children.Remove(pza);
                    Trigger(x, y, pza);
                    if (PBox.Children.Count == 0)
                        Produce();
                    if (!Verify()) {
                        gameOver = true;
                        foreach (Table i in PBox.Children) {
                            DoubleAnimation anim = new DoubleAnimation {
                                From = 1,
                                To = 0,
                                AutoReverse = true,
                                Duration = new Duration(new TimeSpan(0, 0, 0, 0, 600))
                            };
                            Storyboard.SetTarget(anim, i);
                            Storyboard.SetTargetProperty(anim, new PropertyPath("Opacity"));

                            Storyboard storyboard = new Storyboard();
                            storyboard.Children.Add(anim);
                            storyboard.Begin();
                        }
                        Play.Visibility = System.Windows.Visibility.Collapsed;
                        Message.Visibility = System.Windows.Visibility.Visible;
                    }
                } else {
                    pzaSelect.Visibility = System.Windows.Visibility.Visible;
                    (this.Content as Grid).Children.Remove(pza);
                }
            }
        }

        private int Cost(Table pza) {
            int cost = 0;
            for (int c = 0; c < pza.Columns; c++)
                for (int r = 0; r < pza.Rows; r++)
                    if (pza[r, c].IsActive) cost++;
            return cost;
        }

        private bool IsFullColumn(int x) {
            for (int k = 0; k < table.Columns; k++)
                if (!BitMap[x, k]) return false;
            return true;
        }

        private bool IsFullRow(int y) {
            for (int k = 0; k < table.Columns; k++)
                if (!BitMap[k, y]) return false;
            return true;
        }

        private void Trigger(int x, int y, Table pza) {
            List<int> columns = new List<int>();
            List<int> rows = new List<int>();
            for (int c = 0; c < pza.Columns; c++)
                if (IsFullColumn(x + c))
                    columns.Add(x + c);
            for (int r = 0; r < pza.Rows; r++)
                if (IsFullRow(y + r))
                    rows.Add(y + r);
            foreach (int c in columns)
                for (int j = 0; j < table.Rows; j++) {
                    BitMap[c, j] = false;

                    ThicknessAnimation anim = new ThicknessAnimation {
                        From = new Thickness(0),
                        To = new Thickness(5),
                        AutoReverse = true,
                        Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500))
                    };
                    Storyboard.SetTarget(anim, table[j, c]);
                    Storyboard.SetTargetProperty(anim, new PropertyPath("Margin"));

                    Storyboard storyboard = new Storyboard();
                    storyboard.Children.Add(anim);
                    storyboard.Begin();
                    table[j, c].IsActive = false;
                }
            foreach (int r in rows)
                for (int i = 0; i < table.Columns; i++) {
                    BitMap[i, r] = false;

                    ThicknessAnimation anim = new ThicknessAnimation {
                        From = new Thickness(0),
                        To = new Thickness(5),
                        AutoReverse = true,
                        Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500))
                    };
                    Storyboard.SetTarget(anim, table[r, i]);
                    Storyboard.SetTargetProperty(anim, new PropertyPath("Margin"));

                    Storyboard storyboard = new Storyboard();
                    storyboard.Children.Add(anim);
                    storyboard.Begin();
                    table[r, i].IsActive = false;
                }
            int count = columns.Count + rows.Count;
            points += count * 10;
            if (count > 1)
                points += count * 10 + (count * 10 / 2);
            Points.Text = points.ToString();
            if (points > maxPoints) maxPoints = points;
            MaxPoints.Text = maxPoints.ToString();
        }

        private bool Verify() {
            for (int c = 0; c < table.Columns; c++)
                for (int r = 0; r < table.Rows; r++) {
                    //if (table[r, c].IsActive) continue;
                    foreach (Table i in PBox.Children) {
                        if (EmptySpace(c, r, i)) return true;
                    }
                }
            return false;
        }

        private void Build(int x, int y, Table pza) {
            for (int c = 0; c < pza.Columns; c++)
                for (int r = 0; r < pza.Rows; r++)
                    if (pza[r, c].IsActive) {
                        BitMap[x + c, y + r] = true;
                        table[y + r, x + c].IsActive = true;
                        table[y + r, x + c].Background = pza[r, c].Background;
                    }
        }

        private bool EmptySpace(int x, int y, Table pza) {
            for (int c = 0; c < pza.Columns; c++)
                for (int r = 0; r < pza.Rows; r++) {
                    if (x + c >= table.Columns || y + r >= table.Columns) return false;
                    if (BitMap[x + c, y + r] && pza[r, c].IsActive) return false;
                }
            return true;
        }

        private void Pz_MouseMove(object sender, MouseEventArgs e) {
            Table pza = newPza;
            if (move && e.LeftButton == MouseButtonState.Pressed) {
                pza.BeginInit();
                pza.IsMove = true;
                double tMLeft = ((this.Content as Grid).ActualWidth / 2) - (table.ActualWidth / 2) - table.Margin.Left;
                double tMTop = ((this.Content as Grid).ActualHeight / 2) - (table.ActualHeight / 2) - table.Margin.Top;
                pza.Margin = new Thickness(
                e.GetPosition(null).X - origen.X,
                e.GetPosition(null).Y - origen.Y,
                0,
                0);
                int x = (int)((pza.Margin.Left - tMLeft) / table.CellWidth);
                int y = (int)((pza.Margin.Top - tMTop) / table.CellHeight);
                Title = String.Format("X:{0}   Y:{1}", x, y);
                if (pza.Margin.Left >= tMLeft - table.CellWidth / 4 &&
                    pza.Margin.Left + pza.ActualWidth <= tMLeft + table.ActualWidth + table.CellWidth / 4 &&
                    pza.Margin.Top >= tMTop - table.CellHeight / 4 &&
                    pza.Margin.Top + pza.ActualHeight <= tMTop + table.ActualHeight + table.CellHeight / 4 &&
                    EmptySpace(x, y, pza)) {
                    int X = (int)((pza.Margin.Left - tMLeft) / table.CellWidth);
                    int Y = (int)((pza.Margin.Top - tMTop) / table.CellHeight);
                    double newX = (double)X * table.CellWidth + tMLeft;
                    double newY = (double)Y * table.CellHeight + tMTop;
                    pza.Margin = new Thickness(newX, newY, 0, 0);
                }
                pza.EndInit();
            } else if (move) EndDrag(pza);
        }

        private void OnPauseClick(object sender, RoutedEventArgs e) {
            pause.Visibility = System.Windows.Visibility.Hidden;
            Message.Visibility = System.Windows.Visibility.Visible;
            (Message.Children[0] as TextBlock).Text = "Pause";
            Play.Visibility = System.Windows.Visibility.Visible;
        }

        private void OnPlayClick(object sender, RoutedEventArgs e) {
            pause.Visibility = System.Windows.Visibility.Visible;
            Message.Visibility = System.Windows.Visibility.Hidden;
            (Message.Children[0] as TextBlock).Text = "¡Has perdido!";
            Play.Visibility = System.Windows.Visibility.Collapsed;
        }

        void OnButtonClick(object sender, RoutedEventArgs args) {
            gameOver = false;
            var story = ((args.Source as Button).Resources["jiggleAnimation"] as Storyboard);
            story.Completed += story_Completed;
            story.Begin();
            foreach (var i in table.Cells)
                i.IsActive = false;
            BitMap = new bool[10, 10];
            points = 0;
            Points.Text = "0";
        }

        void story_Completed(object sender, EventArgs e) {
            pause.Visibility = System.Windows.Visibility.Visible;
            PBox.Children.Clear();
            Message.Visibility = System.Windows.Visibility.Hidden;
            Play.Visibility = System.Windows.Visibility.Collapsed;
            Produce();
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e) {
            using (System.IO.FileStream fs = new System.IO.FileStream(LocalPath + "\\" + "puzzle.dat", System.IO.FileMode.Create)){
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs)) {
                    var result = Convert.ToBase64String(UnicodeEncoding.Unicode.GetBytes(MaxPoints.Text));
                    sw.WriteLine(result);
                    if (!gameOver) {
                        int ent = 0;
                        for (int r = 0; r < table.Rows; r++) {
                            ent = 0;
                            for (int c = table.Columns - 1; c >= 0; c--) {
                                ent += (BitMap[c, r] ? 1 : 0) * (int)Math.Pow(2, c);
                                (table[r,c].Background as SolidColorBrush).Color.
                            }
                            result = Convert.ToBase64String(UnicodeEncoding.Unicode.GetBytes(ent.ToString()));
                            sw.WriteLine(result);
                        }
                    }
                }
            }
        }
    }
}