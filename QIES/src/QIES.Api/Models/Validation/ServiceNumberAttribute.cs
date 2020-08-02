using System.ComponentModel.DataAnnotations;
using QIES.Common.Record;

namespace QIES.Api.Models.Validation
{
    public class ServiceNumberAttribute : ValidationAttribute
    {
        public bool AllowNull { get; }

        public ServiceNumberAttribute(bool allowNull = false)
        {
            this.AllowNull = allowNull;
        }

        public override bool IsValid(object value) => value is null ? AllowNull : ServiceNumber.IsValid((string)value);
    }
}
