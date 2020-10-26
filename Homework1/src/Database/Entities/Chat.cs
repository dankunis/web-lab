using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Homework1.Database.Entities
{
    public class Chat
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public int AdminId { get; set; }
        public User Admin { get; set; }

        public ICollection<Message> Messages { get; set; }
        public ICollection<UserChat> UserChats { get; set; }
    }
}