using System;
using System.Collections.Generic;
using Framework.Core.Claims;
using Framework.Core.Events;
using Framework.Core.Utilities;
using Framework.Test;
using Framework.Test.TestDoubles;
using NFluent;
using NSubstitute;
using Sales.Domain.Contracts.Model.QuotaTransactions;
using Sales.Domain.Model.Contracts;
using Sales.Domain.Model.Contracts.ProductsInfo;
using Sales.Domain.Model.Quotas;
using Sales.Domain.Model.QuotaTransactions;
using Sales.Domain.Model.QuotaTransactions.Charges;
using Sales.Domain.Model.QuotaTransactions.Discharges;
using Sales.Domain.Model.QuotaTransactions.QuotaTransactionItems;
using Sales.Domain.Model.SaleProducts.SaleActualProducts;
using Sales.Domain.Model.SaleProducts.SaleSpecificProducts;
using Sales.Domain.Model.SalesAgents;
using Sales.Domain.Model.Segments;
using Sales.Domain.Services.QuotaTransactions.Charges;
using Sales.Domain.Services.QuotaTransactions.Discharges;
using Sales.Domain.Services.Segments;
using Sales.Domain.TestsUtil.Models.Contracts;
using Sales.Domain.TestsUtil.Models.ProductsQuery;
using Sales.Domain.TestsUtil.Models.Quotas;
using Sales.Domain.TestsUtil.Models.QuotaTransactions;
using Sales.Domain.TestsUtil.Models.QuotaTransactions.Charges;
using Sales.Domain.TestsUtil.Models.QuotaTransactions.Discharges;
using Sales.Domain.TestsUtil.Models.SaleProducts;
using Sales.Domain.TestsUtil.Models.SalesAgents;
using Sales.Domain.TestsUtil.Models.Segments;
using Sales.Domain.TestsUtil.Models.Stores.Queries;
using Sales.Persistence.NH.Repositories.Contracts;
using Sales.Persistence.NH.Repositories.Quotas;
using Sales.Persistence.NH.Repositories.QuotaTransactions;
using Sales.Persistence.NH.Repositories.SaleProducts;
using Sales.Persistence.NH.Repositories.SalesAgents;
using Sales.Persistence.NH.Repositories.Segments;
using Sales.Query.Model.Model.Products;
using Sales.Query.Model.Model.Products.Actuals;
using Sales.Query.Model.Model.Products.Specifics;
using Sales.Query.Model.Model.Stores;

namespace Sales.Persistence.NH.Tests.Integration.QuotasTransactions.Discharges.Steps
{
    public class DischargeSteps : BasePersistenceTest
    {
        private readonly ISaleActualProductRepository _saleActualProductRepository;
        private readonly ISaleSpecificProductRepository _saleSpecificProductRepository;
        private readonly IContractRepository _contractRepository;
        private readonly ISegmentRepository _segmentRepository;
        private readonly ISaleAgentRepository _saleAgentRepository;
        private readonly IStoreQueryRepository _storeQueryRepository;
        private readonly IQuotaTransactionRepository _quotaTransactionRepository;
        private readonly IQuotaRepository _quotaRepository;
        private readonly IEventPublisher _publisher;
        private IChargeTransactionDomainService _chargeTransactionDomainService;
        private IDischargeTransactionDomainService _dischargeTransactionDomainService;
        private readonly IClock _clock;
        private readonly IClaimHelper _claimHelper;
        private ChargeTestBuilder _chargeTestBuilder;
        private DischargeTestBuilder _dischargeTestBuilder;
        private SaleActualProduct _saleActualProduct;
        private SaleAgent _saleAgent;
        private Segment _segment;
        private Contract _contract;
        private SaleSpecificProduct _saleSpecificProduct;
        private Charge _charge;
        private Discharge _discharge;
        private Quota _quota;
        private SalesInventoryStoreQuery _salesInventoryStoreQuery;
        private List<Contract> _contracts;
        private List<Segment> _segments;

