using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Security;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.Identity;
using Siemplify.Common;
using Siemplify.Server.Common;

namespace Siemplify.Server.WebApi.Infrastructure
{
	public class AuthenticationFilter: IAuthenticationFilter
	{
		#region Constructors

		public AuthenticationFilter(IActiveDirectoryGroupMapper activeDirectoryGroupMapper, IAuthenticationTicketProtector ticketProtector)
		{
			Guard.NotNull(()=>activeDirectoryGroupMapper, activeDirectoryGroupMapper);
			Guard.NotNull(()=>ticketProtector, ticketProtector);

			_activeDirectoryGroupMapper = activeDirectoryGroupMapper;
			_ticketProtector = ticketProtector;
		}

		#endregion 

		#region Data Fields

		private readonly IActiveDirectoryGroupMapper _activeDirectoryGroupMapper;
		private readonly IAuthenticationTicketProtector _ticketProtector; 

		#endregion 

		#region IAuthenticationFilter Members

		public bool AllowMultiple
		{
			get { return false; }
		}
		public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
		{
			var request = context.Request;

			//Process Authentication Token
			if (request.Headers.Contains(Consts.CUSTOM_AUTHENTICATION_HEADER))
			{
				IEnumerable<string> headers;
				request.Headers.TryGetValues(Consts.CUSTOM_AUTHENTICATION_HEADER, out headers);
				var ticket = _ticketProtector.Unprotect(headers.First().Remove(0, Consts.BEARER_TOKEN_PREFIX.Length));

				var roleClaim = ticket.Identity.Claims.FirstOrDefault(x => x.Type == ticket.Identity.RoleClaimType);
				var roles = roleClaim != null ? new[] { roleClaim.Value } : null;

				var identity = new ClaimsIdentity(ticket.Identity.Name);
				identity.AddClaims(ticket.Identity.Claims);
				var principal = new GenericPrincipal(identity, roles);

				context.Principal = principal;

			}
			//Process windows authentication
			else if (context.Principal.Identity.IsAuthenticated && context.Principal.Identity.AuthenticationType == "NTLM")
			{
				//var activeDirectoryRoles = Roles.GetRolesForUser(context.Principal.Identity.Name);
				//var userRoles = _activeDirectoryGroupMapper.ToAppRoles(activeDirectoryRoles).Select(x => x.ToString()).ToArray();

				//var identity = new GenericIdentity(context.Principal.Identity.Name);
				//userRoles.ForEach(x => identity.AddClaim(new Claim(ClaimTypes.Role, x)));
				//context.Principal = new GenericPrincipal(identity, userRoles);
			}
		}

		public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
		{
			//throw new NotImplementedException();
		}

		#endregion

		public void OnAuthenticate(HttpAuthenticationContext context)
		{
			var request = context.Request;

			//Process Authentication Token
			if (request.Headers.Contains(Consts.CUSTOM_AUTHENTICATION_HEADER))
			{
				IEnumerable<string> headers;
				request.Headers.TryGetValues(Consts.CUSTOM_AUTHENTICATION_HEADER, out headers);
				var ticket = _ticketProtector.Unprotect(headers.First().Remove(0, Consts.BEARER_TOKEN_PREFIX.Length));

				var roleClaim = ticket.Identity.Claims.FirstOrDefault(x => x.Type == ticket.Identity.RoleClaimType);
				var roles = roleClaim != null ? new[] { roleClaim.Value } : null;

				var identity = new ClaimsIdentity(ticket.Identity.Name);
				identity.AddClaims(ticket.Identity.Claims);
				var principal = new GenericPrincipal(identity, roles);

				context.Principal = principal;

			}
			//Process windows authentication
			//else if (context.Principal.Identity.IsAuthenticated && context.Principal.Identity.AuthenticationType == "NTLM")
			//{
			//	var activeDirectoryRoles = Roles.GetRolesForUser(context.Principal.Identity.Name);
			//	var userRoles = _activeDirectoryGroupMapper.ToAppRoles(activeDirectoryRoles).Select(x => x.ToString()).ToArray();

			//	var identity = new GenericIdentity(context.Principal.Identity.Name);
			//	userRoles.ForEach(x => identity.AddClaim(new Claim(ClaimTypes.Role, x)));
			//	context.Principal = new GenericPrincipal(identity, userRoles);
			//}
		}

		public void OnChallenge(HttpAuthenticationChallengeContext context)
		{
			
		}
	}

	public static class ExtensionMethods
	{
		public static IEnumerable<string> GetAllRoles(this ClaimsIdentity @this)
		{
			return @this.Claims
					   .Where(c => c.Type == ClaimTypes.Role)
					   .Select(c => c.Value)
					   .ToArray();
		}
	}
}