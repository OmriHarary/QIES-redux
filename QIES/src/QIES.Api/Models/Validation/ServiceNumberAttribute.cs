using System.ComponentModel.DataAnnotations;
using QIES.Common.Record;

namespace QIES.Api.Models.Validation
{
    public class ServiceNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return ServiceNumber.IsValid((string)value);
        }
    }
}
