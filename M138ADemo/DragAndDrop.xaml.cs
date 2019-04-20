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
        private DeviceState lastState = new DeviceState();
        private static Thickness DefaultThickness = new Thickness(2, 2, 2.2, 2);
        public static BindingList<KeyModel> keys = new BindingList<KeyModel>();
        public static DataGrid staticGrid;
        private BindingSource bindingSource = new BindingSource();
        private int currRowIndex = -1;//Helper for drag and drop
        private int buttonsWidths = 35;
        private int simpleWidths = 30;
        private static ImageBrush darkRowBrush = new ImageBrush(Helper.DarkPapSource);
        private static ImageBrush DefaultRowBrush = new ImageBrush(Helper.PapSource);
        private string currentFilePath = "Untitled";
        private DragAndDropViewModel viewModel;
        private int IsExtendedWorkSpace = Configuration.IsCompactWorkSpace ? 0 : 26;
        private Style DefaultCellStyle;
        private Style BorderCellStyle;

        public string CurrentFilePath
        {
            get => currentFilePath;
            set
            {
                if (value != null)
                {
                    this.Title = value;
                    currentFilePath = value;
                }
            }
        }

        public DragAndDrop()
        {
            
            viewModel = new DragAndDropViewModel();
            viewModel.AfterFileOpened += () => { UpdateHighlightedCells(); SetBackGround(); };
            viewModel.UpdateAndRehighlight += (ind) => { UpdateHighlightedCells(ind); SetBackGround(ind); };

            InitializeComponent();
            this.myGrid.InputBindings.Add(new InputBinding(viewModel.OnShowSelectedText, new KeyGesture(Key.C, ModifierKeys.Control)));
            //mCopyColumnTextMenuItem.Command = viewModel.OnShowSelectedText;
            //viewModel.OnShowSelectedText.inp

            viewModel.NotifyToClose += () => this.Close();

            mBackToKeysMenuItem.Command = viewModel.BackCommand;
            mBackToKeysMenuItem.CommandParameter = DragAndDropViewModel.WindowsToBeOpened.AddKeys;

            mBackToSettingsMenuItem.Command = viewModel.BackCommand;
            mBackToSettingsMenuItem.CommandParameter = DragAndDropViewModel.WindowsToBeOpened.MainSettings;

            mBackToKeysMenuItem.SetBinding(System.Windows.Controls.MenuItem.IsEnabledProperty, new Binding()
            {
                Source = viewModel,
                Path = new PropertyPath("IsBackToKeysMenuItemsEnabled"),
                NotifyOnSourceUpdated = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

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
                            Value = Brushes.Black,
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

        private void DragAndDrop_Closing(object sender, CancelEventArgs e)
        {
            viewModel.OnClosingCommand.Execute(e);
        }

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

        private void MSaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SaveAsCommand.Execute(null);
        }

        private void SetBackGround()
        {
            for (int row = 0; row < viewModel.Keys.Count; ++row)
            {
                SetBackGround(row);
            }
        }

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
                        HighlightSelectedRow(int.Parse((string)header.Content) + 2 + IsExtendedWorkSpace);
                        this.myGrid.Focus();
                    }
                }
            }

        }

        private void StaticGridOnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateHighlightedCells();
            SetBackGround();
            if (Configuration.Automatic && Configuration.Encrypt)
            {
                HighlightSelectedRow(3 + IsExtendedWorkSpace);
            }
            if (Configuration.Automatic && Configuration.Decrypt)
            {
                HighlightSelectedRow(2 + Configuration.DecryptIndex + IsExtendedWorkSpace);
                HighlightSelectedRow(3 + IsExtendedWorkSpace);
            }
        }

        private void HighlightSelectedRow(int col)
        {
            for (int i = 0; i < viewModel.Keys.Count; ++i)
            {
                var cell = myGrid.GetCell(i, col);
                //cell.BorderThickness = new Thickness(2, 2, 2, 2);
                cell.BorderBrush = Brushes.Aqua;
            }
        }

        private void UnselectRows()
        {
            myGrid.SelectedIndex = -1;
        }

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

        private static DataGridRow GetDataGridRow(object sender)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    return (DataGridRow)vis;
                }

            return null;
        }

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

        private void UpdateHighlightedCells()
        {
            for (int i = 0; i < viewModel.Keys.Count; ++i)
            {
                UpdateHighlightedCells(i);
            }
        }

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


        //DRAG and DROP
        public delegate Point GetPosition(IInputElement element);

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
        }

        void myGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            Console.WriteLine(myGrid.Columns.ElementAt(0).ActualWidth + myGrid.Columns.ElementAt(1).ActualWidth + myGrid.Margin.Left * 2);
            Console.WriteLine(e.GetPosition(this).X.ToString() + " " + e.GetPosition(this).Y.ToString());
            Console.WriteLine();
            //60 - по моим измерениям
            if (e.GetPosition(this).Y > this.Height - 60) return;

            if (this.Width - 35.5 <= e.GetPosition(this).X) return;

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

        private DataGridRow GetRowItem(int index)
        {
            if (myGrid.ItemContainerGenerator.Status
                != GeneratorStatus.ContainersGenerated)
                return null;
            return myGrid.ItemContainerGenerator.ContainerFromIndex(index)
                as DataGridRow;
        }

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

        int GetCurrentRowIndex(GetPosition pos)
        {
            int curIndex = -1;
            for (int i = 0; i < myGrid.Items.Count; i++)
            {
                DataGridRow itm = GetRowItem(i);
                if (GetMouseTargetRow(itm, pos))
                {
                    curIndex = i;
                    break;
                }
            }
            Console.WriteLine("GetCurrentRowIndex : " + curIndex);
            return curIndex;
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                var v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T ?? GetVisualChild<T>(v);
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        private static DataGridRow GetRow(DataGrid host, int index)
        {
            var row = (DataGridRow)host.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // may be virtualized, bring into view and try again
                host.ScrollIntoView(host.Items[index]);
                row = (DataGridRow)host.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

    }
}
