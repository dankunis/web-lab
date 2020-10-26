using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Homework1.Database.Entities
{
    public class Role
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public RoleNames Name { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }

    public enum RoleNames
    {
        User = 0,
        Manager = 1,
        Admin = 2
    }
}