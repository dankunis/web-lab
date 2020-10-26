using System.Collections.Generic;

namespace Homework1.Models.Responses
{
    public class ChatResponse
    {
        public int ChatId { get; set; }

        public string Name { get; set; }

        public List<MessageResponse> Messages { get; set; }

        public UserInfoResponse Admin { get; set; }

        public List<UserInfoResponse> Users { get; set; }
    }
}