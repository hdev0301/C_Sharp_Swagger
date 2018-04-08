using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Siemplify.DataModel.User;
using Siemplify.Server.Config.Service;

namespace Siemplify.Server.WebApi.Infrastructure
{
	public class ActiveDirectoryGroupMapper:IActiveDirectoryGroupMapper
	{
		private readonly ActiveDirectoryGroupMapping _adminMapping;
		private readonly ActiveDirectoryGroupMapping _analystMapping; 

		public ActiveDirectoryGroupMapper()
		{
			var config = SiemplifyConfigServiceBL.Instance.GetManagementServerConfiguration(); 

			_adminMapping=new ActiveDirectoryGroupMapping()
			{
				Group = ObjectIdentity.Parse(config.AdminGroupName), 
				AppRole = UserRoleEnum.Admin
			};

			_analystMapping = new ActiveDirectoryGroupMapping()
			{
				Group = ObjectIdentity.Parse(config.AnalystGroupName),
				AppRole = UserRoleEnum.Analyst
			};

		}

		public ActiveDirectoryGroupMapping AdminMapping 
		{
			get { return _adminMapping;  }
		}

		public ActiveDirectoryGroupMapping AnalystMaping 
		{
			get { return _analystMapping;  }
		}

	}
}