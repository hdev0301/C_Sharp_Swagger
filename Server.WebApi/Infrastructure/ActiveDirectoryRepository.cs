using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Web;
using Siemplify.Common;
using Siemplify.DataModel.User;

namespace Siemplify.Server.WebApi.Infrastructure
{
	public interface IActiveDirectoryRepository
	{
		ActiveDirectoryUser GetUser(string userName, string domainName = null);
		IEnumerable<ActiveDirectoryUser> GetUsersInGroup(string groupName, string domainName = null);
	}


	public class ActiveDirectoryRepository:IActiveDirectoryRepository
	{
		#region Helper Methods

		private string ParseRoleString(string roleString)
		{
			var parts = roleString.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

			const string groupPrefix = "CN=";
			var tmp = parts.FirstOrDefault(x => x.StartsWith(groupPrefix));
			if (string.IsNullOrEmpty(tmp))
			{
				return string.Empty;
			}

			var groupName = tmp.Remove(0, groupPrefix.Length);

			const string domainPrefix = "DC=";

			string domainName = string.Empty;
			tmp = parts.FirstOrDefault(x => x.StartsWith(domainPrefix));
			if (!string.IsNullOrEmpty(tmp))
			{
				domainName = tmp.Remove(0, domainPrefix.Length);
			}

			return string.IsNullOrEmpty(domainName) ? groupName : domainName + "\\" + groupName;
		}

		private string[] GetUserRoles(DirectoryEntry directoryEntry)
		{
			var memberOfObject = GetProperty<object>(directoryEntry, ActiveDirectoryUserProperties.MEMBER_OF);

			var rolesArray = memberOfObject as object[];
			if (rolesArray != null)
			{
				return rolesArray.Select(x => ParseRoleString(x.ToString())).ToArray();
			}

			var roleString = memberOfObject as string;
			if (roleString != null)
			{
				var roleName = ParseRoleString(roleString);
				return new[] { roleName };
			}

			return new string[0];
		}

		private ActiveDirectoryUser CreateActiveDirectoryUser(Principal principal)
		{
			var userEntry = principal.GetUnderlyingObject() as DirectoryEntry;
			Guard.NotNull(() => principal, principal);


			return new ActiveDirectoryUser()
			{
				DomainName = GetDomainName(userEntry),
				FirstName = GetProperty<string>(userEntry, ActiveDirectoryUserProperties.FIRST_NAME),
				LastName = GetProperty<string>(userEntry, ActiveDirectoryUserProperties.LAST_NAME),
				FullName = GetProperty<string>(userEntry, ActiveDirectoryUserProperties.FULL_NAME),
				ThumbnailImage = GetProperty<byte[]>(userEntry, ActiveDirectoryUserProperties.THUMBNAIL),
				UserName = GetProperty<string>(userEntry, ActiveDirectoryUserProperties.USER_NAME),
				Groups = GetUserRoles(userEntry)
			};

		}

		private PrincipalContext CreatePrincipalContext(string domainName)
		{
			if (string.IsNullOrEmpty(domainName))
			{
				return new PrincipalContext(ContextType.Domain);
			}

			return new PrincipalContext(ContextType.Domain, domainName);
		}

		private T GetProperty<T>(DirectoryEntry entry, string propertyName)
		{
			if (entry.Properties.Contains(propertyName))
			{
				return (T)entry.Properties[propertyName].Value;
			}

			return default(T);
		}

		private string GetDomainName(DirectoryEntry entry)
		{
			if (entry.Parent == null)
			{
				return null; 
			}

			if (entry.Parent.SchemaClassName =="domainDNS")
			{
				return (string)entry.Parent.Properties["name"].Value;
			}

			return GetDomainName(entry.Parent); 
		}

		#endregion

		#region IActiveDirectoryRepository Members

		public ActiveDirectoryUser GetUser(string userName, string domainName = null)
		{
			using (var context = CreatePrincipalContext(domainName))
			{
				var qbeUser = new UserPrincipal(context)
				{
					SamAccountName = userName
				};

				// create your principal searcher passing in the QBE principal    
				using (var searcher = new PrincipalSearcher(qbeUser))
				{
					var result = searcher.FindOne();

					if (result == null)
					{
						return null;
					}

					return CreateActiveDirectoryUser(result);
				}
			}
		}

		public IEnumerable<ActiveDirectoryUser> GetUsersInGroup(string groupName, string domainName = null)
		{
			using (var context = CreatePrincipalContext(domainName))
			{
				using (var group = GroupPrincipal.FindByIdentity(context, groupName))
				{
					if (group == null)
					{
						return new ActiveDirectoryUser[0];
					}

					var users = group.GetMembers(true);

					return users.Select(x => CreateActiveDirectoryUser(x)).ToArray();
				}
			}
		}

		#endregion
	}
}