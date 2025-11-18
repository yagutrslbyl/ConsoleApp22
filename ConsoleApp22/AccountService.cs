using MiniBankSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp22
{
    public class AccountService
    {
        private readonly BankService _bankService;

        public AccountService(BankService bankService)
        {
            _bankService = bankService;
        }

        public void CheckBalance(User user)
        {
            Console.WriteLine($"Your balance: {user.Balance:F2} AZN");
        }

        public void TopUp(User user, double amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("Amount must be greater than zero.");
                return;
            }

            user.Balance += amount;
            Console.WriteLine($"Balance updated. New balance: {user.Balance:F2} AZN (added {amount:F2})");
        }

        public bool ChangePassword(User user, string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 8)
            {
                Console.WriteLine("Password must be at least 8 characters long.");
                return false;
            }
            if (!newPassword.Any(char.IsUpper) || !newPassword.Any(char.IsLower))
            {
                Console.WriteLine("Password must contain at least one uppercase and one lowercase letter.");
                return false;
            }

            user.Password = newPassword;
            Console.WriteLine("Password changed successfully.");
            return true;
        }

        public void Logout(User user)
        {
            user.IsLogged = false;
            Console.WriteLine("Logged out.");
        }
    }

}
