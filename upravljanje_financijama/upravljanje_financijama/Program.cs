using Microsoft.VisualBasic;
using System;
using System.Data.Common;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Transactions;
using System.Xml.Linq;

//auxilary functions

static string NewString(string message)
{
    var myString = "";
    do
    {
        Console.Write(message);
        myString = Console.ReadLine();
    } while (myString == "");
    return myString;
}
static int NewNumber(string message, int upperLimit = int.MaxValue)   //(positive integer)
{
    var myOption = 0;
    do
    {
        Console.Write(message);
        int.TryParse(Console.ReadLine(), out myOption);
    } while (myOption <= 0 || myOption > upperLimit);
    return myOption;
}
static double NewDouble(string message)
{
    var myOption = -1.00;
    do
    {
        Console.Write(message);
        double.TryParse(Console.ReadLine(), out myOption);
    } while (myOption <= 0);
    return myOption;
}
static double NewDoublePosAndNeg(string message)
{
    var myOption = 0.00;
    var answer = false;
    do
    {
        Console.Write(message);
        answer = double.TryParse(Console.ReadLine(), out myOption);
    } while (!answer);
    return myOption;
}
static DateOnly NewDateOnly(string message)
{
    var year = NewNumber($"Enter the year of {message}: ", DateTime.Now.Year);
    var month = NewNumber($"Enter the month of {message}: ", 12);
    var day = NewNumber($"Enter the day of {message}: ", 31);       //not the best solution

    return new DateOnly(year, month, day);
}
static DateTime NewDateTime(string message)
{
    var year = NewNumber($"Enter the year of {message}: ", DateTime.Now.Year);
    var month = NewNumber($"Enter the month of {message}: ",12);
    var day = NewNumber($"Enter the day of {message}: ", 31);
    var hour = NewNumber($"Enter the hour of {message}: ", 23);
    var minutes = NewNumber($"Enter the minutes of {message}: ", 59);
    var seconds = NewNumber($"Enter the seconds of {message}: ", 59);

    return new DateTime(year, month, day, hour, minutes, seconds);
}
static int ChooseOption(List<int> options)
{
    do {
        var myOption = NewNumber("Choose number: ");
        foreach(var item in options)
        {
            if(item == myOption)
                return myOption;
        }
    } while (true);
}
static bool ChooseOptionText(string message = "")
{
    do
    {
        var myOption = NewString(message + "Write y or n: ");
        if (myOption == "y") return true;
        else if (myOption == "n") return false;
    } while (true);
}
static bool ChooseTwoOptions(string message)
{
    Console.Write(message);
    if (ChooseOption(new List<int>() { 1, 2 }) == 1)
        return true;
    else
        return false;
}


static Tuple<int, string, string, DateOnly, List<double>> ChangeUserFormat(int id, Tuple<string, string, DateOnly, List<double>> user)
{
    return Tuple.Create(id, user.Item1, user.Item2, user.Item3, user.Item4);
}
static void PrintUser(Tuple<int, string, string, DateOnly, List<double>> user)
{
    Console.WriteLine($"{user.Item1} - {user.Item2} - {user.Item3} - {user.Item4}");
}


static bool UserExists(int id, Dictionary<int, Tuple<string, string, DateOnly, List<double>>> users)
{
    foreach(var key in users.Keys)
    {
        if (id == key)
            return true;
    }
    return false;
}
static Tuple<int, string, string, DateOnly, List<double>> ChooseUser(List<Tuple<int, string, string, DateOnly, List<double>>> userList)
{
    var optionList = new List<string>() { "y", "n" };
    foreach (var user in userList)
    {
        PrintUser(user);
        if(ChooseOptionText("Do you want this user? "))
            return user;
    }
    return null;
}
static Tuple<int, string, string, DateOnly, List<double>> FindUserByName(Dictionary<int, Tuple<string, string, DateOnly, List<double>>> users)
{
    var answer = true;
    do
    {
        var myList = new List<Tuple<int, string, string, DateOnly, List<double>>>();
        var name = NewString("Choose user. Name: ");
        var surname = NewString("Surname: ");

        foreach (var key in users.Keys)
        {
            var user = users[key];
            var name2 = user.Item1;
            var surname2 = user.Item2;

            if (name == name2 && surname == surname2)
            {
                myList.Add(ChangeUserFormat(key, users[key]));
            }
        }
        if (myList.Count == 0)
        {
            Console.WriteLine("User not found. Want to try again? ");
            answer = ChooseOptionText();
        }
        else
        {
            var user = ChooseUser(myList);
            if (user != null)
                return user;
            Console.WriteLine("No users left. Want to try again? ");
            answer = ChooseOptionText();
        }
    } while (answer);
    return null;
}


