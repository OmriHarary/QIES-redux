using System.ComponentModel.DataAnnotations;
using QIES.Common.Records;

namespace QIES.Api.Models.Validation
{
    public class ServiceNameAttribute : ValidationAttribute
    {
        public override bool IsValid(object value) => ServiceName.IsValid((string)value);
    }
}
