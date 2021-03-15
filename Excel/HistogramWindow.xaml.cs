using LiveCharts;
using LiveCharts.Wpf;
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
    /// Interaction logic for HistogramWindow.xaml
    /// </summary>
    public partial class HistogramWindow : Window
    {
        DataTable dt;
        List<string> numericColumnsNames;

        public HistogramWindow(DataTable dt)
        {
            InitializeComponent();
            this.dt = dt;
            FillBoxData();
            numericColumnsNames = StatsWindow.GetNumericColumnsNames(dt);
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

        private void FillHistogram(string columnName)
        {
            BarCollection = GetSeriesCollection(columnName);

            BarLabels = GetBarLabels(columnName);

            Formatter = value => value.ToString("N");
            DataContext = this;
            chart.AxisX[0].Labels = BarLabels;
            chart.Series = BarCollection;
        }

        private SeriesCollection GetSeriesCollection(string columnName, int columnWidth=1)
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
            ChartValues<double> values = new ChartValues<double>();

            foreach (var key in dict.Keys)
            {
                values.Add(dict[key]);
            }

            series.Add(new ColumnSeries
            {
                Title = columnName,
                Values = values
            });

            return series;
        }

        private List<string> GetBarLabels(string columnName, int columnWidth=1)
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
            List<string> barLabels = new List<string>(allBarLabels.Count / columnWidth + allBarLabels.Count % columnWidth);
            return allBarLabels;
        }

        public Func<double, string> Formatter { get; set; }
        public SeriesCollection BarCollection { get; set; }
        public List<string> BarLabels { get; set; }

        private void boxData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string columnName = boxData.SelectedItem.ToString();
            if (numericColumnsNames.Contains(columnName))
            {
                labelBarWidth.Visibility = Visibility.Visible;
                integerUpDown.Visibility = Visibility.Visible;
            }
            else
            {
                labelBarWidth.Visibility = Visibility.Hidden;
                integerUpDown.Visibility = Visibility.Hidden;
                integerUpDown.Value = 1;
            }
            FillHistogram(columnName);
        }

        private void integerUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (integerUpDown.Value <= 0)
            {
                MessageBox.Show("Нельзя установить ширину меньше 1","Ошибка");
                integerUpDown.Value = 1;
            }
            else
            {

            }
        }
    }

}
