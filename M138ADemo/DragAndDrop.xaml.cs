using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;
using Button = System.Windows.Controls.Button;
using DataGrid = System.Windows.Controls.DataGrid;
using DataGridCell = System.Windows.Controls.DataGridCell;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using DragEventHandler = System.Windows.DragEventHandler;
using MessageBox = System.Windows.Forms.MessageBox;
using PropertyPath = System.Windows.PropertyPath;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Linq;
using M138ADemo.MainClasses;
using M138ADemo.ViewModels;

namespace M138ADemo
{
    /// <summary>
    /// Interaction logic for DragAndDrop.xaml
    /// </summary>
    public partial class DragAndDrop : Window
    {

        /// <summary>
        /// The default thickness.
        /// </summary>
        private static Thickness DefaultThickness = new Thickness(2, 2, 2.2, 2);

        /// <summary>
        /// The index of the curr row.
        /// </summary>
        private int currRowIndex = -1;//Helper for drag and drop

        /// <summary>
        /// The dark row brush.
        /// </summary>
        private static ImageBrush darkRowBrush = new ImageBrush(Helper.DarkPapSource);

        /// <summary>
        /// The default row brush.
        /// </summary>
        private static ImageBrush DefaultRowBrush = new ImageBrush(Helper.PapSource);

        /// <summary>
        /// The current file path.
        /// </summary>
        private string currentFilePath = "Untitled";

        /// <summary>
        /// The view model.
        /// </summary>
        private DragAndDropViewModel viewModel;

        /// <summary>
        /// The is extended work space.
        /// </summary>
        private int IsExtendedWorkSpace = Configuration.IsCompactWorkSpace ? 0 : 26;

        /// <summary>
        /// The default cell style.
        /// </summary>
        private Style DefaultCellStyle;

        /// <summary>
        /// The border cell style.
        /// </summary>
        private Style BorderCellStyle;

