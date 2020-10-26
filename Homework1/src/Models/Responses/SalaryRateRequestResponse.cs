using System;
using Homework1.Database.Entities;
using Qoden.Validation;

namespace Homework1.Models.Responses
{
    public class SalaryRateRequestResponse
    {
        public decimal ValueRate { get; set; }

        public string Description { get; set; }

        public DateTime InvitedAt { get; set; }

        public Status Status { get; set; }
    }
}