using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace iKCoder_Platform_SDK_Kit
{
    public class Data_SqlDataHelper
    {
        private SqlConnection activeconnection;       
        
        public SqlConnection ActiveConnection
        {
            set
            {
                activeconnection = value;
            }
            get
            {
                return activeconnection;
            }
        }

        public bool Static_GetColumnData(DataRow activeDR, string activeColumnName,out string result)
        {
            result = "";
            if (activeDR != null)
            {
                try
                {
                    result = activeDR[activeColumnName].ToString();
                    return true;
                }
                catch(class_Base_AppExceptions err)
                {
                    return false;
                }
            }
            else
                return false;
        }

        public bool Static_GetColumnsFromDT(DataTable activeDT, out List<string> result)
        {
            result = null;
            if (activeDT != null)
            {
                result = new List<string>();
                foreach (DataColumn actriveDC in activeDT.Columns)
                {
                    result.Add(actriveDC.ColumnName);
                }
                return true;
            }
            else
                return false;
        }

        public bool Static_GetColumnsTypeFromDT(DataTable activeDT, out Dictionary<string, Type> result)
        {
            result = null;
            if (activeDT != null)
            {
                result = new Dictionary<string, Type>();
                foreach (DataColumn actriveDC in activeDT.Columns)
                {
                    result.Add(actriveDC.ColumnName,actriveDC.DataType);
                }
                return true;
            }
            else
                return false;
        }

        public bool Static_GetActiveRow(DataTable activeDT, int activeRowIndex, out DataRow result)
        {
            result = null;
            if (activeDT != null)
            {
                try
                {
                    if (activeDT.Rows.Count >= activeRowIndex + 1)
                    {
                        result = activeDT.Rows[activeRowIndex];
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
            else
                return false;
        }

        public bool Action_ExecuteForDS(string executeSql,out DataSet resultDS)
        {
            resultDS = null;
            try
            {                
                if (executeSql == "")
                    return false;
                else
                {
                    if (activeconnection != null)
                    {
                        SqlCommand cmd = new SqlCommand(executeSql);                        
                        cmd.Connection = activeconnection;
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);                        
                        resultDS = new DataSet();
                        sda.Fill(resultDS);
                        return true;
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

        public bool Action_ExecuteForDS(Data_SqlSPEntry activeSPEntry, out DataSet resultDS)
        {
            resultDS = null;
            try
            {
                if (activeSPEntry == null)
                    return false;
                else
                {
                    if (activeconnection != null)
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandText = activeSPEntry.SPName;
                        cmd.CommandType = CommandType.StoredProcedure;
                        foreach (SqlParameter activeParameter in activeSPEntry.ActiveParameters)                        
                            cmd.Parameters.Add(activeParameter);                        
                        cmd.Connection = activeconnection;                        
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        resultDS = new DataSet();
                        sda.Fill(resultDS);
                        return true;
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

        public bool Action_ExecuteForDT(Data_SqlSPEntry activeSPEntry, out DataTable resultDT)
        {
            resultDT = null;
            try
            {
                if (activeSPEntry == null)
                    return false;
                else
                {
                    if (activeconnection != null)
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandText = activeSPEntry.SPName;
                        cmd.CommandType = CommandType.StoredProcedure;
                        foreach (SqlParameter activeParameter in activeSPEntry.ActiveParameters)
                        {
                            SqlParameter newParameter = cmd.CreateParameter();
                            newParameter.ParameterName = activeParameter.ParameterName;
                            newParameter.DbType = activeParameter.DbType;
                            newParameter.Direction = activeParameter.Direction;
                            newParameter.Value = activeParameter.Value;
                            cmd.Parameters.Add(newParameter);
                        }
                        cmd.Connection = activeconnection;             
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);                        
                        resultDT = new DataTable();
                        sda.Fill(resultDT);
                        return true;
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

        public bool Action_ExecuteForDT(string executeSql, out DataTable resultDT)
        {
            resultDT = null;
            try
            {
                if (executeSql == "")
                    return false;
                else
                {
                    if (activeconnection != null)
                    {
                        SqlCommand cmd = new SqlCommand(executeSql);
                        cmd.Connection = activeconnection;
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        resultDT = new DataTable();
                        sda.Fill(resultDT);
                        return true;
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

        public bool Action_ExecuteForNonQuery(Data_SqlSPEntry activeSPEntry)
        {
            try
            {
                if (activeSPEntry == null)
                    return false;
                else
                {
                    if (activeconnection != null)
                    {
                        if (activeconnection != null)
                        {
                            SqlCommand cmd = new SqlCommand();
                            cmd.CommandText = activeSPEntry.SPName;
                            cmd.CommandType = CommandType.StoredProcedure;
                            foreach (SqlParameter activeParameter in activeSPEntry.ActiveParameters)
                            {
                                SqlParameter newParameter = new SqlParameter();
                                newParameter.ParameterName = activeParameter.ParameterName;
                                newParameter.SqlDbType = activeParameter.SqlDbType;
                                newParameter.DbType = activeParameter.DbType;
                                newParameter.Direction = activeParameter.Direction;
                                newParameter.Value = activeParameter.Value;
                                cmd.Parameters.Add(newParameter);
                            }
                            cmd.Connection = activeconnection;         
                            cmd.ExecuteNonQuery();
                            return true;
                        }
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

        public bool Action_ExecuteForNonQuery(string executeSql)
        {
            try
            {
                if (executeSql == "")
                    return false;
                else
                {
                    if (activeconnection != null)
                    {
                        if (activeconnection != null)
                        {
                            SqlCommand cmd = new SqlCommand(executeSql);
                            cmd.Connection = activeconnection;
                            cmd.ExecuteNonQuery();
                            return true;
                        }
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
        

    }
}
