using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using iKCoder_Platform_SDK_Kit;

namespace iKCoder_Platform_SDK_Kit
{

    public class Data_SqlStringHelper
    {             
        public const string SQL_GETALLTABLES_FOR_SQL2008 = "select * from sys.tables where (type = 'U')";
        public const string sql_GETALLTABLES_FOR_SQL2005 = "select * from sys.all_objects where (type = 'U')"; 
    }

    public class Data_SqlHelper
    {        

        public bool ActionAutoCreateSPS(SqlConnection ActiveConnection)
        {
            try
            {
                if (ActiveConnection != null)
                {
                    string sql_getALLTables = Data_SqlStringHelper.SQL_GETALLTABLES_FOR_SQL2008;                                      
                    DataTable TablesInfo = new DataTable();
                    DataTable ColumnInfo = new DataTable();
                    DataTable TypesInfo = new DataTable();
                    if (class_Data_SqlDataHelper.ActionExecuteForDT(ActiveConnection, sql_getALLTables, out TablesInfo))
                    {
                        if (TablesInfo.Rows.Count > 0)
                        {
                            foreach (DataRow activeDR_1 in TablesInfo.Rows)
                            {
                                string sql_getALLColumns = "select * from sys.syscolumns";
                                string tableName = "";
                                class_Data_SqlDataHelper.StaticGetColumnData(activeDR_1, "name", out tableName);
                                string objectID = "";
                                class_Data_SqlDataHelper.StaticGetColumnData(activeDR_1, "object_id", out objectID);
                                if (tableName != "")
                                {
                                    if (objectID != "")
                                    {
                                        sql_getALLColumns = sql_getALLColumns + " where id='" + objectID + "'";
                                        List<string> activeColumn = new List<string>();
                                        List<string> activeKeyColumn = new List<string>();
                                        List<string> filterColumn = new List<string>();                                        
                                        if (class_Data_SqlDataHelper.ActionExecuteForDT(ActiveConnection, sql_getALLColumns, out ColumnInfo))
                                        {
                                            StringBuilder sql_CreateNewSp = new StringBuilder("IF OBJECTPROPERTY(OBJECT_ID(N'SPA_Operation_" + tableName + "'), N'IsProcedure') = 1");
                                            sql_CreateNewSp.AppendLine();
                                            sql_CreateNewSp.AppendLine("DROP PROCEDURE SPA_Operation_" + tableName);
                                            class_Data_SqlDataHelper.ActionExecuteForNonQuery(ActiveConnection, sql_CreateNewSp.ToString());
                                            sql_CreateNewSp.Clear();
                                            sql_CreateNewSp.AppendLine("CREATE PROCEDURE {SPNAME}");
                                            StringBuilder sql_insertSourceColumns = new StringBuilder();
                                            StringBuilder sql_insertValueColumns = new StringBuilder();
                                            sql_CreateNewSp.Replace("{SPNAME}", "SPA_Operation_" + tableName);
                                            sql_CreateNewSp.AppendLine("(");
                                            sql_CreateNewSp.AppendLine("@operation nvarchar(20) = '',");
                                            foreach (DataRow activeDR_2 in ColumnInfo.Rows)
                                            {
                                                string sql_getALLTypes = "select * from sys.types";
                                                string columnname = "";
                                                class_Data_SqlDataHelper.StaticGetColumnData(activeDR_2, "name", out columnname);
                                                string typeid = "";
                                                string length = "";
                                                string status = "";
                                                class_Data_SqlDataHelper.StaticGetColumnData(activeDR_2, "xtype", out typeid);
                                                class_Data_SqlDataHelper.StaticGetColumnData(activeDR_2, "status", out status);
                                                class_Data_SqlDataHelper.StaticGetColumnData(activeDR_2, "prec", out length);
                                                sql_getALLTypes = sql_getALLTypes + " where system_type_id=" + typeid;
                                                class_Data_SqlDataHelper.ActionExecuteForDT(ActiveConnection, sql_getALLTypes, out TypesInfo);
                                                string typename = "";
                                                class_Data_SqlDataHelper.StaticGetColumnData(TypesInfo.Rows[0], "name", out typename);
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
                                            sql_CreateNewSp.AppendLine("if @operation='get'");
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
                                            /*if (activeKeyColumn.Count > 0)
                                            {
                                                sql_CreateNewSp.AppendLine("if ");
                                                foreach (string keyColumn in activeKeyColumn)
                                                {
                                                    sql_CreateNewSp.Append("@" + keyColumn + "<>'' and");
                                                }
                                                sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 4, 4);
                                                sql_CreateNewSp.AppendLine("begin");
                                                sql_CreateNewSp.AppendLine("delete from " + tableName);
                                                sql_CreateNewSp.AppendLine("where ");
                                                foreach (string keyColumn in activeKeyColumn)
                                                {
                                                    sql_CreateNewSp.Append(keyColumn + "=@" + keyColumn + " and");
                                                }
                                                sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 4, 4);
                                                sql_CreateNewSp.AppendLine();
                                                sql_CreateNewSp.AppendLine("end");
                                            }
                                            else
                                            {*/
                                            sql_CreateNewSp.AppendLine("delete from " + tableName);
                                            sql_CreateNewSp.AppendLine(" where ");
                                            foreach (string keyColumn in activeKeyColumn)
                                            {
                                                sql_CreateNewSp.Append(keyColumn + "=@" + keyColumn + " and");
                                            }
                                            sql_CreateNewSp = sql_CreateNewSp.Remove(sql_CreateNewSp.Length - 4, 4);
                                            sql_CreateNewSp.AppendLine("");
                                            //}
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
                else
                    return false;
            }
            catch(class_Base_AppExceptions err)
            {
                return false;
            }
        }

        public List<string> ActionGetAllUserTables(SqlConnection ActiveConnection)
        {             
            List<string> result = new List<string>();
            try
            {
                string sql_getALLTables = Data_SqlStringHelper.SQL_GETALLTABLES_FOR_SQL2008;
                if (ActiveConnection != null)
                {                                       
                    DataTable TablesInfo = new DataTable();
                    class_Data_SqlDataHelper.ActionExecuteForDT(ActiveConnection, sql_getALLTables, out TablesInfo);
                    foreach (DataRow activeDR_1 in TablesInfo.Rows)
                    {
                        string tableName = "";
                        class_Data_SqlDataHelper.StaticGetColumnData(activeDR_1, "name", out tableName);
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

        public List<string> ActionGetAllUserStoreProcs(SqlConnection ActiveConnection)
        {
            List<string> result = new List<string>();
            try
            {
                string sql_getALLTables = "select * from sys.all_objects where type='P' and is_ms_shipped=0";
                if (ActiveConnection != null)
                {                                    
                    DataTable TablesInfo = new DataTable();
                    class_Data_SqlDataHelper.ActionExecuteForDT(ActiveConnection, sql_getALLTables, out TablesInfo);
                    foreach (DataRow activeDR_1 in TablesInfo.Rows)
                    {
                        string tableName = "";
                        class_Data_SqlDataHelper.StaticGetColumnData(activeDR_1, "name", out tableName);
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

        public bool Action_ExecuteCreateSql(List<string> activeTableStructes, string activeDBName,string tableName, SqlConnection ActiveConnection)
        {
            try
            {
                if (activeTableStructes.Count == 0 || activeDBName == "" || ActiveConnection == null)
                    return false;
                else
               {
                    StringBuilder sql_CreateNewSp = new StringBuilder("IF OBJECTPROPERTY(OBJECT_ID(N'" + tableName + "'), N'IsTable') = 1");
                    sql_CreateNewSp.AppendLine();
                    sql_CreateNewSp.AppendLine("DROP TABLE " + tableName);
                    class_Data_SqlDataHelper.ActionExecuteForNonQuery(ActiveConnection, sql_CreateNewSp.ToString());
                    string sql = ActionBuildCreateSqlString(activeTableStructes, tableName, activeDBName);
                    if (sql != "")
                    {
                        if (obj_dataHelper.Action_ExecuteForNonQuery(sql))
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


        public Dictionary<string, Data_SqlSPEntry> Action_AutoLoadingAllSPS(SqlConnection ActiveConnection,string SPType)
        {
            if (ActiveConnection != null)
            {
                class_Data_SqlDataHelper obj=new class_Data_SqlDataHelper();                
                string sql_getallsps = "select * from sys.all_objects where (type = 'P') AND (is_ms_shipped = 0)";
                DataTable activeSPSDT=new DataTable();
                Dictionary<string, Data_SqlSPEntry> result = new Dictionary<string,Data_SqlSPEntry>();
                if (obj.ActionExecuteForDT(ActiveConnection,sql_getallsps, out activeSPSDT))
                {
                    foreach (DataRow activeRow in activeSPSDT.Rows)
                    {
                        Data_SqlSPEntry newSPEntry = new Data_SqlSPEntry();
                        string spName = "";
                        class_Data_SqlDataHelper.StaticGetColumnData(activeRow, "name", out spName);
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
                        class_Data_SqlDataHelper.StaticGetColumnData(activeRow, "object_id", out spObjectID);
                        string sql_paramters = "select * from sys.all_parameters where object_id = " + spObjectID;
                        DataTable activeSPParametersDT=new DataTable();
                        string sql_paramstype = "select * from sys.types";
                        DataTable paramstypeDT=new DataTable();
                        if (!class_Data_SqlDataHelper.ActionExecuteForDT(ActiveConnection, sql_paramstype, out paramstypeDT))
                        {
                            return null;
                        }
                        if (class_Data_SqlDataHelper.ActionExecuteForDT(ActiveConnection, sql_paramters, out activeSPParametersDT))
                        {
                            foreach (DataRow activeParamterRow in activeSPParametersDT.Rows)
                            {
                                string activeSystemType_ID = "";
                                class_Data_SqlDataHelper.StaticGetColumnData(activeParamterRow, "system_type_id", out activeSystemType_ID);
                                string activeUserType_ID = "";
                                class_Data_SqlDataHelper.StaticGetColumnData(activeParamterRow, "user_type_id", out activeUserType_ID);
                                string activeParamsMaxLength = "";
                                class_Data_SqlDataHelper.StaticGetColumnData(activeParamterRow, "max_length", out activeParamsMaxLength);
                                string activeParamsName = "";
                                class_Data_SqlDataHelper.StaticGetColumnData(activeParamterRow, "name", out activeParamsName);
                                string activeIsOutPut = "";
                                class_Data_SqlDataHelper.StaticGetColumnData(activeParamterRow, "is_output", out activeIsOutPut);
                                string max_length = "";
                                class_Data_SqlDataHelper.StaticGetColumnData(activeParamterRow, "max_length", out max_length);
                                string activeDBType="";
                                DataRow[] dbtyps = paramstypeDT.Select("system_type_id=" + activeSystemType_ID + " and user_type_id=" + activeUserType_ID);
                                if (dbtyps.Length > 0)
                                {
                                    class_Data_SqlDataHelper.StaticGetColumnData(dbtyps[0], "name", out activeDBType);
                                    Data_SqlSPEntry.AddSPParameter(ref newSPEntry, activeParamsName, Data_Util.ConventStrTODbtye(activeDBType), ParameterDirection.Input, int.Parse(max_length), null);
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

        public DataTable ExecuteGetSPS(Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper SqlHelperObj, string Server)
        {
            DataTable dt = new DataTable();
            if (activeEntry != null)
            {
                Data_SqlSPEntry.ModifySPParameter(ref activeEntry, "@operation", SqlDbType.NVarChar, ParameterDirection.Input, "get");
                class_Data_SqlDataHelper activeSqlSPHelper = new  class_Data_SqlDataHelper();                
                class_Data_SqlDataHelper.ActionExecuteForDT(SqlHelperObj.Get_ActiveConnection(Server),activeEntry, out dt);
                return dt;
            }
            else
                return null;
        }

        public bool ExecuteInsertSPS(Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper SqlHelperObj, string Server, Dictionary<string, object> ValueMaping, Dictionary<string, int> SizeMaping)
        {
            if (activeEntry != null)
            {
                Data_SqlSPEntry.ModifySPParameter(ref activeEntry, "@operation", SqlDbType.NVarChar, ParameterDirection.Input, "insert");
                foreach (string activeParameter in ValueMaping.Keys)
                {
                    int activeParametersIndex = Data_SqlSPEntry.GetSPParameterIndex(ref activeEntry, activeParameter);
                    if (activeParametersIndex != -1)
                    {
                        SqlParameter activeExistedParamter = activeEntry.ActiveParameters[activeParametersIndex];
                        if (!SizeMaping.ContainsKey(activeParameter))
                            Data_SqlSPEntry.ModifySPParameter(ref activeEntry, activeParameter, activeExistedParamter.SqlDbType, activeExistedParamter.Direction, ValueMaping[activeParameter]);
                        else
                            Data_SqlSPEntry.ModifySPParameter(ref activeEntry, activeParameter, activeExistedParamter.SqlDbType, activeExistedParamter.Direction, ValueMaping[activeParameter], SizeMaping[activeParameter]);

                    }
                }                
                class_Data_SqlDataHelper.ActionExecuteForNonQuery(SqlHelperObj.Get_ActiveConnection(Server), activeEntry);
                return true;
            }
            else
                return false;
        }

        public bool ExecuteUpdateSPS(Data_SqlSPEntry activeEntry, class_Data_SqlConnectionHelper SqlHelperObj, string Server, Dictionary<string, object> ValueMaping, Dictionary<string, int> SizeMaping)
        {
            if (activeEntry != null)
            {
                Data_SqlSPEntry.ModifySPParameter(ref activeEntry, "@operation", SqlDbType.NVarChar, ParameterDirection.Input, "update");                                
                List<string> filterExclude=new List<string>();
                filterExclude.Add("@operation");
                Data_SqlSPEntry.ModifyExcludeSPParameter(ref activeEntry, ValueMaping, filterExclude);
                foreach (string activeParameter in ValueMaping.Keys)
                {
                    int activeParametersIndex = Data_SqlSPEntry.GetSPParameterIndex(ref activeEntry, activeParameter);
                    if (activeParametersIndex != -1)
                    {
                        SqlParameter activeExistedParamter = activeEntry.ActiveParameters[activeParametersIndex];
                        if (!SizeMaping.ContainsKey(activeParameter))
                            Data_SqlSPEntry.ModifySPParameter(ref activeEntry, activeParameter, activeExistedParamter.SqlDbType, activeExistedParamter.Direction, ValueMaping[activeParameter]);
                        else
                            Data_SqlSPEntry.ModifySPParameter(ref activeEntry, activeParameter, activeExistedParamter.SqlDbType, activeExistedParamter.Direction, ValueMaping[activeParameter], SizeMaping[activeParameter]);

                    }                      
                }
                class_Data_SqlDataHelper.ActionExecuteForNonQuery(SqlHelperObj.Get_ActiveConnection(Server), activeEntry);
                return true;
            }
            else
                return false;
        }

        public bool ExecuteDeleteSPS(Data_SqlSPEntry activeEntry,class_Data_SqlConnectionHelper SqlHelperObj, string Server, Dictionary<string, object> ValueMaping, Dictionary<string, int> SizeMaping)
        {
            if (activeEntry != null)
            {

                Data_SqlSPEntry.ModifySPParameter(ref activeEntry, "@operation", SqlDbType.NVarChar, ParameterDirection.Input, "delete");
                foreach (string activeParameter in ValueMaping.Keys)
                {
                    int activeParametersIndex = Data_SqlSPEntry.GetSPParameterIndex(ref activeEntry, activeParameter);
                    if (activeParametersIndex != -1)
                    {
                        SqlParameter activeExistedParamter = activeEntry.ActiveParameters[activeParametersIndex];
                        if (!SizeMaping.ContainsKey(activeParameter))
                            Data_SqlSPEntry.ModifySPParameter(ref activeEntry, activeParameter, activeExistedParamter.SqlDbType, activeExistedParamter.Direction, ValueMaping[activeParameter]);
                        else
                            Data_SqlSPEntry.ModifySPParameter(ref activeEntry, activeParameter, activeExistedParamter.SqlDbType, activeExistedParamter.Direction, ValueMaping[activeParameter], SizeMaping[activeParameter]);

                    }
                }
                class_Data_SqlDataHelper.ActionExecuteForNonQuery(SqlHelperObj.Get_ActiveConnection(Server),activeEntry);
                return true;
            }
            else
                return false;
        }

    }
}
