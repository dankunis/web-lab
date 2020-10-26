using Qoden.Validation;

namespace Homework1.Models
{
    public interface IValidate
    {
        void Validate(IValidator validator);
    }
}