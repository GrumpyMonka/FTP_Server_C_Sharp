using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows;
using static System.Net.WebRequestMethods;

namespace ServerFTP
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow ()
        {
            InitializeComponent();
        }

        private void buttonStatusServer_Click ( object sender, RoutedEventArgs e )
        {
            FTPServer ftpServer = new FTPServer();
        }

        private void Window_Loaded ( object sender, RoutedEventArgs e )
        {
            Logger.loggerFunc = log;
            Logger.Log( "Программа запущена" );
        }

        private void log( string str )
        {
            listBoxLog.Dispatcher.Invoke( new Action( () => listBoxLog.Items.Add( str ) ) );
        }
    }
}