        public DischargeSteps()
        {
            _saleActualProductRepository = new SaleActualProductRepository(Session, _publisher);
            _saleSpecificProductRepository = new SaleSpecificProductRepository(Session, _publisher);
            _publisher = new FakeEventPublisher(); ;
            _contractRepository = new ContractRepository(Session, _publisher);
            _segmentRepository = new SegmentRepository(Session, _publisher);
            _saleAgentRepository = new SaleAgentRepository(Session, _publisher);
            _quotaTransactionRepository = new QuotaTransactionRepository(Session, _publisher);
            _quotaRepository = new QuotaRepository(Session, _publisher);
            _storeQueryRepository = Substitute.For<IStoreQueryRepository>();
            _clock = new ClockStub(DateTime.Now);
            _claimHelper = new ClaimHelperStub();
        }

        public void SaveActualProductAndAssignSpecificProduct()
        {
            Session.BeginTransaction();
            _saleActualProduct = new SaleActualProductTestBuilder().Build();
            _saleActualProductRepository.Create(_saleActualProduct);
            Session.Transaction.Commit();
            Session.Clear();
            Session.BeginTransaction();
            _saleSpecificProduct = new SaleSpecificProductTestBuilder().WithSaleActualProductId(_saleActualProduct.Id)
                .Build();
            _saleSpecificProductRepository.Create(_saleSpecificProduct);
            Session.Transaction.Commit();
            Session.Clear();
        }
        public void SaveContractAndAssignProduct()
        {
            Session.BeginTransaction();
            var contractProductInfo = new ContractProductInfoTestBuilder().WithId(_saleSpecificProduct.Id.DbId)
                .WithItemCode(_saleSpecificProduct.ItemCode).Build();
            contractProductInfo.SetPrice(100000);
            var contractProductsInfo = new List<ContractProductInfo>() { contractProductInfo };
            _contract = new ContractTestBuilder().Build();
            _contractRepository.Create(_contract);
            Session.Transaction.Commit();
            Session.Clear();
            Session.BeginTransaction();
            _contract = _contractRepository.GetBy(_contract.Id);
            _contract.AssignProductInfo(contractProductsInfo, _clock, _claimHelper);
            Session.Transaction.Commit();
            Session.Clear();
        }
        public void SaveSegmentAndAssignProduct()
        {
            Session.BeginTransaction();
            var segmentDomainService = new SegmentDomainService(_saleActualProductRepository, _segmentRepository);
            var productInfo = new ProductInfoTestBuilder().WithActualProductId(_saleActualProduct.Id.DbId).Build();
            _segment = new SegmentTestBuilder().Build();
            _segmentRepository.Create(_segment);
            Session.Transaction.Commit();
            Session.Clear();
            Session.BeginTransaction();
            _segment = _segmentRepository.GetBy(_segment.Id);
            _segment.AddProductInfo(productInfo, segmentDomainService, _clock, _claimHelper);
            Session.Transaction.Commit();
            Session.Clear();
        }

        public void SaveSaleAgentAndSpecifySegmentAndContract()
        {
            Session.BeginTransaction();
            _saleAgent = new SaleAgentIndividualTestBuilder().Build();
            _saleAgentRepository.Create(_saleAgent);
            Session.Transaction.Commit();
            Session.Clear();
            Session.BeginTransaction();
            var saleAgentSegmentInfo = new SaleAgentSegmentInfoTestBuilder().WithSegmentId(_segment.Id.DbId)
                .WithContractId(_contract.Id.DbId).Build();
            _saleAgent = _saleAgentRepository.Get<SaleAgentIndividual>(_saleAgent.Id);
            _saleAgent.AddSegmentInfo(saleAgentSegmentInfo, _clock, _claimHelper);
            Session.Transaction.Commit();
            Session.Clear();
        }
        public void SaveQuotaForSaleAgent()
        {
            Session.BeginTransaction();
            _quota = new QuotaTestBuilder().WithSaleAgentId(_saleAgent.Id).Build();
            _quotaRepository.Create(_quota);
            Session.Transaction.Commit();
            Session.Clear();
        }

