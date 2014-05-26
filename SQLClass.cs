using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Objects;
using System.Data.Sql;
using System.Data.SqlClient;

using BDClassLib;

namespace OnlineAbit2013
{
    /// <summary>
    /// ADO.NET powered SQL class
    /// </summary>
    //public class SQLClass
    //{
    //    private string _connectionString;

    //    public SQLClass(string connString)
    //    {
    //        _connectionString = connString;
    //    }

    //    public DataTable GetDataTable(string query, SortedList<string, object> prms)
    //    {
    //        using (SqlConnection conn = new SqlConnection(_connectionString))
    //        {
    //            conn.Open();

    //            SqlCommand comm = new SqlCommand(query, conn);
    //            if (prms != null)
    //            {
    //                foreach (KeyValuePair<string, object> param in prms)
    //                    comm.Parameters.AddWithValue(param.Key, param.Value);
    //            }
    //            DataTable ret = new DataTable();

    //            using (SqlDataAdapter da = new SqlDataAdapter(comm))
    //            {
    //                da.Fill(ret);
    //            }
    //            conn.Close();
    //            return ret;
    //        }
    //    }

    //    public object GetValue(string query, SortedList<string, object> prms)
    //    {
    //        using (SqlConnection conn = new SqlConnection(_connectionString))
    //        {
    //            conn.Open();

    //            SqlCommand comm = new SqlCommand(query, conn);
    //            foreach (KeyValuePair<string, object> param in prms)
    //                comm.Parameters.AddWithValue(param.Key, param.Value);

    //            object ret = comm.ExecuteScalar();
    //            conn.Close();

    //            if (ret == DBNull.Value)
    //                ret = null;

    //            return ret;
    //        }
    //    }

    //    public int ExecuteQuery(string query, SortedList<string, object> prms)
    //    {
    //        using (SqlConnection conn = new SqlConnection(_connectionString))
    //        {
    //            conn.Open();

    //            SqlCommand comm = new SqlCommand(query, conn);
    //            foreach (KeyValuePair<string, object> param in prms)
    //                comm.Parameters.AddWithValue(param.Key, param.Value);

    //            int res = comm.ExecuteNonQuery();
    //            conn.Close();

    //            return res;
    //        }
    //    }

    //    public string GetStringValue(string query, SortedList<string, object> prms)
    //    {
    //        object obj = GetValue(query, prms);
    //        if (obj == null)
    //            return "";
    //        else
    //            return obj.ToString();
    //    }

    //    public void ExecuteStoredProcedure(string procName)
    //    {
    //        using (SqlConnection conn = new SqlConnection(_connectionString))
    //        {
    //            conn.Open();

    //            SqlCommand comm = new SqlCommand();
    //            comm.CommandType = CommandType.StoredProcedure;

    //            int res = comm.ExecuteNonQuery();
    //            conn.Close();
    //        }
    //    }
    //}

    public static class SqlAddClass
    {
        public static DataTable GetDataTable(this SQLClass bdc, string query, SortedList<string, object> slParams)
        {

            DataSet ds = slParams == null ? bdc.GetDataSet(query) : bdc.GetDataSet(query, slParams);
            DataTable tbl = new DataTable();
            if (ds.Tables.Count > 0)
                tbl = ds.Tables[0];
            return tbl;
        }
    }
}