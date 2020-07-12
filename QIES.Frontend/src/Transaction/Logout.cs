using QIES.Common.Record;

namespace QIES.Frontend.Transaction
{
    public class LogoutRequest
    {
    }

    public class Logout
    {
        private const TransactionCode Code = TransactionCode.EOS;

        public static (TransactionRecord, string) MakeTransaction(LogoutRequest request) => (new TransactionRecord(Code), "Logged out.");
    }
}
