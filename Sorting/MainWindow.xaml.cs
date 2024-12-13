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
using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sorting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> _data;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void GenerateNumbers_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(MaxLinesTextBox.Text.Trim(), out int maxWords) || maxWords <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for max lines.",
                                "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            var random = new Random();
            _data = new List<string>();
            for (int i = 0; i < maxWords; i++)
            {
                _data.Add(random.Next(1, 1000).ToString());
            }
            UpdateListBox();

            MessageBox.Show($"Generated {maxWords} random numbers!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void GenerateWords_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(MaxLinesTextBox.Text.Trim(), out int maxWords) || maxWords <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for max lines.",
                                "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var wordList = new List<string>
            {
                "apple", "orange", "banana", "grape", "kiwi", "cherry", "peach", "plum", "pear", "mango",
                "lemon", "lime", "blueberry", "raspberry", "strawberry", "blackberry", "watermelon",
                "pineapple", "papaya", "apricot"
            };

            var random = new Random();
            _data = new List<string>();
            for (int i = 0; i < maxWords; i++)
            {
                var randomWord = wordList[random.Next(wordList.Count)];
                _data.Add(randomWord);
            }

            UpdateListBox();

            MessageBox.Show($"Generated {maxWords} random words!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateListBox()
        {
            DataListBox.ItemsSource = null;
            DataListBox.ItemsSource = _data;
        }

        private void SortData_Click(object sender, RoutedEventArgs e)
        {
            if (_data == null || !_data.Any())
            {
                MessageBox.Show("No data to sort.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var totalLines = _data.Count; 

            var stopwatch = Stopwatch.StartNew();

            if (NumericSortCheckBox.IsChecked == true)
            {
                if (_data.All(item => int.TryParse(item, out _))) 
                {
                    _data = _data.Select(int.Parse).OrderBy(x => x).Select(x => x.ToString()).ToList();
                }
                else
                {
                    MessageBox.Show("Some items are not numeric and cannot be sorted as numbers.",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                _data.Sort();
            }

            stopwatch.Stop();
            UpdateListBox();

            TimeTextBlock.Text = $"Sorting completed in {stopwatch.ElapsedMilliseconds} ms.\nTotal lines sorted: {totalLines}";

        }


        private void SelectionSort(List<string> data)
        {
            int n = data.Count;
            for (int i = 0; i < n - 1; i++)
            {
                int minIndex = i;
                for (int j = i + 1; j < n; j++)
                {
                    if (string.Compare(data[j], data[minIndex], StringComparison.Ordinal) < 0)
                    {
                        minIndex = j;
                    }
                }
                // Swap elements
                if (minIndex != i)
                {
                    var temp = data[i];
                    data[i] = data[minIndex];
                    data[minIndex] = temp;
                }
            }
        }

        private void BubbleSort(List<string> data)
        {
            int n = data.Count;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (string.Compare(data[j], data[j + 1], StringComparison.Ordinal) > 0)
                    {
                        // Swap elements
                        var temp = data[j];
                        data[j] = data[j + 1];
                        data[j + 1] = temp;
                    }
                }
            }
        }

        private void InsertionSortTest(List<int> list)
        {
            int item, j;
            for (int i = 1; i <= (list.Count - 1); i++)
            {
                // ulozeni prvku
                item = list[i];
                j = i - 1;
                while ((j >= 0) && (list[j] > item))
                {
                    list[j + 1] = list[j];
                    j--;
                }
                list[j + 1] = item;
            }
        }

        private void InsertionSort(List<string> data)
        {
            int n = data.Count;
            for (int i = 1; i < n; i++)
            {
                var key = data[i];
                int j = i - 1;

                // Move elements of data[0..i-1] that are greater than key one position ahead
                while (j >= 0 && string.Compare(data[j], key, StringComparison.Ordinal) > 0)
                {
                    data[j + 1] = data[j];
                    j--;
                }
                data[j + 1] = key;
            }
        }



        private void LoadData_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                Title = "Select a Data File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    IEnumerable<string> lines;
                    var input = MaxLinesTextBox.Text.Trim().ToLower();

                    if (input == "all")
                    {
                        lines = File.ReadLines(openFileDialog.FileName);
                    }
                    else if (int.TryParse(input, out int maxLines) && maxLines > 0)
                    {
                        lines = File.ReadLines(openFileDialog.FileName).Take(maxLines);
                    }
                    else
                    {
                        MessageBox.Show("Please enter a valid positive number or 'all' for the max lines.",
                                        "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    _data = lines.ToList();
                    UpdateListBox();

                    string message = input == "all"
                        ? "All lines were loaded successfully!"
                        : $"Data loaded successfully! Only the first {lines.Count()} lines were read.";

                    MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveData_Click(object sender, RoutedEventArgs e)
        {
            if (_data == null || !_data.Any())
            {
                MessageBox.Show("No data to save. Please sort or load data first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                Title = "Save Sorted Data"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllLines(saveFileDialog.FileName, _data);
                    MessageBox.Show("Data saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}