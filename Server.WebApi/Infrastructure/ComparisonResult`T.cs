using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Siemplify.Server.WebApi.Infrastructure
{
	public class ComparisonResult<T>
	{
		public IEnumerable<T> NewObjects { get; set;  }
		public IEnumerable<T> DeletedObjects { get; set;  }
		public IEnumerable<T> ModifiedObject { get; set;  } 
	}
}