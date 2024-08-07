// ---------------------------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by LinqToDB scaffolding tool (https://github.com/linq2db/linq2db).
// Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
// ---------------------------------------------------------------------------------------------------

using LinqToDB;
using LinqToDB.Mapping;
using LinqToDB.Tools.Comparers;
using System;
using System.Collections.Generic;

#pragma warning disable 1573, 1591
#nullable enable

namespace Cli.All.MySql
{
	[Table("TestMerge1")]
	public class TestMerge1 : IEquatable<TestMerge1>
	{
		[Column("Id"             , DataType = DataType.Int32    , DbType = "int"           , Precision = 10, Scale = 0 , IsPrimaryKey = true)] public int       Id              { get; set; } // int
		[Column("Field1"         , DataType = DataType.Int32    , DbType = "int"           , Precision = 10, Scale = 0                      )] public int?      Field1          { get; set; } // int
		[Column("Field2"         , DataType = DataType.Int32    , DbType = "int"           , Precision = 10, Scale = 0                      )] public int?      Field2          { get; set; } // int
		[Column("Field3"         , DataType = DataType.Int32    , DbType = "int"           , Precision = 10, Scale = 0                      )] public int?      Field3          { get; set; } // int
		[Column("Field4"         , DataType = DataType.Int32    , DbType = "int"           , Precision = 10, Scale = 0                      )] public int?      Field4          { get; set; } // int
		[Column("Field5"         , DataType = DataType.Int32    , DbType = "int"           , Precision = 10, Scale = 0                      )] public int?      Field5          { get; set; } // int
		[Column("FieldInt64"     , DataType = DataType.Int64    , DbType = "bigint"        , Precision = 19, Scale = 0                      )] public long?     FieldInt64      { get; set; } // bigint
		[Column("FieldBoolean"   , DataType = DataType.BitArray , DbType = "bit(1)"        , Precision = 1                                  )] public bool?     FieldBoolean    { get; set; } // bit(1)
		[Column("FieldString"    , DataType = DataType.VarChar  , DbType = "varchar(20)"   , Length    = 20                                 )] public string?   FieldString     { get; set; } // varchar(20)
		[Column("FieldNString"   , DataType = DataType.VarChar  , DbType = "varchar(20)"   , Length    = 20                                 )] public string?   FieldNString    { get; set; } // varchar(20)
		[Column("FieldChar"      , DataType = DataType.Char     , DbType = "char(1)"       , Length    = 1                                  )] public char?     FieldChar       { get; set; } // char(1)
		[Column("FieldNChar"     , DataType = DataType.Char     , DbType = "char(1)"       , Length    = 1                                  )] public char?     FieldNChar      { get; set; } // char(1)
		[Column("FieldFloat"     , DataType = DataType.Single   , DbType = "float"         , Precision = 12                                 )] public float?    FieldFloat      { get; set; } // float
		[Column("FieldDouble"    , DataType = DataType.Double   , DbType = "double"        , Precision = 22                                 )] public double?   FieldDouble     { get; set; } // double
		[Column("FieldDateTime"  , DataType = DataType.DateTime , DbType = "datetime(6)"                                                    )] public DateTime? FieldDateTime   { get; set; } // datetime(6)
		[Column("FieldBinary"    , DataType = DataType.VarBinary, DbType = "varbinary(20)" , Length    = 20                                 )] public byte[]?   FieldBinary     { get; set; } // varbinary(20)
		[Column("FieldGuid"      , DataType = DataType.Char     , DbType = "char(36)"      , Length    = 36                                 )] public string?   FieldGuid       { get; set; } // char(36)
		[Column("FieldDecimal"   , DataType = DataType.Decimal  , DbType = "decimal(24,10)", Precision = 24, Scale = 10                     )] public decimal?  FieldDecimal    { get; set; } // decimal(24,10)
		[Column("FieldDate"      , DataType = DataType.Date     , DbType = "date"                                                           )] public DateTime? FieldDate       { get; set; } // date
		[Column("FieldTime"      , DataType = DataType.Time     , DbType = "time"                                                           )] public TimeSpan? FieldTime       { get; set; } // time
		[Column("FieldEnumString", DataType = DataType.VarChar  , DbType = "varchar(20)"   , Length    = 20                                 )] public string?   FieldEnumString { get; set; } // varchar(20)
		[Column("FieldEnumNumber", DataType = DataType.Int32    , DbType = "int"           , Precision = 10, Scale = 0                      )] public int?      FieldEnumNumber { get; set; } // int

		#region IEquatable<T> support
		private static readonly IEqualityComparer<TestMerge1> _equalityComparer = ComparerBuilder.GetEqualityComparer<TestMerge1>(c => c.Id);

		public bool Equals(TestMerge1? other)
		{
			return _equalityComparer.Equals(this, other!);
		}

		public override int GetHashCode()
		{
			return _equalityComparer.GetHashCode(this);
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as TestMerge1);
		}
		#endregion
	}
}
