using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace Valdo
{
    public class SharedConnections
    {
        private SqlConnection objCon;
        private SqlDataAdapter objdtAdapter;
        private SqlCommand objCmd;
        private DataSet objDtSet;

        public SharedConnections()
        {
            objCon = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["ConnString"]);
        }

        public DataTable GetDataTable(string SQLQuery, string TableName)
        {
            DataTable objDtTable = null;

            if (objCon.State == ConnectionState.Closed)
                objCon.Open();

            objCmd = new SqlCommand(SQLQuery, objCon);
            objCmd.CommandTimeout = 0;
            objDtSet = new DataSet();
            objdtAdapter = new SqlDataAdapter(objCmd);
            objdtAdapter.Fill(objDtSet, TableName);
            objDtTable = objDtSet.Tables[TableName];
            objCon.Close();

            return objDtTable;
        }
    }
}