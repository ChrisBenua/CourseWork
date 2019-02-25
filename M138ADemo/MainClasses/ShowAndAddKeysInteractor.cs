﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M138ADemo.ViewModels;
using M138ADemo.MainClasses;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Data;

namespace M138ADemo.MainClasses
{
    public class ShowAndAddKeysInteractor
    {
        public static ImageBrush DefaultRowBrush = new ImageBrush(Helper.PapSource);

        public AddUsersKeysViewModel viewModel;
        public DataGrid dataGrid;

        public ShowAndAddKeysInteractor(DataGrid grid, AddUsersKeysViewModel model)
        {
            this.dataGrid = grid;
            dataGrid.EnableColumnVirtualization = false;
            dataGrid.EnableRowVirtualization = false;
            this.viewModel = model;

            dataGrid.CellStyle = new Style()
            {
                Setters = { new Setter(DataGridCell.MarginProperty, new Thickness(0, 5 / 1.5, 0, 11 / 1.5)),
                new Setter()
                {
                    Property = DataGridCell.BackgroundProperty,
                    Value = DefaultRowBrush
                },
                new Setter()
                {
                    Property = DataGridCell.BorderBrushProperty,
                    Value = DefaultRowBrush
                },

                },
            };

            dataGrid.RowStyle = new Style()
            {
                Setters =
                {
                    new Setter()
                    {
                        Property = DataGridCell.BackgroundProperty,
                        Value = Brushes.Transparent
                    },
                }
            };

            dataGrid.CanUserResizeRows = false;
            dataGrid.CanUserResizeColumns = false;
            dataGrid.GridLinesVisibility = DataGridGridLinesVisibility.None;
            dataGrid.AutoGenerateColumns = false;
            dataGrid.SelectionMode = DataGridSelectionMode.Extended;
            dataGrid.Background = new ImageBrush(Helper.AluminumSource);
            dataGrid.ItemsSource = viewModel.UserKeys;
            dataGrid.IsReadOnly = true;

            for (int i = 0; i < 26 + 2; ++i)
            {
                if (i < 1)
                {
                    DataGridTemplateColumn col = new DataGridTemplateColumn()
                    {
                        CellTemplate = getDataTemplate(i),
                    };
                    col.Width = new DataGridLength(100);
                    dataGrid.Columns.Add(col);
                }
                else
                {
                    var column = new DataGridTemplateColumn()
                    {
                        CellTemplate = getTextColumnTemplate(i),
                        CellEditingTemplate = getTextColumnTemplate(i),
                        Width = 30,
                    };

                    column.Header = $"{i - 1}";
                    dataGrid.Columns.Add(column);
                }
            }
        }

        public DataTemplate getDataTemplate(int cnt)
        {
            DataTemplate template = new DataTemplate();
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(Button));
            factory.SetValue(Button.TemplateProperty, Application.Current.Resources["RoundButton"]);
            factory.SetValue(Button.BackgroundProperty, Brushes.Red);

            factory.SetValue(Button.ContentProperty, "Удалить");
            factory.SetValue(Button.FontSizeProperty, 15.0);
            factory.AddHandler(Button.ClickEvent, new RoutedEventHandler(DeleteButtonOnClick));
            //factory.SetValue(Button.CommandProperty, viewModel.DeleteRowCommand);
            //factory.SetValue(Button.CommandParameterProperty, dataGrid.Items.Count - 1);
            template.VisualTree = factory;

            return template;
        }

        public void DeleteButtonOnClick(object sender, RoutedEventArgs e)
        {
            KeyForPersistance obj = ((FrameworkElement)sender).DataContext as KeyForPersistance;
            int index = viewModel.UserKeys.IndexOf(obj);
            viewModel.DeleteRowCommand.Execute(index);
        }

        public DataTemplate getTextColumnTemplate(int index)
        {
            DataTemplate template = new DataTemplate();
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBox));
            factory.SetValue(TextBox.FontSizeProperty, 15.0);
            factory.SetValue(TextBox.BackgroundProperty, Brushes.Transparent);
            factory.SetValue(TextBox.IsEnabledProperty, true);
            factory.SetValue(TextBox.BorderBrushProperty, Brushes.Transparent);
            factory.SetValue(TextBox.BorderThicknessProperty, new Thickness(0));
            if (index == 1)
            {
                factory.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(KeyNumberOnPreviewTextInput));

                if (viewModel is ShowKeysViewModel)
                {
                    factory.SetBinding(TextBox.TextProperty, new Binding()
                    {
                        Path = new PropertyPath("Id"),
                        Mode = BindingMode.TwoWay,
                        Converter = new IntToStringConverter(),
                        NotifyOnTargetUpdated = true,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    });
                }
                else
                {
                    factory.SetBinding(TextBox.TextProperty, new Binding()
                    {
                        Path = new PropertyPath("Id"),
                        Mode = BindingMode.OneWayToSource,
                        Converter = new IntToStringConverter(),
                        NotifyOnTargetUpdated = true,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    });
                }

                

                factory.AddHandler(TextBox.LostFocusEvent, new RoutedEventHandler((s, e) =>
                {
                    viewModel.CheckKeysIdsCommand.Execute(null);
                }));
            }
            else
            {
                factory.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(TextBoxOnTextChanged));
                factory.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(TextBoxOnPreviewTextInput));

                if (viewModel is ShowKeysViewModel)
                {
                    factory.SetBinding(TextBox.TextProperty, new Binding()
                    {
                        Path = new PropertyPath($"KeyArr[{index - 2}]"),
                        Mode = BindingMode.TwoWay,
                        NotifyOnSourceUpdated = true,
                        NotifyOnTargetUpdated = true,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    });
                }
                else
                {
                    factory.SetBinding(TextBox.TextProperty, new Binding()
                    {
                        Path = new PropertyPath($"KeyArr[{index - 2}]"),
                        Mode = BindingMode.OneWayToSource,
                        NotifyOnSourceUpdated = true,
                        NotifyOnTargetUpdated = true,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    });
                }   
            }


            template.VisualTree = factory;
            return template;
        }

        public void KeyNumberOnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            //bool check = CheckKeyNumbers();
            viewModel.KeyPreviewInputCommand.Execute((txt.Text + e.Text, e));
        }

        public void TextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Text = Helper.Strip(textBox.Text).ToUpper().Substring(0, Math.Min(1, textBox.Text.Length));

            var sourceCell = DataGridExtensions.FindVisualParent<DataGridCell>(textBox);
            var gridRow = DataGridExtensions.FindVisualParent<DataGridRow>(sourceCell);
            int row = dataGrid.ItemContainerGenerator.IndexFromContainer(gridRow);
            int col = sourceCell.Column.DisplayIndex + 1;
            if (col != dataGrid.Columns.Count) {
                var cell = dataGrid.GetCell(row, col);
                (DataGridExtensions.GetVisualChild<TextBox>(cell) as TextBox).Focus();
                
                
            }

        }

        public void TextBoxOnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            while ((dep != null) && !(dep is DataGridRow))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep is DataGridRow)
            {
                DataGridRow row = dep as DataGridRow;
                int ind = FindRowIndex(row);
                // do something
                TextBox textBox = (TextBox)sender;

                if (Helper.isAlphaString(e.Text) && textBox.Text.Length + e.Text.Length == 1 && viewModel.isSequence(ind, e.Text))
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        public int FindRowIndex(DataGridRow row)
        {
            DataGrid dataGrid1 =
                ItemsControl.ItemsControlFromItemContainer(row)
                as DataGrid;

            int index = dataGrid1.ItemContainerGenerator.
                IndexFromContainer(row);

            return index;
        }
    }
}
