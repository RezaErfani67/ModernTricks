using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

public class DynamicDataObject
{
    public readonly string Name;
    public readonly string Value;
    public readonly string DataType;

    public DynamicDataObject(string _colName, string _colValue, string _colType = "string")
    {
        Name = _colName;
        Value = _colValue;
        DataType = _colType;
    }
}

public class DynamicDataRow
{  
    public List<DynamicDataObject> Columns = new List<DynamicDataObject>();

    public int ColumnCount
    {
        get
        {            
            return Columns.Count;
        }
    }

    public void AddColumn(string _columnName, dynamic _columnValue, string _columnDataType)
    {
        try
        {
            Columns.Add(new DynamicDataObject(_columnName, _columnValue.ToString(), _columnDataType));      
        }
        catch
        {            
        }
    }

    public string GetColumnValue(string _columnName, string _defaultValue = "")
    {
        string _retVal = _defaultValue;
        try
        {
            if (Columns.Where(p=>p.Name == _columnName).FirstOrDefault() != null)
            {
                _retVal = Columns.Where(p => p.Name == _columnName).FirstOrDefault().Value;
            }          
        }
        catch
        {            
        }
        return _retVal;
    }

    public string GetColumnValue(int _idx)
    {
        string _retVal = "";
        try
        {
            int _index = 0;
            foreach (DynamicDataObject _item in Columns)
            {
                if (_index == _idx)
                {
                    _retVal = _item.Value;
                }
                _index ++;
            }   
        }
        catch
        {           
        }
        return _retVal;
    }

    public int GetColumnValueAsInt(string _columnName, int _defaultValue = 0)
    {
        int _retVal = _defaultValue;
        try
        {
            if (Columns.Where(p => p.Name == _columnName).FirstOrDefault() != null)
            {
                _retVal = int.Parse(Columns.Where(p => p.Name == _columnName).FirstOrDefault().Value);
            }
        }
        catch
        {           
        }
        return _retVal;
    }

    public string GetColumnName(int _idx)
    {
        string _retVal = "";
        try
        {
            int _index = 0;
            foreach (DynamicDataObject _item in Columns)
            {
                if (_index == _idx)
                {
                    _retVal = _item.Name;
                }
                _index++;
            }   
        }
        catch
        {           
        }
        return _retVal;
    }
}

public class DynamicDataExport2CSV
{
    public static int Export(List<dynamic> objList, string _filename = "DataExport")
    {
        int _retVal = 0;
        try
        {
            if (objList != null && objList.Count > 0)
            {
                SendHttpContextHeaderInfo(_filename);
                WriteRowData(objList[0], true);

                foreach (DynamicDataRow obj in objList)
                {
                    WriteRowData(obj);
                }
                HttpContext.Current.Response.End();
                _retVal = objList.Count;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return _retVal;
    }

    private static void WriteRowData(dynamic obj, bool _columnNames = false)
    {
        StringBuilder _data = new StringBuilder();
        foreach (DynamicDataObject _column in obj.Columns)
        {
            if (_columnNames)
                AddComma(_column.Name, _data);
            else
                AddComma(_column.Value, _data);
        }
        _data = RemoveLastComma(_data);
        HttpContext.Current.Response.Write(_data.ToString());
        HttpContext.Current.Response.Write(Environment.NewLine);
    }

    private static void AddComma(object value, StringBuilder stringBuilder)
    {
        try
        {
            if (value == null)
            {
                stringBuilder.Append(", ");
            }
            else
            {
                var _data = value;
                string _value = _data.ToString().Replace("\r\n", " ");
                stringBuilder.Append(string.Format("{0}, ", _value.Replace(",", " ")));
            }
        }
        catch
        {
            stringBuilder.Append(", ");
        }
    }

    private static StringBuilder RemoveLastComma(StringBuilder stringBuilder)
    {
        string _line = stringBuilder.ToString();
        int idx = _line.LastIndexOf(',');
        return new StringBuilder(_line.Remove(idx));
    }

    private static void SendHttpContextHeaderInfo(string _filename)
    {
        string attachment = string.Format("attachment; filename={0}.csv", _filename);
        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.ClearHeaders();
        HttpContext.Current.Response.ClearContent();
        HttpContext.Current.Response.AddHeader("content-disposition", attachment);
        HttpContext.Current.Response.ContentType = "text/csv";
        HttpContext.Current.Response.AddHeader("Pragma", "public");
    }    
}
