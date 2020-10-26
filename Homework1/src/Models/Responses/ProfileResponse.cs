using System.Collections.Generic;
using Homework1.Database.Entities;

namespace Homework1.Models.Responses
{
    public class ProfileResponse
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Patronymic { get; set; }

        public string NickName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Description { get; set; }
    }
}