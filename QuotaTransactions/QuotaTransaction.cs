using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Framework.Core.Claims;
using Framework.Core.Events;
using Framework.Core.Utilities;
using Framework.Domain;
using Sales.Domain.Contracts.Events.QuotaTransactions;
using Sales.Domain.Contracts.Model.QuotaTransactions;
using Sales.Domain.Model.Quotas;
using Sales.Domain.Model.QuotaTransactions.QuotaTransactionItems;

namespace Sales.Domain.Model.QuotaTransactions
{
    public abstract class QuotaTransaction : AggregateRootBase<QuotaTransactionsId>
    {
        public DateTime CreateOn { get; private set; }
        public SaleAgentInfo SaleAgentInfo { get; private set; }
        public QuotaId QuotaId { get; private set; }
        public Reason Reason { get; private set; }

        private readonly IList<QuotaTransactionItem> _quotaTransactionItems;
        public IReadOnlyCollection<QuotaTransactionItem> QuotaTransactionItems => new ReadOnlyCollection<QuotaTransactionItem>(_quotaTransactionItems);

        protected QuotaTransaction(QuotaTransactionsId id, IClock clock, SaleAgentInfo saleAgentInfo, QuotaId quotaId, Reason reason, IClaimHelper claimHelper
            , List<QuotaTransactionItem> quotaTransactionItems, IEventPublisher eventPublisher)
            : base(id, eventPublisher, claimHelper.GetUserId())
        {
            CreateOn = clock.Now();
            SaleAgentInfo = saleAgentInfo;
            QuotaId = quotaId;
            Reason = reason;
            this._quotaTransactionItems = quotaTransactionItems;
        }

        protected List<QuotaTransactionItemEvent> MapToQuotaTransactionItemEvent()
        {
            var quotaTransactionItemsEvent = new List<QuotaTransactionItemEvent>();
            foreach (var quotaItem in _quotaTransactionItems)
            {
                var quotaItemEvent = new QuotaTransactionItemEvent(quotaItem.ProductId,
                    quotaItem.ProductName, quotaItem.PersianProductName, quotaItem.ItemCode, quotaItem.BrandId, quotaItem.BrandName, quotaItem.SegmentId, quotaItem.SegmentName, quotaItem.Count, quotaItem.Price);

                quotaTransactionItemsEvent.Add(quotaItemEvent);
            }

            return quotaTransactionItemsEvent;
        }
        protected SaleAgentInfoEvent MapToSaleAgentInfoEvent()
        {
            return new SaleAgentInfoEvent(SaleAgentInfo.SaleAgentId.DbId, SaleAgentInfo.FirstName, SaleAgentInfo.LastName,SaleAgentInfo.SocialSecurityNumber, SaleAgentInfo.CompanyName, SaleAgentInfo.CompanyNationalId, SaleAgentInfo.EconomicCode);
        }

        protected QuotaTransaction() { }

    }
}