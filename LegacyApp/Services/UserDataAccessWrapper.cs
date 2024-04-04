using LegacyApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyApp.Services;

public class UserDataAccessWrapper : IUserDataAccessWrapper
{
	public User AddUser(User user)
	{
		UserDataAccess.AddUser(user);
		return user;
	}
}
