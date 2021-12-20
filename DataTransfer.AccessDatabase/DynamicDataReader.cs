using System;
using System.Data;
using System.Dynamic;
using DataTransfer.AccessDatabase.Utils;

/// <summary>
/// This class provides an easy way to use object.property
/// syntax with a DataReader by wrapping a DataReader into
/// a dynamic object.
/// 
/// The class also automatically fixes up DbNull values
/// (null into .NET and DbNUll)
/// </summary>
// ReSharper disable once CheckNamespace
public class DynamicDataReader : DynamicObject
{
    /// <summary>
    /// Cached Instance of DataReader passed in
    /// </summary>
    private readonly IDataReader _dataReader;

    /// <summary>
    /// Pass in a loaded DataReader
    /// </summary>
    /// <param name="dataReader">DataReader instance to work off</param>
    public DynamicDataReader(IDataReader dataReader)
    {
        _dataReader = dataReader;
    }

    /// <summary>
    /// Returns a value from the current DataReader record
    /// If the field doesn't exist null is returned.
    /// DbNull values are turned into .NET nulls.
    /// </summary>
    /// <param name="binder"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        result = null;

        // 'Implement' common reader properties directly
        if (binder.Name == "IsClosed")
            result = _dataReader.IsClosed;
        else if (binder.Name == "RecordsAffected")
            result = _dataReader.RecordsAffected;
        // lookup column names as fields
        else
        {
            try
            {
                result = _dataReader[binder.Name];
                if (result == DBNull.Value)
                    result = null;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        return true;
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
    {
        // Implement most commonly used method
        if (binder.Name == "Read")
            result = _dataReader.Read();
        else if (binder.Name == "Close")
        {
            _dataReader.Close();
            result = null;
        }
        else
            // call other DataReader methods using Reflection (slow - not recommended)
            // recommend you use full DataReader instance
            result = ReflectionUtils.CallMethod(_dataReader, binder.Name, args);

        return true;
    }
}