        public void BuildQuotaTransactionItemForCharge()
        {
            _contracts = new List<Contract>() { _contract };
            _segments = new List<Segment>() { _segment };
            var quotaTransactionItem = new QuotaTransactionItemTestBuilder().WithActualProductId(_saleActualProduct.Id.DbId)
                .WithProductId(_saleSpecificProduct.Id.DbId).WithItemCode(_saleSpecificProduct.ItemCode).WithCount(5).WithSaleAgent(_saleAgent).WithSegments(_segments)
                .WithContracts(_contracts).Build();
            var quotaTransactionItems = new List<QuotaTransactionItem>() { quotaTransactionItem };
            _salesInventoryStoreQuery = new SaleInventoryStoreQueryTestBuilder().WithStockId(_saleSpecificProduct.Id.DbId).WithCountLogicalBalance(100).Build();
            var salesInventoryStoreQueries = new List<SalesInventoryStoreQuery>() { _salesInventoryStoreQuery };
            _storeQueryRepository.GetAllAvailableBy(Arg.Any<List<long>>()).Returns(salesInventoryStoreQueries);
            _chargeTransactionDomainService = new ChargeTransactionDomainService(_storeQueryRepository);
            _chargeTestBuilder = new ChargeTestBuilder().WithQuotaTransactionItem(quotaTransactionItems);
        }

        public void SaveCharge()
        {
            Session.BeginTransaction();
            _charge = _chargeTestBuilder.WithQuotaId(_quota.Id).WithChargeQuotaTransactionDomainService(_chargeTransactionDomainService).BuildCharge();
            _quotaTransactionRepository.Create(_charge);
            Session.Transaction.Commit();
            Session.Clear();
        }
        public void BuildQuotaTransactionItemForDischarge()
        {
            var mobileM10QuotaTransactionItem = new QuotaTransactionItemTestBuilder().WithActualProductId(_saleActualProduct.Id.DbId)
                .WithProductId(_saleSpecificProduct.Id.DbId).WithItemCode(_saleSpecificProduct.ItemCode).WithCount(4).WithSaleAgent(_saleAgent).WithSegments(_segments)
                .WithContracts(_contracts).Build();
            var quotaTransactionItems = new List<QuotaTransactionItem>() { mobileM10QuotaTransactionItem };
            _dischargeTransactionDomainService = new DischargeTransactionDomainService(_quotaTransactionRepository);
            _dischargeTestBuilder = new DischargeTestBuilder().WithQuotaId(_charge.QuotaId).WithQuotaTransactionItem(quotaTransactionItems);
        }
        public void SaveDischarge()
        {
            Session.BeginTransaction();
            _discharge = _dischargeTestBuilder.WithQuotaId(_quota.Id).WithReason(Reason.Ui).WithDischargeQuotaTransactionDomainService(_dischargeTransactionDomainService).BuildDischarge();
            _quotaTransactionRepository.Create(_discharge);
            Session.Transaction.Commit();
            Session.Clear();
        }
        public void SaveDischargeBuildBaseOnSaleDraftOrderConfirmedDischarge()
        {
            Session.BeginTransaction();
            _discharge = _dischargeTestBuilder.WithQuotaId(_quota.Id).WithReason(Reason.SaleAgentDraftOrderCreated).BuildBaseOnSaleDraftOrderConfirmedDischarge();
            _quotaTransactionRepository.Create(_discharge);
            Session.Transaction.Commit();
            Session.Clear();
        }
        public void SavedProperlyOfDischarge()
        {
            Session.BeginTransaction();
            var expectedDischarge = _quotaTransactionRepository.Get<Discharge>(_discharge.Id);
            Session.Transaction.Commit();
            Session.Clear();

            Check.That(expectedDischarge).IsEqualTo(_discharge);
        }
    }
}