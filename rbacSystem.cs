using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

class Program
{
    //ArrayList
    static List<User> users = new List<User>();
    static User loggedInUser = null;
    static string? generatedToken = null;

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("[RBAC-SYSTEM]: Welcome... \n1. Login\n2. Register\n3. Exit");
            Console.Write("Choose an option: ");
            int num1 = Convert.ToInt32(Console.ReadLine());

            switch (num1)
            {
                case 1: Login(); break;
                case 2: Register(); break;
                case 3: Console.Write("[RBAC-SYSTEM]: Exit ..."); return;
                default: Console.WriteLine("Invalid input. Choose from 1 to 3 only.");
                break;
            }
        }
    }

    static void Register()
    {
        Console.Write("First Name: "); 
        string firstName = Console.ReadLine();
        Console.Write("Last Name: "); 
        string lastName = Console.ReadLine();
        Console.Write("Middle Name (Optional): "); 
        string middleName = Console.ReadLine();
        Console.Write("Contact Number: "); 
        string contact = Console.ReadLine();
        Console.Write("Email: "); 
        string email = Console.ReadLine();
        Console.Write("Username: "); 
        string username = Console.ReadLine();
        Console.Write("Password: "); 
        string password = Console.ReadLine();
        Console.Write("Role (User/Admin): "); 
        string role = Console.ReadLine();

        if (!IsValidName(firstName) || !IsValidName(lastName) || !IsValidName(middleName))
        {
            Console.WriteLine("Name must contain letters only. \nRegistration failed. Try Again.");
            return;
        }

        if (!IsValidContact(contact))
        {
            Console.WriteLine("Contact number must start with '09' and be 11 characters long. \nRegistration failed. Try Again.");
            return;
        }
        
        if (!IsValidEmail(email))
        {
            Console.WriteLine("Email must contain '@' and end with 'gmail.com'. \nRegistration failed. Try Again.");
            return;
        }
        
        if (!IsValidUsername(username))
        {
            Console.WriteLine("Username must be between 5 to 20 characters and unique. \n Registration failed. Try Again.");
            return;
        }

        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(username))
        {
            Console.WriteLine("Password or Username cannot be empty! \nRegistration failed. Try Again.");
            return;
        }
        
        if (!IsValidPassword(password))
        {
            Console.WriteLine("Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one number. \nRegistration failed. Try again.");
            return;
        }
        
        if (string.IsNullOrWhiteSpace(role) || (role != "User" && role != "Admin"))
        {
            Console.WriteLine("Invalid input. Type User or Admin only. \nRegistration failed. Try Again.");
            return;
        }
        if (users.Any(u => u.Username == username || u.Email == email))
        {
            Console.WriteLine("Username or Email already exists! Registration failed. Try Again.");
            return;
        }

        users.Add(new User(firstName, lastName, middleName, contact, email, username, password, role));
        Console.WriteLine("Registration successful!");
    }

    static void Login()
    {
        int attempts = 3;
        while (attempts > 0)
        {
            Console.Write("Username: "); string username = Console.ReadLine();
            Console.Write("Password: "); string password = Console.ReadLine();

            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                loggedInUser = user;
                generatedToken = ReverseStr(password) + username;
                Console.Write("\nLogin successful!\n");
                Console.Write("Generated Token: " + generatedToken+ "\n");
                MainMenu();
                return;
            }
            else
            {
                attempts--;
                Console.WriteLine("Invalid username or password. " +attempts+ " out of 3 attempts left");
            }
        }
        Console.WriteLine("Too many failed attempts. Exiting system.");
        Environment.Exit(0);
    }

    static void MainMenu()
    {
        while (loggedInUser != null && generatedToken != null)
        {
            Console.WriteLine("\n1. View Profile\n2. View All Profiles (Admin)\n3. Change Password\n4. Logout");
            Console.Write("Choose an option: ");
            int num2= Convert.ToInt32(Console.ReadLine());

            switch (num2)
            {
                case 1: ViewProfile(); break;
                case 2: ViewAllProfiles(); break;
                case 3: ChangePassword(); break;
                case 4: Logout(); return;
                default: Console.WriteLine("Invalid input. Choose from 1 to 4 only.");
                break;
            }
        }
    }

    static void ViewProfile()
    {
        Console.WriteLine($"\nName: {loggedInUser.FirstName} {loggedInUser.MiddleName} {loggedInUser.LastName}");
        Console.WriteLine($"Contact: {loggedInUser.Contact}\nEmail: {loggedInUser.Email}\nRole: {loggedInUser.Role}");
    }

    static void ViewAllProfiles()
    {
        if (loggedInUser.Role != "Admin")
        {
            Console.WriteLine("Access Denied: This feature is for Admins only!");
            return;
        }
        
        Console.WriteLine("\nRegistered Users:");
        
        foreach (var user in users)
            Console.WriteLine($"Username: {user.Username}, Role: {user.Role}");
    }

    static void ChangePassword()
    {
        Console.Write("Enter Old Password: ");
        string oldPassword = Console.ReadLine();
        
        if (oldPassword != loggedInUser.Password)
        {
            Console.WriteLine("Incorrect Old Password!");
            return;
        }

        Console.Write("Enter New Password: ");
        string newPassword = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(newPassword))
        {
            Console.WriteLine("Password cannot be empty!");
            return;
        }

        if (!IsValidPassword(newPassword))
        {
            Console.WriteLine("Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one number. \nChanging Password Failed");
            return;
        }
        
        loggedInUser.Password = newPassword;
        generatedToken = ReverseStr(newPassword);
        Console.WriteLine("Generated Token: "+generatedToken);
        Console.WriteLine("Password changed successfully!");
    }

    static void Logout()
    {
        Console.WriteLine("Logging out...");
        loggedInUser = null;
        generatedToken = null;
    }
    
    static string ReverseStr(string str)
    {
        char[] arr = str.ToCharArray();
        Array.Reverse(arr);
        return new string(arr);
    }
    
    static bool IsValidPassword(string password)
    {
        return password.Length >= 8 &&
               Regex.IsMatch(password, @"[A-Z]") &&
               Regex.IsMatch(password, @"[a-z]") &&
               Regex.IsMatch(password, @"\d");
    }
    
    static bool IsValidContact(string contact)
    {
        return Regex.IsMatch(contact, "^09\\d{9}$");
    }
    
     static bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, "^[a-zA-Z0-9._%+-]+@gmail\\.com$");
    }
    
    static bool IsValidUsername(string username)
    {
        return username.Length >= 5 && username.Length <= 20 && !users.Any(u => u.Username == username);
    }
    
    static bool IsValidName(string name)
    {
        return Regex.IsMatch(name, "^[a-zA-Z]+$");
    }
}

class User
{
    public string FirstName, LastName, MiddleName, Contact, Email, Username, Password, Role;
    
    public User(string firstName, string lastName, string middleName, string contact, string email, string username, string password, string role)
    {
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        Contact = contact;
        Email = email;
        Username = username;
        Password = password;
        Role = role;
    }
}
