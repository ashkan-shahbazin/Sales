using System.Collections.Generic;
using Framework.Core.Claims;
using Framework.Core.Events;
using Framework.Core.Utilities;
using Sales.Domain.Contracts.Model.QuotaTransactions;
using Sales.Domain.Model.Contracts;
using Sales.Domain.Model.Quotas;
using Sales.Domain.Model.QuotaTransactions.Charges;
using Sales.Domain.Model.QuotaTransactions.Discharges;
using Sales.Domain.Model.QuotaTransactions.QuotaTransactionItems;
using Sales.Domain.Model.SalesAgents;
using Sales.Domain.Model.Segments;
using Sales.Domain.Services.QuotaTransactions.Charges;
using Sales.Domain.Services.QuotaTransactions.Discharges;

namespace Sales.Domain.Model.QuotaTransactions.Factories
{
    public static class Factory
    {
        public static Charge BuildCharge(QuotaTransactionsId id, IClock clock, SaleAgentInfo saleAgentInfo, QuotaId quotaId, Reason reason, List<QuotaTransactionItem> quotaTransactionItems, IChargeTransactionDomainService chargeTransactionDomainService, IClaimHelper claimHelper, IEventPublisher publisher)
        {
            return Charge.BuildCharge(id, clock, saleAgentInfo, quotaId, reason, claimHelper, quotaTransactionItems, chargeTransactionDomainService, publisher);
        }
        public static Charge BuildChargeBaseOnSaleDraftOrderRejected(QuotaTransactionsId id, IClock clock, SaleAgentInfo saleAgentInfo, QuotaId quotaId, Reason reason, List<QuotaTransactionItem> quotaTransactionItems, IClaimHelper claimHelper, IEventPublisher publisher)
        {
            return Charge.BuildBaseOnSaleDraftOrderRejected(id, clock, saleAgentInfo, quotaId, reason, claimHelper, quotaTransactionItems, publisher);
        }
        public static Discharge BuildDischarge(QuotaTransactionsId id, IClock clock, SaleAgentInfo saleAgentInfo, QuotaId quotaId, Reason reason, List<QuotaTransactionItem> quotaTransactionItems, IDischargeTransactionDomainService dischargeTransactionDomainService, IEventPublisher publisher, IClaimHelper claimHelper)
        {
            return Discharge.BuildDischarge(id, clock, saleAgentInfo, quotaId, reason, quotaTransactionItems, dischargeTransactionDomainService, claimHelper, publisher);
        }
        public static Discharge BuildDischargeBaseOnSaleDraftOrderCreated(QuotaTransactionsId id, IClock clock, SaleAgentInfo saleAgentInfo, QuotaId quotaId, Reason reason, List<QuotaTransactionItem> quotaTransactionItems, IEventPublisher publisher, IClaimHelper claimHelper)
        {
            return Discharge.BuildDischargeBaseOnSaleDraftOrderCreated(id, clock, saleAgentInfo, quotaId, reason, quotaTransactionItems, claimHelper, publisher);
        }

        public static QuotaTransactionItem CreateQuotaTransactionItem(long actualProductQueryId, long productId, string productName, string persianProductName, string itemCode, long brandId, string brandName, List<Segment> segments, SaleAgent saleAgent, int count, List<Contract> contracts)
        {
            return new QuotaTransactionItem(actualProductQueryId, productId, productName, persianProductName, itemCode, brandId, brandName, segments, saleAgent, count, contracts);
        }

        public static SaleAgentInfo CreateSaleAgentIndividualInfo(SaleAgentId saleAgentId, string firstName, string lastName,string socialSecurityNumber)
        {
            return SaleAgentInfo.BuildForIndividual(saleAgentId, firstName, lastName, socialSecurityNumber);
        }
        public static SaleAgentInfo CreateSaleAgentLegalInfo(SaleAgentId saleAgentId, string companyName, string companyNationalId, string economicCode)
        {
            return SaleAgentInfo.BuildForLegal(saleAgentId, companyName, companyNationalId, economicCode);
        }
    }
}