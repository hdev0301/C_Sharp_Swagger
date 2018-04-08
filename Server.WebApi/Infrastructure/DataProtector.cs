using Microsoft.Owin.Security;
using System;
using Microsoft.Owin.Security.OAuth;
using Siemplify.Common;

namespace Siemplify.Server.WebApi.Infrastructure
{
	public interface IAuthenticationTicketProtector : ISecureDataFormat<AuthenticationTicket>
	{
	}

	public class AuthenticationTicketProtector : IAuthenticationTicketProtector
	{
		#region Constructors

		public AuthenticationTicketProtector(OAuthBearerAuthenticationOptions oauthOptions)
		{
			Guard.NotNull(() => oauthOptions, oauthOptions);

			_oauthOptions = oauthOptions; 
		}

		#endregion Constructors

		#region Data Fields

		private readonly OAuthBearerAuthenticationOptions _oauthOptions;

		#endregion 


		#region IAuthenticationTicketProtector Members

		public string Protect(AuthenticationTicket data)
		{
			return _oauthOptions.AccessTokenFormat.Protect(data); 
		}

		public AuthenticationTicket Unprotect(string protectedText)
		{
			return _oauthOptions.AccessTokenFormat.Unprotect(protectedText); 
		}

		#endregion IAuthenticationTicketProtector Members
	}
}