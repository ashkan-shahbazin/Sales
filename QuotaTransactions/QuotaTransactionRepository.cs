using System.Collections.Generic;
using System.Linq;
using Framework.Core.Events;
using Framework.NH;
using NHibernate;
using Sales.Domain.Model.Quotas;
using Sales.Domain.Model.QuotaTransactions;

namespace Sales.Persistence.NH.Repositories.QuotaTransactions
{
    public class QuotaTransactionRepository : IQuotaTransactionRepository
    {
        private readonly ISession _session;
        private readonly IEventPublisher _publisher;
        public QuotaTransactionRepository(ISession session, IEventPublisher publisher)
        {
            _session = session;
            _publisher = publisher;
        }

        public QuotaTransactionsId GetNextId()
        {
            var idValue = _session.GetNextSequence("QuotaTransactionsSeq");
            return new QuotaTransactionsId(idValue);
        }

        public T Get<T>(QuotaTransactionsId id) where T : QuotaTransaction
        {
            var reservationTransaction = _session.Query<T>().FirstOrDefault(x => x.Id.DbId == id.DbId); ;
            if (reservationTransaction != null)
                reservationTransaction.SetPublisher(_publisher);

            return reservationTransaction;
        }

        public void Create<T>(T aggregate) where T : QuotaTransaction
        {
            _session.Save(aggregate);
        }

        public List<QuotaTransaction> GetAllAvailableBy(QuotaId quotaId)
        {
            return _session.Query<QuotaTransaction>()
                .Where(x => x.QuotaId.DbId == quotaId.DbId).ToList();
        }
    }
}
