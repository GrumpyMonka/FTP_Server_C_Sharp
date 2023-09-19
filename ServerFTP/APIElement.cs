using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    internal class APIElement
    {
        public enum TypeMessage
        {
            None,
            Hello,
            GetFiles
        }

        public Guid guid { get; set; } = Guid.NewGuid();
        public TypeMessage type_message { get; set; } = TypeMessage.None;
        public string data { get; set; } = string.Empty;
        public long data_size { get; set; } = 0;

        public APIElement()
        {

        }

        public void CloneMessage( APIElement element )
        {
            guid = element.guid;
            type_message = element.type_message;
            data_size = element.data_size;
        }
    }
}
