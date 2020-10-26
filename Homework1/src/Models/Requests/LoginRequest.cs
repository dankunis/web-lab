using Qoden.Validation;

namespace Homework1.Models.Requests
{
    public class LoginRequest : IValidate
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public void Validate(IValidator validator)
        {
            validator.CheckDataMember(this, x => x.Email).NotEmpty().IsEmail();
            validator.CheckDataMember(this, x => x.Password).NotEmpty().IsPassword();
        }
    }
}