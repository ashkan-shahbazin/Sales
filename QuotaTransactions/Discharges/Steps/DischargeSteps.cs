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
using Sales.Domain.TestsUtil.Models.QuotaTransactions;
using Sales.Domain.TestsUtil.Models.QuotaTransactions.Charges;
using Sales.Domain.TestsUtil.Models.QuotaTransactions.Discharges;
using Sales.Domain.TestsUtil.Models.SaleProducts;
using Sales.Domain.TestsUtil.Models.SalesAgents;
using Sales.Domain.TestsUtil.Models.Segments;
using Sales.Domain.TestsUtil.Models.Stores.Queries;
using Sales.Query.Model.Model.Stores;

namespace Sales.Domain.Tests.Unit.Models.QuotaTransactions.Discharges.Steps
{
    public class DischargeSteps
    {
        private readonly ISaleActualProductRepository _saleActualProductRepository;
        private readonly ISaleSpecificProductRepository _saleSpecificProductRepository;
        private readonly ISegmentRepository _segmentRepository;
        private readonly IStoreQueryRepository _storeQueryRepository;
        private readonly IQuotaTransactionRepository _quotaTransactionRepository;
        private IDischargeTransactionDomainService _dischargeTransactionDomainService;
        private IChargeTransactionDomainService _chargeTransactionDomainService;
        private readonly IClock _clock;
        private readonly IClaimHelper _claimHelper;
        private ChargeTestBuilder _chargeTestBuilder;
        private DischargeTestBuilder _dischargeTestBuilder;
        private SaleActualProduct _saleActualProduct;
        private SaleAgent _saleAgent;
        private Segment _segment;
        private Contract _contract;
        private SaleSpecificProduct _mobileM20;
        private SaleSpecificProduct _mobileM30;
        private SaleSpecificProduct _mobileM10;
        private Charge _charge;
        private Discharge _discharge;
        private SalesInventoryStoreQuery _salesInventoryMobileM10StoreQuery;
        private SalesInventoryStoreQuery _salesInventoryMobileM20StoreQuery;
        private SalesInventoryStoreQuery _salesInventoryMobileM30StoreQuery;
        private List<Contract> _contracts;
        private List<Segment> _segments;
        private Action _dischargeOn;

        public DischargeSteps()
        {
            _saleActualProductRepository = Substitute.For<ISaleActualProductRepository>();
            _saleSpecificProductRepository = Substitute.For<ISaleSpecificProductRepository>();
            _segmentRepository = Substitute.For<ISegmentRepository>();
            _storeQueryRepository = Substitute.For<IStoreQueryRepository>();
            _quotaTransactionRepository = Substitute.For<IQuotaTransactionRepository>();
            _clock = new ClockStub(DateTime.Now);
            _claimHelper = new ClaimHelperStub();
        }

