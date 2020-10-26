using System.ComponentModel.DataAnnotations.Schema;

namespace Homework1.Database.Entities
{
    public class ManagementArea
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int ManagerId { get; set; }
        public User Manager { get; set; }
    }
}