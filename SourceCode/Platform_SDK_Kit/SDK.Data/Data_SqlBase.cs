using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;


namespace iKCoder_Platform_SDK_Kit
{

    public enum enum_SqlConnectionType
    {
        SqlServer = "sqlserver",
        MySql = "mysql"
    }

    public class Data_PlatformDBConnection
    {
        public enum_SqlConnectionType activeConnectionType
        {
            set;
            get;
        }
    }

    public class class_data_SqlServerConnectionItem : Data_PlatformDBConnection
    {
        public SqlConnection ActiveConnection
        {
            set;
            get;
        }
    }

    public class class_data_MySqlConnectionItem : Data_PlatformDBConnection
    {
        public MySqlConnection ActiveConnection
        {
            set;
            get;
        }
    }

}
