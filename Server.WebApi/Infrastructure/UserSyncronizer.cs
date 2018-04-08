using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Siemplify.Common;
using Siemplify.DataModel.User;
using Siemplify.Server.DataAccessors;
using Siemplify.Server.DataAccessors.ModelData;

namespace Siemplify.Server.WebApi.Infrastructure
{
	public interface IUserSyncronizer
	{
		void Syncronize(); 
	}

	public class UserSyncronizer:IUserSyncronizer
	{
		#region Constructors

		public UserSyncronizer(IUserService userService, IUnitOfWorkFactory unitOfWorkFactory, IActiveDirectoryGroupMapper groupMapper, IActiveDirectoryRepository activeDirectoryRepository)
		{
			Guard.NotNull(()=>userService, userService);
			Guard.NotNull(() => unitOfWorkFactory, unitOfWorkFactory);
			Guard.NotNull(()=>groupMapper, groupMapper);
			Guard.NotNull(()=>activeDirectoryRepository, activeDirectoryRepository);

			_groupMapper = groupMapper;
			_userService = userService; 
			_unitOfWorkFactory = unitOfWorkFactory;
			_activeDirectoryReposity = activeDirectoryRepository;
		}

		#endregion 

		#region Data Fields

		private readonly IActiveDirectoryGroupMapper _groupMapper; 
		private readonly IUserService _userService; 
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IActiveDirectoryRepository _activeDirectoryReposity; 

		#endregion

		#region Helper Methods

		private UserProfile ActiveDirectoryToDbUser(ActiveDirectoryUser activeDirectoryUser)
		{
			return new UserProfile()
			{
				FirstName = activeDirectoryUser.FirstName,
				LastName = activeDirectoryUser.LastName,
				Email = activeDirectoryUser.EMail,
				ImageBase64 = activeDirectoryUser.ThumbnailImage==null ? null:  Convert.ToBase64String(activeDirectoryUser.ThumbnailImage),
				UserName = activeDirectoryUser.DomainName+"\\"+ activeDirectoryUser.UserName,
				UserType = UserType.Windows
			};
		}

		private ComparisonResult<UserProfile> CompareUsers()
		{

			using (var unitOfWork = _unitOfWorkFactory.CreateAccountUnitOfWork())
			{
				var dbUsers = unitOfWork.UserProfileRepository.GetWindowsUsers().ToArray();
				var dbUserNames = dbUsers.Select(x => x.UserName).ToArray();

				var adminGroup = _groupMapper.AdminMapping.Group; 
				var administrators = _activeDirectoryReposity.GetUsersInGroup(adminGroup.Name, adminGroup.DomainName);

				var analystGroup = _groupMapper.AnalystMaping.Group; 
				var analysts = _activeDirectoryReposity.GetUsersInGroup(analystGroup.Name, analystGroup.DomainName);

				var activeDirectoryUsers = administrators.Union(analysts, new ActiveDirectoryUserDistinctEqualityComparer()).ToArray();
				var activeDirectoryUserNames = activeDirectoryUsers.Select(x => x.DomainName+"\\"+ x.UserName).ToArray();
				
				

				return new ComparisonResult<UserProfile>()
				{
					NewObjects = activeDirectoryUsers.Where(x => !dbUserNames.Contains(x.DomainName+"\\"+ x.UserName))
													 .Select(x =>
													 {
														 var user = ActiveDirectoryToDbUser(x);
														 if (administrators.Any(y => y.DomainName == x.DomainName && y.UserName == x.UserName))
														 {
															 user.Roll=UserRoleEnum.Admin;
														 }
														 else if (analysts.Any(y => y.DomainName == x.DomainName && y.UserName == x.UserName))
														 {
															 user.Roll = UserRoleEnum.Analyst;
														 }
														 else
														 {
															throw new UnauthorizedAccessException();		 
														 }

														 return user;
													 }).ToArray(),
					DeletedObjects = dbUsers.Where(x => !activeDirectoryUserNames.Contains(x.UserName))
				};
			}
		}

		private void SyncronizeUsers(ComparisonResult<UserProfile> comparisonResult)
		{
			using (var unitOfWork = _unitOfWorkFactory.CreateAccountUnitOfWork())
			{
				if (comparisonResult.NewObjects != null)
				{
					foreach (var newUser in comparisonResult.NewObjects)
					{
						_userService.AddUser(newUser);
					}
				}

				if (comparisonResult.DeletedObjects != null)
				{
					foreach (var removedUser in comparisonResult.DeletedObjects)
					{
						_userService.DeleteUser(removedUser.UserName);
					}
				}

				if (comparisonResult.ModifiedObject != null)
				{
					foreach (var modifiedUser in comparisonResult.ModifiedObject)
					{
						var dbUser = unitOfWork.UserProfileRepository.GetByID(modifiedUser.Id);

						dbUser.FirstName = modifiedUser.FirstName;
						dbUser.LastName = modifiedUser.LastName;
					}
					
					unitOfWork.SaveChanges(); 
				}
			}
		}

		#endregion 

		#region IUserSyncronizer Members

		public void Syncronize()
		{
			var comparisonResult = CompareUsers(); 
			SyncronizeUsers(comparisonResult);
		}

		#endregion
	}
}