static bool Age(DateOnly myDate)
{
    var age = DateTime.Now.Year - myDate.Year;
    if (DateTime.Now < myDate.ToDateTime(new TimeOnly(0,0,0)).AddYears(age)) 
        age--;
    if(age > 30)
        return true;
    else
        return false;
}
static bool Minus(List<double> accounts)
{
    foreach (var account in accounts)
    {
        if (account < 0)
            return true;
    }
    return false;
}





// main functions

static void MainMenu(Dictionary<int, Tuple<string, string, DateOnly, List<double>>> users, Dictionary<int, Tuple<int,int,double, string, bool, int, DateTime>> transactions)
{
    var stay = true;
    do
    {
        Console.Clear();
        Console.Write("1 - Users\n2 - Accounts\n3 - Exit the application\n");

        switch (ChooseOption(new List<int>() { 1, 2, 3 }))
        {
            case 1:
                MenuUsers(users);
                Console.ReadKey();
                break;
            case 2:
                MenuAccount(users, transactions);
                Console.ReadKey();
                break;
            default:
                stay = false;
                break;
        }
    } while (stay);
    Console.Clear();
}
static void MenuUsers(Dictionary<int, Tuple<string, string, DateOnly, List<double>>> users)
{
    Console.Write("\n1 - New User\n2 - Delete User\n3 - Edit User\n4 - View User\n");

    switch (ChooseOption(new List<int>() { 1, 2, 3, 4 }))
    {
        case 1:
            UsersCreateUser(users);
            break;
        case 2:
            MenuUsersDelete(users);
            break;
        case 3:
            MenuUsersEditID(users);
            break;
        default:
            MenuViewUser(users);
            break;
    }
}
static void UsersCreateUser(Dictionary<int, Tuple<string, string, DateOnly, List<double>>> users)
{
    var id = 0;
    do
    {
        id++;
    } while (UserExists(id, users));
    var name = NewString("Enter new name: ");
    var surname = NewString("Enter new surname: ");
    var date = NewDateOnly("birth");

    users[id] = Tuple.Create(name,surname, date, new List<double> {100.00,0.00,0.00});
}
static void MenuUsersDelete(Dictionary<int, Tuple<string, string, DateOnly, List<double>>> users)
{
    Console.Write("\n1 - Delete by number\n2 - Delete by name and surname\n");
   
    switch (ChooseOption(new List<int>() { 1, 2 }))
    {
        case 1:
            UsersDeleteID(users);
            break;
        default:
            UsersDeleteName(users);
            break;
    }
}
static void UsersDeleteID(Dictionary<int, Tuple<string, string, DateOnly, List<double>>> users)
{
    var id = -1;
    do
    {
        id = NewNumber("Enter valid ID: ");
    } while (UserExists(id, users) == false);
    
    users.Remove(id);
}
static void UsersDeleteName(Dictionary<int, Tuple<string, string, DateOnly, List<double>>> users)
{
    var user = FindUserByName(users);
    if(user != null)
        users.Remove(user.Item1);
}
static void MenuUsersEditID(Dictionary<int, Tuple<string, string, DateOnly, List<double>>> users)
{
    var id = -1;
    do
    {
        id = NewNumber("Enter valid ID: ");
    } while (UserExists(id,users)==false);

    var user = users[id];
    var name = user.Item1;
    var surname = user.Item2;
    var year = user.Item3.Year;
    var month = user.Item3.Month;
    var day = user.Item3.Day;
    var firstaccount = user.Item4[0];
    var secondaccount = user.Item4[1];
    var thirdaccount = user.Item4[2];

    Console.Write("\n1 - Name\n2 - Surname\n3 - Year of birth\n4 - Month of birth\n5 - Day of birth\n6 - Tekući\n7 - Žiro\n8 - Prepaid\n");
    
    switch (ChooseOption(new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 }))
    {
        case 1:
            name = NewString("Enter name: ");
            break;
        case 2:
            surname = NewString("Enter surname: ");
            break;
        case 3:
            year = NewNumber("Enter year of birth: ", DateTime.Now.Year);
            break;
        case 4:
            month = NewNumber("Enter month of birth: ", 12);
            break;
        case 5:
            day = NewNumber("Enter day of birth: ", 31);
            break;
        case 6:
            firstaccount = NewDoublePosAndNeg("Enter the amount of money in the account: ");
            break;
        case 7:
            secondaccount = NewDoublePosAndNeg("Enter the amount of money in the account: ");
            break;
        default:
            thirdaccount = NewDoublePosAndNeg("Enter the amount of money in the account: ");
            break;
    }
    users[id] = Tuple.Create(name, surname, new DateOnly(year, month, day), new List<double> { firstaccount, secondaccount, thirdaccount });
}
static void MenuViewUser(Dictionary<int, Tuple<string, string, DateOnly, List<double>>> users)
{
    Console.Write("\n1 - View all users\n2 - View users older than 30\n3 - View users with minus on account\n");

    switch (ChooseOption(new List<int>() { 1, 2, 3 }))
    {
        case 1:
            ViewUserAll(users);
            break;
        case 2:
            ViewUserOlder(users);
            break;
        default:
            ViewUserMinus(users);
            break;
    }
}
static void ViewUserAll(Dictionary<int, Tuple<string, string, DateOnly, List<double>>> users)
{
    var myList = new List<Tuple<int, string, string, DateOnly, List<double>>>();
    foreach(var key in users.Keys)
    {
        myList.Add(ChangeUserFormat(key, users[key]));
    }
    myList.Sort((a,b) => a.Item3.CompareTo(b.Item3));

    foreach(var user in myList)
    {
        PrintUser(user);
    }
}
static void ViewUserOlder(Dictionary<int, Tuple<string, string, DateOnly, List<double>>> users)
{
    foreach(var key in users.Keys)
    {
        if (Age(users[key].Item3))
            PrintUser(ChangeUserFormat(key, users[key]));
    }
}
static void ViewUserMinus(Dictionary<int, Tuple<string, string, DateOnly, List<double>>> users)
{
    foreach (var key in users.Keys)
    {
        if (Minus(users[key].Item4))
            PrintUser(ChangeUserFormat(key, users[key]));
    }
}



