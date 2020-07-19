using System.ComponentModel.DataAnnotations;
using QIES.Common.Record;

namespace QIES.Api.Models.Validation
{
    public class ServiceDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return ServiceDate.IsValid((string)value);
        }
    }
}
