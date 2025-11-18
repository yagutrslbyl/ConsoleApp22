using MiniBankSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp22
{
    public class MenuService
    {
        private readonly BankService _bankService;
        private readonly AccountService _accountService;
        private User _currentUser = null;

        public MenuService(BankService bankService, AccountService accountService)
        {
            _bankService = bankService;
            _accountService = accountService;
        }

        public void Start()
        {
            while (true)
            {
                if (_currentUser == null || !_currentUser.IsLogged)
                    ShowPreLoginMenu();
                else
                    ShowPostLoginMenu();
            }
        }

        private void ShowPreLoginMenu()
        {
            Console.WriteLine("\n--- MINI BANK SYSTEM ---");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Find user");
            Console.WriteLine("0. Exit");
            Console.Write("Choose: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1": Register(); break;
                case "2": Login(); break;
                case "3": FindUser(); break;
                case "0": Environment.Exit(0); break;
                default: Console.WriteLine("Invalid choice."); break;
            }
        }

        private void ShowPostLoginMenu()
        {
            Console.WriteLine($"\n--- Welcome {_currentUser.Name} {_currentUser.Surname} ({(_currentUser.IsAdmin ? "Admin" : "User")}) ---");
            Console.WriteLine("1. Check Balance");
            Console.WriteLine("2. Top up balance");
            Console.WriteLine("3. Change password");
            Console.WriteLine("4. Bank user list (Admin only)");
            Console.WriteLine("5. Block user (Admin only)");
            Console.WriteLine("0. Log out");
            Console.Write("Choose: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1": _accountService.CheckBalance(_currentUser); break;
                case "2": TopUp(); break;
                case "3": ChangePassword(); break;
                case "4": ShowBankUserList(); break;
                case "5": BlockUser(); break;
                case "0": _accountService.Logout(_currentUser); _currentUser = null; break;
                default: Console.WriteLine("Invalid choice."); break;
            }
        }

        private void Register()
        {
            Console.Write("Name: ");
            var name = Console.ReadLine();
            Console.Write("Surname: ");
            var surname = Console.ReadLine();
            Console.Write("Email: ");
            var email = Console.ReadLine();
            Console.Write("Password: ");
            var pwd = ReadPassword();
            Console.Write("Is Admin (y/n): ");
            var isAdminStr = Console.ReadLine();
            bool isAdmin = isAdminStr?.Trim().ToLower() == "y";

            var (success, error) = _bankService.AddUser(name, surname, email, pwd, isAdmin);
            if (!success)
                Console.WriteLine($"Registration failed: {error}");
            else
                Console.WriteLine("Registration successful. You can now login.");
        }

        private void Login()
        {
            Console.Write("Email: ");
            var email = Console.ReadLine();
            Console.Write("Password: ");
            var pwd = ReadPassword();

            var user = _bankService.FindUser(email);
            if (user == null)
            {
                Console.WriteLine("User not found.");
                return;
            }
            if (user.IsBlocked)
            {
                Console.WriteLine("User is blocked. Contact admin.");
                return;
            }
            if (user.Password != pwd)
            {
                Console.WriteLine("Invalid credentials.");
                return;
            }

            user.IsLogged = true;
            _currentUser = user;
            Console.WriteLine($"Login successful. Welcome {user.Name}!");
        }
        private void FindUser()
        {
            Console.Write("Email to find: ");
            var email = Console.ReadLine();
            var user = _bankService.FindUser(email);
            if (user == null)
                Console.WriteLine("User not found.");
            else
                Console.WriteLine(user);
        }

        private void TopUp()
        {
            Console.Write("Enter amount to add: ");
            var s = Console.ReadLine();
            if (double.TryParse(s, out double amt))
                _accountService.TopUp(_currentUser, amt);
            else
                Console.WriteLine("Invalid amount.");
        }

        private void ChangePassword()
        {
            Console.Write("New password: ");
            var newPwd = ReadPassword();
            _accountService.ChangePassword(_currentUser, newPwd);
        }

        private void ShowBankUserList()
        {
            if (!_currentUser.IsAdmin)
            {
                Console.WriteLine("Access denied. Admins only.");
                return;
            }

            var users = _bankService.GetAllUsers();
            Console.WriteLine("--- Bank Users ---");
            foreach (var u in users)
            {
                Console.WriteLine($"{u.Name} {u.Surname}");
            }
        }

        private void BlockUser()
        {
            if (!_currentUser.IsAdmin)
            {
                Console.WriteLine("Access denied. Admins only.");
                return;
            }

            Console.Write("Email to block: ");
            var email = Console.ReadLine();
            var success = _bankService.BlockUser(email);
            Console.WriteLine(success ? "User blocked." : "User not found.");
        }

        private string ReadPassword()
        {
            // simple password read (shows * for each char)
            var pwd = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;
                if (key == ConsoleKey.Backspace && pwd.Length > 0)
                {
                    pwd = pwd[0..^1];
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    pwd += keyInfo.KeyChar;
                    Console.Write("*");
                }
            } while (key != ConsoleKey.Enter);
            Console.WriteLine();
            return pwd;
        }
    }

}
