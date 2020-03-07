using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;

namespace Tubes_Stima_2
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

        private void Browse1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog FDialog = new OpenFileDialog
            {
                Title = "Browse File 1", // Nanti ganti namanya
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,
                ReadOnlyChecked = true,
                ShowReadOnly = true
            };
            if (FDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Directory1.Text = FDialog.FileName;
            }
        }

        private void Browse2_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog FDialog = new OpenFileDialog
            {
                Title = "Browse File 2",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,
                ReadOnlyChecked = true,
                ShowReadOnly = true
            };
            if (FDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Directory2.Text = FDialog.FileName;
            }
        }

        private void Credits_Click(object sender, RoutedEventArgs e)
        {
            var CreditWindow = new Window1();
            CreditWindow.Show();
        }

        private void ShowGraph_Click(object sender, RoutedEventArgs e)
        {
            string file1 = Directory1.Text;
            string file2 = Directory2.Text;
            string query = Query.Text;
        }

        private 
    }
}
