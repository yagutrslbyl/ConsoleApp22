
using ConsoleApp22;
using MiniBankSystem;

class Program
{
    static void Main(string[] args)
    {
        var bank = new Bank { Id = 1 };
        var bankService = new BankService(bank);
        var accountService = new AccountService(bankService);
        var menu = new MenuService(bankService, accountService);

        
        bankService.AddUser("Admin", "User", "admin@bank.local", "Admin123", true);

        menu.Start();
    }
}