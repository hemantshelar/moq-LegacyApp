using LegacyApp.Interfaces;
using System;
using System.Threading.Tasks;
namespace LegacyApp;
public class UserService : IUserService
{
	private readonly IClientRepository _clientRepository;
	private readonly IUserDataAccessWrapper _userDataAccess;
	private readonly IUserCreditServiceClientWrapper _userCreditServiceClientWrapper;
	public UserService(IClientRepository clientRepository, IUserDataAccessWrapper userDataAccess, IUserCreditServiceClientWrapper userCreditServiceClientWrapper)
	{
		_clientRepository = clientRepository;
		_userDataAccess = userDataAccess;
		_userCreditServiceClientWrapper = userCreditServiceClientWrapper;
	}
	public async Task<User> AddUser(string firstName, string surname, string email, DateTime dateOfBirth, int clientId)
	{
		//We can move this to a separate class ( SingleResponsibility Principle )
		ValidateUser(firstName, surname, email, dateOfBirth);

		//Use injected dependency - IClientRepository
		//var clientRepository = new ClientRepository();
		var client = await _clientRepository.GetByIdAsync(clientId);

		var user = new User
		{
			Client = client,
			DateOfBirth = dateOfBirth,
			EmailAddress = email,
			Firstname = firstName,
			Surname = surname
		};

		#region We should move this to a separate class - CreditLimitCalculator
		if (client.Name == "VeryImportantClient")
		{
			// Skip credit check
			user.HasCreditLimit = false;
		}
		else if (client.Name == "ImportantClient")
		{
			// Do credit check and double credit limit
			user.HasCreditLimit = true;
			var creditLimit = _userCreditServiceClientWrapper.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth);
			creditLimit = creditLimit * 2;
			user.CreditLimit = creditLimit;
		}
		else
		{
			// Do credit check
			user.HasCreditLimit = true;
			var creditLimit = _userCreditServiceClientWrapper.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth);
			user.CreditLimit = creditLimit;
		}

		if (user.HasCreditLimit && user.CreditLimit < 500)
		{
			throw new InvalidOperationException("insufficient credit limit");
		}
		#endregion

		//Inject legacy dependency - UserDataAccess
		//UserDataAccess.AddUser(user);
		_userDataAccess.AddUser(user);

		return user;
	}

	/// <summary>
	/// This should be moved to a separate class ( SingleResponsibility Principle )
	/// </summary>
	/// <param name="firstName"></param>
	/// <param name="surname"></param>
	/// <param name="email"></param>
	/// <param name="dateOfBirth"></param>
	/// <exception cref="InvalidOperationException"></exception>
	public void ValidateUser(string firstName, string surname, string email, DateTime dateOfBirth)
	{
		if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(surname))
		{
			throw new InvalidOperationException("user firstname / surname is required ");
		}

		//User regular expression for email validation
		if (!email.Contains("@") && !email.Contains("."))
		{
			throw new InvalidOperationException("user email is invalid ");
		}

		var now = DateTime.Now;
		int age = now.Year - dateOfBirth.Year;
		if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)) age--;

		if (age < AppConstants.MIN_AGE)
		{
			throw new InvalidOperationException($"user should be older than {AppConstants.MIN_AGE} years");
		}
	}
}
