using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.OAuth;
using Siemplify.Common;
using Siemplify.Common.ExtensionMethods;
using Siemplify.Common.IoC;
using Siemplify.DataModel.Settings;
using Siemplify.DataModel.User;
using Siemplify.Server.Common.Services;
using Siemplify.Server.DataAccessors.Users;

namespace Siemplify.Server.WebApi.Authentication
{
    public class SiemplifyAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly UsersAccessor _userProfileAccessor = new UsersAccessor();
        private static readonly ISessionManagementService _sessionManagement = IoC.Resolve<ISessionManagementService>();
        private static readonly IConfigurationService _configurationService = IoC.Resolve<IConfigurationService>();
        private static readonly DomainDetails _domainDetails;
        static SiemplifyAuthorizeAttribute()
        {
            var moduleSettings = _configurationService.GetModuleSettings(DomainDetails.MOUDLE_NAME);
            _domainDetails = new DomainDetails(moduleSettings);
            var adCredentials = GetActiveDirectoryCredentials();
            try
            {
                if (_domainDetails.AdminDomainGroup.IsNotEmpty() &&
                    !UserAndDomainHelper.GroupExistsInDomain(_domainDetails.AdminDomainGroup, adCredentials))
                {
                    Logger.Instance.Warn(
                        string.Format("Configuration error: Admin group \"{0}\" not found in domain. Users might have a problem logging with Windows authentication",
                            _domainDetails.AdminDomainGroup), LoggerConsts.AccountGeneral);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Warn(
                    string.Format("Configuration error: Admin group \"{0}\" not found in domain. Users might have a problem logging with Windows authentication. Error: {1}",
                        _domainDetails.AdminDomainGroup, ex.Message), LoggerConsts.AccountGeneral);
            }

            try
            {
                if (_domainDetails.AnalystDomainGroup.IsNotEmpty() &&
                    !UserAndDomainHelper.GroupExistsInDomain(_domainDetails.AnalystDomainGroup, adCredentials))
                {
                    Logger.Instance.Warn(
                        string.Format("Configuration error: Analyst group \"{0}\" not found in domain. Users might have a problem logging with Windows authentication",
                            _domainDetails.AnalystDomainGroup), LoggerConsts.AccountGeneral);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Warn(
                    string.Format("Configuration error: Analyst group \"{0}\" not found in domain. Users might have a problem logging with Windows authentication. Error: {1}",
                        _domainDetails.AnalystDomainGroup, ex.Message), LoggerConsts.AccountGeneral);
            }
            
        }



        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            IEnumerable<string> values = new List<string>();
            var found = actionContext.Request.Headers.TryGetValues("MY_APP_KEY", out values);
            if (actionContext.RequestContext.Principal == null || actionContext.RequestContext.Principal.Identity == null)
                return false;

            var identity = actionContext.RequestContext.Principal.Identity;
            
            var isInternalAuth = identity.AuthenticationType == OAuthDefaults.AuthenticationType;
            var isAuthenticated = isInternalAuth 
                ? HandleInternalAuthentication(actionContext) 
                : HandleWindowsAuthentication(actionContext);

            // if user isn't authenticated, he isn't authorized.
            if (isAuthenticated == false)
                return false;

//            if (_configurationService.GetMainConfiguration().ManagementServer.IsUsingSession)
            {
                var hasActiveSession = _sessionManagement.HasActiveSession(identity.Name);


                if (!hasActiveSession)
                {
                    // For internal users, all authenticated calls must have an active session.
                    // Windows auth, an authenticated user must have an active session 
                    // OR must be calling WindowsLogin to create one.
                    if (isInternalAuth || actionContext.ActionDescriptor.ActionName != "WindowsLogin")
                    {
                        return false;
                    }
                }

                _sessionManagement.UpdateUserActivity(identity.Name);
            }

            return true;
        }

        private bool HandleWindowsAuthentication(HttpActionContext actionContext)
        {
            var mgmtConfig = _configurationService.GetManagementServerConfiguration();
            var windowsPrincipal = (WindowsPrincipal) actionContext.RequestContext.Principal;

            UserRoleEnum? roleToAssign = null;
            if (windowsPrincipal.IsInRole(_domainDetails.AdminDomainGroup))
            {
                roleToAssign = UserRoleEnum.Admin;
            }
            else if (windowsPrincipal.IsInRole(_domainDetails.AnalystDomainGroup))
            {
                roleToAssign = UserRoleEnum.Analyst;
            }

            if (roleToAssign == null)
            {
                Logger.Instance.Warn(string.Format("Blocked connection attempt by Windows account {0} not in Admin or Analyst group.", windowsPrincipal.Identity.Name),
                    LoggerConsts.AccountLogInError);
                return false;
            }

            var profile = _userProfileAccessor.GetUserProfile(windowsPrincipal.Identity.GetUserName());

            if (profile == null)
            {
                if (!mgmtConfig.AutoCreateUsers)
                {
                    Logger.Instance.Warn(string.Format("Windows account {0} is authorized but does not have profile.", windowsPrincipal.Identity.Name),
                        LoggerConsts.AccountLogInError);
                    return false;
                }

                var userDetails = UserAndDomainHelper.GetUserPrincipal(windowsPrincipal.Identity.GetUserName(), GetActiveDirectoryCredentials());
                var user = new UserProfile
                {
                    FirstName = userDetails.GivenName,
                    LastName = userDetails.Surname,
                    UserName = windowsPrincipal.Identity.Name,
                    Email = userDetails.EmailAddress,
                    Role = roleToAssign.Value,
                    UserType = UserType.Windows,
                    ImageBase64 = null
                };

                _userProfileAccessor.AddOrUpdateUserProfile(user);
            }
            else
            {
                if (profile.Role != roleToAssign.Value)
                {
                    profile.Role = roleToAssign.Value;
                    _userProfileAccessor.AddOrUpdateUserProfile(profile);
                }

                if (profile.IsDisabled)
                {
                    Logger.Instance.Debug(string.Format("Blocked login attempt by disabled user {0}", profile.UserName), LoggerConsts.AccountLogInError);
                    return false;
                }
            }
            return true;
        }

        private bool HandleInternalAuthentication(HttpActionContext actionContext)
        {
            var isAuthorized = base.IsAuthorized(actionContext);
            if (!isAuthorized)
                Logger.Instance.Debug(string.Format("Internal user {0} not authorized using built-in Bearer auth mechanism.", actionContext.RequestContext.Principal.Identity.GetUserName()), LoggerConsts.AccountLogInError);
            return isAuthorized;
        }

        internal static NetworkCredential GetActiveDirectoryCredentials()
        {
            var moduleSettings = _configurationService.GetModuleSettings(DomainDetails.MOUDLE_NAME);
            
            if (moduleSettings == null || moduleSettings .Count == 0)
                return null;
            var domainDetails = new DomainDetails(moduleSettings);
            if (!domainDetails.IsValid())
            {
                return null;
            }
            return new NetworkCredential(domainDetails.DomainName + "\\" + domainDetails.Username, domainDetails.Password);
        }
    }
}