using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

            DataTable dt = GetDataTableFromCSV(openFileDialog.FileName);
            if (dt != null)
            {
                dataGrid.ItemsSource = dt.DefaultView;
            }
        }

        private DataTable GetDataTableFromCSV(string path)
        {
            try
            {
                OleDbConnection conn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + System.IO.Path.GetDirectoryName(path) + ";Extended Properties=\"Text;HDR == YES;FMT = Delimited\"");
                conn.Open();
                string strQuery = "Select * from [" + System.IO.Path.GetFileName(path) + "]";
                OleDbDataAdapter da = new OleDbDataAdapter(strQuery, conn);
                DataSet ds = new System.Data.DataSet();
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
