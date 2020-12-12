using System.Collections.Generic;

namespace Sales.Gateway.Facade.Contracts.QuotaTransactions.Commands.Discharges
{
    public class DischargeCommand : BaseQuotaTransactionCommand
    {
        public long SaleAgentId { get; set; }

        public DischargeCommand(long saleAgentId, List<QuotaInfoCommand> quotaInfosCommand)
            : base(quotaInfosCommand)
        {
            SaleAgentId = saleAgentId;
        }
    }
}