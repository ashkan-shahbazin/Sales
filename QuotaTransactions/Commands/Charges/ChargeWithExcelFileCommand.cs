using System.Collections.Generic;

namespace Sales.Gateway.Facade.Contracts.QuotaTransactions.Commands.Charges
{
    public class ChargeWithExcelFileCommand : BaseCumulativeQuotaTransactionCommand
    {

        public ChargeWithExcelFileCommand(List<CumulativeQuotaTransactionCommand> quotaTransactionsCommand)
            : base(quotaTransactionsCommand)
        {}
    }
}
