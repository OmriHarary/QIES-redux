using System.ComponentModel.DataAnnotations;
using QIES.Common.Records;

namespace QIES.Api.Models.Validation
{
    public class ServiceNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
            => value is string serviceNum && ServiceNumber.IsValid(serviceNum);
    }
}
