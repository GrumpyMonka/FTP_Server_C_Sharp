using System;
using System.Data;
using System.IO;
using System.Windows;

namespace ServerFTP
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string dbFilePath = "DB_Files.sqlite";
        private DBManager dbManager = null;
        
        public MainWindow ()
        {
            InitializeComponent();

        }

        private void buttonStatusServer_Click ( object sender, RoutedEventArgs e )
        {

        }
        private void Window_Loaded ( object sender, RoutedEventArgs e )
        {

            Logger.loggerFunc = log;
            Logger.Log( "Программа запущена" );

            dbManager = new DBManager( dbFilePath );
        }

        private void log( in string str )
        {
            listBoxLog.Items.Add( str );
        }
    }
}