        /// <summary>
        /// Gets or sets the current file path.
        /// </summary>
        /// <value>The current file path.</value>
        public string CurrentFilePath
        {
            get => currentFilePath;
            set
            {
                if (value != null)
                {
                    this.Title = value;
                    this.viewModel.Title = value;
                    currentFilePath = value;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.DragAndDrop"/> class.
        /// </summary>
        public DragAndDrop()
        {
            viewModel = new DragAndDropViewModel();
            CurrentFilePath = "Untitled";
            viewModel.AfterFileOpened += () => { UpdateHighlightedCells(); SetBackGround(); };
            viewModel.UpdateAndRehighlight += (ind) => { UpdateHighlightedCells(ind); SetBackGround(ind); };

            InitializeComponent();
            this.myGrid.InputBindings.Add(new InputBinding(viewModel.SaveCommand, new KeyGesture(Key.S, ModifierKeys.Control)));
            this.myGrid.InputBindings.Add(new InputBinding(viewModel.OnShowSelectedText, new KeyGesture(Key.C, ModifierKeys.Control)));
            //mCopyColumnTextMenuItem.Command = viewModel.OnShowSelectedText;
            //viewModel.OnShowSelectedText.inp

            viewModel.NotifyToClose += () => this.Close();

            mBackToKeysMenuItem.Command = viewModel.BackCommand;
            mBackToKeysMenuItem.CommandParameter = DragAndDropViewModel.WindowsToBeOpened.AddKeys;

            mBackToSettingsMenuItem.Command = viewModel.BackCommand;
            mBackToSettingsMenuItem.CommandParameter = DragAndDropViewModel.WindowsToBeOpened.MainSettings;

            

            this.SetBinding(TitleProperty, new Binding()
            {
                Source = viewModel,
                Path = new PropertyPath("Title"),
                Mode = BindingMode.OneWay,
                NotifyOnSourceUpdated = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

            //this.Title = CurrentFilePath;
            myGrid.Loaded += StaticGridOnLoaded;
            darkRowBrush.Transform = new ScaleTransform(1, 0.85);

            myGrid.RowStyle = new Style()
            {
                Setters =
                {
                    new Setter()
                    {
                        Property = BorderThicknessProperty,
                        Value = new Thickness(0,0,0,0),
                    },
                    new Setter()
                    {
                        Property = BackgroundProperty,
                        Value = null
                    },
                    /*new Setter()
                    {
                        Property = FocusableProperty,
                        Value = false
                    }*/
                },
                Triggers = {new Trigger()
                {
                    Property = DataGridRow.IsSelectedProperty,
                    Value = true,
                    Setters = {}
                }}

            };
            
            myGrid.CellStyle = new Style()
            {
                Setters = {new Setter(MarginProperty, new Thickness(0, 5/1.5, 0, 11/1.5))},
                Triggers = {new Trigger()
                {
                    Property = DataGridCell.IsSelectedProperty,
                    Value = true,
                    Setters = { new Setter()
                        {
                            Property = BorderBrushProperty,
                            Value = DefaultRowBrush,
                        },
                        new Setter()
                        {
                            Property = ForegroundProperty,
                            Value = Brushes.Black
                        }, new Setter()
                        {
                            Property = BackgroundProperty,
                            Value = DefaultRowBrush
                        }, new Setter()
                        {
                            Property = BorderThicknessProperty,
                            Value = DefaultThickness
                        }
                    }
                } }
            };
            this.DefaultCellStyle = myGrid.CellStyle;
            this.BorderCellStyle = new Style(typeof(DataGridCell))
            {
                Setters = { new Setter(MarginProperty, new Thickness(0)) , new Setter(BackgroundProperty, new SolidColorBrush(Color.FromRgb(4 * 16 + 6, 4 * 16 + 7, 3 * 16 + 14))) },

            };

            myGrid.SelectedCellsChanged += MyGrid_SelectedCellsChanged;
           
            myGrid.CanUserResizeRows = false;
            myGrid.CanUserResizeColumns = false;
            myGrid.GridLinesVisibility = DataGridGridLinesVisibility.None;
            myGrid.AutoGenerateColumns = false;
            myGrid.SelectionMode = DataGridSelectionMode.Extended;
            myGrid.IsReadOnly = true;
            myGrid.ItemsSource = viewModel.Keys;
            myGrid.CanUserSortColumns = false;
            for (int i = 0; i < 29 + 26 + IsExtendedWorkSpace; ++i)
            {
                var column = new DataGridTextColumn();
                column.Width = new DataGridLength(30);
                
                
                if (i <= 1)
                {
                    DataGridTemplateColumn col = new DataGridTemplateColumn()
                    {
                        CellTemplate = getDataTemplate(i),
                    };
                    col.Width = new DataGridLength(30);
                    myGrid.Columns.Add(col);
                }
                /*else if (i == 2) {
                    row.Binding = new Binding()
                    {
                        Path = new PropertyPath("IdNumber"),
                        Converter = new IntToStringConverter(),
                        Mode = BindingMode.TwoWay,
                        BindsDirectlyToSource = true,
                        //XPath = "."
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };
                    myGrid.Columns.Add(row);

                }*/
                else
                {
                    column.Binding = new Binding()
                    {
                        Path = new PropertyPath($"KeyArr[{i - 2}]"),
                        //Converter = new CharToStringConverter(),
                        //Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger =  UpdateSourceTrigger.PropertyChanged,
                        BindsDirectlyToSource = true,
                    };
                    if (Configuration.IsCompactWorkSpace)
                    {
                        column.Header = (i - 2).ToString();
                        
                    }
                    else
                    {
                        if (i - 2 - 26 >= 0)
                        {
                            column.Header = (i - 2 - 26).ToString();
                        }
                    }
                    myGrid.Columns.Add(column);

                }
            }

            if (!Configuration.IsCompactWorkSpace)
            {
                var column = new DataGridTextColumn();
                column.Width = new DataGridLength(30);
                myGrid.Columns.Add(column);
            }


            myGrid.Background = new ImageBrush(Helper.AluminumSource); // Helper.AluminumSource;
            //DragAndDrop Setup
            myGrid.AllowDrop = true;
            myGrid.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(myGrid_PreviewMouseLeftButtonDown);
            myGrid.Drop += new DragEventHandler(DropRow);
            //UpdateHighlightedCells();
            myGrid.ColumnHeaderStyle = new Style()
            {
                TargetType = typeof(DataGridColumnHeader),
                Setters = {
                    new EventSetter()
                    {
                        Event = MouseDoubleClickEvent,
                        Handler = new MouseButtonEventHandler(HeaderClick)
                    }
                }
            };

            mSaveAsMenuItem.Click += MSaveAsMenuItem_Click;
            mOpenMenuItem.Command = viewModel.OpenMenuItemCommand;
            this.Closing += DragAndDrop_Closing;
        }

        /// <summary>
        /// Drags the and drop closing.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void DragAndDrop_Closing(object sender, CancelEventArgs e)
        {
            viewModel.OnClosingCommand.Execute(e);
        }

        /// <summary>
        /// Mies the grid selected cells changed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void MyGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (e.AddedCells.Count > 0)
            {
                for (int row = 0; row < viewModel.Keys.Count; ++row)
                {
                    for (int col = 0; col < 2; ++col)
                    {
                        var cell = myGrid.GetCell(row, col);
                        cell.Background = null;
                        cell.BorderBrush = null;
                    }
                }
            }
            else
            {
                SetBackGround();
            }
        }

        /// <summary>
        /// MSs the ave as menu item click.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void MSaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SaveAsCommand.Execute(null);
        }

        /// <summary>
        /// Sets the back ground.
        /// </summary>
        private void SetBackGround()
        {
            for (int row = 0; row < viewModel.Keys.Count; ++row)
            {
                SetBackGround(row);
            }
        }

        /// <summary>
        /// Sets the back ground.
        /// </summary>
        /// <param name="row">Row.</param>
        private void SetBackGround(int row)
        {
            int borderColumn = Configuration.IsCompactWorkSpace ? 0 : 1;
            for (int col = 2; col < 29 + 26 + IsExtendedWorkSpace + borderColumn; ++col)
            {

                if (!Configuration.IsCompactWorkSpace && (col == 26+2 || col == 26 + 26 + 26 + 3))//колонка рамки
                {
                    var cell = myGrid.GetCell(row, col);
                    //cell.Background = new ImageBrush(Helper.AluminumSource);
                    //cell.BorderBrush = new ImageBrush(Helper.AluminumSource);
                    cell.Content = null;

                    ImageBrush silver = new ImageBrush(Helper.BorderSource);
                    //int sz = viewModel.Keys.Count;
                    //silver.Stretch = Stretch.Uniform;
                    //silver.Viewbox = new Rect(0, row * (double)1 / sz, 1, 1 * (double)1 / sz);
                    //Console.WriteLine($"Row: {row} with viewBox: {silver.Viewbox}");

                    cell.Background = silver;

                    //cell.Background = new SolidColorBrush(Color.FromRgb(166,166, 166));
                    cell.BorderBrush = silver;
                    cell.Margin = new Thickness(0);
                    //cell.OverridesDefaultStyle = true;
                    //cell.Style = this.BorderCellStyle;
                }
                else
                {
                    var cell = myGrid.GetCell(row, col);
                    //cell.OverridesDefaultStyle = true;

                    cell.Style = this.DefaultCellStyle;
                    if (viewModel.Keys[row].KeyArr[col - 2] != " ")
                    {
                        cell.Background = DefaultRowBrush;
                    }
                    else
                    {
                        cell.Background = null;
                        cell.BorderBrush = null;
                    }
                }
                
                
            }
        }

        /// <summary>
        /// Headers the click.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void HeaderClick(object sender, MouseButtonEventArgs e)
        {
            DataGridColumnHeader header = (DataGridColumnHeader) sender;

            if (header.Content is String)
            {
                if (header != null)
                {
                    int headerIndex = int.Parse((string)header.Content);
                    if (headerIndex <= 26 && headerIndex > 0)
                    {
                        viewModel.SelectedColumn = headerIndex + IsExtendedWorkSpace;
                        HighlightSelectedColumn(int.Parse((string)header.Content) + 2 + IsExtendedWorkSpace);
                        this.myGrid.Focus();
                    }
                }
            }

        }

        /// <summary>
        /// Statics the grid on loaded.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void StaticGridOnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateHighlightedCells();
            SetBackGround();
            if (Configuration.Automatic && Configuration.Encrypt)
            {
                HighlightSelectedColumn(3 + IsExtendedWorkSpace);
            }
            if (Configuration.Automatic && Configuration.Decrypt)
            {
                HighlightSelectedColumn(2 + Configuration.DecryptIndex + IsExtendedWorkSpace);
                HighlightSelectedColumn(3 + IsExtendedWorkSpace);
            }
        }

        /// <summary>
        /// Highlights the selected row.
        /// </summary>
        /// <param name="col">Col.</param>
        private void HighlightSelectedColumn(int col)
        {
            for (int i = 0; i < viewModel.Keys.Count; ++i)
            {
                var cell = myGrid.GetCell(i, col);
                //cell.BorderThickness = new Thickness(2, 2, 2, 2);
                cell.BorderBrush = Brushes.Aqua;
            }
        }

        /// <summary>
        /// Unselects the rows.
        /// </summary>
        private void UnselectRows()
        {
            myGrid.SelectedIndex = -1;
        }

        /// <summary>
        /// Gets the data template.
        /// </summary>
        /// <returns>The data template.</returns>
        /// <param name="cnt">Count.</param>
        private DataTemplate getDataTemplate(int cnt)
        {
            DataTemplate template = new DataTemplate();
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(Button));
            factory.SetValue(Button.TemplateProperty, Application.Current.Resources["RoundButtonLeftRight"]);
            factory.SetValue(Button.BackgroundProperty, Brushes.AliceBlue);
           // factory.SetValue(Button.FocusableProperty, false);
            string[] arr = {"<", ">"};
            
            factory.SetValue(Button.ContentProperty, arr[cnt]);
            RoutedEventHandler[] funs =
                {new RoutedEventHandler(BtnOnClickLeft), new RoutedEventHandler(BtnOnClickRight)};
            factory.AddHandler(Button.ClickEvent, funs[cnt]);
            template.VisualTree = factory;

            return template;
        }

        /// <summary>
        /// Gets the data grid row.
        /// </summary>
        /// <returns>The data grid row.</returns>
        /// <param name="sender">Sender.</param>
        private static DataGridRow GetDataGridRow(object sender)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    return (DataGridRow)vis;
                }

