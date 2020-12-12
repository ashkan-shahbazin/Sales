using System.Collections.Generic;

namespace Sales.Gateway.Facade.Contracts.QuotaTransactions.Commands.Discharges
{
    public class DischargeBaseOnSaleAgentDraftOrderCreatedCommand : BaseQuotaTransactionCommand
    {
        public string SocialSecurityNumber { get; set; }
        public string EconomicCode { get; set; }

        public DischargeBaseOnSaleAgentDraftOrderCreatedCommand(string socialSecurityNumber, string economicCode, List<QuotaInfoCommand> quotaInfosCommand)
            : base(quotaInfosCommand)
        {
            SocialSecurityNumber = socialSecurityNumber;
            EconomicCode = economicCode;
        }
    }
}