//auxilary functions

static Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> SelectedTransactions(int key, int account, Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions)
{
    var selectedtransactions = new Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>>();
    foreach (var id in transactions.Keys)
    {
        var transaction = transactions[id];
        if (transaction.Item1 == key && transaction.Item2 == account)
        {
            selectedtransactions[id] = transaction;
        }
    }
    return selectedtransactions;
}
static bool TransactionExists(int id, Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions)
{
    foreach(var key in transactions.Keys)
    {
        if (key == id)
            return true;
    }
    return false;
}
static string Type(bool type)
{
    if (type) return "income";
    else return "expense";
}
static string ConvertCategory(int category)
{
    switch (category)
    {
        case 1:
            return "food";
        case 2:
            return "car";
        case 3:
            return "concerts";
        case 4:
            return "sport";
        default:
            return "kids";
    }
}
static void PrintTransactionAll(int id, Tuple<int, int, double, string, bool, int, DateTime> transaction)
{
    Console.WriteLine($"ID: {id} - Amount: {transaction.Item3} - Description: {transaction.Item4} - Type: {Type(transaction.Item5)} - Category: {ConvertCategory(transaction.Item6)} - Date: {transaction.Item7}");
}
static void PrintTransaction(Tuple<int, int, double, string, bool, int, DateTime> transaction)
{
    Console.WriteLine($"Type: {Type(transaction.Item5)} - Amount: {transaction.Item3} - Description: {transaction.Item4} - Category: {ConvertCategory(transaction.Item6)} - Date: {transaction.Item7}");
}



//main functions

