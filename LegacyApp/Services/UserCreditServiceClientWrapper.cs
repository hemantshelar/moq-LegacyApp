using LegacyApp.Interfaces;
using System;

namespace LegacyApp.Services;

//Implement IDisposable to dispose legacy service - UserCreditServiceClient
public class UserCreditServiceClientWrapper : IUserCreditServiceClientWrapper
{
	private readonly UserCreditServiceClient userCreditService;
	public UserCreditServiceClientWrapper()
	{
		userCreditService = new UserCreditServiceClient();
	}
	public int GetCreditLimit(string firstname, string surname, DateTime dateOfBirth)
	{
		var creditLimit = userCreditService.GetCreditLimit(firstname, surname, dateOfBirth);
		return creditLimit;
	}
}