            return null;
        }

        /// <summary>
        /// Buttons the on click left.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void BtnOnClickLeft(object sender, RoutedEventArgs e)
        {
            
            var row = GetDataGridRow(sender);
            if (row == null)
            {
                return;
            }
            int ind = row.GetIndex();
            viewModel.ShiftCommand.Execute((ind, DragAndDropViewModel.KeyShiftEnum.Left));
            UpdateHighlightedCells();
            //this.Focus();
        }

        /// <summary>
        /// Buttons the on click right.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void BtnOnClickRight(object sender, RoutedEventArgs e)
        {
            var row = GetDataGridRow(sender);
            if (row == null)
            {
                return;
            }
            int ind = row.GetIndex();
            viewModel.ShiftCommand.Execute((ind, DragAndDropViewModel.KeyShiftEnum.Right));
            UpdateHighlightedCells();
            //this.Focus();
        }

        /// <summary>
        /// Updates the highlighted cells.
        /// </summary>
        private void UpdateHighlightedCells()
        {
            for (int i = 0; i < viewModel.Keys.Count; ++i)
            {
                UpdateHighlightedCells(i);
            }
        }

        /// <summary>
        /// Updates the highlighted cells.
        /// </summary>
        /// <param name="row">Row.</param>
        private void UpdateHighlightedCells(int row)
        {
            int topBorder = Configuration.IsCompactWorkSpace ? viewModel.Keys[0].Key.Length * 2 + 3 : 26 + 26 + 26 + 3;

            for (int j = 0; j < topBorder; ++j)
            {
                var c = myGrid.GetCell(row, j);
                c.BorderThickness = DefaultThickness;
                if (j > 1)
                {
                    if ((j == 26 + 2 || j == 26 + 26 + 26 + 3) && !Configuration.IsCompactWorkSpace)
                    {
                        ImageBrush silver = new ImageBrush(Helper.BorderSource);
                        int sz = viewModel.Keys.Count;
                        //silver.Stretch = Stretch.Uniform;

                        //silver.Viewbox = new Rect(0, row * (double)1 / sz, 1, (double)1 / sz);
                        
                        c.BorderBrush = silver;
                        //c.BorderBrush = new SolidColorBrush(Color.FromRgb(4 * 16 + 6, 4 * 16 + 7, 3 * 16 + 14));
                    }
                    else if (viewModel.Keys[row].Shift >= 0 && j > 2 && viewModel.Keys[row].KeyArr[(j - 3)] != " ")
                    {
                        c.BorderBrush = DefaultRowBrush;
                    }
                    else if (viewModel.Keys[row].KeyArr[(j - 2)] != " ")
                    {
                        c.BorderBrush = DefaultRowBrush;
                    }
                    else
                    {
                        c.BorderBrush = null;
                    }
                }
                else c.BorderBrush = null;
                //Console.WriteLine(c.BorderThickness);

                //c.Background = Brushes.Red;
            }
            var cell = myGrid.GetCell(row, viewModel.Keys[row].LastIndex + 3 + IsExtendedWorkSpace);
            //cell.BorderThickness = new Thickness(1, 0.6, 1, 0.6);
            cell.BorderBrush = Brushes.Red;
        }


