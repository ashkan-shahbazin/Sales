using Framework.Core.Utilities;
using Framework.Test;
using NFluent;
using NSubstitute;
using Sales.Domain.TestsUtil.Models.QuotaTransactions;
using Xunit;

namespace Sales.Domain.Tests.Unit.Models.QuotaTransactions
{
    public class QuotaTransactionTests
    {
        private readonly IClock _clock;
        private readonly long _userId;
        public QuotaTransactionTests()
        {
            _clock = Substitute.For<IClock>();
            _userId = GenerateRandom.Number();
        }
        [Fact]
        public void should_be_constructed_properly()
        {
            var builder = new QuotaTransactionTestBuilder();

            var quotaTransaction = builder.Build();

            Check.That(quotaTransaction.Id).IsEqualTo(builder.Id);
            Check.That(quotaTransaction.CreateOn).IsEqualTo(builder.CreateOn.Now());
            Check.That(quotaTransaction.SaleAgentInfo).IsEqualTo(builder.SaleAgentInfo);
            Check.That(quotaTransaction.QuotaId).IsEqualTo(builder.QuotaId);
            Check.That(quotaTransaction.QuotaTransactionItems).ContainsExactly(builder.QuotaTransactionItems);
            Check.That(quotaTransaction.CreatorUserId).IsNotEqualTo(null);
            Check.That(quotaTransaction.ActionTime).IsEqualTo(null);
            Check.That(quotaTransaction.ActionUserId).IsEqualTo(null);
            Check.That(quotaTransaction.IsDeleted).IsEqualTo(false);
            Check.That(quotaTransaction.IsActive).IsEqualTo(true);
        }
    }
}
