using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MiniBankSystem
{
    

   
    
    public class BankService
    {
        private readonly Bank _bank;
        private int _nextUserId = 1;

        public BankService(Bank bank)
        {
            _bank = bank ?? throw new ArgumentNullException(nameof(bank));
        }

        public (bool Success, string Error) AddUser(string name, string surname, string email, string password, bool isAdmin)
        {
                     if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
                return (false, "Name must be at least 3 characters long.");
            if (string.IsNullOrWhiteSpace(surname) || surname.Length < 3)
                return (false, "Surname must be at least 3 characters long.");
            if (!IsValidEmail(email))
                return (false, "Email is invalid. It must contain exactly one '@'.");
            if (FindUser(email) != null)
                return (false, "Email already exists in the system.");
            if (!IsValidPassword(password))
                return (false, "Password is invalid. Minimum 8 chars, at least one uppercase and one lowercase letter.");

            var user = new Users
            {
                Id = _nextUserId++,
                Name = name.Trim(),
                Surname = surname.Trim(),
                Email = email.Trim(),
                Password = password,
                Balance = 0.0,
                IsAdmin = isAdmin,
                IsBlocked = false,
                IsLogged = false
            };

            _bank.Users.Add(user);
            return (true, null);
        }

        public User FindUser(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            return _bank.Users.FirstOrDefault(u => string.Equals(u.Email, email.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public bool BlockUser(string email)
        {
            var user = FindUser(email);
            if (user == null) return false;
            user.IsBlocked = true;
            user.IsLogged = false;
            return true;
        }

        public IEnumerable<User> GetAllUsers() => _bank.Users;

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var trimmed = email.Trim();
            
            return trimmed.Count(c => c == '@') == 1;
        }

        private bool IsValidPassword(string pwd)
        {
            if (string.IsNullOrEmpty(pwd) || pwd.Length < 8) return false;
            bool hasUpper = pwd.Any(char.IsUpper);
            bool hasLower = pwd.Any(char.IsLower);
            return hasUpper && hasLower;
        }
    }

   
  
}
