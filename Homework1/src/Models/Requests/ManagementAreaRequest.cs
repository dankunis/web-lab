using Qoden.Validation;

namespace Homework1.Models.Requests
{
    public class ManagementAreaRequest : IValidate
    {
        public string UserEmail { get; set; }
        public string ManagerEmail { get; set; }

        public void Validate(IValidator validator)
        {
            validator.CheckDataMember(this, x => x.UserEmail).NotEmpty().IsEmail();
            validator.CheckDataMember(this, x => x.ManagerEmail).NotEmpty().IsEmail();
        }
    }
}