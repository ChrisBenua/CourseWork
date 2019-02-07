using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace M138ADemo
{
    /// <summary>
    /// Interaction logic for AddUsersKey.xaml
    /// </summary>
    public partial class AddUsersKey : Window
    {

        private Dictionary<Button, int> AddBtnToInd = new Dictionary<Button, int>();
        Dictionary<int, List<UIElement>> elemsInRow = new Dictionary<int, List<UIElement>>();
        private Dictionary<Button, int> DeleteBtnToInd = new Dictionary<Button, int>();
        private Dictionary<int, string> keysInRow = new Dictionary<int, string>();
        private Dictionary<TextBox, Pair<int, int>> where = new Dictionary<TextBox, Pair<int, int>>();
        List<RowDefinition> rowDefinitions = new List<RowDefinition>();
        
        int numberOfCols = 26 + 1 + 1;
        int numberOfLines = 1;
        private const int MaxLines = 30;
        private List<int> widths = new List<int>();
        private TextBox[,] textBoxs;
        private Image backgroundImage;
        public AddUsersKey()
        {
            InitializeComponent();

            setBackground();
            UpdateBackGround();

            textBoxs = new TextBox[MaxLines, numberOfCols];
            for (int i = 0; i < numberOfCols; ++i)
            {
                if (i < 1)
                    widths.Add(60);
                else if (i == 1)
                    widths.Add(45);
                else
                {
                    widths.Add(30);
                }
            }

            setUpRowAndCols();
            AddMenu();

        }

        void UpdateNextButton()
        {

        }

        #region SetUpGrid

        void UpdateBackGround()
        {
            if (Grid.RowDefinitions.Count > 1)
            {
                backgroundImage.Visibility = Visibility.Visible;
            }
            else
            {
                backgroundImage.Visibility = Visibility.Hidden;
            }
        }

        void AddDeleteButtons()
        {
            Button deleteButton = new Button();
            deleteButton.Template = (ControlTemplate)Application.Current.Resources["RoundButtonNoTriggers"];
            deleteButton.Background = Brushes.Red;
            deleteButton.Margin = new Thickness(2);
            deleteButton.Content = "Удалить";
            deleteButton.BorderBrush = new SolidColorBrush(Colors.Black);
            deleteButton.BorderThickness=new Thickness(0.5);
            deleteButton.Click += DeleteButtonOnClick;
            Grid.SetRow(deleteButton, numberOfLines - 1);
            Grid.SetColumn(deleteButton, 0);
            Grid.Children.Add(deleteButton);
            elemsInRow[numberOfLines-1].Add(deleteButton);
            DeleteBtnToInd[deleteButton] = numberOfLines - 1;
        }

        private void DeleteButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button) sender;
            int ind = DeleteBtnToInd[btn];
            DeleteBtnToInd.Remove(btn);
            ReIndex(ind);
            Grid.Children.Remove(btn);
            

            numberOfLines--;
            UpdateBackGround();
        }

        void ReIndex(int ind)
        {
            List<Button> btns = new List<Button>();
            foreach (var el in DeleteBtnToInd.Keys)
            {
                if (DeleteBtnToInd[el] > ind)
                {
                    btns.Add(el);
                }
            }

            for (int i = 0; i < btns.Count; ++i)
            {
                DeleteBtnToInd[btns[i]]--;
            }

            foreach (var el in elemsInRow[ind])
            {
                Grid.Children.Remove(el);
            }
            Grid.RowDefinitions.RemoveAt(ind);

            for (int i = ind; i < numberOfLines - 1; ++i)
            {

                for (int j = 0; j < elemsInRow[i].Count; ++j)
                {
                    if (elemsInRow[i][j] is TextBox && @where.ContainsKey((TextBox)elemsInRow[i][j]))
                    {
                        //@where.Remove((TextBox)elemsInRow[i][j]);
                    }
                }
                elemsInRow[i] = elemsInRow[i + 1];

                

                for (int j = 0; j < elemsInRow[i].Count; ++j)
                {
                    Grid.SetRow(elemsInRow[i][j], i);
                    if (elemsInRow[i][j] is TextBox && @where.ContainsKey((TextBox) elemsInRow[i][j]))
                    {
                        TextBox box = (TextBox) elemsInRow[i][j];
                        @where[box].first--;
                        textBoxs[@where[box].first, @where[box].second] = (TextBox) elemsInRow[i][j];
                    }
                }
            }
            Console.WriteLine("");
            elemsInRow.Remove(elemsInRow.Count);

        }


        void setBackground()
        {
            //Grid.Children.Remove(backgroundImage);

            backgroundImage = new Image();
            backgroundImage.Source = Helper.AluminumSource;
            backgroundImage.Stretch = Stretch.UniformToFill;
            Grid.SetRow(backgroundImage, 1);
            Grid.SetRowSpan(backgroundImage, 30);
            Grid.SetColumn(backgroundImage, 1);
            Grid.SetColumnSpan(backgroundImage, 27);
            Grid.Children.Add(backgroundImage);
        }

        void setUpRowAndCols()
        {
            for (int i = 0; i < numberOfCols; ++i)
            {
                ColumnDefinition def = new ColumnDefinition();
                def.Width = new GridLength(widths[i]);
                Grid.ColumnDefinitions.Add(def);
            }
            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(40);
            Grid.RowDefinitions.Add(row);
            rowDefinitions.Add(row);
        }

        public void AddMenu()
        {
            Button addButton = new Button();
            addButton.Template = (ControlTemplate) Application.Current.Resources["RoundButtonAdd"];
            addButton.Background = Brushes.LightGreen;

            addButton.BorderThickness = new Thickness(0.5);
            addButton.Content = "Добавить ключ";
            addButton.Margin = new Thickness(20, 5, 20, 5);
            addButton.Click += ItemOnClick;
            
            Grid.SetRow(addButton, 0);
            Grid.SetColumn(addButton, 0);
            Grid.SetColumnSpan(addButton, 3);
            Grid.Children.Add(addButton);

            Button endButton = new Button();
            endButton.Template = (ControlTemplate) Application.Current.Resources["RoundButtonNoTriggers"];
            endButton.Background = Brushes.Red;
            endButton.BorderThickness = new Thickness(0.5);
            endButton.Content = "Закончить";
            endButton.Margin = new Thickness(20, 5, 20, 5);
            endButton.Click += EndButtonOnClick;
            Grid.SetRow(endButton, 0);
            Grid.SetColumn(endButton, 6);
            Grid.SetColumnSpan(endButton, 5);
            Grid.Children.Add(endButton);
        }

        private void EndButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (!CheckAllKeysAreFilled())
            {
                MessageBox.Show("Ошибка", "Все ключи должны быть заполнены польностью", MessageBoxButton.OK);
            }
            else if (!CheckKeyNumbers())
            {
                MessageBox.Show("Ошибка", "Все ключи должны быть разными");
            }
            else
            {
                for (int row = 1; row < numberOfLines; ++row)
                {
                    Configuration.lst.Add(ConstructKey(row));
                }
                Close();
            }
        }

       

        bool CheckAllKeysAreFilled()
        {
            bool ans = true;
            for (int row = 1; row < numberOfLines; ++row)
                ans &= KeyForRow(row).Length == 26;
            return ans;
        }

        string KeyForRow(int row)
        {
            string ans = "";
            for (int i = 2; i < numberOfCols; ++i)
            {
                ans += textBoxs[row, i].Text;
            }

            return ans;
        }

        Pair<int, string> ConstructKey(int row)
        {
            Pair<int, string> pair = new Pair<int, string>(0, "");
            pair.first = int.Parse(textBoxs[row, 1].Text);
            pair.second = KeyForRow(row);
            return pair;
        }

        #endregion

        #region Buttons

        private void ItemOnClick(object sender, RoutedEventArgs e)
        {
            if (numberOfLines > Helper.MaxKeys)
            {
                MessageBox.Show("Ошибка", "Слишком много строк", MessageBoxButton.OK);
            }
            else
            {
                AddNewKey();
            }
        }

        public void AddNewKey()
        {

            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(30);
            
            Grid.RowDefinitions.Add(row);
            rowDefinitions.Add(row);

            Image image = new Image();
            image.Source = Helper.PapSource;
            image.Margin = new Thickness(0, 4, 0, 2);
            image.Stretch = Stretch.UniformToFill;
            Grid.SetRow(image, numberOfLines);
            Grid.SetColumn(image, 1);
            Grid.SetColumnSpan(image, Grid.ColumnDefinitions.Count);
            Grid.Children.Add(image);

            if (!elemsInRow.ContainsKey(numberOfLines))
            {
                elemsInRow.Add(numberOfLines, new List<UIElement>());
                //elemsInRow[numberOfLines] = new List<UIElement>();
                //elemsInRow[numberOfLines].Add(image);
            }
            elemsInRow[numberOfLines].Add(image);

            //add TExtBox for id
            TextBox txBox = new TextBox();
            txBox.TextChanged += TextBoxOnTextChanged;
            txBox.Background = null;
            txBox.BorderThickness = new Thickness(0);
            txBox.Margin = new Thickness(4, 8, 0, 4);
            txBox.LostFocus += KeyIdOnLostFocus;
            @where[txBox] = Pair<int, int>.MakePair(numberOfLines, 1);
            textBoxs[numberOfLines, 1] = txBox;
            Grid.SetRow(txBox, numberOfLines);
            Grid.SetColumn(txBox, 1);
            Grid.Children.Add(txBox);
            txBox.PreviewTextInput += KeyNumberOnPreviewTextInput;
            elemsInRow[numberOfLines].Add(txBox);

            for (int i = 2; i < numberOfCols; ++i)
            {
                Rectangle rectangle = new Rectangle();
                rectangle.HorizontalAlignment = HorizontalAlignment.Right;
                rectangle.Width = 1;
                rectangle.Margin = new Thickness(5, 6, 0, 3);
                rectangle.Stroke = new SolidColorBrush(Colors.Black);

                TextBox textBox = new TextBox();
                textBox.FontSize = 20;
                textBox.BorderThickness = new Thickness(0);
                textBox.Background = null;
                textBox.Margin = new Thickness(2, 4, 0, 4);
                textBox.TextAlignment = TextAlignment.Center;
                textBox.PreviewTextInput += TextBoxOnPreviewTextInput;
                textBox.TextChanged += TextBoxOnTextChanged;
                textBoxs[numberOfLines, i] = textBox;
                @where[textBox] = Pair<int, int>.MakePair(numberOfLines, i);
                

                Grid.SetRow(rectangle, numberOfLines);
                Grid.SetColumn(rectangle, i-1);
                Grid.SetRow(textBox, numberOfLines);
                Grid.SetColumn(textBox, i);
                Grid.Children.Add(textBox);
                Grid.Children.Add(rectangle);
                elemsInRow[numberOfLines].Add(textBox);
                elemsInRow[numberOfLines].Add(rectangle);
            }
            numberOfLines++;
            AddDeleteButtons();
            UpdateBackGround();

        }

        private void KeyIdOnLostFocus(object sender, RoutedEventArgs e)
        {
            if (!CheckKeyNumbers())
            {
                MessageBox.Show("Ошибка", "Номера ключей повторяются", MessageBoxButton.OK);
            }
        }

        bool CheckKeyNumbers()
        {
            SortedSet<int> set = new SortedSet<int>(Configuration.forbiddenKeys);
            for (int i = 0; i < numberOfLines; ++i)
            {
                if (textBoxs[i, 1] != null && textBoxs[i, 1].Text.Length > 0)
                {
                    if (set.Contains(int.Parse(textBoxs[i, 1].Text)))
                    {
                        return false;
                    }

                    set.Add(int.Parse(textBoxs[i, 1].Text));
                }
            }

            return true;
        }

        private void KeyNumberOnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox txt = (TextBox) sender;
            //bool check = CheckKeyNumbers();
            if (Helper.reg.IsMatch(txt.Text + e.Text) && int.Parse(txt.Text+ e.Text) <= Helper.MaxKeys)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;

                MessageBox.Show("Тут не число или оно слишком большое", "Ошибка", MessageBoxButton.OK);
            }
        }

        private bool IsSequence(int row, int col, string txt)
        {
            txt = txt.ToUpper();
            SortedSet<string> set = new SortedSet<string>();
            for (int i = 2; i < numberOfCols; ++i)
            {
                string currTxt = textBoxs[row, i].Text;
                if (i == col)
                {
                    currTxt = txt;
                }
                if (currTxt == String.Empty)
                {
                    continue;
                }
                //Console.WriteLine("{0}  {1}", currTxt, set.Contains(currTxt));
                if (set.Contains(currTxt))
                {
                    MessageBox.Show("Ошибка", "Повторяющиеся символы в строке " + row, MessageBoxButton.OK);
                    return false;
                }

                set.Add(currTxt);
            }

            return true;
        }

        #endregion
        private void TextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Text = Helper.Strip(textBox.Text).ToUpper();
        }

        private void TextBoxOnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox) sender;
            
            if (Helper.isAlphaString(e.Text) && textBox.Text.Length + e.Text.Length == 1 && IsSequence(@where[textBox].first, @where[textBox].second, e.Text))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }

        }
    }
}
