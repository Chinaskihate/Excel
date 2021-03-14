using System;
using System.Collections.Generic;
using System.Data;
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

namespace Excel
{
    /// <summary>
    /// Interaction logic for StatsWindow.xaml
    /// </summary>
    public partial class StatsWindow : Window
    {
        DataTable dt;
        double average = 1;

        public StatsWindow(DataTable dt)
        {
            InitializeComponent();
            this.dt = dt;
            boxData.ItemsSource = GetNumericColumnsNames();
        }

        private List<string> GetNumericColumnsNames()
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

        private void boxData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetLabels();
        }

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
