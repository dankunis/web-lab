using Qoden.Validation;

namespace Homework1.Models.Requests
{
    public class CreateRateRequestRequest : IValidate
    {
        public decimal ValueRate { get; set; }

        public string Description { get; set; }

        public void Validate(IValidator validator)
        {
            validator.CheckDataMember(this, x => x.ValueRate).Greater(0, "Rate should be greater than 0");
            validator.CheckDataMember(this, x => x.Description).NotEmpty();
        }
    }
}