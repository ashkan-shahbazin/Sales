using System.Collections.Generic;
using Framework.Application;

namespace Sales.Gateway.Facade.Contracts.QuotaTransactions.Commands
{
    public class BaseQuotaTransactionCommand : ICommand
    {
        public List<QuotaInfoCommand> QuotaInfosCommand { get; set; }

        public BaseQuotaTransactionCommand(List<QuotaInfoCommand> quotaInfosCommand)
        {
            QuotaInfosCommand = quotaInfosCommand;
        }
    }
}