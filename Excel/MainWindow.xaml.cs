using System;
using System.Windows;
using System.Data.OleDb;
using Microsoft.Win32;
using System.Data;

namespace Excel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Таблица с данными.
        /// </summary>
        DataTable dt;

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Открытие файла и чтение.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы событий. </param>
        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "csv файл (*.csv)|*.csv";
            openFileDialog.Title = "Открыть CSV файл";
            string path = string.Empty;

            try
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    path = openFileDialog.FileName;
                }

                dt = GetDataTableFromCSV(path);
                if (dt != null)
                {
                    // Костыль, в описании метода объяснил.
                    FixColumnNames(path);
                    SetDefaultValues();
                    dataGrid.ItemsSource = dt.DefaultView;
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not load.{Environment.NewLine}{ex.Message}", "Ошибка");
                return;
            }
        }

        /// <summary>
        /// Получение данных из CSV файла.
        /// </summary>
        /// <param name="path"> Путь к файлу. </param>
        /// <returns> Таблица с данными. </returns>
        private DataTable GetDataTableFromCSV(string path)
        {
            try
            {
                string foo = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + System.IO.Path.GetDirectoryName(path) +
                     ";Extended Properties=\"Text;HDR=NO;IMEX=1\"";
                using (OleDbConnection conn = new OleDbConnection(foo))
                {
                    conn.Open();
                    string strQuery = "Select * from [" + System.IO.Path.GetFileName(path) + "]";
                    OleDbDataAdapter da = new OleDbDataAdapter(strQuery, conn);
                    DataSet ds = new System.Data.DataSet();
                    da.Fill(ds);
                    DataTable data = ds.Tables[0];
                    ds.Tables.RemoveAt(0);
                    return data;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
                return null;
            }
        }

        /// <summary>
        /// Метод чинит названия столбцов.
        /// (Возникала проблема с исчезновением столбцов
        /// в некоторых файлах)
        /// </summary>
        /// <param name="path"> Путь к файлу. </param>
        private void FixColumnNames(string path)
        {
            try
            {
                dt.Rows.RemoveAt(0);
                string foo = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + System.IO.Path.GetDirectoryName(path) +
                     ";Extended Properties=\"Text;HDR=YES;IMEX=0\"";
                using (OleDbConnection conn = new OleDbConnection(foo))
                {
                    conn.Open();
                    string strQuery = "Select * from [" + System.IO.Path.GetFileName(path) + "]";
                    OleDbDataAdapter da = new OleDbDataAdapter(strQuery, conn);
                    DataSet ds = new System.Data.DataSet();
                    da.Fill(ds);
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        string correctName = ds.Tables[0].Columns[i].ColumnName;
                        dt.Columns[i].ColumnName = correctName.Replace('/', '|');
                    }
                    ds.Tables.RemoveAt(0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        /// <summary>
        /// Метод ставит значения по умолчанию.
        /// </summary>
        private void SetDefaultValues()
        {
            DataTable dtWithDefault = new DataTable();
            // Разделение столбцов на типы.
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                bool isNumberColumn = true;
                double numb = 0;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    if (dt.Rows[j][i].GetType() == typeof(DBNull))
                        continue;
                    if (!double.TryParse(dt.Rows[j][i].ToString(), out numb))
                    {
                        object ob = dt.Rows[j][i];
                        isNumberColumn = false;
                        break;
                    }
                    dt.Rows[j][i] = numb;
                }
                if (isNumberColumn)
                {
                    dtWithDefault.Columns.Add(dt.Columns[i].ColumnName, typeof(double));
                }
                else
                {
                    dtWithDefault.Columns.Add(dt.Columns[i].ColumnName, typeof(string));
                }
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dtWithDefault.Rows.Add(dtWithDefault.NewRow());
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dtWithDefault.Columns[j].DataType == typeof(double))
                    {
                        if (dt.Rows[i][j].GetType() == typeof(DBNull))
                        {
                            dtWithDefault.Rows[i][j] = 0;
                            continue;
                        }
                    }
                    dtWithDefault.Rows[i][j] = dt.Rows[i][j];
                }
            }
            dt = dtWithDefault;
        }

        /// <summary>
        /// Открытия окна с статистикой.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы событий. </param>
        private void statisticButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dt != null)
                {
                    StatsWindow sw = new StatsWindow(dt);
                    sw.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        /// <summary>
        /// Открытия окна с гистограммами.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы событий. </param>
        private void histogramButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dt != null)
                {
                    HistogramWindow hw = new HistogramWindow(dt);
                    hw.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        /// <summary>
        /// Открытия окна c графиком.
        /// </summary>
        /// <param name="sender"> Отправитель.</param>
        /// <param name="e"> Аргументы событий. </param>
        private void chartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dt != null)
                {
                    ChartWindow cw = new ChartWindow(dt);
                    cw.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }
    }
}