static void MenuAccount(Dictionary<int, Tuple<string, string, DateOnly, List<double>>> users, Dictionary<int, Tuple<int,int,double, string, bool, int, DateTime>> transactions)
{
    var user = FindUserByName(users);
    if (user == null)
        return;

    Console.Write($"\n1 - View account\n2 - Send money\n");
    switch (ChooseOption(new List<int>() { 1, 2 }))
    {
        case 1:
            MenuViewAccount(user, transactions);
            break;
        default:
            MenuSendMoney(user.Item1, users, transactions);
            break;
    }
}
static void MenuViewAccount(Tuple<int, string, string, DateOnly, List<double>> user, Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions)
{
    Console.Write($"\n1 - Tekući\n2 - Žiro\n3 - Prepaid\n");
    var account = ChooseOption(new List<int>() { 1, 2, 3 }) -1;

    int key = user.Item1;
    var selectedTransactions = SelectedTransactions(key,account,transactions);

    Console.Write("\n1 - New Transaction\n2 - Delete Transaction\n3 - Edit Transaction\n4 - View Transaction\n5 - Financial report\n");

    switch (ChooseOption(new List<int>() { 1, 2, 3, 4, 5 }))
    {
        case 1:
            NewTransaction(key, account, transactions);
            break;
        case 2:
            MenuDeleteTransaction(transactions, selectedTransactions);
            break;
        case 3:
            EditTransactionID(transactions, selectedTransactions);
            break;
        case 4:
            MenuViewTransaction(selectedTransactions);
            break;
        default:
            MenuFinancialReport(selectedTransactions);
            break;
    }
}
static void NewTransaction(int key, int account, Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions)
{
    var id = 0;
    var amount = 0.00;
    do
    {
        id++;
    } while (TransactionExists(id, transactions));
    
    amount = NewDouble("Enter amount of money (2 decimal places): ");
    var type = ChooseTwoOptions(("1 - Income\n2 - Expense\n"));
    var category = NewNumber("1 - Food\n2 - Car\n3 - Concerts\n4 - Sport\n5 - Kids\n", 5);
    var description = "standard transaction";
    if (ChooseOptionText("Do you want to leave description as standard transaction? ") == false)
        description = NewString("Enter description: ");
    var datetime = DateTime.Now;

    if(!ChooseTwoOptions("\n1 - current transaction\n2 - earlier transaction\n"))
        datetime = NewDateTime("transaction");

    transactions[id] = Tuple.Create(key,account, amount,description, type, category, datetime);
    Console.WriteLine("Transaction added successfully.");
}
static void MenuDeleteTransaction(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions, Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> selectedTransactions)
{
    Console.Write("\n1 - Delete transaction by ID\n2 - Delete transactions lower than ?€ \n3 - Delete transactions higher than ?€\n4 - Delete income transactions\n5 - Delete expense transactions\n6 - Delete all transactions from the same category\n");

    switch (ChooseOption(new List<int>() { 1, 2, 3, 4, 5, 6 }))
    {
        case 1:
            DeleteTransactionID(transactions, selectedTransactions);
            break;
        case 2:
            DeleteTransactionLower(transactions, selectedTransactions);
            break;
        case 3:
            DeleteTransactionHigher(transactions, selectedTransactions);
            break;
        case 4:
            DeleteTransactionIncome(transactions, selectedTransactions);
            break;
        case 5:
            DeleteTransactionExpense(transactions, selectedTransactions);
            break;
        default:
            DeleteTransactionCategory(transactions, selectedTransactions);
            break;
    }
}
static void DeleteTransactionID(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions, Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> selectedTransactions)
{
    var id = 0;
    do
    {
        id = NewNumber("Enter new ID: ");
    } while (TransactionExists(id, selectedTransactions) ==false);

    foreach(var key in selectedTransactions.Keys)
    {
        if (key == id)
        {
            PrintTransactionAll(key, transactions[key]);
            if (ChooseOptionText("Do you want to delete this transaction? "))
            {
                transactions.Remove(key);
                Console.WriteLine($"Transaction with ID {key} deleted successfully.");
            }
            else
                Console.WriteLine($"Transaction with ID {key} is not deleted.");
            break;
        }
    }
}
static void DeleteTransactionLower(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions, Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> selectedTransactions)
{
    var limit = NewNumber("Enter upper limit: ");
    foreach (var key in selectedTransactions.Keys)
    {
        if (transactions[key].Item3 < limit)
        {
            PrintTransactionAll(key, transactions[key]);
            if (ChooseOptionText("Do you want to delete this transaction? "))
            {
                transactions.Remove(key);
                Console.WriteLine($"Transaction with ID {key} deleted successfully.");
            }
            else
                Console.WriteLine($"Transaction with ID {key} is not deleted.");
        }
    }
}
static void DeleteTransactionHigher(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions, Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> selectedTransactions)
{
    var limit = NewNumber("Enter lower limit: ");
    foreach (var key in selectedTransactions.Keys)
    {
        if (transactions[key].Item3 > limit)
        {
            PrintTransactionAll(key, transactions[key]);
            if (ChooseOptionText("Do you want to delete this transaction? "))
            {
                transactions.Remove(key);
                Console.WriteLine($"Transaction with ID {key} deleted successfully.");
            }
            else
                Console.WriteLine($"Transaction with ID {key} is not deleted.");
        }
    }
}
static void DeleteTransactionIncome(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions, Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> selectedTransactions)
{
    foreach(var key in selectedTransactions.Keys)
    {
        if (transactions[key].Item5)
        {
            PrintTransactionAll(key, transactions[key]);
            if (ChooseOptionText("Do you want to delete this transaction? "))
            {
                transactions.Remove(key);
                Console.WriteLine($"Transaction with ID {key} deleted successfully.");
            }
            else
                Console.WriteLine($"Transaction with ID {key} is not deleted.");
        }
    }
}
static void DeleteTransactionExpense(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions, Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> selectedTransactions)
{
    foreach (var key in selectedTransactions.Keys)
    {
        if (transactions[key].Item5 == false)
        {
            PrintTransactionAll(key, transactions[key]);
            if (ChooseOptionText("Do you want to delete this transaction? "))
            {
                transactions.Remove(key);
                Console.WriteLine($"Transaction with ID {key} deleted successfully.");
            }
            else
                Console.WriteLine($"Transaction with ID {key} is not deleted.");
        }
    }
}
static void DeleteTransactionCategory(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions, Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> selectedTransactions)
{
    var category = NewNumber("\n1 - Food\n2 - Car\n3 - Concerts\n4 - Sport\n5 - Kids\n", 5);
    foreach (var key in selectedTransactions.Keys)
    {
        if (transactions[key].Item6 == category)
        {
            PrintTransactionAll(key, transactions[key]);
            if(ChooseOptionText("Do you want to delete this transaction? "))
            {
                transactions.Remove(key);
                Console.WriteLine($"Transaction with ID {key} deleted successfully.");
            }
            else
                Console.WriteLine($"Transaction with ID {key} is not deleted.");
        }
    }
}
static void EditTransactionID(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions, Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> selectedTransactions)
{
    var id = 0;
    do
    {
        id = NewNumber("Enter valid ID: ");
    } while (TransactionExists(id, selectedTransactions) ==false);

    PrintTransactionAll(id, transactions[id]);
    if (ChooseOptionText("Do you want to edit this transaction? ") == false)
    {
        Console.WriteLine($"Transaction with ID {id} is not edited.");
        return;
    }

    var transaction = transactions[id];
    var amount = transaction.Item3;
    var type = transaction.Item5;
    var description = transaction.Item4;
    var date = transaction.Item7;
    var month = date.Month;
    var year = date.Year;
    var day = date.Day;
    var seconds = date.Second;
    var minutes = date.Minute;
    var hour = date.Hour;
    var category = transaction.Item6;

    //can't change user or account
    Console.Write("\n1 - Amount\n2 - Description\n3 - Type\n4 - Category\n5 - Year\n6 - Month\n7 - Day\n");

    switch (ChooseOption(new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }))
    {
        case 1:
            amount = NewDouble("Enter amount: ");
            break;
        case 2:
            description = NewString("Enter description: ");
            break;
        case 3:
            type = ChooseTwoOptions(("1 - Income\n2 - Expense"));
            break;
        case 4:
            category = NewNumber("1 - Food\n2 - Car\n3 - Concerts\n4 - Sport\n5 - Kids\n", 5);
            break;
        case 5:
            year = NewNumber("Enter year: ", DateTime.Now.Year);
            break;
        case 6:
            month = NewNumber("Enter month: ", 12);
            break;
        case 7:
            day = NewNumber("Enter day: ", 31);
            break;
        case 8:
            hour = NewNumber("Enter hours: ", 23);
            break;
        case 9:
            minutes = NewNumber("Enter minutes: ", 59);
            break;
        default:
            seconds = NewNumber("Enter seconds: ", 59);
            break;
    }

    transactions[id] = Tuple.Create(transaction.Item1,transaction.Item2,amount,description,type,category, new DateTime(year, month, day, hour, minutes, seconds));
    Console.WriteLine($"Transaction with ID {id} edited successfully.");
}
static void MenuViewTransaction(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> selectedTransactions)
{
    Console.Write("\n1 - All transactions\n2 - Amount ascending\n3 - Amount descending\n4 - Sorted by description\n5 - Date ascending\n6 - Date descending\n7 - Income\n8 - Expense\n9 - Category\n10 - Type and category\n");

    switch (ChooseOption(new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }))
    {
        case 1:
            ViewTransactionsAll(selectedTransactions);
            break;
        case 2:
            ViewTransactionsAmountAscending(selectedTransactions);
            break;
        case 3:
            ViewTransactionsAmountDescending(selectedTransactions);
            break;
        case 4:
            ViewTransactionsDescription(selectedTransactions);
            break;
        case 5:
            ViewTransactionsDateAscending(selectedTransactions);
            break;
        case 6:
            ViewTransactionsDateDescending(selectedTransactions);
            break;
        case 7:
            ViewTransactionsIncome(selectedTransactions);
            break;
        case 8:
            ViewTransactionsExpense(selectedTransactions);
            break;
        case 9:
            ViewTransactionsCategory(selectedTransactions);
            break;
        default:
            ViewTransactionsTypeCategory(selectedTransactions);
            break;
    }
}
static void ViewTransactionsAll(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions)
{
    foreach(var id in transactions.Keys)
    {
        PrintTransaction(transactions[id]);
    }
}
static void ViewTransactionsAmountAscending(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions)
{
    var myList = new List<Tuple<int, int, double, string, bool, int, DateTime>>();
    foreach (var id in transactions.Keys)
    {
        myList.Add(transactions[id]);
    }
    myList.Sort((a, b) => a.Item3.CompareTo(b.Item3));
    foreach (var transaction in myList)
    {
        PrintTransaction(transaction);
    }
}
static void ViewTransactionsAmountDescending(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions)
{
    var myList = new List<Tuple<int, int, double, string, bool, int, DateTime>>();
    foreach (var id in transactions.Keys)
    {
        myList.Add(transactions[id]);
    }
    myList.Sort((b, a) => a.Item3.CompareTo(b.Item3));
    foreach (var transaction in myList)
    {
        PrintTransaction(transaction);
    }
}
static void ViewTransactionsDescription(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions)
{
    var myList = new List<Tuple<int, int, double, string, bool, int, DateTime>>();
    foreach (var id in transactions.Keys)
    {
        myList.Add(transactions[id]);
    }
    myList.Sort((a, b) => a.Item4.CompareTo(b.Item4));
    foreach (var transaction in myList)
    {
        PrintTransaction(transaction);
    }
}
static void ViewTransactionsDateAscending(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions)
{
    var myList = new List<Tuple<int, int, double, string, bool, int, DateTime>>();
    foreach (var id in transactions.Keys)
    {
        myList.Add(transactions[id]);
    }
    myList.Sort((a, b) => a.Item7.CompareTo(b.Item7));
    foreach (var transaction in myList)
    {
        PrintTransaction(transaction);
    }
}
static void ViewTransactionsDateDescending(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions)
{
    var myList = new List<Tuple<int, int, double, string, bool, int, DateTime>>();
    foreach (var id in transactions.Keys)
    {
        myList.Add(transactions[id]);
    }
    myList.Sort((b, a) => a.Item7.CompareTo(b.Item7));
    foreach (var transaction in myList)
    {
        PrintTransaction(transaction);
    }
}
static void ViewTransactionsIncome(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions)
{
    foreach (var id in transactions.Keys)
    {
        if (transactions[id].Item5)
            PrintTransaction(transactions[id]);
    }
}
static void ViewTransactionsExpense(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions)
{
    foreach (var id in transactions.Keys)
    {
        if (transactions[id].Item5 == false)
            PrintTransaction(transactions[id]);
    }
}
static void ViewTransactionsCategory(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions)
{
    var category = NewNumber("1 - Food\n2 - Car\n3 - Concerts\n4 - Sport\n5 - Kids\n", 5);
    foreach (var id in transactions.Keys)
    {
        if (transactions[id].Item6 == category)
            PrintTransaction(transactions[id]);
    }
}
static void ViewTransactionsTypeCategory(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions)
{
    var type = ChooseTwoOptions(("1 - Income\n2 - Expense"));
    var category = NewNumber("1 - Food\n2 - Car\n3 - Concerts\n4 - Sport\n5 - Kids\n", 5);
    foreach (var id in transactions.Keys)
    {
        if (transactions[id].Item6 == category && transactions[id].Item5== type)
            PrintTransaction(transactions[id]);
    }
}
static void MenuFinancialReport(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> selectedTransactions)
{
    Console.Write("\n1 - Account balance\n2 - Number of transactions\n3 - Report by year and month\n4 - Percentage of cost share for the selected category\n5 - Average amount for year and month\n6 - Average amount for category\n");

    switch (ChooseOption(new List<int>() { 1, 2, 3, 4, 5, 6 }))
    {
        case 1:
            FinancialReportAccountBalance(selectedTransactions);
            break;
        case 2:
            FinancialReportNumberOfTransactions(selectedTransactions);
            break;
        case 3:
            FinancialReportYearMonth(selectedTransactions);
            break;
        case 4:
            FinancialReportExpenses(selectedTransactions);
            break;
        case 5:
            FinancialReportAverageYearMonth(selectedTransactions);
            break;
        default:
            FinancialReportAverageCategory(selectedTransactions);
            break;
    }
}
static void FinancialReportAccountBalance(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions)
{
    var balance = 0.00;
    foreach (var id in transactions.Keys)
    {
        if (transactions[id].Item5)
            balance += transactions[id].Item3;
        else
            balance -= transactions[id].Item3;
    }
    Console.WriteLine($"Balance is {balance}€");
    if (balance < 0)
        Console.WriteLine("The account balance is in the red");
}
static void FinancialReportNumberOfTransactions(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions)
{
    Console.WriteLine($"Total number of transactions is {transactions.Count}");
}
static void FinancialReportYearMonth(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions)
{
    var year = NewNumber("Enter year: ", 2024);
    var month = NewNumber("Enter month: ", 12);
    var income = 0.00;
    var expense = 0.00;

    foreach (var id in transactions.Keys)
    {
        if (transactions[id].Item7.Year == year && transactions[id].Item7.Month == month)
        {
            if (transactions[id].Item5)
                income += transactions[id].Item3;
            else
                expense += transactions[id].Item3;
        }
    }
    Console.WriteLine($"Income for {month}/{year} is {income}€ and expense is {expense}");
}
static void FinancialReportExpenses(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions)
{
    var all = 0.00;
    var allcategory = 0.00;
    var category = NewNumber("1 - Food\n2 - Car\n3 - Concerts\n4 - Sport\n5 - Kids\n", 5);

    foreach (var id in transactions.Keys)
    {
        if (transactions[id].Item6 == category)
        {
            allcategory += transactions[id].Item3;
        }
        all += transactions[id].Item3;
    }
    Console.WriteLine($"Percentage of cost share for {category} is {allcategory / all * 100}%");
}
static void FinancialReportAverageYearMonth(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions)
{
    var year = NewNumber("Enter year: ", 2024);
    var month = NewNumber("Enter month: ", 12);
    var all = 0.00;
    var counter = 0;

    foreach (var id in transactions.Keys)
    {
        if (transactions[id].Item7.Year == year && transactions[id].Item7.Month == month)
        {
            counter++;
            all += transactions[id].Item3;
        }
    }
    Console.WriteLine($"Average transaction for {month}/{year} is {all / counter}");
}
static void FinancialReportAverageCategory(Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions)
{
    var all = 0.00;
    var counter = 0;
    var category = NewNumber("1 - Food\n2 - Car\n3 - Concerts\n4 - Sport\n5 - Kids\n", 5);

    foreach (var id in transactions.Keys)
    {
        if (transactions[id].Item6 == category)
        {
            counter++;
            all += transactions[id].Item3;
        }
    }
    Console.WriteLine($"Average transaction for {category} is {all / transactions.Count}");
}



