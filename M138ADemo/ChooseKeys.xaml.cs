using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace M138ADemo
{
    /// <summary>
    /// Interaction logic for ChooseKeys.xaml
    /// </summary>
    public partial class ChooseKeys : Window
    {

        public ObservableCollection<Pair<int, String>> lst = new ObservableCollection<Pair<int, string>>();

        private List<Button> ChooseButtons = new List<Button>();
        private List<Button> ClickedButtins = new List<Button>();
        private Dictionary<Button, Pair<int, String>> dict = new Dictionary<Button, Pair<int, string>>();
        public ChooseKeys()
        {
            lst = Generator.GenerateRandomKeys(numberOfKeys: 30, seed: 0);

            InitializeComponent();
            int numberOfRows = lst.Count;
            int numberOfCol = 26 + 1 + 1;

            ConstructGrid(numberOfRows, numberOfCol);

            //Add buttons to bottom

            Button AddButton = new Button();
            AddButton.Template = (ControlTemplate) Application.Current.Resources["RoundButtonSimple"];
            //AddButton.Background = Brushes.Red;
            AddButton.Margin=new Thickness(0,00,0,10);
            AddButton.FontSize = 14;
            AddButton.Content = "Добавить выбранные";
            Grid.SetRow(AddButton, lst.Count);
            Grid.SetColumn(AddButton, 20);
            Grid.SetColumnSpan(AddButton, 7);
            Grid.SetRowSpan(AddButton, 2);
            Grid.Children.Add(AddButton);
            AddButton.Click += AddButtonOnClick;
            

            /*Button EndButton = new Button();
            EndButton.Template = (ControlTemplate)Application.Current.Resources["RoundButtonSimple"];
            EndButton.Margin = new Thickness(0,0,0,10);
            EndButton.FontSize = 14;
            EndButton.Content = "Закончить";
            Grid.SetRow(EndButton, lst.Count);
            Grid.SetColumn(EndButton, 10);
            Grid.SetRowSpan(EndButton, 2);
            Grid.SetColumnSpan(EndButton, 7);
            Grid.Children.Add(EndButton);*/


            AddBackgroundToRows();

            //Adding textboxes to write number in rows sequence
            List<TextBox> textBoxes = new List<TextBox>();
            TextBlock[,] textBoxArray = new TextBlock[numberOfRows, numberOfCol];

            AddTextBoxesAndBlocks(numberOfRows, numberOfCol, textBoxArray);
        }

        private void AddButtonOnClick(object sender, RoutedEventArgs e)
        {
            foreach (var el in ClickedButtins)
            {
                Configuration.lst.Add(dict[el]);
                Console.WriteLine(dict[el].first);
            }

            MessageBox.Show("Ключи были добавлены корректно", "", MessageBoxButton.OK);
            this.Close();
        }

        private void AddTextBoxesAndBlocks(int numberOfRows, int numberOfCol, TextBlock[,] textBoxArray)
        {
            for (int i = 0; i < numberOfRows; ++i)
            {
                Button btn = new Button();
                btn.Template = (ControlTemplate)Application.Current.Resources["RoundButton"];
                
                btn.Background = Brushes.Aqua;
                btn.BorderBrush = Brushes.Black;
                btn.Click += SelectedRow;
                btn.Margin = new Thickness(0, 4, 3, 5);
                ChooseButtons.Add(btn);
                Grid.SetRow(btn, i);
                Grid.SetColumn(btn, 0);
                Grid.Children.Add(btn);
                dict[btn] = lst[i];
            }

            //second column
            for (int i = 0; i < numberOfRows; ++i)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = lst[i].first.ToString();
                textBlock.Margin = new Thickness(0, 5, 0, 0);
                textBlock.IsEnabled = false;
                Grid.SetRow(textBlock, i);
                Grid.SetColumn(textBlock, 1);
                Grid.Children.Add(textBlock);
            }

            for (int i = 0; i < numberOfRows; ++i)
            {
                for (int j = 2; j < numberOfCol; ++j)
                {
                    int realInd = j - 2;

                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = new string(Helper.ToUpper(lst[i].second[realInd]), 1);
                    textBlock.Margin = new Thickness(0, 5, 0, 0);
                    textBlock.TextAlignment = TextAlignment.Center;
                    Rectangle rectangle = new Rectangle();
                    rectangle.HorizontalAlignment = HorizontalAlignment.Right;
                    rectangle.Width = 1;
                    rectangle.Margin = new Thickness(5, 8, 0, 10);
                    rectangle.Stroke = new SolidColorBrush(Colors.Black);
                    //textBlock.Background = new ImageBrush() {ImageSource = imageSource};
                    textBoxArray[i, j] = textBlock;
                    //Console.WriteLine(new string(lst[i].second[realInd], 1));
                    Grid.SetRow(rectangle, i);
                    Grid.SetColumn(rectangle, j);
                    Grid.SetRow(textBlock, i);
                    Grid.SetColumn(textBlock, j);
                    Grid.Children.Add(textBlock);
                    Grid.Children.Add(rectangle);
                }
            }
        }

        private void SelectedRow(object sender, RoutedEventArgs e)
        {
            
            Button currBtn = (Button) sender;

            if (currBtn.Background == Brushes.Aqua)
            {
                currBtn.Background = Brushes.Blue;
                ClickedButtins.Add(currBtn);
            }
            else
            {
                currBtn.Background = Brushes.Aqua;
                Predicate<Button> myPredicate = button => button == e.Source;
                
                ClickedButtins.Remove(ClickedButtins.Find(myPredicate));
            }
        }

        private void TextBoxOnLostFocus(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AddBackgroundToRows()
        {
            ImageSource imageSourcePaper = new BitmapImage(new Uri("pack://application:,,,/Images/texture_paper2.jpg"));
            //Add backgroung image to rows
            for (int i = 0; i < Grid.RowDefinitions.Count - 2; ++i)
            {
                Image image = new Image();
                image.Source = imageSourcePaper;
                image.Margin = new Thickness(0, 5, 0, 7);
                image.Stretch = Stretch.UniformToFill;
                Grid.SetRow(image, i);
                Grid.SetColumn(image, 0);
                Grid.SetColumnSpan(image, Grid.ColumnDefinitions.Count);
                Grid.Children.Add(image);
            }
        }

        private void ConstructGrid(int numberOfRows, int numberOfCol)
        {
            ImageSource imageSource = new BitmapImage(new Uri("pack://application:,,,/Images/aluminum_texture1.jpg"));
            Grid.Background = new ImageBrush(imageSource);
            for (int i = 0; i < numberOfRows + 2; ++i)
            {
                RowDefinition currRow = new RowDefinition();
                currRow.Height = new GridLength(30);


                Grid.RowDefinitions.Add(currRow);
            }

            for (int i = 0; i < numberOfCol; ++i)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                if (i == 0)
                {
                    columnDefinition.Width = new GridLength(45);
                }
                else if (i == 1)
                {
                    columnDefinition.Width = new GridLength(60);
                }
                else
                {
                    columnDefinition.Width = new GridLength(30);
                }

                Grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }
    }
}
