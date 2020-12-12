using System.Collections.Generic;
using System.Linq;
using Framework.Core.Claims;
using Framework.Core.Events;
using Framework.Core.Utilities;
using Sales.Domain.Contracts.Events.QuotaTransactions;
using Sales.Domain.Contracts.Model.QuotaTransactions;
using Sales.Domain.Model.Quotas;
using Sales.Domain.Model.QuotaTransactions.Exceptions;
using Sales.Domain.Model.QuotaTransactions.QuotaTransactionItems;
using Sales.Domain.Services.QuotaTransactions.Discharges;

namespace Sales.Domain.Model.QuotaTransactions.Discharges
{
    public class Discharge : QuotaTransaction
    {
        private Discharge(QuotaTransactionsId id, IClock createOn, SaleAgentInfo saleAgentInfo, QuotaId quotaId, Reason reason, List<QuotaTransactionItem> quotaTransactionItems
            , IDischargeTransactionDomainService dischargeTransactionDomainService, IClaimHelper claimHelper, IEventPublisher eventPublisher)
        : base(id, createOn, saleAgentInfo, quotaId, reason, claimHelper, quotaTransactionItems, eventPublisher)
        {
            GuardAgainstInventoryOfQuotaAreLessThanCountOfQuotaItemRequested(quotaId, quotaTransactionItems, dischargeTransactionDomainService);
            var saleAgentInfoEvent = MapToSaleAgentInfoEvent();
            var quotaTransactionItemsEvent = MapToQuotaTransactionItemEvent();
            Publish(new Discharged(Id.DbId, CreateOn, saleAgentInfoEvent, quotaId.DbId, reason, quotaTransactionItemsEvent, CreatorUserId, claimHelper.GetUserName()));
        }

        public static Discharge BuildDischarge(QuotaTransactionsId id, IClock createOn, SaleAgentInfo saleAgentInfo, QuotaId quotaId, Reason reason, List<QuotaTransactionItem> quotaTransactionItems, IDischargeTransactionDomainService dischargeTransactionDomainService, IClaimHelper claimHelper, IEventPublisher eventPublisher)
        {
            return new Discharge(id, createOn, saleAgentInfo, quotaId, reason, quotaTransactionItems, dischargeTransactionDomainService, claimHelper, eventPublisher);
        }
        private static void GuardAgainstInventoryOfQuotaAreLessThanCountOfQuotaItemRequested(QuotaId quotaId, List<QuotaTransactionItem> quotaTransactionItems,
            IDischargeTransactionDomainService dischargeTransactionDomainService)
        {
            var stocksThatDoesNotInventory = dischargeTransactionDomainService.GetTheStocksThatDoesNotInventoryInQuotas(quotaId, quotaTransactionItems);
            if (stocksThatDoesNotInventory.Any())
            {
                var messages = BuildExceptionMessages(stocksThatDoesNotInventory);
                throw new InventoriesOfQuotaAreLessThanCountOfQuotaItemRequestedException(messages.Separate());
            }
        }

        private static List<string> BuildExceptionMessages(List<InventoryStockInfo> inventoriesStocksInfo)
        {
            var messages = new List<string>();
            foreach (var inventoryStockInfo in inventoriesStocksInfo)
            {
                string message = $"StockId: {inventoryStockInfo.StockId} - ItemCode: {inventoryStockInfo.ItemCode} - StockName:{inventoryStockInfo.StockName} - Count: {inventoryStockInfo.Count}";
                messages.Add(message);
            }

            return messages;
        }
        private Discharge(QuotaTransactionsId id, IClock createOn, SaleAgentInfo saleAgentInfo, QuotaId quotaId, Reason reason, List<QuotaTransactionItem> quotaTransactionItems
            , IClaimHelper claimHelper, IEventPublisher eventPublisher)
            : base(id, createOn, saleAgentInfo, quotaId, reason, claimHelper, quotaTransactionItems, eventPublisher)
        {
            var saleAgentInfoEvent = MapToSaleAgentInfoEvent();
            var quotaTransactionItemsEvent = MapToQuotaTransactionItemEvent();
            Publish(new DischargedBaseOnSaleDraftOrderCreated(Id.DbId, CreateOn, saleAgentInfoEvent, quotaId.DbId, reason, quotaTransactionItemsEvent, CreatorUserId, claimHelper.GetUserName()));
        }

        public static Discharge BuildDischargeBaseOnSaleDraftOrderCreated(QuotaTransactionsId id, IClock createOn, SaleAgentInfo saleAgentInfo, QuotaId quotaId, Reason reason, List<QuotaTransactionItem> quotaTransactionItems, IClaimHelper claimHelper, IEventPublisher eventPublisher)
        {
            return new Discharge(id, createOn, saleAgentInfo, quotaId, reason, quotaTransactionItems, claimHelper, eventPublisher);
        }
        protected Discharge() { }
    }
}