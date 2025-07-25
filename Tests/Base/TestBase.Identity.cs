﻿using System;

using LinqToDB;
using LinqToDB.Data;

namespace Tests
{
	// contains methods to reset identity field counter for pre-filled tables with identity used in tests:
	// - Person
	// - AllTypes
	// This allows tests with identity tables generate stable baselines even if they executed out-of-order.
	// We intentionally don't remove extra records and allow it to fail in that cases to allow us detect
	// tests that forget to cleanup and leave junk records in those tables
	public partial class TestBase
	{
		protected void ResetPersonIdentity(string context)
		{
			using var _ = new DisableBaseline("Test support code");

			var provider = GetProviderName(context, out var _);

			var lastValue = 4;

			string[]? sql = CustomizationSupport.Interceptor.InterceptResetPersonIdentity(context, lastValue);

			if (sql == null)
			{
				switch (provider)
				{
					case string prov when prov.IsAnyOf(TestProvName.AllAccess):
						sql = new[]
						{
						"ALTER TABLE Doctor DROP CONSTRAINT PersonDoctor",
						"ALTER TABLE Patient DROP CONSTRAINT PersonPatient",
						$"ALTER TABLE Person ALTER COLUMN PersonID COUNTER({lastValue + 1}, 1)",
						"ALTER TABLE Doctor ADD CONSTRAINT PersonDoctor FOREIGN KEY (PersonID) REFERENCES Person ON UPDATE CASCADE ON DELETE CASCADE",
						"ALTER TABLE Patient ADD CONSTRAINT PersonPatient FOREIGN KEY (PersonID) REFERENCES Person ON UPDATE CASCADE ON DELETE CASCADE",
					};
						break;
					case string prov when prov.IsAnyOf(ProviderName.DB2):
						sql = new[] { $"ALTER TABLE \"Person\" ALTER COLUMN \"PersonID\" RESTART WITH {lastValue + 1}" };
						break;
					case string prov when prov.IsAnyOf(TestProvName.AllFirebird):
						sql = new[] { $"SET GENERATOR \"PersonID\" TO {lastValue}" };
						break;
					case string prov when prov.IsAnyOf(TestProvName.AllInformix):
						sql = new[]
						{
						// reset serial to 0 (without it MODIFY will not work as it cannot decrease SERIAL value)
						"INSERT INTO Person (PersonID, FirstName, LastName, MiddleName, Gender) VALUES (2147483647, '1', '2', '3', 'M')",
						// cleanup
						$"DELETE FROM Person WHERE PersonID > {lastValue}",
						// reset serial to next value
						$"ALTER TABLE Person MODIFY (PersonID SERIAL({lastValue + 1}))",
						// MODIFY erase all PK/FK constraints for modified column
						$"ALTER TABLE Person ADD CONSTRAINT PRIMARY KEY (PersonID)",
						$"ALTER TABLE Patient ADD CONSTRAINT(FOREIGN KEY (PersonID) REFERENCES Person (PersonID))",
						$"ALTER TABLE Doctor ADD CONSTRAINT(FOREIGN KEY (PersonID) REFERENCES Person (PersonID))",

					};
						break;
					case string prov when prov.IsAnyOf(TestProvName.AllMySql):
						sql = new[] { $"ALTER TABLE Person AUTO_INCREMENT = {lastValue + 1}" };
						break;
					case string prov when prov.IsAnyOf(TestProvName.AllOracle):
						sql = new[] {
						$"DROP SEQUENCE \"PersonSeq\"",
						$"CREATE SEQUENCE \"PersonSeq\" MINVALUE 1 START WITH {lastValue + 1}"
					};
						break;
					case string prov when prov.IsAnyOf(TestProvName.AllPostgreSQL):
						sql = new[] { $"ALTER SEQUENCE \"Person_PersonID_seq\" RESTART WITH {lastValue + 1}" };
						break;
					case string prov when prov.IsAnyOf(TestProvName.AllSapHana):
						// SAP HANA doesn't allow identity management at all
						sql = new[]
						{
						"ALTER TABLE \"Doctor\" DROP CONSTRAINT \"FK_Doctor_Person\"",
						"ALTER TABLE \"Patient\" DROP CONSTRAINT \"FK_Patient_Person\"",
						"RENAME TABLE \"Person\" TO \"Person_OLD\"",
						@"
CREATE COLUMN TABLE ""Person"" (
	""PersonID"" INTEGER CS_INT NOT NULL GENERATED BY DEFAULT AS IDENTITY,
	""FirstName"" NVARCHAR(50) NOT NULL ,
	""LastName"" NVARCHAR(50) NOT NULL ,
	""MiddleName"" NVARCHAR(50) NULL ,
	""Gender"" CHAR(1) NOT NULL ,
	 PRIMARY KEY (""PersonID"")
)",
						"INSERT INTO \"Person\" SELECT \"PersonID\", \"FirstName\", \"LastName\", \"MiddleName\", \"Gender\" FROM \"Person_OLD\"",
						"DROP TABLE \"Person_OLD\"",
						"ALTER TABLE \"Doctor\" ADD CONSTRAINT \"FK_Doctor_Person\" FOREIGN KEY (\"PersonID\") REFERENCES \"Person\" (\"PersonID\") ON UPDATE CASCADE ON DELETE CASCADE",
						"ALTER TABLE \"Patient\" ADD CONSTRAINT \"FK_Patient_Person\" FOREIGN KEY (\"PersonID\") REFERENCES \"Person\" (\"PersonID\") ON UPDATE CASCADE ON DELETE CASCADE"
					};
						break;
					case string prov when prov.IsAnyOf(TestProvName.AllSqlServer):
						sql = new[] { $"DBCC CHECKIDENT ('Person', RESEED, {lastValue})" };
						break;
					case string prov when prov.IsAnyOf(ProviderName.SqlCe):
						sql = new[] { $"ALTER TABLE Person ALTER COLUMN PersonID IDENTITY({lastValue + 1},1)" };
						break;
					case string prov when prov.IsAnyOf(TestProvName.AllSybase):
						sql = new[] { $"sp_chgattribute Person, 'identity_burn_max', 0, '{lastValue}'" };
						break;
					case string prov when prov.IsAnyOf(TestProvName.AllSQLite):
						// specify schema explicitly because after temp table creation (Issue4671Test)
						// default schema for this table changes to temp on windows (sqlite bug?)
						sql = new[] { $"UPDATE main.sqlite_sequence SET seq = {lastValue} WHERE name = 'Person'" };
						break;
					default:
#pragma warning disable RS0030 // Do not use banned APIs
						Console.WriteLine($"Unknown provider: {provider}");
#pragma warning restore RS0030 // Do not use banned APIs
						break;
				}
			}

			if (sql != null)
			{
				using (var db = new DataConnection(provider))
				{
					foreach (var query in sql)
						db.Execute(query);
				}
			}
		}

