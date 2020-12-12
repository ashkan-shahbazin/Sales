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
using Sales.Domain.Services.QuotaTransactions.Charges;

namespace Sales.Domain.Model.QuotaTransactions.Charges
{
    public class Charge : QuotaTransaction
    {
        private Charge(QuotaTransactionsId id, IClock createOn, SaleAgentInfo saleAgentInfo, QuotaId quotaId, Reason reason, IClaimHelper claimHelper, List<QuotaTransactionItem> quotaTransactionItems, IChargeTransactionDomainService chargeTransactionDomainService, IEventPublisher eventPublisher)
        : base(id, createOn, saleAgentInfo, quotaId, reason, claimHelper, quotaTransactionItems, eventPublisher)
        {
            GuardAgainstLogicalBalanceOfItemsAreLessThanCountOfQuotaItemRequested(quotaTransactionItems, chargeTransactionDomainService);
            var saleAgentInfoEvent = MapToSaleAgentInfoEvent();
            var quotaTransactionItemsEvent = MapToQuotaTransactionItemEvent();
            Publish(new Charged(Id.DbId, CreateOn, saleAgentInfoEvent, quotaId.DbId, reason, quotaTransactionItemsEvent, CreatorUserId, claimHelper.GetUserName()));
        }

        public static Charge BuildCharge(QuotaTransactionsId id, IClock createOn, SaleAgentInfo saleAgentInfo, QuotaId quotaId, Reason reason, IClaimHelper claimHelper, List<QuotaTransactionItem> quotaTransactionItems, IChargeTransactionDomainService chargeTransactionDomainService, IEventPublisher eventPublisher)
        {
            return new Charge(id, createOn, saleAgentInfo, quotaId, reason, claimHelper, quotaTransactionItems, chargeTransactionDomainService, eventPublisher);
        }
        private static void GuardAgainstLogicalBalanceOfItemsAreLessThanCountOfQuotaItemRequested(List<QuotaTransactionItem> quotaTransactionItems,
            IChargeTransactionDomainService chargeTransactionDomainService)
        {
            var stocksThatDoesNotInventory = chargeTransactionDomainService.GetTheStocksThatDoesNotInventory(quotaTransactionItems);
            if (stocksThatDoesNotInventory.Any())
            {
                var messages = BuildExceptionMessages(stocksThatDoesNotInventory);
                throw new LogicalBalanceOfItemsAreLessThanCountOfQuotaItemRequestedException(messages.Separate());
            }
        }

        private static List<string> BuildExceptionMessages(List<InventoryStockInfo> inventoriesStocksInfo)
        {
            var messages = new List<string>();
            foreach (var inventoryStockInfo in inventoriesStocksInfo)
            {
                string message = $"{inventoryStockInfo.StockId}-{inventoryStockInfo.ItemCode}-{inventoryStockInfo.StockName}-{inventoryStockInfo.Count}";
                messages.Add(message);
            }

            return messages;
        }

        private Charge(QuotaTransactionsId id, IClock createOn, SaleAgentInfo saleAgentInfo, QuotaId quotaId, Reason reason, IClaimHelper claimHelper, List<QuotaTransactionItem> quotaTransactionItems, IEventPublisher eventPublisher)
            : base(id, createOn, saleAgentInfo, quotaId, reason, claimHelper, quotaTransactionItems, eventPublisher)
        {
            var saleAgentInfoEvent = MapToSaleAgentInfoEvent();
            var quotaTransactionItemsEvent = MapToQuotaTransactionItemEvent();
            Publish(new ChargedBaseOnSaleDraftOrderRejected(Id.DbId, CreateOn, saleAgentInfoEvent, quotaId.DbId, reason, quotaTransactionItemsEvent, CreatorUserId, claimHelper.GetUserName()));
        }

        public static Charge BuildBaseOnSaleDraftOrderRejected(QuotaTransactionsId id, IClock createOn, SaleAgentInfo saleAgentInfo, QuotaId quotaId, Reason reason, IClaimHelper claimHelper, List<QuotaTransactionItem> quotaTransactionItems, IEventPublisher eventPublisher)
        {
            return new Charge(id, createOn, saleAgentInfo, quotaId, reason, claimHelper, quotaTransactionItems, eventPublisher);
        }
        protected Charge() { }
    }
}