static void MenuSendMoney(int key, Dictionary<int, Tuple<string, string, DateOnly, List<double>>> users, Dictionary<int, Tuple<int,int,double, string, bool, int, DateTime>> transactions)
{
    Console.Write("1 - Send money to your other account\n2 - Send money to someone else\n");

    switch(ChooseOption(new List<int>() { 1, 2 }))
    {
        case 1:
            SendMoneyYourself(key,users,transactions);
            break;
        default:
            SendMoney(key,users,transactions);
            break;
    }
}
static void SendMoneyYourself(int key, Dictionary<int, Tuple<string, string, DateOnly, List<double>>> users, Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions)
{
    Console.Write($"Choose account you are sending money from.\n1 - Tekući\n2 - Žiro\n3 - Prepaid");
    var account = ChooseOption(new List<int>() { 1, 2, 3 }) - 1;

    Console.Write($"Choose account you are sending money to.\n1 - Tekući\n2 - Žiro\n3 - Prepaid");
    var account2 = -1;
    do
    {
        account2 = ChooseOption(new List<int>() { 1, 2, 3 }) - 1;
    } while (account==account2);

    var id = 0;
    var amount = 0.00;
    do
    {
        id++;
    } while (TransactionExists(id, transactions));

    amount = NewDouble("Enter amount of money (2 decimal places): ");
    var category = NewNumber("1 - Food\n2 - Car\n3 - Concerts\n4 - Sport\n5 - Kids\n", 5);
    var description = "standard transaction";
    if (ChooseOptionText("Do you want to leave description as standard transaction? ") == false)
        description = NewString("Enter description: ");
    var datetime = DateTime.Now;

    if (ChooseTwoOptions("1 - current transaction\n2 - earlier transaction\n"))
        datetime = NewDateTime("transaction");

    transactions[id] = Tuple.Create(key, account, amount, description, false, category, datetime);
    do
    {
        id++;
    } while (TransactionExists(id, transactions));
    transactions[id] = Tuple.Create(key, account2, amount, description, true, category, datetime);

    var user = users[key];
    var myList = new List<double>() { user.Item4[0] - amount, user.Item4[1], user.Item4[2] };
    if (account == 1)
        myList = new List<double>() { user.Item4[0], user.Item4[1] - amount, user.Item4[2] };
    else if (account == 2)
        myList = new List<double>() { user.Item4[0], user.Item4[1], user.Item4[2] - amount };

    users[key] = Tuple.Create(user.Item1, user.Item2, user.Item3, myList);

    user = users[key];
    myList = new List<double>() { user.Item4[0] + amount, user.Item4[1], user.Item4[2] };
    if (account2 == 1)
        myList = new List<double>() { user.Item4[0], user.Item4[1] + amount, user.Item4[2] };
    else if (account2 == 2)
        myList = new List<double>() { user.Item4[0], user.Item4[1], user.Item4[2] + amount };

    users[key] = Tuple.Create(user.Item1, user.Item2, user.Item3, myList);

    Console.WriteLine("Money sent uccessfully.");
}
static void SendMoney(int key, Dictionary<int, Tuple<string, string, DateOnly, List<double>>> users, Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>> transactions)
{
    Console.Write($"Choose account you are sending money from.\n1 - Tekući\n2 - Žiro\n3 - Prepaid\n");
    var account = ChooseOption(new List<int>() { 1, 2, 3 }) - 1;

    var key2 = 0;
    do
    {
        key2 = NewNumber("Enter valid ID of person you are sending money to: ");
    } while (TransactionExists(key2, transactions) == false && key2==key);

    Console.Write($"Choose account you are sending money to.\n1 - Tekući\n 2 - Žiro\n3 - Prepaid\n");
    var account2 = ChooseOption(new List<int>() { 1, 2, 3 }) - 1;


    var amount = NewDouble("Enter amount of money (2 decimal places): ");
    var category = NewNumber("1 - Food\n2 - Car\n3 - Concerts\n4 - Sport\n5 - Kids\n", 5);
    var description = "standard transaction";
    if (ChooseOptionText("Do you want to leave description as standard transaction? ") == false)
        description = NewString("Enter description: ");
    var datetime = DateTime.Now;

    if (!ChooseTwoOptions("\n1 - current transaction\n2 - earlier transaction\n"))
        datetime = NewDateTime("transaction");

    var id = 0;
    do
    {
        id++;
    } while (TransactionExists(id, transactions));
    transactions[id] = Tuple.Create(key, account, amount, description, false, category, datetime);

    var user = users[key];
    var myList = new List<double>() { user.Item4[0] - amount, user.Item4[1], user.Item4[2] };
    if (account == 1)
        myList = new List<double>() { user.Item4[0], user.Item4[1] - amount, user.Item4[2] };
    else if (account == 2)
        myList = new List<double>() { user.Item4[0], user.Item4[1], user.Item4[2] - amount };

    users[key] = Tuple.Create(user.Item1, user.Item2, user.Item3, myList);

    do
    {
        id++;
    } while (TransactionExists(id, transactions));
    transactions[id] = Tuple.Create(key2, account2, amount, description, true, category, datetime);

    user = users[key2];
    myList = new List<double>() { user.Item4[0] + amount, user.Item4[1], user.Item4[2] };
    if (account == 1)
        myList = new List<double>() { user.Item4[0], user.Item4[1] + amount, user.Item4[2] };
    else if (account == 2)
        myList = new List<double>() { user.Item4[0], user.Item4[1], user.Item4[2] + amount };

    users[key2] = Tuple.Create(user.Item1, user.Item2, user.Item3, myList);

    Console.WriteLine("Money sent uccessfully.");
}



