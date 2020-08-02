using System.ComponentModel.DataAnnotations;
using QIES.Common.Record;

namespace QIES.Api.Models.Validation
{
    public class NumberTicketsAttribute : ValidationAttribute
    {
        public override bool IsValid(object value) => NumberTickets.IsValid(int.Parse((string)value));
    }
}