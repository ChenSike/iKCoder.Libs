using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using iKCoder_Platform_SDK_Kit;

namespace iKCoder_Platform_SDK_Kit
{

    public class class_Data_SqlStringHelper
    {             
        public static string SQL_GETALLTABLES_FOR_SQL2008 = "select * from sys.tables where (type = 'U')";
        public static string SQL_GETALLTABLES_FOR_SQL2005 = "select * from sys.all_objects where (type = 'U')";
        private static string SQL_GETALLTABLES_FOR_MYSQL = "select table_name from information_schema.tables where table_schema = {schemaname}";

        public static string Get_SQL_GETALLTABLES_FOR_MYSQL(string schemaname)
        {
            return SQL_GETALLTABLES_FOR_MYSQL.Replace("{schemaname}", schemaname);
        }
    }

    public class class_Data_SqlHelper
    {

        public bool ActionAutoCreateSPS(class_data_PlatformDBConnection ActiveConnection)
        {
            try
            {
                if (ActiveConnection != null && ActiveConnection.activeDatabaseType == enum_DatabaseType.SqlServer)
                {
                    string sql_getALLTables = class_Data_SqlStringHelper.SQL_GETALLTABLES_FOR_SQL2008;                                      
                    DataTable TablesInfo = new DataTable();
                    DataTable ColumnInfo = new DataTable();
                    DataTable TypesInfo = new DataTable();
                    if (class_Data_SqlDataHelper.ActionExecuteSQLForDT(ActiveConnection, sql_getALLTables, out TablesInfo))
                    {
                        if (TablesInfo.Rows.Count > 0)
                        {
                            foreach (DataRow activeDR_1 in TablesInfo.Rows)
                            {
                                string sql_getALLColumns = "select * from sys.syscolumns";
                                string tableName = "";
                                class_Data_SqlDataHelper.GetColumnData(activeDR_1, "name", out tableName);
                                string objectID = "";
                                class_Data_SqlDataHelper.GetColumnData(activeDR_1, "object_id", out objectID);
                                if (tableName != "")
                                {
                                    if (objectID != "")
                                    {
                                        sql_getALLColumns = sql_getALLColumns + " where id='" + objectID + "'";
                                        List<string> activeColumn = new List<string>();
                                        List<string> activeKeyColumn = new List<string>();
                                        List<string> filterColumn = new List<string>();
                                        List<string> filterTypeList = new List<string>();
                                        if (class_Data_SqlDataHelper.ActionExecuteSQLForDT(ActiveConnection, sql_getALLColumns, out ColumnInfo))
                                        {
                                            StringBuilder sql_CreateNewSp = new StringBuilder("IF OBJECTPROPERTY(OBJECT_ID(N'spa_peration_" + tableName + "'), N'IsProcedure') = 1");
                                            sql_CreateNewSp.AppendLine();
                                            sql_CreateNewSp.AppendLine("DROP PROCEDURE spa_operation_" + tableName);
                                            class_Data_SqlDataHelper.ActionExecuteForNonQuery(ActiveConnection, sql_CreateNewSp.ToString());
                                            sql_CreateNewSp.Clear();
                                            sql_CreateNewSp.AppendLine("CREATE PROCEDURE {SPNAME}");
                                            StringBuilder sql_insertSourceColumns = new StringBuilder();
                                            StringBuilder sql_insertValueColumns = new StringBuilder();
                                            sql_CreateNewSp.Replace("{SPNAME}", "spa_operation_" + tableName);
                                            sql_CreateNewSp.AppendLine("(");
                                            sql_CreateNewSp.AppendLine("@operation nvarchar(20) = '',");
                                            foreach (DataRow activeDR_2 in ColumnInfo.Rows)
                                            {
                                                string sql_getALLTypes = "select * from sys.types";
                                                string columnname = "";
                                                class_Data_SqlDataHelper.GetColumnData(activeDR_2, "name", out columnname);
                                                string typeid = "";
                                                string length = "";
                                                string status = "";
                                                class_Data_SqlDataHelper.GetColumnData(activeDR_2, "xtype", out typeid);
                                                class_Data_SqlDataHelper.GetColumnData(activeDR_2, "status", out status);
                                                class_Data_SqlDataHelper.GetColumnData(activeDR_2, "prec", out length);
                                                sql_getALLTypes = sql_getALLTypes + " where system_type_id=" + typeid;
                                                class_Data_SqlDataHelper.ActionExecuteSQLForDT(ActiveConnection, sql_getALLTypes, out TypesInfo);
                                                string typename = "";
                                                class_Data_SqlDataHelper.GetColumnData(TypesInfo.Rows[0], "name", out typename);
                                                if (typename.Contains("nvarchar"))
                                                {
                                                    if (Int32.Parse(length) < 0)
                                                        sql_CreateNewSp.AppendLine("@" + columnname + " nvarchar(max) = null ,");
                                                    else
                                                        sql_CreateNewSp.AppendLine("@" + columnname + " nvarchar(" + length + ") = null ,");
                                                }
                                                else if (typename.Contains("char"))
                                                    sql_CreateNewSp.AppendLine("@" + columnname + " char(" + length + ") = null ,");
                                                else if (typename.Contains("varcahr"))
                                                    sql_CreateNewSp.AppendLine("@" + columnname + " varchar(" + length + ") = null ,");
                                                else if (typename.Contains("binary"))
                                                {
                                                    sql_CreateNewSp.AppendLine("@" + columnname + " binary(" + length + ") = null ,");
                                                    filterColumn.Add(columnname);
                                                }
                                                else if (typename.Contains("varbinary"))
                                                {
                                                    sql_CreateNewSp.AppendLine("@" + columnname + " varbinary(" + length + ") = null ,");
                                                    filterColumn.Add(columnname);
                                                }
                                                else if (typename.Contains("nchar"))
                                                    sql_CreateNewSp.AppendLine("@" + columnname + " nchar(" + length + ") = null ,");
                                                else if (typename.Contains("decimal"))
                                                    sql_CreateNewSp.AppendLine("@" + columnname + " decimal" + " = null ,");
                                                else
                                                {
                                                    sql_CreateNewSp.AppendLine("@" + columnname + " " + typename + " = null ,");
                                                    filterColumn.Add(columnname);
                                                }
                                                if (typename.Contains("ntext"))
                                                    filterTypeList.Add(columnname);
                                                activeColumn.Add(columnname);
                                                if (status == "128")
                                                    activeKeyColumn.Add(columnname);
                                                if (status != "128")
                                                {
                                                    sql_insertSourceColumns.Append("[" + columnname + "],");
                                                    sql_insertValueColumns.Append("@" + columnname + ",");
                                                }
                                            }
                                            sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 3, 3);
                                            if (sql_insertSourceColumns.Length > 0)
                                                sql_insertSourceColumns = sql_insertSourceColumns.Remove(sql_insertSourceColumns.Length - 1, 1);
                                            sql_insertValueColumns = sql_insertValueColumns.Remove(sql_insertValueColumns.Length - 1, 1);
                                            sql_CreateNewSp.AppendLine(")");
                                            sql_CreateNewSp.AppendLine("AS");
                                            sql_CreateNewSp.AppendLine("if @operation='select'");
                                            sql_CreateNewSp.AppendLine("begin");
                                            sql_CreateNewSp.AppendLine("select * from [" + tableName + "]");
                                            sql_CreateNewSp.AppendLine("end");
                                            sql_CreateNewSp.AppendLine("else if @operation='insert'");
                                            sql_CreateNewSp.AppendLine("begin");
                                            sql_CreateNewSp.AppendLine("insert into " + tableName + "(" + sql_insertSourceColumns + ") values(" + sql_insertValueColumns + ")");
                                            sql_CreateNewSp.AppendLine("end");
                                            foreach (string activeCommonColumn in activeColumn)
                                            {
                                                if (!activeKeyColumn.Contains(activeCommonColumn))
                                                {
                                                    if (!filterColumn.Contains(activeCommonColumn))
                                                        sql_CreateNewSp.AppendLine("if @operation='update' and @" + activeCommonColumn + " is not null");
                                                    else
                                                        sql_CreateNewSp.AppendLine("if @operation='update'");
                                                    sql_CreateNewSp.AppendLine("begin");
                                                    sql_CreateNewSp.AppendLine("update " + tableName);
                                                    sql_CreateNewSp.AppendLine("set " + activeCommonColumn + "=@" + activeCommonColumn);
                                                    if (activeKeyColumn.Count > 0)
                                                    {
                                                        sql_CreateNewSp.AppendLine("where ");
                                                        foreach (string keyColumn in activeKeyColumn)
                                                        {
                                                            sql_CreateNewSp.Append(keyColumn + "=@" + keyColumn + " and");
                                                        }
                                                        sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 4, 4);
                                                        sql_CreateNewSp.AppendLine();
                                                    }
                                                    sql_CreateNewSp.AppendLine("end");
                                                }
                                            }
                                            sql_CreateNewSp.AppendLine("else if @operation='delete'");
                                            sql_CreateNewSp.AppendLine("begin");
                                            sql_CreateNewSp.AppendLine("delete from " + tableName);
                                            sql_CreateNewSp.AppendLine(" where ");
                                            foreach (string keyColumn in activeKeyColumn)
                                            {
                                                sql_CreateNewSp.Append(keyColumn + "=@" + keyColumn + " and ");
                                            }
                                            sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 5, 5);
                                            sql_CreateNewSp.AppendLine("");                       
                                            sql_CreateNewSp.AppendLine("end");
                                            sql_CreateNewSp.AppendLine("else if @operation='selectkey'");
                                            sql_CreateNewSp.AppendLine("begin");
                                            sql_CreateNewSp.AppendLine("select * from [" + tableName + "] where ");
                                            foreach (string keyColumn in activeKeyColumn)
                                            {
                                                sql_CreateNewSp.Append(keyColumn + "=@" + keyColumn + " or ");
                                            }
                                            sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 4, 4);
                                            sql_CreateNewSp.AppendLine("");
                                            sql_CreateNewSp.AppendLine("end");
                                            sql_CreateNewSp.AppendLine("else if @operation='selectcondition'");
                                            sql_CreateNewSp.AppendLine("begin");
                                            sql_CreateNewSp.AppendLine("select * from [" + tableName + "] where ");
                                            foreach (string selectColumn in activeColumn)
                                            {
                                                if(!filterTypeList.Contains(selectColumn))
                                                    sql_CreateNewSp.Append(selectColumn + "=@" + selectColumn + " or ");
                                            }
                                            sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 4, 4);
                                            sql_CreateNewSp.AppendLine("");
                                            sql_CreateNewSp.AppendLine("end");
                                            class_Data_SqlDataHelper.ActionExecuteForNonQuery(ActiveConnection, sql_CreateNewSp.ToString());

                                        }
                                    }
                                }
                            }
                        }
                    }
                    return true;
                }
                else if (ActiveConnection != null && ActiveConnection.activeDatabaseType == enum_DatabaseType.MySql)
                {
                    class_data_MySqlConnectionItem mysqlActiveConnectionItem = (class_data_MySqlConnectionItem)ActiveConnection;
                    string sql_getALLTables = class_Data_SqlStringHelper.Get_SQL_GETALLTABLES_FOR_MYSQL(mysqlActiveConnectionItem.ActiveConnection.Database);
                    string sql_getALLColumns = "Select COLUMN_NAME,COLUMN_TYPE,COLUMN_KEY,EXTRA from INFORMATION_SCHEMA.COLUMNS Where table_schema = '{schemaname}' and table_name = '{tablename}'";
                    string name_sp = "spa_operation_";
                    sql_getALLColumns = sql_getALLColumns.Replace("{schemaname}", ((class_data_MySqlConnectionItem) ActiveConnection).ActiveConnection.Database);                    
                    DataTable TableInfo = new DataTable();     
                    StringBuilder sql_CreateNewSp = new StringBuilder();
                    if (class_Data_SqlDataHelper.ActionExecuteSQLForDT(ActiveConnection,sql_getALLTables,out TableInfo))
                    {
                        foreach (DataRow activeTable in TableInfo.Rows)
                        {
                            string sql_getALLSPInfo = "select * from mysql.proc where db = '{schemaname}' and 'type' = 'PROCEDURE'";
                            string tableName = "";
                            class_Data_SqlDataHelper.GetColumnData(activeTable, "name", out tableName);
                            sql_getALLSPInfo = sql_getALLSPInfo.Replace("{schemaname}",((class_data_MySqlConnectionItem)ActiveConnection).ActiveConnection.Database);
                            DataTable TableSPInfos = new DataTable();
                            List<string> tmpSelectedColumsLst = new List<string>();
                            Dictionary<string, string> tmpSelectedKeyColumnsDirc = new Dictionary<string, string>();
                            if (class_Data_SqlDataHelper.ActionExecuteSQLForDT(ActiveConnection, sql_getALLTables, out TableSPInfos))
                            {
                                foreach(DataRow activeSP in TableSPInfos.Rows)
                                {
                                    string spname = "";
                                    class_Data_SqlDataHelper.GetColumnData(activeSP, "name", out spname);
                                    string sql_dropProcedure = "drop procedure " + spname;
                                    class_Data_SqlDataHelper.ActionExecuteForNonQuery(ActiveConnection, sql_dropProcedure);
                                }
                                if (tableName != "")
                                {                                    
                                    sql_getALLColumns = sql_getALLColumns.Replace("{tablename}", tableName);
                                    DataTable TableColumnsInfo = new DataTable();
                                    if (class_Data_SqlDataHelper.ActionExecuteSQLForDT(ActiveConnection, sql_getALLTables, out TableColumnsInfo))
                                    {
                                        sql_CreateNewSp.AppendLine("create procedure " + name_sp + tableName);
                                        if (TableColumnsInfo.Rows.Count > 0)
                                        {
                                            sql_CreateNewSp.AppendLine("(");
                                            sql_CreateNewSp.AppendLine("@operation,varchar(40),");
                                            foreach (DataRow activeColumnInfoRow in TableColumnsInfo.Rows)
                                            {
                                                string column_name = "";
                                                string column_type = "";
                                                string column_extra = "";
                                                string column_key = "";
                                                class_Data_SqlDataHelper.GetColumnData(activeColumnInfoRow, "COLUMN_NAME", out column_name);
                                                class_Data_SqlDataHelper.GetColumnData(activeColumnInfoRow, "COLUMN_TYPE", out column_type);
                                                class_Data_SqlDataHelper.GetColumnData(activeColumnInfoRow, "COLUMN_KEY", out column_key);
                                                class_Data_SqlDataHelper.GetColumnData(activeColumnInfoRow, "EXTRA", out column_extra);
                                                if (column_key == "PRI")
                                                    tmpSelectedKeyColumnsDirc.Add(column_name, column_type);
                                                if (column_extra == "auto_increment")
                                                    continue;
                                                sql_CreateNewSp.AppendLine("@" + column_name + " " + column_type + ",");
                                                if (!tmpSelectedColumsLst.Contains(column_name))
                                                    tmpSelectedColumsLst.Add(column_name);
                                            }
                                            sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 1, 1);
                                            sql_CreateNewSp.AppendLine(")");
                                        }
                                        sql_CreateNewSp.AppendLine("begin");
                                        sql_CreateNewSp.AppendLine("if @operation='select' then");
                                        sql_CreateNewSp.AppendLine("select * from " + tableName);
                                        sql_CreateNewSp.AppendLine("else if @operation='insert' then");
                                        sql_CreateNewSp.AppendLine("insert into " + ((class_data_MySqlConnectionItem)ActiveConnection).ActiveConnection.Database + "." + tableName + "(");
                                        foreach (DataRow activeColumnInfoRow in TableColumnsInfo.Rows)
                                        {
                                            string column_name = "";
                                            string column_extra = "";
                                            class_Data_SqlDataHelper.GetColumnData(activeColumnInfoRow, "COLUMN_NAME", out column_name);
                                            class_Data_SqlDataHelper.GetColumnData(activeColumnInfoRow, "EXTRA", out column_extra);
                                            if (column_extra != "auto_increment")
                                                sql_CreateNewSp.AppendLine(column_name + ",");
                                        }
                                        sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 1, 1);
                                        sql_CreateNewSp.AppendLine(")");
                                        sql_CreateNewSp.AppendLine(" values(");
                                        foreach (DataRow activeColumnInfoRow in TableColumnsInfo.Rows)
                                        {
                                            string column_name = "";
                                            string column_extra = "";
                                            class_Data_SqlDataHelper.GetColumnData(activeColumnInfoRow, "COLUMN_NAME", out column_name);
                                            class_Data_SqlDataHelper.GetColumnData(activeColumnInfoRow, "EXTRA", out column_extra);
                                            if (column_extra != "auto_increment")
                                                sql_CreateNewSp.AppendLine("@" + column_name + ",");
                                        }
                                        sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 1, 1);
                                        sql_CreateNewSp.AppendLine(")");
                                        foreach (string activeSelectedColumn in tmpSelectedColumsLst)
                                        {
                                            sql_CreateNewSp.AppendLine("else if @operation='update' and @" + activeSelectedColumn + "IS NULL then");
                                            sql_CreateNewSp.AppendLine("update " + tableName);
                                            sql_CreateNewSp.AppendLine("set " + activeSelectedColumn + " = " + "@" + activeSelectedColumn);
                                            if (tmpSelectedKeyColumnsDirc.Keys.Count > 0)
                                            {
                                                sql_CreateNewSp.AppendLine("where ");
                                                foreach (string keyColumn in tmpSelectedKeyColumnsDirc.Keys)
                                                    sql_CreateNewSp.AppendLine(keyColumn + " = @" + keyColumn + " and ");
                                                sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 6, 5);
                                            }
                                        }
                                        sql_CreateNewSp.AppendLine("else if @operation='delete' then");
                                        sql_CreateNewSp.AppendLine("delete from " + tableName);
                                        if (tmpSelectedKeyColumnsDirc.Keys.Count > 0)
                                        {
                                            sql_CreateNewSp.AppendLine(" where ");
                                            foreach (string keyColumn in tmpSelectedKeyColumnsDirc.Keys)
                                                sql_CreateNewSp.AppendLine(keyColumn + " = @" + keyColumn + " and ");
                                            sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 6, 5);
                                        }
                                        sql_CreateNewSp.AppendLine("else if @operation='selectkey' then");
                                        sql_CreateNewSp.AppendLine("select * from " + tableName);
                                        if (tmpSelectedKeyColumnsDirc.Keys.Count > 0)
                                        {
                                            sql_CreateNewSp.AppendLine(" where ");
                                            foreach (string keyColumn in tmpSelectedKeyColumnsDirc.Keys)
                                                sql_CreateNewSp.AppendLine(keyColumn + " = @" + keyColumn + " and ");
                                            sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 6, 5);
                                        }
                                        sql_CreateNewSp.AppendLine("else if @operation='selectcondition' then");
                                        sql_CreateNewSp.AppendLine("select * from " + tableName);
                                        if(tmpSelectedColumsLst.Count>0)
                                        {
                                            sql_CreateNewSp.AppendLine(" where ");
                                            foreach(string activeSelectedColumn in tmpSelectedColumsLst)
                                                sql_CreateNewSp.AppendLine(activeSelectedColumn + " = @" + activeSelectedColumn + " or ");
                                            sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 6, 5);
                                        }
                                        sql_CreateNewSp.AppendLine("end if");
                                        sql_CreateNewSp.AppendLine("end");

                                    }
                                }
                            }
                            else
                                return false;
                        }
                    }
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public List<string> ActionGetAllUserTables(class_data_PlatformDBConnection ActiveConnection)
        {             
            List<string> result = new List<string>();
            try
            {
                string sql_getALLTables = class_Data_SqlStringHelper.SQL_GETALLTABLES_FOR_SQL2008;
                if (ActiveConnection != null)
                {                                       
                    DataTable TablesInfo = new DataTable();
                    class_Data_SqlDataHelper.ActionExecuteSQLForDT(ActiveConnection, sql_getALLTables, out TablesInfo);
                    foreach (DataRow activeDR_1 in TablesInfo.Rows)
                    {
                        string tableName = "";
                        class_Data_SqlDataHelper.GetColumnData(activeDR_1, "name", out tableName);
                        result.Add(tableName);
                    }
                    return result;

                }
                else
                    return result;
            }
            catch(class_Base_AppExceptions err)
            {
                return result;
            }
        }

        public List<string> ActionGetAllUserStoreProcs(class_data_PlatformDBConnection ActiveConnection)
        {
            List<string> result = new List<string>();
            try
            {
                string sql_getALLTables = "select * from sys.all_objects where type='P' and is_ms_shipped=0";
                if (ActiveConnection != null)
                {                                    
                    DataTable TablesInfo = new DataTable();
                    class_Data_SqlDataHelper.ActionExecuteSQLForDT(ActiveConnection, sql_getALLTables, out TablesInfo);
                    foreach (DataRow activeDR_1 in TablesInfo.Rows)
                    {
                        string tableName = "";
                        class_Data_SqlDataHelper.GetColumnData(activeDR_1, "name", out tableName);
                        result.Add(tableName);
                    }
                    return result;

                }
                else
                    return result;
            }
            catch(class_Base_AppExceptions err)
            {
                return result;
            }
        }

        public string ActionBuildCreateSqlString(List<string> activeTableStructs,string tableName,string activeDBName)
        {
            if (activeTableStructs.Count == 0)
                return "";
            else
            {
                StringBuilder sql_Result = new StringBuilder();
                sql_Result.AppendLine("USE [" + activeDBName + "]");
                sql_Result.AppendLine("CREATE TABLE " + tableName);
                sql_Result.AppendLine("(");
                foreach (string activeTableStruct in activeTableStructs)
                {
                    sql_Result.AppendLine(activeTableStruct);
                }
                sql_Result.AppendLine(")");
                return sql_Result.ToString();
            }
        }

        public bool ActionExecuteCreateSql(List<string> activeTableStructes, string activeDBName, string tableName, class_data_PlatformDBConnection activeConnection)
        {
            try
            {
                if (activeTableStructes.Count == 0 || activeDBName == "" || activeConnection == null)
                    return false;
                else
               {
                    StringBuilder sql_CreateNewSp = new StringBuilder("IF OBJECTPROPERTY(OBJECT_ID(N'" + tableName + "'), N'IsTable') = 1");
                    sql_CreateNewSp.AppendLine();
                    sql_CreateNewSp.AppendLine("DROP TABLE " + tableName);
                    class_Data_SqlDataHelper.ActionExecuteForNonQuery(activeConnection, sql_CreateNewSp.ToString());
                    string sql = ActionBuildCreateSqlString(activeTableStructes, tableName, activeDBName);
                    if (sql != "")
                    {
                        if (class_Data_SqlDataHelper.ActionExecuteForNonQuery(activeConnection, sql))
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
            }
            catch(class_Base_AppExceptions err)
            {
                return false;
            }
        }


        public Dictionary<string, class_Data_SqlSPEntry> ActionAutoLoadingAllSPS(class_data_PlatformDBConnection activeConnection, string SPType)
        {
            if (activeConnection != null)
            {                               
                string sql_getallsps = "select * from sys.all_objects where (type = 'P') AND (is_ms_shipped = 0)";
                DataTable activeSPSDT=new DataTable();
                Dictionary<string, class_Data_SqlSPEntry> result = new Dictionary<string,class_Data_SqlSPEntry>();
                if (class_Data_SqlDataHelper.ActionExecuteSQLForDT(activeConnection, sql_getallsps, out activeSPSDT))
                {
                    foreach (DataRow activeRow in activeSPSDT.Rows)
                    {
                        class_Data_SqlSPEntry newSPEntry = new class_Data_SqlSPEntry();
                        string spName = "";
                        class_Data_SqlDataHelper.GetColumnData(activeRow, "name", out spName);
                        if (SPType != "")
                        {
                            if (SPType == class_Data_SqlSPEntryType.SelectAction)
                            {
                                if (!spName.StartsWith(class_Data_SqlSPEntryNameFiler.StartNamed_SelectAction))
                                    continue;
                            }
                            else if (SPType == class_Data_SqlSPEntryType.UpdateAction)
                            {
                                if (!spName.StartsWith(class_Data_SqlSPEntryNameFiler.StartNamed_Update))
                                    continue;
                            }
                        }
                        newSPEntry.SPName = spName;
                        newSPEntry.KeyName = spName;                        
                        string spObjectID = "";
                        class_Data_SqlDataHelper.GetColumnData(activeRow, "object_id", out spObjectID);
                        string sql_paramters = "select * from sys.all_parameters where object_id = " + spObjectID;
                        DataTable activeSPParametersDT=new DataTable();
                        string sql_paramstype = "select * from sys.types";
                        DataTable paramstypeDT=new DataTable();
                        if (!class_Data_SqlDataHelper.ActionExecuteSQLForDT(activeConnection, sql_paramstype, out paramstypeDT))
                        {
                            return null;
                        }
                        if (class_Data_SqlDataHelper.ActionExecuteSQLForDT(activeConnection, sql_paramters, out activeSPParametersDT))
                        {
                            foreach (DataRow activeParamterRow in activeSPParametersDT.Rows)
                            {
                                string activeSystemType_ID = "";
                                class_Data_SqlDataHelper.GetColumnData(activeParamterRow, "system_type_id", out activeSystemType_ID);
                                string activeUserType_ID = "";
                                class_Data_SqlDataHelper.GetColumnData(activeParamterRow, "user_type_id", out activeUserType_ID);
                                string activeParamsMaxLength = "";
                                class_Data_SqlDataHelper.GetColumnData(activeParamterRow, "max_length", out activeParamsMaxLength);
                                string activeParamsName = "";
                                class_Data_SqlDataHelper.GetColumnData(activeParamterRow, "name", out activeParamsName);
                                string activeIsOutPut = "";
                                class_Data_SqlDataHelper.GetColumnData(activeParamterRow, "is_output", out activeIsOutPut);
                                string max_length = "";
                                class_Data_SqlDataHelper.GetColumnData(activeParamterRow, "max_length", out max_length);
                                string activeDBType="";
                                DataRow[] dbtyps = paramstypeDT.Select("system_type_id=" + activeSystemType_ID + " and user_type_id=" + activeUserType_ID);
                                if (dbtyps.Length > 0)
                                {
                                    class_Data_SqlDataHelper.GetColumnData(dbtyps[0], "name", out activeDBType);
                                    newSPEntry.SetNewParameter(activeParamsName, Data_Util.ConventStrTODbtye(activeDBType), ParameterDirection.Input, int.Parse(max_length), null);                                    
                                }
                                else
                                    continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                        result.Add(newSPEntry.KeyName, newSPEntry);
                    }
                }
                return result;
            }
            else
                return null;
        }

        public DataTable ExecuteSelectSPForDT(class_Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper connectionHelper, string connectionKeyName)
        {
            DataTable dt = new DataTable();
            if (activeEntry != null)
            {
                activeEntry.ModifyParameterValue("@operation", "select");
                class_Data_SqlDataHelper activeSqlSPHelper = new  class_Data_SqlDataHelper();
                class_Data_SqlDataHelper.ActionExecuteStoreProcedureForDT(connectionHelper.Get_ActiveConnection(connectionKeyName), activeEntry, out dt);
                return dt;
            }
            else
                return null;
        }

        public DataTable ExecuteSelectSPKeyForDT(class_Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper connectionHelper, string connectionKeyName)
        {
            DataTable dt = new DataTable();
            if (activeEntry != null)
            {
                activeEntry.ModifyParameterValue("@operation", "selectkey");
                class_Data_SqlDataHelper activeSqlSPHelper = new class_Data_SqlDataHelper();
                class_Data_SqlDataHelper.ActionExecuteStoreProcedureForDT(connectionHelper.Get_ActiveConnection(connectionKeyName), activeEntry, out dt);
                return dt;
            }
            else
                return null;
        }

        public DataTable ExecuteSelectSPConditionForDT(class_Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper connectionHelper, string connectionKeyName)
        {
            DataTable dt = new DataTable();
            if (activeEntry != null)
            {
                activeEntry.ModifyParameterValue("@operation", "selectcondition");
                class_Data_SqlDataHelper activeSqlSPHelper = new class_Data_SqlDataHelper();
                class_Data_SqlDataHelper.ActionExecuteStoreProcedureForDT(connectionHelper.Get_ActiveConnection(connectionKeyName), activeEntry, out dt);
                return dt;
            }
            else
                return null;
        }

        public class_data_PlatformDBDataReader ExecuteSelectSPConditionForDR(class_Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper connectionHelper, string connectionKeyName)
        {            
           class_data_PlatformDBDataReader activeDataReader = null;
            if (activeEntry != null)
            {
                activeEntry.ModifyParameterValue("@operation", "selectcondition");
                class_Data_SqlDataHelper activeSqlSPHelper = new class_Data_SqlDataHelper();
                class_Data_SqlDataHelper.ActionExecuteStoreProcedureForDR(connectionHelper.Get_ActiveConnection(connectionKeyName), activeEntry, out activeDataReader);
                return activeDataReader;
            }
            else
                return null;      
        }

        public class_data_PlatformDBDataReader ExecuteSelectSPKeyForDR(class_Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper connectionHelper, string connectionKeyName)
        {
            class_data_PlatformDBDataReader activeDataReader = null;
            if (activeEntry != null)
            {
                activeEntry.ModifyParameterValue("@operation", "selectkey");
                class_Data_SqlDataHelper activeSqlSPHelper = new class_Data_SqlDataHelper();
                class_Data_SqlDataHelper.ActionExecuteStoreProcedureForDR(connectionHelper.Get_ActiveConnection(connectionKeyName), activeEntry, out activeDataReader);
                return activeDataReader;
            }
            else
                return null;    
        }

        public class_data_PlatformDBDataReader ExecuteSelectSPForDR(class_Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper connectionHelper, string connectionKeyName)
        {
            class_data_PlatformDBDataReader activeDataReader = null;
            if (activeEntry != null)
            {
                activeEntry.ModifyParameterValue("@operation", "select");
                class_Data_SqlDataHelper activeSqlSPHelper = new class_Data_SqlDataHelper();
                class_Data_SqlDataHelper.ActionExecuteStoreProcedureForDR(connectionHelper.Get_ActiveConnection(connectionKeyName), activeEntry, out activeDataReader);
                return activeDataReader;
            }
            else
                return null;             
        }

        public bool ExecuteInsertSP(class_Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper connectionHelper, string connectionKeyName)
        {
            if (activeEntry != null)
            {
                activeEntry.ModifyParameterValue("@operation", "insert");
                class_Data_SqlDataHelper.ActionExecuteSPForNonQuery(connectionHelper.Get_ActiveConnection(connectionKeyName), activeEntry);
                return true;
            }
            else
                return false;
        }

        public bool ExecuteUpdateSP(class_Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper connectionHelper, string connectionKeyName)
        {
            if (activeEntry != null)
            {
                activeEntry.ModifyParameterValue("@operation", "update");
                class_Data_SqlDataHelper.ActionExecuteSPForNonQuery(connectionHelper.Get_ActiveConnection(connectionKeyName), activeEntry);
                return true;
            }
            else
                return false;
        }

        public bool ExecuteDeleteSP(class_Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper connectionHelper, string connectionKeyName)
        {
            if (activeEntry != null)
            {
                activeEntry.ModifyParameterValue("@operation", "delete");

                class_Data_SqlDataHelper.ActionExecuteSPForNonQuery(connectionHelper.Get_ActiveConnection(connectionKeyName), activeEntry);
                return true;
            }
            else
                return false;
        }

    }
}
