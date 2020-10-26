using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Homework1.Models.Requests;

namespace Homework1.Database.Entities
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Patronymic { get; set; }

        public string NickName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime InvitedAt { get; set; }

        public string Description { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public int? CurrentSalaryRateId { get; set; }
        public PayrollHistory CurrentSalaryRate { get; set; }

        public ICollection<Chat> ChatAdmins { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<UserChat> UserChats { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<ManagementArea> Users { get; set; }
        public ICollection<ManagementArea> Managers { get; set; }
        public ICollection<PayrollHistory> PayrollHistory { get; set; }
        public ICollection<SalaryRateRequest> UserRequests { get; set; }
        public ICollection<SalaryRateRequest> ManagerReplies { get; set; }

        public void UpdateInfo(UpdateUserInfoRequest request)
        {
            FirstName = request.FirstName;
            LastName = request.LastName;
            Patronymic = request.Patronymic;
            NickName = request.NickName;
            Email = request.Email;
            PhoneNumber = request.PhoneNumber;
            Description = request.Description;
        }
    }
}