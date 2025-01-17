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
                string selectedAlgorithm = (AlgorithmComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                switch (selectedAlgorithm)
                {
                    case "Selection Sort":
                        SelectionSort(_data);
                        break;
                    case "Bubble Sort":
                        BubbleSort(_data);
                        break;
                    case "Insertion Sort":
                        InsertionSort(_data);
                        break;
                    case "Heap Sort":
                        HeapSort(_data);
                        break;
                    case "Merge Sort":
                        MergeSort(_data);
                        break;
                    case "Quick Sort":
                        QuickSort(_data);
                        break;
                    case "Radix Sort":
                        RadixSort(_data);
                        break;
                    default:
                        throw new Exception();
                }
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


        private void HeapSort(List<string> data)
        {
            int n = data.Count;

            // Build a max heap
            for (int i = n / 2 - 1; i >= 0; i--)
            {
                Heapify(data, n, i);
            }

            // Extract elements from the heap one by one
            for (int i = n - 1; i > 0; i--)
            {
                // Move current root to the end
                var temp = data[0];
                data[0] = data[i];
                data[i] = temp;

                // Call heapify on the reduced heap
                Heapify(data, i, 0);
            }
        }

        private void Heapify(List<string> data, int n, int i)
        {
            int largest = i; // Initialize largest as root
            int left = 2 * i + 1; // Left child index
            int right = 2 * i + 2; // Right child index

            // If left child is larger than root
            if (left < n && string.Compare(data[left], data[largest], StringComparison.Ordinal) > 0)
            {
                largest = left;
            }

            // If right child is larger than the largest so far
            if (right < n && string.Compare(data[right], data[largest], StringComparison.Ordinal) > 0)
            {
                largest = right;
            }

            // If largest is not root
            if (largest != i)
            {
                var temp = data[i];
                data[i] = data[largest];
                data[largest] = temp;

                // Recursively heapify the affected subtree
                Heapify(data, n, largest);
            }
        }

        private void MergeSort(List<string> data)
        {
            if (data.Count <= 1)
            {
                return; // Base case: a list with 0 or 1 elements is already sorted.
            }

            // Split the list into two halves
            int mid = data.Count / 2;
            var left = data.GetRange(0, mid);
            var right = data.GetRange(mid, data.Count - mid);

            // Recursively sort each half
            MergeSort(left);
            MergeSort(right);

            // Merge the sorted halves
            Merge(data, left, right);
        }

        private void Merge(List<string> data, List<string> left, List<string> right)
        {
            int i = 0, j = 0, k = 0;

            // Compare elements from left and right lists and merge them in sorted order
            while (i < left.Count && j < right.Count)
            {
                if (string.Compare(left[i], right[j], StringComparison.Ordinal) <= 0)
                {
                    data[k++] = left[i++];
                }
                else
                {
                    data[k++] = right[j++];
                }
            }

            // Copy any remaining elements from the left list
            while (i < left.Count)
            {
                data[k++] = left[i++];
            }

            // Copy any remaining elements from the right list
            while (j < right.Count)
            {
                data[k++] = right[j++];
            }
        }



        private void QuickSort(List<string> data)
        {
            QuickSort(data, 0, data.Count - 1);
        }

        private void QuickSort(List<string> data, int low, int high)
        {
            if (low < high)
            {
                // Partition the array and get the pivot index
                int pivotIndex = Partition(data, low, high);

                // Recursively sort elements before and after the pivot
                QuickSort(data, low, pivotIndex - 1);
                QuickSort(data, pivotIndex + 1, high);
            }
        }

        private int Partition(List<string> data, int low, int high)
        {
            // Choose the last element as the pivot
            var pivot = data[high];
            int i = low - 1; // Index of smaller element

            for (int j = low; j < high; j++)
            {
                // If the current element is less than or equal to the pivot
                if (string.Compare(data[j], pivot, StringComparison.Ordinal) <= 0)
                {
                    i++;

                    // Swap data[i] and data[j]
                    var temp = data[i];
                    data[i] = data[j];
                    data[j] = temp;
                }
            }

            // Swap data[i + 1] and data[high] (the pivot)
            var tempPivot = data[i + 1];
            data[i + 1] = data[high];
            data[high] = tempPivot;

            return i + 1; // Return the partitioning index
        }

        private void RadixSort(List<string> data)
        {
            if (data == null || data.Count <= 1)
            {
                return; // Base case: already sorted or empty list.
            }

            // Find the maximum length of strings in the list
            int maxLength = data.Max(str => str.Length);

            // Sort by each character position, starting from the least significant (rightmost)
            for (int pos = maxLength - 1; pos >= 0; pos--)
            {
                CountingSortByCharacter(data, pos);
            }
        }

        private void CountingSortByCharacter(List<string> data, int charPosition)
        {
            int n = data.Count;
            var output = new string[n];
            var count = new int[256]; // Assuming ASCII characters

            // Initialize count array
            for (int i = 0; i < 256; i++)
            {
                count[i] = 0;
            }

            // Count occurrences of characters at the specified position
            foreach (var str in data)
            {
                int charIndex = charPosition < str.Length ? str[charPosition] : 0; // 0 for padding
                count[charIndex]++;
            }

            // Update count[i] to hold the actual position of this character in output
            for (int i = 1; i < 256; i++)
            {
                count[i] += count[i - 1];
            }

            // Build the output array by placing elements in sorted order
            for (int i = n - 1; i >= 0; i--)
            {
                int charIndex = charPosition < data[i].Length ? data[i][charPosition] : 0; // 0 for padding
                output[count[charIndex] - 1] = data[i];
                count[charIndex]--;
            }

            // Copy the sorted output array back into the original list
            for (int i = 0; i < n; i++)
            {
                data[i] = output[i];
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