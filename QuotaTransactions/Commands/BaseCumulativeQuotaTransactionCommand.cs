using System.Collections.Generic;
using Framework.Application;

namespace Sales.Gateway.Facade.Contracts.QuotaTransactions.Commands
{
    public abstract class BaseCumulativeQuotaTransactionCommand : ICommand
    {
        public List<CumulativeQuotaTransactionCommand> CumulativeQuotaTransactionCommands { get; set; }

        protected BaseCumulativeQuotaTransactionCommand(List<CumulativeQuotaTransactionCommand> cumulativeQuotaTransactionCommands)
        {
            CumulativeQuotaTransactionCommands = cumulativeQuotaTransactionCommands;
        }
    }
}