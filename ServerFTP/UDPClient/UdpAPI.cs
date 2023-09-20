using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrumpyMonk
{
    internal class UdpAPI
    {
        public enum TypePacket
        { 
            Unknown = 0,
            Ping,
            Message
        }

        public Guid PacketGuid { get; set; } = Guid.NewGuid();
        public TypePacket PacketType { get; set; } = TypePacket.Unknown;
        public string PacketData { get; set; } = string.Empty;
        public int PacketTimestamp { get; set; } = DateTime.Now.Millisecond;
    }
}
