using Qoden.Validation;

namespace Homework1.Models.Requests
{
    public class SendMessageRequest : IValidate
    {
        public int ChatId { get; set; }
        public string Message { get; set; }

        public void Validate(IValidator validator)
        {
            validator.CheckDataMember(this, x => x.Message).NotEmpty();
        }
    }
}