        /// <summary>
        /// Get positio delegate.
        /// </summary>
        public delegate Point GetPosition(IInputElement element);

        /// <summary>
        /// Drops the row.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void DropRow(object sender, DragEventArgs e)
        {
            int index;
            if (currRowIndex < 0)
            {
                return;
            }

            index = GetCurrentRowIndex(e.GetPosition);

            if (index < 0)
                return;
            if (index == currRowIndex)
                return;
            if (index == myGrid.Items.Count - 1)
            {
                MessageBox.Show("This row-index cannot be drop");
                return;
            }
            
            KeyModel changedProduct = viewModel.Keys[currRowIndex];

            viewModel.Keys.RemoveAt(currRowIndex);
            viewModel.Keys.Insert(index, changedProduct);
            SetBackGround();
            UpdateHighlightedCells();
        }

        /// <summary>
        /// Mies the grid preview mouse left button down.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void myGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            Console.WriteLine(myGrid.Columns.ElementAt(0).ActualWidth + myGrid.Columns.ElementAt(1).ActualWidth + myGrid.Margin.Left * 2);
            Console.WriteLine(e.GetPosition(this).X.ToString() + " " + e.GetPosition(this).Y.ToString());
            Console.WriteLine();
            //60 - по моим измерениям
            if (e.GetPosition(this).Y > this.ActualHeight - 60) return;

