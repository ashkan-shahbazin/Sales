using System.Collections.Generic;

namespace Sales.Gateway.Facade.Contracts.QuotaTransactions.Commands.Charges
{
    public class ChargeBaseOnDraftOrderForSaleAgentRejectedCommand : BaseQuotaTransactionCommand
    {
        public string SocialSecurityNumber { get; set; }
        public string EconomicCode { get; set; }

        public ChargeBaseOnDraftOrderForSaleAgentRejectedCommand(string socialSecurityNumber, string economicCode, List<QuotaInfoCommand> quotaInfosCommand)
            : base(quotaInfosCommand)
        {
            SocialSecurityNumber = socialSecurityNumber;
            EconomicCode = economicCode;
        }
    }
}