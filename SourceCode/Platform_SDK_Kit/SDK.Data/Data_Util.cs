using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace iKCoder_Platform_SDK_Kit
{
    public class Data_Util
    {
        public static DbType ConventStrTOCommonDbtye(string DBType)
        {
            switch (DBType)
            {
                case "text":
                    return DbType.String;
                case "date":
                    return DbType.Date;
                case "time":
                    return DbType.Time;
                case "smallint":
                    return DbType.Int16;
                case "int":
                    return DbType.Int32;
                case "real":
                case "decimal":
                    return DbType.Decimal;
                case "datetime":
                    return DbType.DateTime;
                case "float":
                    return DbType.Single;
                case "varchar":
                case "nvarchar":
                    return DbType.String;
                case "bit":
                    return DbType.Byte;
                case "binary":
                case "image":
                    return DbType.Binary;
                default:
                    return DbType.String;
            }
        }

        public static SqlDbType ConventStrTODbtye(string DBType)
        {
            switch (DBType)
            {
                case "text":
                    return SqlDbType.Text;                    
                case "date":
                    return SqlDbType.Date;                    
                case "time":
                    return SqlDbType.Time;                    
                case "smallint":
                    return SqlDbType.Int;                    
                case "int":
                    return SqlDbType.BigInt;
                case "real":
                case "decimal":
                    return SqlDbType.Decimal;
                case "datetime":
                    return SqlDbType.DateTime;
                case "float":
                    return SqlDbType.Float;
                case "varchar":
                case "nvarchar":
                    return SqlDbType.NVarChar;
                case "bit":
                    return SqlDbType.Bit;
                case "binary":
                    return SqlDbType.Binary;
                case "image":
                    return SqlDbType.Image;
                default:
                    return SqlDbType.NVarChar;
            }
        }        

    }
}
