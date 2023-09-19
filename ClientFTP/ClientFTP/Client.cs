using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;
using API;
using System.Net;

namespace ClientFTP
{
    internal class Client
    {
        UdpClient udpClient = null;

        public delegate void NewMessageHandler ( string message );
        public event NewMessageHandler NewMessage;
        APIElement lastMessage { get; set; }
        public Client()
        {
            udpClient = new UdpClient();
            udpClient.Connect( "127.0.0.1", 5000 );
            Console.WriteLine( "Подключение к 127.0.0.1:5000" );
            NewMessage += NewServerMessage;

            Task.Run( () =>
            {
                SocketAccepter();
            } );
        }
        private void SocketAccepter ()
        {
            while ( true )
            {
                IPEndPoint endPoint = new IPEndPoint( IPAddress.Any, 0 );
                var buffer = udpClient.Receive( ref endPoint );

                Task.Run( () =>
                {
                    NewMessage?.Invoke( Encoding.UTF8.GetString( buffer ) );
                } );
            }
        }

        private void NewServerMessage( string message )
        {
            APIElement element = JsonConvert.DeserializeObject<APIElement>( message );
            switch ( element.type_message )
            {
                case APIElement.TypeMessage.None:
                    MessageProcessing_None( ref element );
                    break;
                case APIElement.TypeMessage.Hello:
                    MessageProcessing_Hello( ref element );
                    break;
                case APIElement.TypeMessage.GetFiles:
                    MessageProcessing_GetFiles( ref element );
                    break;
            };
        }

        private void MessageProcessing_None( ref APIElement element)
        {

        }

        private void MessageProcessing_Hello( ref APIElement element )
        {
            MessageBox.Show( "Сервер ответил! Его сообщение: " + element.data );
        }

        private void MessageProcessing_GetFiles( ref APIElement element )
        {
            MessageBox.Show( "Сервер ответил! Его сообщение: " + element.data );
        }

        public async void SendMessage( string message )
        {
            APIElement elem = new APIElement()
            {
                type_message = APIElement.TypeMessage.GetFiles,
                data = message
            };
            lastMessage = elem;

            byte[] buffer = Encoding.UTF8.GetBytes( JsonConvert.SerializeObject( elem ) );

            int bytes = await udpClient.SendAsync( buffer, buffer.Length );
            Console.WriteLine( "Отправлены данные: " + bytes + " bytes." );
        }
    }
}
