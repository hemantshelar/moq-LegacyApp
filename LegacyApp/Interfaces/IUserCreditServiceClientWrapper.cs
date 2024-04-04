using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyApp.Interfaces
{
	public interface IUserCreditServiceClientWrapper
	{
		int GetCreditLimit(string firstname, string surname, DateTime dateOfBirth);
	}
}