            if (this.ActualWidth - 35.5 <= e.GetPosition(this).X) return;

            if (e.GetPosition(this).Y < 30)
            {
                UpdateHighlightedCells();
                return;
            }
            if (e.GetPosition(this).X < myGrid.Columns.ElementAt(0).ActualWidth + myGrid.Columns.ElementAt(1).ActualWidth + myGrid.Margin.Left * 2)
            {
                return;
            }
            currRowIndex = GetCurrentRowIndex(e.GetPosition);
            UpdateHighlightedCells();
            if (currRowIndex < 0)
            {
                //UpdateHighlightedCells();
                UnselectRows();
                return;
            }

            myGrid.SelectedIndex = currRowIndex;
            
            KeyModel selectedEmp = myGrid.Items[currRowIndex] as KeyModel;
            if (selectedEmp == null)
                return;
            DragDropEffects dragDropEffects = DragDropEffects.Move;
            if (DragDrop.DoDragDrop(myGrid, selectedEmp, dragDropEffects)
                != DragDropEffects.None)
            {
                myGrid.SelectedItem = selectedEmp;
            }
        }

        /// <summary>
        /// Gets the mouse target row.
        /// </summary>
        /// <returns><c>true</c>, if mouse target row was gotten, <c>false</c> otherwise.</returns>
        /// <param name="theTarget">The target.</param>
        /// <param name="position">Position.</param>
        private bool GetMouseTargetRow(Visual theTarget, GetPosition position)
        {
            if (theTarget == null)
            {
                return false;
            }
            Rect rect = VisualTreeHelper.GetDescendantBounds(theTarget);
            Point point = position((IInputElement)theTarget);
            return rect.Contains(point);
        }

        /// <summary>
        /// Gets the index of the current row.
        /// </summary>
        /// <returns>The current row index.</returns>
        /// <param name="pos">Position.</param>
        int GetCurrentRowIndex(GetPosition pos)
        {
            int curIndex = -1;
            for (int i = 0; i < myGrid.Items.Count; i++)
            {
                DataGridRow itm = this.myGrid.GetRow(i);
                if (GetMouseTargetRow(itm, pos))
                {
                    curIndex = i;
                    break;
                }
            }
            Console.WriteLine("GetCurrentRowIndex : " + curIndex);
            return curIndex;
        }

    }
}
