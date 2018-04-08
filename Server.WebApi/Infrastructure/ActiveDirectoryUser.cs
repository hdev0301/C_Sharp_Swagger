using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Siemplify.Server.WebApi.Infrastructure
{
	public class ActiveDirectoryUser
	{
		#region Properties

		public string DomainName { get; set; }
		public string UserName { get; set;  }
		public string EMail { get; set;  }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string FullName { get; set;  }
		public byte[] ThumbnailImage { get; set;  }
		public string[] Groups { get; set;  }

		#endregion 
	}
}