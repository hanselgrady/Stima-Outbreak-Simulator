using System;
using System.Collections.Generic;
using System.Collections;
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
using System.IO;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.Msagl.Drawing;

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
            Window1 CreditWindow = new Window1();
            CreditWindow.Show();
        }

        private void ShowGraph_Click(object sender, RoutedEventArgs e)
        {
            // BUAT GRAF
            GViewer viewer = new GViewer();
            Graph graf = new Graph("Graf Persebaran");
            // PELUANG TERSEBARNYA DARI 1 KOTA KE KOTA LAIN
            Dictionary<string, float> peluang = new Dictionary<string, float>();
            // JUMLAH PENDUDUK SUATU KOTA
            Dictionary<string, int> jumlah = new Dictionary<string, int>();
            // WAKTU AGAR SUATU KOTA TERINFEKSI
            Dictionary<string, int> waktu = new Dictionary<string, int>();
            // BUAT ANTRIAN SIMPUL HIDUP
            ArrayList queue = new ArrayList();
            // BUAT ANTRIAN HUBUNGAN KOTA YANG BERHASIL DISEBAR
            Dictionary<string, int> check = new Dictionary<string, int>();

            // FILE 1 : PETA SELURUH DUNIA
            string line;
            string fileloc1 = Directory1.Text;
            StreamReader file1 = new StreamReader(@fileloc1);
            bool first = true;
            
            while ((line = file1.ReadLine()) != null)
            {
                if (first)
                {
                    first = false;
                    // BUAT PATOKAN JUMLAH 
                    int numRel = int.Parse(line);
                }
                else
                {
                    string[] temp = line.Split(' ');
                    peluang.Add(temp[0] + ">" + temp[1], float.Parse(temp[2]));
                }
            }
            file1.Close();

            // FILE 2 : POPULASI DI SUATU DAERAH
            string fileloc2 = Directory2.Text;
            StreamReader file2 = new StreamReader(@fileloc2);
            first = true;
            while((line = file2.ReadLine()) != null)
            {
                if (first)
                {
                    first = false;
                    string firstCity = line.Split()[1];
                    waktu.Add(firstCity, 0);
                    foreach(KeyValuePair<string, float> entry in peluang)
                    {
                        // CEK KALO ADA YG SESUAI
                        if (entry.Key.Split('>')[0] == firstCity)
                        {
                            // MASUKKIN KE QUEUE KALO SESUAI
                            queue.Add(entry.Key);
                        }
                    }
                }
                else
                {
                    string[] temp = line.Split();
                    jumlah.Add(temp[0], int.Parse(temp[1]));
                    if (!waktu.ContainsKey(temp[0]))
                    {
                        // -999 KALAU BELUM TERINFEKSI
                        waktu.Add(temp[0], -999);
                    }
                }
            }
            string query = Query.Text;
            int days = int.Parse(query);
            string source, target, temp1;
            // SELAMA QUEUE MASIH ISI
            while (queue.Count > 0)
            {
                // CEK ELEMEN TERDEPAN
                temp1 = (string)queue[0];
                source = temp1.Split('>')[0];
                target = temp1.Split('>')[1];
                queue.RemoveAt(0);
                // CARI APAKAH AKAN TERSEBAR
                int populasi = jumlah[source];
                int totalhari = days - waktu[source];
                float prob = peluang[temp1];
                // KALAU TERSEBAR
                if ((float)populasi * prob / (1 + (populasi - 1) * Math.Pow(Math.E, -0.25 * totalhari)) > 1)
                {
                    // KALAU TARGET BELUM TERINFEKSI
                    // CARI KAPAN PENYEBARANNYA
                    int waktusampai = 1;
                    while ((float)populasi * prob / (1 + (populasi - 1) * Math.Pow(Math.E, -0.25 * waktusampai)) <= 1)
                    {
                        waktusampai++;
                    }
                    check.Add(temp1, waktusampai);
                    // TAMBAHKAN ELEMEN
                    if (waktu[target] == -999) {
                        waktu[target] = waktu[source] + waktusampai;
                        foreach (KeyValuePair<string, float> entry in peluang)
                        {
                            // CEK KALO ADA YG SESUAI
                            if (entry.Key.Split('>')[0] == target)
                            {
                                // MASUKKIN KE QUEUE KALO SESUAI
                                queue.Add(entry.Key);
                            }
                        }
                    }
                }
            }
            // BUAT GRAF SESUAI DATA YANG TERSEDIA
            // BUAT NODE SESUAI DENGAN KOTA YANG ADA
            foreach (KeyValuePair<string, int> entry in waktu)
            {
                if (entry.Value == -999)
                {
                    // JIKA KOTA BELUM TERINFEKSI, BERI WARNA PUTIH
                    graf.AddNode(entry.Key).Attr.FillColor = Microsoft.Msagl.Drawing.Color.White;
                }
                else
                {
                    // JIKA KOTA SUDAH TERINFEKSI, BERI WARNA MERAH
                    graf.AddNode(entry.Key).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
                }
                graf.FindNode(entry.Key).Attr.Shape = Microsoft.Msagl.Drawing.Shape.Circle;
            }
            // BERI PANAH YANG MENUNJUKKAN KETERHUBUNGAN ANTAR KOTA
            foreach (KeyValuePair<string, float> entry in peluang)
            {
                if (check.ContainsKey(entry.Key))
                {
                    // KALAU HUBUNGAN KOTA 1 DENGAN TETANGGANYA SUDAH TERINFEKSI, BERI GARIS MERAH
                    graf.AddEdge(entry.Key.Split('>')[0], entry.Key.Split('>')[1]).Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
                }
                else
                {
                    // KALAU HUBUNGAN KOTA 1 DENGAN TETANGGANYA BELUM TERINFEKSI, BERI GARIS MERAH
                    graf.AddEdge(entry.Key.Split('>')[0], entry.Key.Split('>')[1]).Attr.Color = Microsoft.Msagl.Drawing.Color.Black;
                }
            }
            // TAMPILKAN GRAF
            viewer.Graph = graf;
            Form showgraph = new Form();
            showgraph.Size = new System.Drawing.Size(1000, 1000);
            showgraph.SuspendLayout();
            viewer.Dock = DockStyle.Fill;
            showgraph.Controls.Add(viewer);
            showgraph.ResumeLayout();
            showgraph.ShowDialog();
            Window2 ShowGraph = new Window2();
            ShowGraph.ShowDialog();
        }
        // FUNGSI LOGISTIK : (float)populasi * prob / (1 + (populasi - 1) * Math.Pow(Math.E, -0.25 * totalhari))
        // FUNGSI LOGISTIK : (float)populasi / (1 + (populasi - 1) * Math.Pow(Math.E, -0.25 * waktusampai)) * prob
        // FUNGSI LINEAR : ((float)(totalhari * populasi) / 20) * prob
        // FUNGSI LINEAR : ((float)(waktusampai * populasi) / 20) * prob

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Directory1.Text = "";
            Directory2.Text = "";
            Query.Text = "";
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }
    }
}
