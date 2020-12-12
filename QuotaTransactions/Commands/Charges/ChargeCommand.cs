using System.Collections.Generic;

namespace Sales.Gateway.Facade.Contracts.QuotaTransactions.Commands.Charges
{
    public class ChargeCommand : BaseQuotaTransactionCommand
    {
        public long SaleAgentId { get; set; }
        public ChargeCommand(long saleAgentId, List<QuotaInfoCommand> quotaInfosCommand)
            : base(quotaInfosCommand)
        {
            SaleAgentId = saleAgentId;
        }
    }
}