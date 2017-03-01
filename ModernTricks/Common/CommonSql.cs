using ModernTricks.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

public static class CommonSql
{
    public static List<dynamic> ExecuteStoredProcedure(REPORTS _report, string _rawParams, Controller _controller)
    {
        List<dynamic> _dataRows = new List<dynamic>();
        try
        {
            using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["RBAC_Model"].ConnectionString))
            {
                using (var command = new SqlCommand(_report.StoredProcedureName, sqlConn) { CommandType = CommandType.StoredProcedure })
                {
                    //Read parameters...
                    string[] _parameters = _rawParams.Split('\\');
                    foreach (string _param in _parameters)
                    {
                        try
                        {
                            string _paramName = _param.Substring(0, _param.IndexOf('=')).Trim();
                            string _paramValue = _param.Substring(_param.IndexOf('=') + 1).Trim();

                            if (!string.IsNullOrEmpty(_paramValue))
                            {
                                dynamic _paramSqlValue = _paramValue;
                                switch (_report.PARAMETERS.Where(p => p.ParameterName == _paramName).FirstOrDefault().ParameterType.ToLower())
                                {
                                    case "datetime":
                                        _paramSqlValue = FormatDate4SqlStoredProcedureParameter(DateTime.ParseExact(_paramValue, "MM/dd/yyyy", CultureInfo.InvariantCulture));
                                        break;
                                    case "date":
                                        _paramSqlValue = FormatDate4SqlStoredProcedureParameter(DateTime.ParseExact(_paramValue, "MM/dd/yyyy", CultureInfo.InvariantCulture));
                                        break;
                                    case "int":
                                        _paramSqlValue = int.Parse(_paramValue);
                                        break;
                                    default:
                                        _paramSqlValue = _paramValue;
                                        break;
                                }
                                command.Parameters.Add(new SqlParameter(_paramName, _paramSqlValue));
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    sqlConn.Open();
                    SqlDataReader dbReader = command.ExecuteReader();

                    while (dbReader.Read())
                    {
                        DynamicDataRow _row = new DynamicDataRow();
                        for (int i = 0; i < dbReader.FieldCount; i++)
                        {
                            _row.AddColumn(dbReader.GetName(i), GetValue(dbReader, i), dbReader.GetDataTypeName(i).ToString());
                        }
                        _dataRows.Add(_row);
                    }
                    sqlConn.Close();
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return _dataRows;
    }

    public static List<dynamic> RunSql(string _sql, Controller _controller)
    {
        List<dynamic> _dataRows = new List<dynamic>();
        try
        {
            using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["RBAC_Model"].ConnectionString))
            {
                sqlConn.Open();
                SqlCommand sqlComm = new SqlCommand(_sql, sqlConn);
                sqlComm.CommandType = CommandType.Text;
                SqlDataReader dReader = sqlComm.ExecuteReader();

                while (dReader.Read())
                {
                    DynamicDataRow _row = new DynamicDataRow();
                    for (int i = 0; i < dReader.FieldCount; i++)
                    {
                        _row.AddColumn(dReader.GetName(i), GetValue(dReader, i), dReader.GetDataTypeName(i).ToString());
                    }
                    _dataRows.Add(_row);
                }
                sqlConn.Close();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return _dataRows;
    }

    public static string FormatDate4SqlStoredProcedureParameter(DateTime _date)
    {
        string _retVal = _date.ToShortDateString();
        try
        {
            _retVal = string.Format("{0}-{1}-{2}", _date.Year, _date.Month, _date.Day);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return _retVal;
    }

    public static string GetValue(SqlDataReader _reader, int _fieldPosition, string _defaultValue = "")
    {
        string _retVal = string.Empty;
        try
        {
            _retVal = _reader.GetValue(_fieldPosition).ToString().Trim();
            if (string.IsNullOrEmpty(_retVal))
                _retVal = _defaultValue;

            if (_retVal.Trim() == "Nil")
                _retVal = string.Empty;
        }
        catch
        {
            _retVal = _defaultValue;
        }
        return _retVal;
    }

    public static dynamic GetValue(SqlDataReader _reader, int _fieldPosition)
    {
        dynamic _retVal = null;
        try
        {
            Type obj = _reader.GetValue(_fieldPosition).GetType();
            if (obj.FullName == "System.DateTime")
            {
                _retVal = ((DateTime)_reader.GetValue(_fieldPosition)).ToShortDateString();
            }
            else
                _retVal = _reader.GetValue(_fieldPosition);
        }
        catch
        {
        }
        return _retVal;
    }

    public static int GetIntValue(SqlDataReader _reader, int _fieldPosition, int _defaultValue = 0)
    {
        int _retVal = _defaultValue;
        try
        {
            _retVal = _reader.GetInt32(_fieldPosition);
        }
        catch
        {
            _retVal = _defaultValue;
        }
        return _retVal;
    }
}

