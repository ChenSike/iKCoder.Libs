using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using MySql.Data;


namespace iKCoder_Platform_SDK_Kit
{

    public enum enum_DatabaseType
    {
        SqlServer = 1,
        MySql = 2
    }

    public class class_data_PlatformDBConnection
    {
        public enum_DatabaseType activeDatabaseType
        {
            set;
            get;
        }        
    }

    public class class_data_PlatformDBDataReader
    {
        public enum_DatabaseType activeDatabaseType
        {
            set;
            get;
        }
    }

    public class class_data_PlatformDBCommand
    {
        public enum_DatabaseType activeDatabaseType
        {
            set;
            get;
        }
    }

    public class class_data_MySqlCommand:class_data_PlatformDBCommand
    {
        public MySqlCommand ActiveCommand
        {
            set;
            get;
        }
    }

    public class class_data_SqlServerCommand : class_data_PlatformDBCommand
    {
        public SqlCommand ActiveCommand
        {
            set;
            get;
        }
    }

    public class class_data_SqlServerDataReader:class_data_PlatformDBDataReader
    {
        public SqlDataReader ActiveDataReader
        {
            set;
            get;
        }
    }

    public class class_data_MySqlDataReader:class_data_PlatformDBDataReader
    {
        public MySqlDataReader ActiveDataReader
        {
            set;
            get;
        }
    }

    public class class_data_SqlServerConnectionItem : class_data_PlatformDBConnection
    {
        public SqlConnection ActiveConnection
        {
            set;
            get;
        }
    }

    public class class_data_MySqlConnectionItem : class_data_PlatformDBConnection
    {
        public MySqlConnection ActiveConnection
        {
            set;
            get;
        }
    }

}
