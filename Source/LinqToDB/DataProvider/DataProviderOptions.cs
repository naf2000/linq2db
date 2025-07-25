﻿using System.Collections.Generic;

using LinqToDB.Common;
using LinqToDB.Common.Internal;
using LinqToDB.Data;

namespace LinqToDB.DataProvider
{
	/// <param name="BulkCopyType">
	/// Default bulk copy mode, used by <see cref="DataConnectionExtensions.BulkCopy{T}(DataConnection, IEnumerable{T})"/>
	/// methods, if mode is not specified explicitly.
	/// </param>
	public abstract record DataProviderOptions<T>
	(
		BulkCopyType BulkCopyType
	)
		: IOptionSet
		where T : DataProviderOptions<T>, new()
	{
		protected DataProviderOptions() : this(BulkCopyType.Default)
		{
		}

		protected DataProviderOptions(DataProviderOptions<T> original)
		{
			BulkCopyType = original.BulkCopyType;
		}

		int? _configurationID;
		int IConfigurationID.ConfigurationID
		{
			get
			{
				if (_configurationID == null)
				{
					using var idBuilder = new IdentifierBuilder();
					_configurationID = CreateID(idBuilder.Add(BulkCopyType)).CreateID();
				}

				return _configurationID.Value;
			}
		}

		protected abstract IdentifierBuilder CreateID(IdentifierBuilder builder);

		#region Default Options

		static T _default = new();

		/// <summary>
		/// Gets default <see cref="DataProviderOptions{T}"/> instance.
		/// </summary>
		public static T Default
		{
			get => _default;
			set
			{
				_default = value;
				DataConnection.ResetDefaultOptions();
				DataConnection.ConnectionOptionsByConfigurationString.Clear();
			}
		}

		/// <inheritdoc />
		IOptionSet IOptionSet.Default => Default;

		#endregion
	}
}
