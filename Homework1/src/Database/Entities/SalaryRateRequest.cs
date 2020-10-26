using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Homework1.Database.Entities
{
    public class SalaryRateRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid RequestId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        
        public decimal ValueRate { get; set; }
        
        public string Description { get; set; }
        
        public DateTime InvitedAt { get; set; }
        
        public Status Status { get; set; }

        public int? ManagerId { get; set; }
        public User Manager { get; set; }

        public string Reply { get; set; }

        public string InternalComment { get; set; }
    }

    public enum Status
    {
        Considered,
        Approved,
        NotApproved
    }
}