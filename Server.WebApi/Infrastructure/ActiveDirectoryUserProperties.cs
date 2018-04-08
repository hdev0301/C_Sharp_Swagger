using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Siemplify.Server.WebApi.Infrastructure
{
	public static class ActiveDirectoryUserProperties
	{
		public const string DOMAIN = "dc";
		public const string FIRST_NAME = "givenName";
		public const string LAST_NAME = "sn";
		public const string MEMBER_OF = "memberOf";
		public const string FULL_NAME = "name";
		public const string THUMBNAIL = "thumbnailPhoto";
		public const string USER_NAME = "sAMAccountName"; 
	}
}