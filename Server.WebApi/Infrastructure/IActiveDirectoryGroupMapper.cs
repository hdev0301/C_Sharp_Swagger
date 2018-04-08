using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Siemplify.DataModel.User;

namespace Siemplify.Server.WebApi.Infrastructure
{
	public interface IActiveDirectoryGroupMapper
	{
		ActiveDirectoryGroupMapping AdminMapping { get;  }
		ActiveDirectoryGroupMapping AnalystMaping { get;  }
	}
}