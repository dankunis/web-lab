using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Homework1.Database.Entities
{
    public class UserChat
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int ChatId { get; set; }
        public Chat Chat { get; set; }

        public bool Flag { get; set; }
    }
}