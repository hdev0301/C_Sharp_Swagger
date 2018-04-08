using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Siemplify.Common;
using Siemplify.DataModel.User;

namespace Siemplify.Server.WebApi.Infrastructure
{
	public class ActiveDirectoryGroupMapping
	{
		#region Constructors

		public ActiveDirectoryGroupMapping()
		{
		}

		#endregion 

		#region Methods

		public override string ToString()
		{
			return string.Format("Group: {0}, AppRole: {1}", Group, AppRole);
		}

		#endregion 

		#region Properties

		public ObjectIdentity Group { get; set;  }
		public UserRoleEnum AppRole { get; set; }

		#endregion 
	}
}