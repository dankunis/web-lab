using Qoden.Validation;

namespace Homework1.Models.Requests
{
    public class CreateUserRequest : IValidate
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Patronymic { get; set; }

        public string NickName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public string Description { get; set; }

        public string DepartmentName { get; set; }

        public void Validate(IValidator validator)
        {
            validator.CheckDataMember(this, x => x.Email).NotEmpty().IsEmail();
            validator.CheckValue(Password.Length).GreaterOrEqualTo(8);
            validator.CheckDataMember(this, x => x.NickName).NotEmpty();
            validator.CheckDataMember(this, x => x.Description).NotEmpty();
        }
    }
}