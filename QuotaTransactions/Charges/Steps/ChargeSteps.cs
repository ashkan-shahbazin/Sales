using System;
using System.Collections.Generic;
using Framework.Core.Claims;
using Framework.Core.Utilities;
using Framework.Test;
using Framework.Test.TestDoubles;
using NFluent;
using NSubstitute;
using Sales.Domain.Contracts.Model.QuotaTransactions;
using Sales.Domain.Model.Contracts;
using Sales.Domain.Model.Contracts.ProductsInfo;
using Sales.Domain.Model.QuotaTransactions.Charges;
using Sales.Domain.Model.QuotaTransactions.QuotaTransactionItems;
using Sales.Domain.Model.SaleProducts.SaleActualProducts;
using Sales.Domain.Model.SaleProducts.SaleSpecificProducts;
using Sales.Domain.Model.SalesAgents;
using Sales.Domain.Model.Segments;
using Sales.Domain.Services.QuotaTransactions.Charges;
using Sales.Domain.Services.Segments;
using Sales.Domain.TestsUtil.Models.Contracts;
using Sales.Domain.TestsUtil.Models.QuotaTransactions;
using Sales.Domain.TestsUtil.Models.QuotaTransactions.Charges;
using Sales.Domain.TestsUtil.Models.SaleProducts;
using Sales.Domain.TestsUtil.Models.SalesAgents;
using Sales.Domain.TestsUtil.Models.Segments;
using Sales.Domain.TestsUtil.Models.Stores.Queries;
using Sales.Query.Model.Model.Stores;

namespace Sales.Domain.Tests.Unit.Models.QuotaTransactions.Charges.Steps
{
    public class ChargeSteps
    {
        private readonly ISaleActualProductRepository _saleActualProductRepository;
        private readonly ISaleSpecificProductRepository _saleSpecificProductRepository;
        private readonly ISegmentRepository _segmentRepository;
        private readonly IStoreQueryRepository _storeQueryRepository;
        private IChargeTransactionDomainService _chargeTransactionDomainService;
        private readonly IClock _clock;
        private readonly IClaimHelper _claimHelper;
        private ChargeTestBuilder _chargeTestBuilder;
        private SaleActualProduct _saleActualProduct;
        private SaleAgent _saleAgent;
        private Segment _segment;
        private Contract _contract;
        private SaleSpecificProduct _saleSpecificProduct;
        private Charge _charge;
        private SalesInventoryStoreQuery _salesInventoryStoreQuery;
        private Action _chargeOn;
        private Action _quotaTransactionItem;

        public ChargeSteps()
        {
            _saleActualProductRepository = Substitute.For<ISaleActualProductRepository>();
            _saleSpecificProductRepository = Substitute.For<ISaleSpecificProductRepository>();
            _segmentRepository = Substitute.For<ISegmentRepository>();
            _storeQueryRepository = Substitute.For<IStoreQueryRepository>();
            _clock = new ClockStub(DateTime.Now);
            _claimHelper = new ClaimHelperStub();
        }

        public void BuildActualProductAndAssignSpecificProduct()
        {
            _saleActualProduct = new SaleActualProductTestBuilder().Build();
            _saleSpecificProduct = new SaleSpecificProductTestBuilder().WithSaleActualProductId(_saleActualProduct.Id)
                .Build();
        }
        public void BuildContractAndAssignProduct()
        {
            var contractProductInfo = new ContractProductInfoTestBuilder().WithId(_saleSpecificProduct.Id.DbId)
                .WithItemCode(_saleSpecificProduct.ItemCode).Build();
            contractProductInfo.SetPrice(100000);
            var contractProductsInfo = new List<ContractProductInfo>() { contractProductInfo };
            _contract = new ContractTestBuilder().Build();
            _contract.AssignProductInfo(contractProductsInfo, _clock, _claimHelper);
        }
        public void BuildSegmentAndAssignProduct()
        {
            _saleActualProductRepository.HasActualProductWithThis(_saleActualProduct.Id.DbId).Returns(true);
            _segmentRepository.HasAnySegmentWithThisProductInfo(_saleActualProduct.Id.DbId).Returns(false);
            var segmentDomainService = new SegmentDomainService(_saleActualProductRepository, _segmentRepository);
            var productInfo = new ProductInfoTestBuilder().WithActualProductId(_saleActualProduct.Id.DbId).Build();
            _segment = new SegmentTestBuilder().Build();
            _segment.AddProductInfo(productInfo, segmentDomainService, _clock, _claimHelper);
        }

