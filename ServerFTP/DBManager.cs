using SQLite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ServerFTP
{
    internal class DBManager
    {
        private string pathToDB;
        private SQLiteConnection dbConnection;

        public List<FileRecord> fileRecordList { get; set; }
        public DBManager ( in string pathToDB )
        {
            this.pathToDB = pathToDB;
            if ( !File.Exists( pathToDB ) )
            {
                Logger.Log( "БД не найдена, попытка создать!" );
                dbConnection = new SQLiteConnection( pathToDB );
                dbConnection.CreateTable<FileRecord>();
                Logger.Log( "БД создана: " + Path.GetFullPath( pathToDB ) );
            }
            else
            {
                dbConnection = new SQLiteConnection( pathToDB );
            }

            Logger.Log( "ДБ загружена! Идёт разбор данных..." );
            LoadDataFromDB();
            Logger.Log( "Данные загружены, кол-во записей: " + fileRecordList.Count + ". Размер данных в ОЗУ: " + GetSizeObject( fileRecordList ) + " Byte." );
        }
        
        private long GetSizeObject ( object obj )
        {
            using ( Stream stream = new MemoryStream() )
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize( stream, obj );
                return stream.Length;
            }
        }

        private void LoadDataFromDB()
        {
            fileRecordList = dbConnection.Table<FileRecord>().ToList();
        }
    }
}