var users = new Dictionary<int, Tuple<string, string, DateOnly, List<double>>>() 
{
    {134, Tuple.Create("Ante","Tol",new DateOnly(2000,10,23),new List<double>{108,11,-8.88})},
    {12,Tuple.Create("Luka","Beko",new DateOnly(1977,3,7), new List<double>{56.1,-3,23.75}) },
    {27,Tuple.Create("Luka","Beko",new DateOnly(1981,11,23), new List<double>{0,0,73.75}) }
};
var transactions = new Dictionary<int, Tuple<int, int, double, string, bool, int, DateTime>>() 
{
    {13, Tuple.Create(134,0,25.00,"standard transaction",true,1,new DateTime(2022,11,3,20,15,3))},
    {16, Tuple.Create(134,0,25.45,"standard transaction",true,2,new DateTime(2024,1,3,16,1,4))},
    {53, Tuple.Create(12,1,254.00,"standard transaction",true,1,new DateTime(2023,11,9,0,0,0))},
    {143, Tuple.Create(27,2,12.75,"standard transaction",true,3,new DateTime(2012,3,3,20,5,0))},
    {29, Tuple.Create(27,0,16.00,"standard transaction",true,3,new DateTime(2012,12,6,0,15,3))}
};

MainMenu(users, transactions);