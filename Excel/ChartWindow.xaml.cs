using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Data;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LiveCharts.Defaults;
using Microsoft.Win32;
using System.IO;

namespace Excel
{
    /// <summary>
    /// Interaction logic for ChartWindow.xaml
    /// </summary>
    public partial class ChartWindow : Window
    {
        /// <summary>
        /// Таблица с данными.
        /// </summary>
        DataTable dt;

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="dt"> Таблица с данными. </param>
        public ChartWindow(DataTable dt)
        {
            InitializeComponent();
            this.dt = dt;
            FillDataX();
            FillDataY();
        }
        
        /// <summary>
        /// Нужно для работы графика.
        /// </summary>
        public Func<double, string> Formatter { get; set; }

        /// <summary>
        /// Данные для графика.
        /// </summary>
        public SeriesCollection SeriesPoints { get; set; }

        /// <summary>
        /// Получение данных для графика.
        /// </summary>
        /// <returns> Данные для графика. </returns>
        private SeriesCollection GetSeries()
        {
            Dictionary<double, double[]> dict = new Dictionary<double, double[]>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string columnX = boxDataX.SelectedItem.ToString();
                double key = Convert.ToDouble(dt.Rows[i][columnX]);
                if (dict.ContainsKey(key))
                {
                    dict[key][1]++;
                }
                else
                {
                    string columnY = boxDataY.SelectedItem.ToString();
                    double value = Convert.ToDouble(dt.Rows[i][columnY]);
                    dict.Add(key, new[] { value, 1.0 });
                }
            }

            var values = new ChartValues<ObservablePoint>();
            List<double> sortedKeys = dict.Keys.ToList();
            sortedKeys.Sort();
            foreach (var key in sortedKeys)
            {
                values.Add(new ObservablePoint(key, dict[key][0]/dict[key][1]));
            }


            var result = new SeriesCollection
            {
                new LineSeries
                {
                    Values=values
                }
            };
            return result;
        }

        /// <summary>
        /// Заполнение осей возможными данными.
        /// </summary>
        private void FillDataX()
        {
            boxDataX.ItemsSource = StatsWindow.GetNumericColumnsNames(dt);
        }

        /// <summary>
        /// Заполнение осей возможными данными.
        /// </summary>
        private void FillDataY()
        {
            boxDataY.ItemsSource = StatsWindow.GetNumericColumnsNames(dt);
        }

        /// <summary>
        /// Изменение столбца для оси X.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы событий. </param>
        private void boxDataX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (boxDataX.SelectedIndex != -1 && boxDataY.SelectedIndex != -1)
            {
                Formatter = value => value.ToString() + "N";
                SeriesPoints = GetSeries();
                chart.Series = SeriesPoints;
                chart.AxisX[0].Title = boxDataX.SelectedItem.ToString();
                saveButton.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Изменение столбца для оси Y.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы событий. </param>
        private void boxDataY_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (boxDataX.SelectedIndex != -1 && boxDataY.SelectedIndex != -1)
            {
                Formatter = value => value.ToString() + "N";
                SeriesPoints = GetSeries();
                chart.Series = SeriesPoints;
                chart.AxisY[0].Title = boxDataY.SelectedItem.ToString();
                saveButton.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Метод при нажатии кнопки сохранить.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы событий. </param>
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла{Environment.NewLine}{ex.Message}", "Ошибка");
            }
        }

        /// <summary>
        /// Сохранение графика в PNG.
        /// </summary>
        /// <param name="visual"> График. </param>
        /// <param name="fileName"> Путь сохранения. </param>
        private void SaveToPng(FrameworkElement visual, string fileName)
        {
            var encoder = new PngBitmapEncoder();
            EncodeVisual(visual, fileName, encoder);
        }

        /// <summary>
        /// Непосредственное сохранение графика. 
        /// </summary>
        /// <param name="visual"> График. </param>
        /// <param name="fileName"> Путь сохранения. </param>
        /// <param name="encoder"> Кодировщик. </param>
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
