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

        private void FillHistogram(string columnName)
        {
            BarCollection = GetSeriesCollection(columnName);

            BarLabels = GetBarLabels(columnName);

            Formatter = value => value.ToString("N");
            DataContext = this;
            chart.AxisX[0].Labels = BarLabels;
            chart.Series = BarCollection;
        }

        private SeriesCollection GetSeriesCollection(string columnName)
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

        private List<string> GetBarLabels(string columnName)
        {
            List<string> barLabels = new List<string>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string barLabel = dt.Rows[i][columnName].ToString();
                if (!barLabels.Contains(barLabel))
                {
                    barLabels.Add(barLabel);
                }
            }
            return barLabels;
        }

        public Func<double, string> Formatter { get; set; }
        public SeriesCollection BarCollection { get; set; }
        public List<string> BarLabels { get; set; }

        private void boxData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillHistogram(boxData.SelectedItem.ToString());
        }
    }

}
