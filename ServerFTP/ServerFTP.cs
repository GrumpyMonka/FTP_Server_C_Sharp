using API;
using Newtonsoft.Json;
using ServerFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ServerFTP
{
    internal class FTPServer
    {
        private const int MY_MTU = 1024; // Kb
        private const string dbFilePath = "DB_Files.sqlite";
        private DBManager dbManager = null;
        private Server udpServer = null;

        public bool isSeverRunning { get; private set; }
        public FTPServer ()
        {
            isSeverRunning = false;
            Task.Run( () =>
            {
                dbManager = new DBManager( dbFilePath );

                udpServer = new Server();
                udpServer.NewMessage += NewClientMessage;
                udpServer.ServerStart();

                isSeverRunning = true;
                Logger.Log( "FTP-Сервер готов к использованию!" );
            } );
        }

        private void NewClientMessage ( Server.ClientMessage clientMessage )
        {
            APIElement element = JsonConvert.DeserializeObject<APIElement>( clientMessage.Message );
            switch ( element.type_message )
            {
            case APIElement.TypeMessage.None:
                    MessageProcessing_None( clientMessage, element );
                    break;
            case APIElement.TypeMessage.Hello:
                    MessageProcessing_Hello( clientMessage, element );
                    break;
            case APIElement.TypeMessage.GetFiles:
                    MessageProcessing_GetFiles( clientMessage, element );
                    break;
            };
            Logger.Log( $"Новое сообщение от пользователя: ({clientMessage.Message.Length})" + clientMessage.Message );
        }

        private void SendMessage ( Server.ClientMessage clientMessage, APIElement element )
        {
            element.data_size = sizeof( char ) * element.data.Length;
            int countMTU = (int)( element.data_size + MY_MTU - 1 ) / MY_MTU;
            int lengthMTU = MY_MTU / sizeof( char );
            //List<APIElement> list_messages = new List<APIElement>( countMTU );
            for ( int i = 0; i < countMTU; ++i )
            {
                APIElement temp_element = new APIElement();
                temp_element.CloneMessage( element );
                temp_element.data = element.data.Substring( lengthMTU * i, Math.Min( lengthMTU, element.data.Length ) );
                Server.ClientMessage temp_client_message = new Server.ClientMessage()
                {
                    EndPointClient = clientMessage.EndPointClient,
                    Message = JsonConvert.SerializeObject( temp_element )
                };
                udpServer.SendMessage( temp_client_message );
                //list_messages.Add( temp_element );  
            }
        }

        private void MessageProcessing_None ( Server.ClientMessage clientMessage, APIElement element )
        {
            Logger.Log( "Пустое сообщение!" );
        }

        private void MessageProcessing_Hello ( Server.ClientMessage clientMessage, APIElement element )
        {
            Logger.Log( "Пользователь нас поприветсвовал с сообщением: " + element.data );
            element.data = "Привет, я Сервер!";
            SendMessage( clientMessage, element );
        }

        private void MessageProcessing_GetFiles ( Server.ClientMessage clientMessage, APIElement element )
        {
            element.data = JsonConvert.SerializeObject( dbManager.fileRecordList );
            SendMessage( clientMessage, element );
        }
    }
}
