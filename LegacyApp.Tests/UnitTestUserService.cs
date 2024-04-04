using LegacyApp.Interfaces;
using Moq;

namespace LegacyApp.Tests
{
	[TestClass]
	public class UnitTestUserService
	{
		private Mock<IClientRepository> _mockClientRepository = null;
		private Mock<IUserDataAccessWrapper> _mockUserDataAccessWrapper = null;
		private Mock<IUserCreditServiceClientWrapper> _mockUserCreditServiceClientWrapper = null;

		[TestInitialize]
		public void Setup()
		{
			_mockUserDataAccessWrapper = new Mock<IUserDataAccessWrapper>();
			_mockUserCreditServiceClientWrapper = new Mock<IUserCreditServiceClientWrapper>();
			_mockClientRepository = new Mock<IClientRepository>();
			_mockClientRepository
				.Setup(x => x.GetByIdAsync())
				.ReturnsAsync(
								new Client { Name = "VeryImportantClient" }
							);
		}

		[TestMethod]
		public void addUserMethodShouldAddVeryImportantClient()
		{
			//Arrange
			IUserService userService = new UserService(_mockClientRepository.Object,
				_mockUserDataAccessWrapper.Object,
				_mockUserCreditServiceClientWrapper.Object
				);

			var firstName = "VeryImportantClient";
			var surname = "John";
			var email = "John@gmail.com";
			var dateOfBirth = new DateTime(1988, 6, 5);
			var clientId = 0;

			//Act
			var addedUser = userService.AddUser(firstName, surname, email, dateOfBirth, clientId);

			//Assert
			Assert.IsNotNull(addedUser);
			Assert.AreEqual(addedUser.Result.Client.Name, "VeryImportantClient");
			Assert.AreEqual(addedUser.Result.HasCreditLimit, false);

		}

		[TestMethod]
		public void afterAddingImportantClientHasCreditLimitShouldBeSetToTrue()
		{
			//Arrange
			IUserService userService = new UserService(_mockClientRepository.Object,
			_mockUserDataAccessWrapper.Object,
			_mockUserCreditServiceClientWrapper.Object
			);

			_mockClientRepository
				.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
				.ReturnsAsync(
								new Client { Name = "ImportantClient" }
							);
			_mockUserCreditServiceClientWrapper
				.Setup(x => x.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
				.Returns(1000);

			var firstName = "ImportantClient";
			var surname = "John";
			var email = "John@gmail.com";
			var dateOfBirth = new DateTime(1988, 6, 5);
			var clientId = 0;

			//Act
			var addedUser = userService.AddUser(firstName, surname, email, dateOfBirth, clientId);

			//Assert
			Assert.AreEqual(addedUser.Result.HasCreditLimit, true);
		}

		[TestMethod]
		public void addUserShouldThrowExceptionIf_firstNameFieldisEmpty()
		{
			//Arrange
			IUserService userService = new UserService(_mockClientRepository.Object,
			_mockUserDataAccessWrapper.Object,
			_mockUserCreditServiceClientWrapper.Object
			);

			var firstName = "";
			var surname = "John";
			var email = "John@gmail.com";
			var dateOfBirth = new DateTime(1988, 6, 5);
			var clientId = 0;

			//Act
			var exception = Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
			userService.AddUser(firstName, surname, email, dateOfBirth, clientId));

			//Assert
			var expectedMessage = "user firstname / surname is required ";
			Assert.AreEqual(exception.Result.Message, expectedMessage);
		}
	}
}