        public void BuildSaleAgentAndSpecifySegmentAndContract()
        {
            _saleAgent = new SaleAgentIndividualTestBuilder().Build();
            var saleAgentSegmentInfo = new SaleAgentSegmentInfoTestBuilder().WithSegmentId(_segment.Id.DbId)
                .WithContractId(_contract.Id.DbId).Build();
            _saleAgent.AddSegmentInfo(saleAgentSegmentInfo, _clock, _claimHelper);
        }
        public void BuildSaleAgentAndSpecifyContractWithoutSegment()
        {
            _saleAgent = new SaleAgentTestBuilder().Build();
            var saleAgentSegmentInfo = new SaleAgentSegmentInfoTestBuilder()
                .WithContractId(_contract.Id.DbId).Build();
            _saleAgent.AddSegmentInfo(saleAgentSegmentInfo, _clock, _claimHelper);
        }

        public void BuildQuotaTransactionItemForCharge()
        {
            var contracts = new List<Contract>() { _contract };
            var segments = new List<Segment>() { _segment };
            var quotaTransactionItem = new QuotaTransactionItemTestBuilder().WithActualProductId(_saleActualProduct.Id.DbId)
                .WithProductId(_saleSpecificProduct.Id.DbId).WithCount(5).WithSaleAgent(_saleAgent).WithSegments(segments)
                .WithContracts(contracts).Build();
            var quotaTransactionItems = new List<QuotaTransactionItem>() { quotaTransactionItem };
            _salesInventoryStoreQuery = new SaleInventoryStoreQueryTestBuilder().WithStockId(_saleSpecificProduct.Id.DbId).WithCountLogicalBalance(100).Build();
            var salesInventoryStoreQueries = new List<SalesInventoryStoreQuery>() { _salesInventoryStoreQuery };
            _storeQueryRepository.GetAllAvailableBy(Arg.Any<List<long>>()).Returns(salesInventoryStoreQueries);
            _chargeTransactionDomainService = new ChargeTransactionDomainService(_storeQueryRepository);
            _chargeTestBuilder = new ChargeTestBuilder().WithQuotaTransactionItem(quotaTransactionItems);
        }

        public void BuildCharge()
        {
            _charge = _chargeTestBuilder.WithReason(Reason.Ui).WithChargeQuotaTransactionDomainService(_chargeTransactionDomainService).BuildCharge();
        } 
        public void BuildChargeBaseOnSaleDraftOrderRejected()
        {
            _charge = _chargeTestBuilder.WithReason(Reason.SaleAgentDraftOrderRejected).BuildBaseOnSaleDraftOrderRejected();
        }

