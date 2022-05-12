using System;
using System.Collections.Generic;
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

namespace Puzzle {
    public class Cell : UserControl {
        private bool isActive;
        private bool flag;
        public virtual string Name { get; set; }

        static readonly DependencyProperty isActiveProperty = DependencyProperty.Register(
            "IsActive", typeof(bool),
            typeof(Cell),
            new PropertyMetadata(false, OnIsActiveChanged));

        public static DependencyProperty IsActiveProperty { get { return isActiveProperty; } }

        static void OnIsActiveChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            (obj as Cell).OnIsActiveChanged(args);
        }
        void OnIsActiveChanged(DependencyPropertyChangedEventArgs args) {
            if (args.Property == IsActiveProperty)
                isActive = (bool)args.NewValue;
        }

        public bool IsActive {
            set { SetValue(IsActiveProperty, value); }
            get { return (bool)GetValue(IsActiveProperty); }
        }

        static readonly DependencyProperty flagProperty = DependencyProperty.Register(
            "Flag", typeof(bool),
            typeof(Cell),
            new PropertyMetadata(false, OnFlagChanged));

        public static DependencyProperty FlagProperty { get { return flagProperty; } }

        static void OnFlagChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            (obj as Cell).OnFlagChanged(args);
        }
        void OnFlagChanged(DependencyPropertyChangedEventArgs args) {
            if (args.Property == FlagProperty)
                flag = (bool)args.NewValue;
        }

        public bool Flag {
            set { SetValue(FlagProperty, value); }
            get { return (bool)GetValue(FlagProperty); }
        }
    }

    /// <summary>
    /// Lógica de interacción para Table.xaml
    /// </summary>
    public partial class Table : UserControl {
        private int rows;
        private int columns;
        private double cellWidth;
        private double cellHeight;
        private bool isMove;

        private Size SizeOf(Array array) {
            if (array.Rank == 2)
                return new Size(array.GetUpperBound(0) + 1, array.GetUpperBound(1) + 1);
            return new Size(-1, -1);
        }

        public Table() {
            InitializeComponent();
            rows = 1; columns = 1;
            cellWidth = 30;
            cellHeight = 30;
            this.Width = columns * cellWidth;
            this.Height = rows * cellHeight;
        }

        public Table(Table table) {
            InitializeComponent();
            int rows = table.Rows, columns = table.Columns;
            cellWidth = 30;
            cellHeight = 30;
            this.Width = Columns * cellWidth;
            this.Height = Rows * cellHeight;
            this.rows = rows; this.columns = columns;
            while (columns-- > 0)
                Base.ColumnDefinitions.Add(new ColumnDefinition());
            while (rows-- > 0)
                Base.RowDefinitions.Add(new RowDefinition());
            CellWidth = table.CellWidth;
            CellHeight = table.CellHeight;
            for (int c = 0; c < this.columns; c++)
                for (int r = 0; r < this.rows; r++) {
                    Cell celda = new Cell();
                    celda.Name = "[" + r + "," + c + "]";
                    celda.IsActive = table[r, c].IsActive;
                    celda.Style = table[0, 0].Style;
                    celda.Background = table[r, c].Background;
                    Grid.SetColumn(celda, c);
                    Grid.SetRow(celda, r);
                    Base.Children.Add(celda);
                }
        }

        public Table(int[,] matriz) {
            InitializeComponent();
            int rows = (int)SizeOf(matriz).Height, columns = (int)SizeOf(matriz).Width;
            this.rows = rows; this.columns = columns;
            cellWidth = 30;
            cellHeight = 30;
            this.Width = Columns * cellWidth;
            this.Height = Rows * cellHeight;
            while (columns-- > 0)
                Base.ColumnDefinitions.Add(new ColumnDefinition());
            while (rows-- > 0)
                Base.RowDefinitions.Add(new RowDefinition());

            for (int c = 0; c < this.columns; c++)
                for (int r = 0; r < this.rows; r++) {
                    Cell celda = new Cell();
                    celda.Name = "[" + r + "," + c + "]";
                    celda.IsActive = matriz[c, r] != 0;
                    Grid.SetColumn(celda, c);
                    Grid.SetRow(celda, r);
                    Base.Children.Add(celda);
                }
        }

        public Table(int rows, int columns) {
            InitializeComponent();
            this.rows = rows; this.columns = columns;
            cellWidth = 30;
            cellHeight = 30;
            this.Width = Columns * cellWidth;
            this.Height = Rows * cellHeight;
            while (columns-- > 0)
                Base.ColumnDefinitions.Add(new ColumnDefinition());
            while (rows-- > 0)
                Base.RowDefinitions.Add(new RowDefinition());

            for (int c = 0; c < this.columns; c++)
                for (int r = 0; r < this.rows; r++) {
                    Cell celda = new Cell();
                    celda.Name = "[" + r + "," + c + "]";
                    celda.IsActive = false;
                    Grid.SetColumn(celda, c);
                    Grid.SetRow(celda, r);
                    Base.Children.Add(celda);
                }
        }

        public Table(int rows, int columns, bool isActive) {
            InitializeComponent();
            cellWidth = 30;
            cellHeight = 30;
            this.Width = Columns * cellWidth;
            this.Height = Rows * cellHeight;
            this.rows = rows; this.columns = columns;
            while (columns-- > 0)
                Base.ColumnDefinitions.Add(new ColumnDefinition());
            while (rows-- > 0)
                Base.RowDefinitions.Add(new RowDefinition());

            for (int c = 0; c < this.columns; c++)
                for (int r = 0; r < this.rows; r++) {
                    Cell celda = new Cell();
                    celda.Name = "[" + r + "," + c + "]";
                    celda.IsActive = true;
                    Grid.SetColumn(celda, c);
                    Grid.SetRow(celda, r);
                    Base.Children.Add(celda);
                }
        }

        public double CellWidth {
            get { return cellWidth; }
            set {
                cellWidth = value;
                foreach (var c in Base.ColumnDefinitions) c.Width = new GridLength(value);
                this.Width = columns * value;
            }
        }

        public double CellHeight {
            get { return cellHeight; }
            set {
                cellHeight = value;
                foreach (var r in Base.RowDefinitions) r.Height = new GridLength(value);
                this.Height = rows * value;
            }
        }

        public int Rows {
            get { return rows; }
            set {
                if (value <= 0) return;
                rows = value;
                this.Height = rows * cellHeight;
                while (Base.RowDefinitions.Count > value) {
                    for (int i = 0; i < Base.Children.Count; i++) {
                        if (Grid.GetRow(Base.Children[i]) == Base.RowDefinitions.Count - 1)
                            Base.Children.RemoveAt(i);
                    }
                    Base.RowDefinitions.RemoveAt(Base.RowDefinitions.Count - 1);
                }
                while (Base.RowDefinitions.Count < value) {
                    Base.RowDefinitions.Add(new RowDefinition());
                    int r = Base.RowDefinitions.Count - 1;
                    for (int c = 0; c < columns; c++) {
                        if (!Exists("[" + r + "," + c + "]")) {
                            Cell celda = new Cell();
                            celda.Name = "[" + r + "," + c + "]";
                            celda.IsActive = false;
                            Grid.SetColumn(celda, c);
                            Grid.SetRow(celda, r);
                            Base.Children.Add(celda);
                        }
                    }
                }
            }
        }

        public int Columns {
            get { return columns; }
            set {
                if (value <= 0) return;
                columns = value;
                this.Width = columns * cellWidth;
                while (Base.ColumnDefinitions.Count > value) {
                    for (int i = 0; i < Base.Children.Count; i++) {
                        if (Grid.GetColumn(Base.Children[i]) == Base.ColumnDefinitions.Count - 1)
                            Base.Children.RemoveAt(i);
                    }
                    Base.ColumnDefinitions.RemoveAt(Base.ColumnDefinitions.Count - 1);
                }
                while (Base.ColumnDefinitions.Count < value) {
                    Base.ColumnDefinitions.Add(new ColumnDefinition());
                    int c = Base.ColumnDefinitions.Count - 1;
                    for (int r = 0; r < rows; r++) {
                        if (!Exists("[" + r + "," + c + "]")) {
                            Cell celda = new Cell();
                            celda.Name = "[" + r + "," + c + "]";
                            celda.IsActive = false;
                            Grid.SetColumn(celda, c);
                            Grid.SetRow(celda, r);
                            Base.Children.Add(celda);
                        }
                    }
                }
            }
        }

        private bool Exists(string p) {
            foreach (Cell i in Base.Children)
                if (i.Name == p) return true;
            return false;
        }

        private int Index(string p) {
            int j = 0;
            foreach (Cell i in Base.Children) {
                if (i.Name == p) return j;
                j++;
            }
            return -1;
        }

        public ICollection<Cell> Cells {
            get {
                ICollection<Cell> cells = new List<Cell>();
                foreach (UIElement i in Base.Children)
                    cells.Add(i as Cell);
                return cells;
            }
        }

        public Cell this[int rows, int columns] {
            get {
                if (0 > rows || rows >= Rows) throw new Exception("Índice de fila fuera de rango.");
                if (0 > columns || columns >= Columns) throw new Exception("{Índice de columna fuera de rango.");
                //return Base.Children[Index("[" + rows + "," + columns + "]")] as Cell;
                return Base.Children[Rows * columns + rows] as Cell;
            }
        }

        static readonly DependencyProperty cellStyleProperty = DependencyProperty.Register(
            "CellStyle", typeof(Style),
            typeof(Table),
            new PropertyMetadata(new Cell().Style, OnCellStyleChanged));

        public static DependencyProperty CellStyleProperty { get { return cellStyleProperty; } }

        static void OnCellStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            (obj as Table).OnCellStyleChanged(args);
        }
        void OnCellStyleChanged(DependencyPropertyChangedEventArgs args) {
            if (args.Property == CellStyleProperty)
                foreach (Cell i in Base.Children) i.Style = args.NewValue as Style;
        }

        public Style CellStyle {
            set { SetValue(CellStyleProperty, value); }
            get { return (Style)GetValue(CellStyleProperty); }
        }

        public bool IsMove {
            get{ return IsMove;}
            set {
                isMove = value;
                foreach (Cell i in Base.Children) i.Flag = value;
            }
        }
    }
}
