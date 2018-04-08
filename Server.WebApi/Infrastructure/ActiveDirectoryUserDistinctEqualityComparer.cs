using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Siemplify.Server.WebApi.Infrastructure
{
	public class ActiveDirectoryUserDistinctEqualityComparer:IEqualityComparer<ActiveDirectoryUser>
	{
		public bool Equals(ActiveDirectoryUser x, ActiveDirectoryUser y)
		{
			if (ReferenceEquals(x, y)) return true;

			if (x == null || y == null)
			{
				return false;
			}

			return x.DomainName == y.DomainName && x.UserName == y.UserName;
		}

		public int GetHashCode(ActiveDirectoryUser obj)
		{
			if (ReferenceEquals(obj, null)) return 0;

			if (string.IsNullOrEmpty(obj.DomainName))
			{
				return obj.UserName.GetHashCode();
			}

			return obj.DomainName.GetHashCode() ^ obj.UserName.GetHashCode();
		}
	}
}