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
        private Server server = null;
        
        public MainWindow ()
        {
            InitializeComponent();
        }

        private void buttonStatusServer_Click ( object sender, RoutedEventArgs e )
        {
            if ( server == null )
            {
                server = new Server();
                server.NewMessage += NewClientMessage;
                server.ServerStart();
                buttonStatusServer.Content = "Остановить сервер";
            }
            else
            {
                server.Close();
                server = null;
                buttonStatusServer.Content = "Запустить сервер";
            }
        }

        private void NewClientMessage( Server.ClientMessage clientMessage )
        {
            Logger.Log( "Новое сообщение от пользователя: " + clientMessage.Message );
        }

        private void Window_Loaded ( object sender, RoutedEventArgs e )
        {

            Logger.loggerFunc = log;
            Logger.Log( "Программа запущена" );

            dbManager = new DBManager( dbFilePath );
        }

        private void log( in string str )
        {
            listBoxLog.Invoke( new Action() { } );
        }
    }
}
