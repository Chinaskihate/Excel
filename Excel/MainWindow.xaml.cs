using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Windows;
using System.Data.OleDb;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Data;

namespace Excel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataTable dt;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "csv файл (*.csv)|*.csv";
            openFileDialog.Title = "Открыть CSV файл";
            string path = string.Empty;

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    path = openFileDialog.FileName;
                }

                catch (Exception)
                {
                    MessageBox.Show("Could not load.");
                    return;
                }
            }

            dt = GetDataTableFromCSV(path);
            if (dt != null)
            {
                FixColumnNames(path);
                SetDefaultValues();
                dataGrid.ItemsSource = dt.DefaultView;
            }
            else
            {
                return;
            }

            //string mess = string.Empty;
            //for (int i = 0; i < dt.Columns.Count; i++)
            //{
            //    mess += dt.Rows[0][i];
            //    double sum = 0;
            //    for (int j = 1; j < dt.Rows.Count; j++)
            //    {
            //        try
            //        {
            //            object ob = dt.Rows[j][i];
            //            sum += Convert.ToDouble(dt.Rows[j][i]);
            //        }
            //        catch (Exception)
            //        {
            //            break;
            //        }
            //    }
            //    mess += sum + " " + Environment.NewLine;
            //}

            //MessageBox.Show(mess);
        }

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

        private void SetDefaultValues()
        {
            DataTable dtWithDefault = new DataTable();
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

        private void statisticButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
