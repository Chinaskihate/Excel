using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Excel
{
    /// <summary>
    /// Interaction logic for StatsWindow.xaml
    /// </summary>
    public partial class StatsWindow : Window
    {
        /// <summary>
        /// Таблица с данными,
        /// среднее значение(для удобства).
        /// </summary>
        DataTable dt;
        double average = 1;

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="dt"> Таблица с данными. </param>
        public StatsWindow(DataTable dt)
        {
            InitializeComponent();
            this.dt = dt;
            boxData.ItemsSource = GetNumericColumnsNames(dt);
        }

        /// <summary>
        /// Метод возвращающий названия столбцов
        /// с числовыми значениями.
        /// </summary>
        /// <param name="dt"> Таблица с данными. </param>
        /// <returns> Список названий столбцов с числовыми данными. </returns>
        static public List<string> GetNumericColumnsNames(DataTable dt)
        {
            List<string> names = new List<string>();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (dt.Columns[i].DataType == typeof(double))
                {
                    names.Add(dt.Columns[i].ColumnName);
                }
            }
            return names;
        }

        /// <summary>
        /// Метод при изменении значения выбранного столбца.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы событий. </param>
        private void boxData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (boxData.SelectedIndex != -1)
            {
                SetLabels();
            }
        }

        /// <summary>
        /// Установка значений в Label-ах.
        /// </summary>
        private void SetLabels()
        {
            if (dt.Rows.Count != 0)
            {
                SetAverage();
                SetMedian();
                SetDeviation();
                SetDispersion();
            }
        }

        /// <summary>
        /// Установка среднего значения.
        /// </summary>
        private void SetAverage()
        {
            string columnName = boxData.SelectedItem.ToString();
            double sum = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sum += Convert.ToDouble(dt.Rows[i][columnName]);
            }
            averageLabel.Content = sum / dt.Rows.Count;
            average = sum / dt.Rows.Count;
        }

        /// <summary>
        /// Установка медианы.
        /// </summary>
        private void SetMedian()
        {
            string columnName = boxData.SelectedItem.ToString();
            List<double> values = new List<double>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                values.Add(Convert.ToDouble(dt.Rows[i][columnName]));
            }
            values.Sort();
            int index = values.Count / 2;
            if (values.Count % 2 == 0)
            {
                medianLabel.Content = (values[index] + values[index + 1]) / 2;
            }
            else
            {
                medianLabel.Content = values[values.Count / 2];
            }
        }

        /// <summary>
        /// Установка среднеквадратичного отклонения.
        /// </summary>
        private void SetDeviation()
        {
            string columnName = boxData.SelectedItem.ToString();
            double sum = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                double numb = Convert.ToDouble(dt.Rows[i][columnName]);
                sum += Math.Pow((numb - average), 2);
            }
            deviationLabel.Content = Math.Sqrt(sum / dt.Rows.Count);
        }

        /// <summary>
        /// Установка дисперсии.
        /// </summary>
        private void SetDispersion()
        {
            string columnName = boxData.SelectedItem.ToString();
            Dictionary<double, double> dict = new Dictionary<double, double>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                double numb = Convert.ToDouble(dt.Rows[i][columnName]);
                if (dict.ContainsKey(numb))
                {
                    dict[numb] += 1;
                }
                else
                {
                    dict.Add(numb, 1);
                }
            }

            double mathExpectation = 0;
            foreach (var key in dict.Keys)
            {
                mathExpectation += key * dict[key] / dt.Rows.Count;
            }

            double dispersion = 0;
            foreach (var key in dict.Keys)
            {
                dispersion += dict[key] * Math.Pow((key - mathExpectation),2) / dt.Rows.Count;
            }

            dispesionLabel.Content = dispersion;
        }
    }
}
