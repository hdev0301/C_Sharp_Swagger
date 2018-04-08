using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Transactions;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Siemplify.Common;
using Siemplify.DataModel.User;
using Siemplify.Server.DataAccessors;
using Siemplify.Server.DataAccessors.CaseActivities;
using Siemplify.Server.DataAccessors.ModelData;

namespace Siemplify.Server.WebApi.Infrastructure
{
	public interface IUserService
	{ 
		void AddUser(UserProfile user, string password=null);
		void DeleteUser(string userName);
		IEnumerable<string> GetWindowsUserRoles(string userName);
		void ChangePassword(string userName, string password);
	}

	public class UserService:IUserService
	{
		#region Constructors

		public UserService(IUnitOfWorkFactory unitOfWorkFactory, IActiveDirectoryRepository activeDirectoryRepository)
		{
			Guard.NotNull(() => unitOfWorkFactory, unitOfWorkFactory);
			Guard.NotNull(()=>activeDirectoryRepository, activeDirectoryRepository);

			_activeDirectoryRepository = activeDirectoryRepository;
			_unitOfWorkFactory = unitOfWorkFactory;
		}

		#endregion 

		#region Data Fields

		private readonly IActiveDirectoryRepository _activeDirectoryRepository;
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;

		#endregion 

		#region Helper Methods

		private UserStore<IdentityUser> CreateUserStore(DbContext dbContext)
		{
			return new UserStore<IdentityUser>(dbContext);
		}
		
		private UserManager<IdentityUser> CreateUserManager(UserStore<IdentityUser> userStore)
		{
			var userManager = new UserManager<IdentityUser>(userStore);

			var validator = new UserValidator<IdentityUser>(userManager);
			validator.AllowOnlyAlphanumericUserNames = false;
			userManager.UserValidator = validator;

			return userManager;
		}

		private UserManager<IdentityUser> CreateUserManager(DbContext dbContext)
		{
			var userStore = CreateUserStore(dbContext); 
			return CreateUserManager(userStore); 
		}

		#endregion 

		#region IUserService Members

		public IEnumerable<string> GetWindowsUserRoles(string userName)
		{
			var identity= ObjectIdentity.Parse(userName);

			var user = _activeDirectoryRepository.GetUser(identity.Name, identity.DomainName);
			return (user == null) ? new string[0] : user.Groups; 
		}

		public void ChangePassword(string userName, string password)
		{
			using (var unitOfWork = _unitOfWorkFactory.CreateAccountUnitOfWork())
			{
				var userStore = CreateUserStore(unitOfWork.CurrentContext); 
				var userManager = CreateUserManager(userStore);
				
				var user = userManager.FindByName(userName);
				var hashedNewPassword = userManager.PasswordHasher.HashPassword(password);

				userStore.SetPasswordHashAsync(user, hashedNewPassword)
					.ContinueWith((t) => userStore.UpdateAsync(user)).Wait();
			}
		}

		public void AddUser(UserProfile user, string password=null)
		{
			var login = new IdentityUser
			{
				UserName = user.UserName,
			};

			using (var unitOfWork = _unitOfWorkFactory.CreateAccountUnitOfWork())
			{
				using (var transaction = unitOfWork.BeginTransaction())
				{
					try
					{
						var userManager = CreateUserManager(unitOfWork.CurrentContext);
						var registerResult = (string.IsNullOrEmpty(password))?  userManager.Create(login): userManager.Create(login, password);
						
						if (!registerResult.Succeeded)
						{
							transaction.Rollback();

							string errorMessage=string.Empty;

							foreach (var error in registerResult.Errors)
							{
								errorMessage += error + "\n";
							}

							throw new ApplicationException(errorMessage);
						}

						unitOfWork.UserProfileRepository.Add(user);
						unitOfWork.SaveChanges();	

						transaction.Commit();
					}
					catch (Exception)
					{
						transaction.Rollback();
						throw;
					}
				}
			}	
		}
		
		public void DeleteUser(string userName)
		{
			using (var unitOfWork = _unitOfWorkFactory.CreateAccountUnitOfWork())
			{
				using (var transaction = unitOfWork.BeginTransaction())
				{
					try
					{
						var userManager = CreateUserManager(unitOfWork.CurrentContext);
						var user = userManager.FindByName(userName);
						if (user == null)
						{
							throw new InvalidOperationException(string.Format("Cannot delete user {0}: User not found ", userName));
						}

						var result = userManager.Delete(user);
						if (!result.Succeeded)
						{
							transaction.Rollback();
							throw new Exception("Error removing user account from identity store");
						}

						var userProfile = unitOfWork.UserProfileRepository.GetUserByUserName(userName);
						unitOfWork.UserProfileRepository.Delete(userProfile);
						unitOfWork.SaveChanges();

						transaction.Commit();

						using (var systemUnitOfWork = _unitOfWorkFactory.CreateSystemUnitOfWork())
						{
							systemUnitOfWork.NotificationRepository.DeleteNotificationsByUser(userName);
							systemUnitOfWork.SaveChanges(); 
						}
					}
					catch (Exception)
					{
						transaction.Rollback();
						throw;
					}
				}
			}

		}

		#endregion 
	}
}