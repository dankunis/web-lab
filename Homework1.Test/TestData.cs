using System;
using System.Linq;
using Homework1.Database.Entities;

namespace Homework1.Test
{
    public static class TestData
    {
        public static User User = new User()
        {
            Id = 1,
            FirstName = "user",
            LastName = "user",
            Patronymic = "user",
            NickName = "user",
            Password = "12345",
            Email = "user@gmail.com",
            PhoneNumber = "0-000-000-00-00",
            InvitedAt = DateTime.Parse("2019-03-18"),
            Description = "I'm user"
        };

        public static User User2 = new User()
        {
            Id = 7,
            FirstName = "user2",
            LastName = "user2",
            Patronymic = "user2",
            NickName = "user2",
            Password = "12345",
            Email = "user2@gmail.com",
            PhoneNumber = "0-000-000-00-00",
            InvitedAt = DateTime.Parse("2019-03-18"),
            Description = "I'm user"
        };

        public static User Admin = new User()
        {
            Id = 3,
            FirstName = "admin",
            LastName = "admin",
            Patronymic = "admin",
            NickName = "admin",
            Password = "12345",
            Email = "admin@gmail.com",
            PhoneNumber = "0-000-000-00-00",
            InvitedAt = DateTime.Parse("2019-03-18"),
            Description = "I'm admin",
        };

        public static User Manager = new User()
        {
            Id = 2,
            FirstName = "manager",
            LastName = "manager",
            Patronymic = "manager",
            NickName = "manager",
            Password = "12345",
            Email = "manager@gmail.com",
            PhoneNumber = "0-000-000-00-00",
            InvitedAt = DateTime.Parse("2019-03-18"),
            Description = "I'm manager",
        };

        public static User ModifyUser() => new User()
        {
            FirstName = "modifyUser",
            LastName = "modifyUser",
            Patronymic = "modifyUser",
            NickName = RandomString(12),
            Email = RandomString(12) + "@gmail.com",
            Password = "12345",
            PhoneNumber = "0-000-000-00-00",
            InvitedAt = DateTime.Parse("2019-02-03"),
            Description = "I'm user"
        };

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}