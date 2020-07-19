using System.ComponentModel.DataAnnotations;
using QIES.Common.Record;

namespace QIES.Api.Models.Validation
{
    public class ServiceNameAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return ServiceName.IsValid((string)value);
        }
    }
}
