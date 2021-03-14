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

            dt = GetDataTableFromCSV(openFileDialog.FileName);
            if (dt != null)
            {
                dataGrid.ItemsSource = dt.DefaultView;
            }
            else
            {
                return;
            }

            string mess = string.Empty;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                mess += dt.Rows[0][i];
                double sum = 0;
                for (int j = 1; j < dt.Rows.Count; j++)
                {
                    try
                    {
                        object ob = dt.Rows[j][i];
                        sum += Convert.ToDouble(dt.Rows[j][i]);
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
                mess += sum + " " + Environment.NewLine;
            }

            MessageBox.Show(mess);
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
                    return ds.Tables[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
                return null;
            }
        }

        private void statisticButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
