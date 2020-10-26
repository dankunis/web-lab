using System;

namespace Homework1.Models.Responses
{
    public class MessageResponse
    {
        public int ChatId { get; set; }

        public string UserEmail { get; set; }

        public string Text { get; set; }

        public DateTime InvitedAt { get; set; }
    }
}