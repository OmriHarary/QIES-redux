using System.ComponentModel.DataAnnotations;
using QIES.Common.Record;

namespace QIES.Api.Models.Validation
{
    public class ServiceDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value) => ServiceDate.IsValid((string)value);
    }
}
