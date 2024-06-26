using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqToDB.Mapping
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class SetterTypeAttribute : MappingAttribute
	{
		public Type? Type { get; set; }
		public override string GetObjectID()
		{
			return Type?.ToString() ?? string.Empty;
		}
	}
}