        public void BuildActualProductAndAssignSpecificProductsWithIds(long mobileM20Id, long mobileM30Id, long mobileM10Id)
        {
            _saleActualProduct = new SaleActualProductTestBuilder().Build();
            _mobileM10 = new SaleSpecificProductTestBuilder().WithSaleActualProductId(_saleActualProduct.Id)
                .Build();
            _mobileM20 = new SaleSpecificProductTestBuilder().WithSaleActualProductId(_saleActualProduct.Id)
                .Build();
            _mobileM30 = new SaleSpecificProductTestBuilder().WithSaleActualProductId(_saleActualProduct.Id)
                .Build();
        }
        public void BuildContractAndAssignProducts()
        {
            var mobileM10ContractProductInfo = new ContractProductInfoTestBuilder().WithId(_mobileM10.Id.DbId)
                .WithItemCode(_mobileM10.ItemCode).Build();
            mobileM10ContractProductInfo.SetPrice(100000);

            var mobileM20ContractProductInfo = new ContractProductInfoTestBuilder().WithId(_mobileM20.Id.DbId)
                .WithItemCode(_mobileM20.ItemCode).Build();
            mobileM20ContractProductInfo.SetPrice(100000);

            var mobileM30ContractProductInfo = new ContractProductInfoTestBuilder().WithId(_mobileM30.Id.DbId)
                .WithItemCode(_mobileM30.ItemCode).Build();
            mobileM30ContractProductInfo.SetPrice(100000);
            var contractProductsInfo = new List<ContractProductInfo>() { mobileM10ContractProductInfo, mobileM20ContractProductInfo, mobileM30ContractProductInfo };
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

        public void BuildThreeQuotaTransactionItemForChargeWithCount(int mobileM10Count, int mobileM20Count, int mobileM30Count)
        {
            _contracts = new List<Contract>() { _contract };
            _segments = new List<Segment>() { _segment };
            var _mobileM10quotaTransactionItem = new QuotaTransactionItemTestBuilder().WithActualProductId(_saleActualProduct.Id.DbId)
                .WithProductId(_mobileM10.Id.DbId).WithItemCode(_mobileM10.ItemCode).WithProductId(_mobileM10.Id.DbId).WithCount(mobileM10Count)
                .WithSaleAgent(_saleAgent).WithSegments(_segments)
                .WithContracts(_contracts).Build();

            var _mobileM20quotaTransactionItem = new QuotaTransactionItemTestBuilder().WithActualProductId(_saleActualProduct.Id.DbId)
                .WithProductId(_mobileM20.Id.DbId).WithItemCode(_mobileM20.ItemCode).WithProductId(_mobileM20.Id.DbId).WithCount(mobileM20Count)
                .WithSaleAgent(_saleAgent).WithSegments(_segments).WithContracts(_contracts).Build();

            var _mobileM30quotaTransactionItem = new QuotaTransactionItemTestBuilder().WithActualProductId(_saleActualProduct.Id.DbId)
                .WithProductId(_mobileM30.Id.DbId).WithItemCode(_mobileM30.ItemCode).WithProductId(_mobileM30.Id.DbId).WithCount(mobileM30Count)
                .WithSaleAgent(_saleAgent).WithSegments(_segments).WithContracts(_contracts).Build();
            var quotaTransactionItems = new List<QuotaTransactionItem>() { _mobileM10quotaTransactionItem, _mobileM20quotaTransactionItem, _mobileM30quotaTransactionItem };
            _salesInventoryMobileM10StoreQuery = new SaleInventoryStoreQueryTestBuilder().WithStockId(_mobileM10.Id.DbId).WithCountLogicalBalance(100).Build();
            _salesInventoryMobileM20StoreQuery = new SaleInventoryStoreQueryTestBuilder().WithStockId(_mobileM20.Id.DbId).WithCountLogicalBalance(100).Build();
            _salesInventoryMobileM30StoreQuery = new SaleInventoryStoreQueryTestBuilder().WithStockId(_mobileM30.Id.DbId).WithCountLogicalBalance(100).Build();
            var salesInventoryStoreQueries = new List<SalesInventoryStoreQuery>() { _salesInventoryMobileM10StoreQuery, _salesInventoryMobileM20StoreQuery, _salesInventoryMobileM30StoreQuery };
            _storeQueryRepository.GetAllAvailableBy(Arg.Any<List<long>>()).Returns(salesInventoryStoreQueries);
            _chargeTransactionDomainService = new ChargeTransactionDomainService(_storeQueryRepository);
            _chargeTestBuilder = new ChargeTestBuilder().WithQuotaTransactionItem(quotaTransactionItems);
        }
        public void BuildThreeQuotaTransactionItemForDischargeWithCount(int mobileM10Count, int mobileM20Count, int mobileM30Count)
        {
            var mobileM10QuotaTransactionItem = new QuotaTransactionItemTestBuilder().WithActualProductId(_saleActualProduct.Id.DbId)
                .WithProductId(_mobileM10.Id.DbId).WithItemCode(_mobileM10.ItemCode).WithCount(mobileM10Count).WithSaleAgent(_saleAgent).WithSegments(_segments)
                .WithContracts(_contracts).Build();

            var mobileM20QuotaTransactionItem = new QuotaTransactionItemTestBuilder().WithActualProductId(_saleActualProduct.Id.DbId)
                .WithProductId(_mobileM20.Id.DbId).WithItemCode(_mobileM20.ItemCode).WithCount(mobileM20Count).WithSaleAgent(_saleAgent).WithSegments(_segments)
                .WithContracts(_contracts).Build();

            var mobileM30QuotaTransactionItem = new QuotaTransactionItemTestBuilder().WithActualProductId(_saleActualProduct.Id.DbId)
                .WithProductId(_mobileM30.Id.DbId).WithItemCode(_mobileM30.ItemCode).WithCount(mobileM30Count).WithSaleAgent(_saleAgent).WithSegments(_segments)
                .WithContracts(_contracts).Build();
            var quotaTransactionItems = new List<QuotaTransactionItem>() { mobileM10QuotaTransactionItem, mobileM20QuotaTransactionItem, mobileM30QuotaTransactionItem };
            
            var quotaTransactions = new List<QuotaTransaction>() { _charge };
            
            _quotaTransactionRepository.GetAllAvailableBy(_charge.QuotaId).Returns(quotaTransactions);
            _dischargeTransactionDomainService = new DischargeTransactionDomainService(_quotaTransactionRepository);
            _dischargeTestBuilder = new DischargeTestBuilder().WithQuotaId(_charge.QuotaId).WithQuotaTransactionItem(quotaTransactionItems);
        }

        public void BuildCharge()
        {
            _charge = _chargeTestBuilder.WithChargeQuotaTransactionDomainService(_chargeTransactionDomainService).BuildCharge();
        }
        public void BuildDischarge()
        {
            _discharge = _dischargeTestBuilder.WithReason(Reason.Ui).WithDischargeQuotaTransactionDomainService(_dischargeTransactionDomainService).BuildDischarge();
        }
        public void BuildDischargeBaseOnSaleDraftOrderConfirmedDischarge()
        {
            _discharge = _dischargeTestBuilder.WithReason(Reason.SaleAgentDraftOrderCreated).BuildBaseOnSaleDraftOrderConfirmedDischarge();
        }
        public void BuildDischargeOn()
        {
            _dischargeOn =() => _dischargeTestBuilder.WithDischargeQuotaTransactionDomainService(_dischargeTransactionDomainService).BuildDischarge();
        }

        public void ConstructedProperlyOfDischarge()
        {
            Check.That(_discharge.Id).IsEqualTo(_dischargeTestBuilder.Id);
            Check.That(_discharge.CreateOn).IsEqualTo(_dischargeTestBuilder.CreateOn.Now());
            Check.That(_discharge.QuotaId).IsEqualTo(_dischargeTestBuilder.QuotaId);
            Check.That(_discharge.QuotaTransactionItems).ContainsExactly(_dischargeTestBuilder.QuotaTransactionItems);
            Check.That(_discharge.CreatorUserId).IsNotEqualTo(null);
            Check.That(_discharge.ActionTime).IsEqualTo(null);
            Check.That(_discharge.ActionUserId).IsEqualTo(null);
            Check.That(_discharge.IsDeleted).IsEqualTo(false);
            Check.That(_discharge.IsActive).IsEqualTo(true);
        }

        public void HeGetAnErrorOnChargeOfTye<T>() where T : Exception
        {
            Check.ThatCode(_dischargeOn).Throws<T>();
        }
    }
}