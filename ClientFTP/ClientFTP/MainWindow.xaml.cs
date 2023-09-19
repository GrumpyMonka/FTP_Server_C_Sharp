using ClientFTP.TabelModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace ClientFTP
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BindingList<FilesTabelModel> FilesList;
        public MainWindow ()
        {
            InitializeComponent();
        }

        private void Button_LoadFile_Click ( object sender, RoutedEventArgs e )
        {   
            Client client = new Client();
            client.SendMessage( "Привет" );
            /*
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "*.*|Select Folder";
            saveFileDialog.FileName = "Save Files Here";

            if ( saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK )
            {
                string savePath = Path.GetDirectoryName( saveFileDialog.FileName );
                System.Windows.MessageBox.Show( savePath );
            }
            */
        }

        private void Window_Loaded ( object sender, RoutedEventArgs e )
        {
            FilesList = new BindingList<FilesTabelModel>()
            { 
                new FilesTabelModel
                {
                    FileName = "test1",
                    FileSize = 100
                },
                new FilesTabelModel
                {
                    FileName = "test2",
                    FileSize = 200
                },
                new FilesTabelModel
                {
                    FileName = "test3",
                    FileSize = 300
                }
            };

            dgFilesList.ItemsSource = FilesList;
        }
    }
}