        public void ConstructedProperlyOfCharge()
        {
            Check.That(_charge.Id).IsEqualTo(_chargeTestBuilder.Id);
            Check.That(_charge.CreateOn).IsEqualTo(_chargeTestBuilder.CreateOn.Now());
            Check.That(_charge.QuotaId).IsEqualTo(_chargeTestBuilder.QuotaId);
            Check.That(_charge.Reason).IsEqualTo(_chargeTestBuilder.Reason);
            Check.That(_charge.SaleAgentInfo).IsEqualTo(_chargeTestBuilder.SaleAgentInfo);
            Check.That(_charge.QuotaTransactionItems).ContainsExactly(_chargeTestBuilder.QuotaTransactionItems);
            Check.That(_charge.CreatorUserId).IsNotEqualTo(null);
            Check.That(_charge.ActionTime).IsEqualTo(null);
            Check.That(_charge.ActionUserId).IsEqualTo(null);
            Check.That(_charge.IsDeleted).IsEqualTo(false);
            Check.That(_charge.IsActive).IsEqualTo(true);
        }
        public void BuildInventoryStoreWithCountOfLogicalBalance(int count)
        {
            _salesInventoryStoreQuery = new SaleInventoryStoreQueryTestBuilder().WithStockId(_saleSpecificProduct.Id.DbId).WithCountLogicalBalance(count).Build();
        }
        public void BuildQuotaTransactionItemRequestedWithCount(int count)
        {
            var contracts = new List<Contract>() { _contract };
            var segments = new List<Segment>() { _segment };
            var quotaTransactionItem = new QuotaTransactionItemTestBuilder().WithActualProductId(_saleActualProduct.Id.DbId)
                .WithProductId(_saleSpecificProduct.Id.DbId).WithCount(count).WithSaleAgent(_saleAgent).WithSegments(segments)
                .WithContracts(contracts).Build();
            var quotaTransactionItems = new List<QuotaTransactionItem>() { quotaTransactionItem };
            var salesInventoryStoreQueries = new List<SalesInventoryStoreQuery>() { _salesInventoryStoreQuery };
            _storeQueryRepository.GetAllAvailableBy(Arg.Any<List<long>>()).Returns(salesInventoryStoreQueries);
            _chargeTransactionDomainService = new ChargeTransactionDomainService(_storeQueryRepository);
            _chargeTestBuilder = new ChargeTestBuilder().WithQuotaTransactionItem(quotaTransactionItems);
        }
        public void BuildChargeOn()
        {
            _chargeOn = () => _chargeTestBuilder.WithChargeQuotaTransactionDomainService(_chargeTransactionDomainService).BuildCharge();
        }

        public void BuildSegmentWithoutAssignActualProduct()
        {
            _saleActualProductRepository.HasActualProductWithThis(_saleActualProduct.Id.DbId).Returns(true);
            _segmentRepository.HasAnySegmentWithThisProductInfo(_saleActualProduct.Id.DbId).Returns(false);
            _segment = new SegmentTestBuilder().Build();
        }

        public void BuildQuotaTransactionItem()
        {
            var segments = new List<Segment>() { _segment };
            var contracts = new List<Contract>() { _contract };
            _quotaTransactionItem = () => new QuotaTransactionItemTestBuilder().WithActualProductId(_saleActualProduct.Id.DbId)
                .WithProductId(_saleSpecificProduct.Id.DbId).WithCount(5).WithSaleAgent(_saleAgent).WithSegments(segments)
                .WithContracts(contracts).Build();
        }

        public void BuildSaleAgentAndSpecifySegmentWithoutContract()
        {
            _saleAgent = new SaleAgentTestBuilder().Build();
            var saleAgentSegmentInfo = new SaleAgentSegmentInfoTestBuilder().WithSegmentId(_segment.Id.DbId).Build();
            _saleAgent.AddSegmentInfo(saleAgentSegmentInfo, _clock, _claimHelper);
        }
        public void BuildContractAndDoNotAssignSpecificProduct()
        {
            var contractProductInfo = new ContractProductInfoTestBuilder().Build();
            contractProductInfo.SetPrice(100000);
            var contractProductsInfo = new List<ContractProductInfo>() { contractProductInfo };
            _contract = new ContractTestBuilder().Build();
            _contract.AssignProductInfo(contractProductsInfo, _clock, _claimHelper);
        }
        public void BuildContractAndAssignProductWithoutPrice()
        {
            var contractProductInfo = new ContractProductInfoTestBuilder()
                .WithId(_saleSpecificProduct.Id.DbId).WithItemCode(_saleSpecificProduct.ItemCode)
                .Build();
            var contractProductsInfo = new List<ContractProductInfo>() { contractProductInfo };
            _contract = new ContractTestBuilder().Build();
            _contract.AssignProductInfo(contractProductsInfo, _clock, _claimHelper);
        }
        public void HeGetAnErrorOfTye<T>() where T : Exception
        {
            Check.ThatCode(_quotaTransactionItem).Throws<T>();
        }
        public void HeGetAnErrorOnChargeOfTye<T>() where T : Exception
        {
            Check.ThatCode(_chargeOn).Throws<T>();
        }
    }
}