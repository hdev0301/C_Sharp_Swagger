using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Siemplify.Common;

namespace Siemplify.Server.WebApi.Infrastructure
{
	public class ObjectIdentity

	{
		#region Methods

		#endregion 

		#region Methods

		public static ObjectIdentity Parse(string fullName)
		{
			Guard.NotNullOrEmpty(()=>fullName, fullName);
			var tmp = fullName.Split(new[] {"\\"}, StringSplitOptions.RemoveEmptyEntries);

			if (tmp.Length == 1)
			{
				return new ObjectIdentity()
				{
					Name=tmp[0]
				};
			}

			if (tmp.Length == 2)
			{
				return new ObjectIdentity()
				{
					DomainName = tmp[0],
					Name = tmp[1]
				};
			}

			throw new FormatException(fullName + " is invalid value for ActiveDirectoryObject. Format should be <DOMAIN_NAME>\\<NAME> or <NAME>");
		}

		#endregion 

		#region Methods

		public override string ToString()
		{
			if (string.IsNullOrEmpty(DomainName))
			{
				return Name; 
			}

			return DomainName + "\\" + Name; 
		}

		#endregion 

		#region Properties

		public string DomainName { get; set;  }
		public string Name { get; set; }

		#endregion 
	}
}