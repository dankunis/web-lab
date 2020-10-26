using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Homework1.Database.Entities
{
    public class PayrollHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; }
        
        public decimal ValueRate { get; set; }
        
        public DateTime Date { get; set; }

        // Optional
        public User CurrentUser { get; set; }
    }
}