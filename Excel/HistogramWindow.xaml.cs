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
        }

        public Func<double,string> Formatter { get; set; }
    }
}
