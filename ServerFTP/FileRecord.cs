using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using ColumnAttribute = SQLite.ColumnAttribute;
using TableAttribute = SQLite.TableAttribute;

namespace ServerFTP
{
    [Serializable]
    [Table("Files")]
    internal class FileRecord
    {
        [PrimaryKey, AutoIncrement]
        [Column( "id" )]
        public int ID { get; set; }

        [Column( "fileName" )]
        public string FileName { get; set; }

        [Column( "fileSize" )]
        public int FileSize { get; set; }
    }
}
