using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;

namespace Excel
{
    /// <summary>
    /// Interaction logic for HistogramWindow.xaml
    /// </summary>
    public partial class HistogramWindow : Window
    {
        DataTable dt;

        public HistogramWindow(DataTable dt)
        {
            InitializeComponent();
            this.dt = dt;
            FillBoxData();
        }

        private void FillBoxData()
        {
            List<string> names = new List<string>();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                names.Add(dt.Columns[i].ColumnName);
            }
            boxData.ItemsSource = names;
        }

        private void FillHistogram(string columnName, int columnWidth = 1)
        {
            BarLabels = GetBarLabels(columnName, columnWidth);
            BarCollection = GetSeriesCollection(columnName, columnWidth);

            Formatter = value => value.ToString("N");
            DataContext = this;
            chart.AxisX[0].Labels = BarLabels;
            chart.Series = BarCollection;
        }

        private SeriesCollection GetSeriesCollection(string columnName, int columnWidth = 1)
        {
            SeriesCollection series = new SeriesCollection();
            Dictionary<string, double> dict = new Dictionary<string, double>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string key = dt.Rows[i][columnName].ToString();
                if (dict.ContainsKey(key))
                {
                    dict[key]++;
                }
                else
                {
                    dict.Add(key, 1);
                }
            }

            ChartValues<double> allValues = new ChartValues<double>();

            foreach (var key in dict.Keys)
            {
                allValues.Add(dict[key]);
            }

            ChartValues<double> values = new ChartValues<double>();
            for (int i = 0; i < allValues.Count; i++)
            {
                if (i % columnWidth == 0)
                {
                    values.Add(allValues[i]);
                }
                else
                {
                    int index = (i - i % columnWidth) / columnWidth;
                    values[index] += allValues[i];
                }
            }

            series.Add(new ColumnSeries
            {
                Title = columnName,
                Values = values
            });

            return series;
        }

        private List<string> GetBarLabels(string columnName, int columnWidth = 1)
        {
            List<string> allBarLabels = new List<string>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string barLabel = dt.Rows[i][columnName].ToString();
                if (!allBarLabels.Contains(barLabel))
                {
                    allBarLabels.Add(barLabel);
                }
            }
            if (columnWidth > allBarLabels.Count)
            {
                throw new ArgumentException("ColumnWidth can't be more than number of bars.");
            }

            List<string> barLabels = new List<string>();
            for (int i = 0; i < allBarLabels.Count; i++)
            {
                if (i % columnWidth == 0)
                {
                    barLabels.Add(allBarLabels[i]);
                }
                else
                {
                    int index = (i - i % columnWidth) / columnWidth;
                    barLabels[index] += $",{allBarLabels[i]}";
                }
            }

            return barLabels;
        }

        public Func<double, string> Formatter { get; set; }
        public SeriesCollection BarCollection { get; set; }
        public List<string> BarLabels { get; set; }

        private void boxData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string columnName = boxData.SelectedItem.ToString();
            integerUpDown.Value = 1;
            FillHistogram(columnName);
            colorPicker.Visibility = Visibility.Visible;
            labelColor.Visibility = Visibility.Visible;
            saveButton.Visibility = Visibility.Visible;
        }

        private void integerUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!(boxData is null || boxData.SelectedIndex == -1))
            {
                if (integerUpDown.Value <= 0)
                {
                    MessageBox.Show("Нельзя установить ширину меньше 1", "Ошибка");
                    integerUpDown.Value = 1;
                }
                else
                {
                    try
                    {
                        FillHistogram(boxData.SelectedItem.ToString(), (int)integerUpDown.Value);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show($"Слишком большая ширина столбцов!{Environment.NewLine}{ex.Message}", "Ошибка");
                    }
                }
            }
        }

        private void colorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            chart.SeriesColors = new ColorsCollection
            {
                (System.Windows.Media.Color)colorPicker.SelectedColor
            };
            FillHistogram(boxData.SelectedItem.ToString(),(int)integerUpDown.Value);
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "png images (*.png)|*.png";
                if (saveFileDialog.ShowDialog() == true)
                {
                    SaveToPng(chart, saveFileDialog.FileName);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла{Environment.NewLine}{ex.Message}", "Ошибка");
            }
        }

        private void SaveToPng(FrameworkElement visual, string fileName)
        {
            var encoder = new PngBitmapEncoder();
            EncodeVisual(visual, fileName, encoder);
        }

        private static void EncodeVisual(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            var bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            var frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);
            using (var stream = File.Create(fileName)) encoder.Save(stream);
        }
    }
}
