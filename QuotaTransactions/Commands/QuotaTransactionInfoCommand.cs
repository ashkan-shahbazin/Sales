using System.Collections.Generic;

namespace Sales.Gateway.Facade.Contracts.QuotaTransactions.Commands
{
    public class QuotaTransactionInfoCommand
    {
        public long SaleAgentId { get; set; }
        public List<QuotaInfoCommand> QuotasInfoCommand { get; set; }

        public QuotaTransactionInfoCommand(long saleAgentId, List<QuotaInfoCommand> quotasInfoCommand)
        {
            SaleAgentId = saleAgentId;
            QuotasInfoCommand = quotasInfoCommand;
        }
    }
}