using System.Collections.Generic;

namespace Sales.Gateway.Facade.Contracts.QuotaTransactions.Commands.Discharges
{
    public class DischargeWithExcelFileCommand : BaseCumulativeQuotaTransactionCommand
    {

        public DischargeWithExcelFileCommand(List<CumulativeQuotaTransactionCommand> quotaTransactionsCommand)
            : base(quotaTransactionsCommand)
        {}
    }
}