		protected void ResetAllTypesIdentity(string context)
		{
			using var _ = new DisableBaseline("Test support code");

			var provider = GetProviderName(context, out var _);

			var lastValue = 2;
			var keepIdentityLastValue = 0;

			string[]? sql = CustomizationSupport.Interceptor.InterceptResetAllTypesIdentity(context, lastValue, keepIdentityLastValue);

			if (sql == null)
			{
				switch (provider)
				{
					case string prov when prov.IsAnyOf(TestProvName.AllAccess):
						sql = new[]
						{
						$"ALTER TABLE AllTypes ALTER COLUMN ID COUNTER({lastValue + 1}, 1)",
					};
						break;
					case string prov when prov.IsAnyOf(ProviderName.DB2):
						sql = new[]
						{
						$"ALTER TABLE AllTypes ALTER COLUMN ID RESTART WITH {lastValue + 1}",
						$"ALTER TABLE \"KeepIdentityTest\" ALTER COLUMN \"ID\" RESTART WITH {keepIdentityLastValue + 1}",
						};
						break;
					case string prov when prov.IsAnyOf(TestProvName.AllFirebird):
						sql = new[] { $"SET GENERATOR \"AllTypesID\" TO {lastValue}" };
						break;
					case string prov when prov.IsAnyOf(TestProvName.AllInformix):
						sql = new[]
						{
						// reset serial to 0 (without it MODIFY will not work as it cannot decrease SERIAL value)
						"INSERT INTO AllTypes (ID) VALUES (2147483647)",
						// cleanup
						$"DELETE FROM AllTypes WHERE ID > {lastValue}",
						// reset serial to next value
						$"ALTER TABLE AllTypes MODIFY (ID SERIAL({lastValue + 1}))",
						// MODIFY erase all PK/FK constraints for modified column
						$"ALTER TABLE AllTypes ADD CONSTRAINT PRIMARY KEY (ID)",
					};
						break;
					case string prov when prov.IsAnyOf(TestProvName.AllMySql):
						sql = new[] { $"ALTER TABLE `AllTypes` AUTO_INCREMENT = {lastValue + 1}" };
						break;
					case string prov when prov.IsAnyOf(TestProvName.AllOracle):
						sql = new[] {
						$"DROP SEQUENCE \"AllTypesSeq\"",
						$"CREATE SEQUENCE \"AllTypesSeq\" MINVALUE 1 START WITH {lastValue + 1}"
					};
						break;
					case string prov when prov.IsAnyOf(TestProvName.AllPostgreSQL):
						sql = new[] { $"ALTER SEQUENCE \"AllTypes_ID_seq\" RESTART WITH {lastValue + 1}" };
						break;
					case string prov when prov.IsAnyOf(TestProvName.AllSapHana):
						// SAP HANA doesn't allow identity management at all
						sql = new[]
						{
						"RENAME TABLE \"AllTypes\" TO \"AllTypes_OLD\"",
						@"
CREATE COLUMN TABLE ""AllTypes""
(
	ID INTEGER CS_INT NOT NULL GENERATED BY DEFAULT AS IDENTITY,
	""bigintDataType"" BIGINT NULL,
	""smallintDataType"" SMALLINT NULL,
	""decimalDataType"" DECIMAL NULL,
	""smalldecimalDataType"" SMALLDECIMAL NULL,
	""intDataType"" INTEGER NULL,
	""tinyintDataType"" TINYINT NULL,
	""floatDataType"" FLOAT NULL,
	""realDataType"" REAL NULL,

	""dateDataType"" DATE NULL,
	""timeDataType"" TIME NULL,
	""seconddateDataType"" SECONDDATE NULL,
	""timestampDataType"" TIMESTAMP NULL,

	""charDataType"" CHAR(1) NULL,
	""char20DataType"" CHAR(20) NULL,
	""varcharDataType"" VARCHAR(20) NULL,
	""textDataType"" TEXT NULL,
	""shorttextDataType"" SHORTTEXT(20) NULL,
	""ncharDataType"" NCHAR(1) NULL,
	""nchar20DataType"" NCHAR(20) NULL,
	""nvarcharDataType"" NVARCHAR(20) NULL,
	""alphanumDataType"" ALPHANUM(20) NULL,

	""binaryDataType"" BINARY(10) NULL,
	""varbinaryDataType"" VARBINARY(10) NULL,

	""blobDataType"" BLOB NULL,
	""clobDataType"" CLOB NULL,
	""nclobDataType"" NCLOB NULL,
	PRIMARY KEY (""ID"")
)",
						"INSERT INTO \"AllTypes\" SELECT ID, \"bigintDataType\", \"smallintDataType\", \"decimalDataType\", \"smalldecimalDataType\", \"intDataType\", \"tinyintDataType\", \"floatDataType\", \"realDataType\", \"dateDataType\", \"timeDataType\", \"seconddateDataType\", \"timestampDataType\", \"charDataType\", \"char20DataType\", \"varcharDataType\", \"textDataType\", \"shorttextDataType\", \"ncharDataType\", \"nchar20DataType\", \"nvarcharDataType\", \"alphanumDataType\", \"binaryDataType\", \"varbinaryDataType\", \"blobDataType\", \"clobDataType\", \"nclobDataType\" FROM \"AllTypes_OLD\"",
						"DROP TABLE \"AllTypes_OLD\"",
					};
						break;
					case string prov when prov.IsAnyOf(TestProvName.AllSqlServer):
						sql = new[] { $"DBCC CHECKIDENT ('AllTypes', RESEED, {lastValue})" };
						break;
					case string prov when prov.IsAnyOf(ProviderName.SqlCe):
						sql = new[] { $"ALTER TABLE AllTypes ALTER COLUMN ID IDENTITY({lastValue + 1},1)" };
						break;
					case string prov when prov.IsAnyOf(TestProvName.AllSybase):
						sql = new[]
						{
							$"sp_chgattribute AllTypes, 'identity_burn_max', 0, '{lastValue}'",
							$"sp_chgattribute KeepIdentityTest, 'identity_burn_max', 0, '{keepIdentityLastValue}'"
						};
						break;
					case string prov when prov.IsAnyOf(TestProvName.AllSQLite):
						// specify schema explicitly because after temp table creation (Issue4671Test)
						// default schema for this table changes to temp on windows (sqlite bug?)
						sql = new[] { $"UPDATE main.sqlite_sequence SET seq = {lastValue} WHERE name = 'AllTypes'" };
						break;
				}
			}

			if (sql != null)
			{
				using (var db = new DataConnection(provider))
				{
					foreach (var query in sql)
						db.Execute(query);
				}
			}
		}

		protected void ResetTestSequence(string context)
		{
			using var _ = new DisableBaseline("Test support code");

			var provider = GetProviderName(context, out var _);

			var lastValue = 0;

			string[]? sql = CustomizationSupport.Interceptor.InterceptResetTestSequence(context, lastValue);

			if (sql == null)
			{
				switch (provider)
				{
					case string prov when prov.IsAnyOf(TestProvName.AllPostgreSQL):
						sql = new[] { $"ALTER SEQUENCE sequencetestseq RESTART WITH {lastValue + 1}" };
						break;
				}
			}

			if (sql != null)
			{
				using (var db = new DataConnection(provider))
				{
					foreach (var query in sql)
						db.Execute(query);
				}
			}
		}
	}
}
