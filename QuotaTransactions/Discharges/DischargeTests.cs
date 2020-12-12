using Sales.Domain.Model.QuotaTransactions.Exceptions;
using Sales.Domain.Tests.Unit.Models.QuotaTransactions.Discharges.Steps;
using TestStack.BDDfy;
using Xunit;

namespace Sales.Domain.Tests.Unit.Models.QuotaTransactions.Discharges
{
    public class DischargeTests
    {
        private readonly DischargeSteps _step;

        public DischargeTests()
        {
            _step = new DischargeSteps();
        }

        [Fact]
        public void should_be_constructed_properly_base_on_sale_draft_order_confirmed()
        {
            this.Given(a => _step.BuildActualProductAndAssignSpecificProductsWithIds(10,11,12))
                .And(a => _step.BuildContractAndAssignProducts())
                .And(a => _step.BuildSegmentAndAssignProduct())
                .And(a => _step.BuildSaleAgentAndSpecifySegmentAndContract())
                .And(a => _step.BuildThreeQuotaTransactionItemForChargeWithCount(10,10,10))
                .And(a => _step.BuildCharge())
                .And(a => _step.BuildThreeQuotaTransactionItemForDischargeWithCount(10, 10, 10))
                .When(a => _step.BuildDischargeBaseOnSaleDraftOrderConfirmedDischarge())
                .Then(a => _step.ConstructedProperlyOfDischarge())
                .BDDfy();
        }
        
        [Fact]
        public void should_be_constructed_properly()
        {
            this.Given(a => _step.BuildActualProductAndAssignSpecificProductsWithIds(10,11,12))
                .And(a => _step.BuildContractAndAssignProducts())
                .And(a => _step.BuildSegmentAndAssignProduct())
                .And(a => _step.BuildSaleAgentAndSpecifySegmentAndContract())
                .And(a => _step.BuildThreeQuotaTransactionItemForChargeWithCount(10,10,10))
                .And(a => _step.BuildCharge())
                .And(a => _step.BuildThreeQuotaTransactionItemForDischargeWithCount(10, 10, 10))
                .When(a => _step.BuildDischarge())
                .Then(a => _step.ConstructedProperlyOfDischarge())
                .BDDfy();
        }

        [Fact]
        public void if_inventories_of_quota_are_less_than_count_of_quota_item_requested_should_return_error()
        {
            this.Given(a => _step.BuildActualProductAndAssignSpecificProductsWithIds(10,11,12))
                .And(a => _step.BuildContractAndAssignProducts())
                .And(a => _step.BuildSegmentAndAssignProduct())
                .And(a => _step.BuildSaleAgentAndSpecifySegmentAndContract())
                .And(a => _step.BuildThreeQuotaTransactionItemForChargeWithCount(10,10,10))
                .And(a => _step.BuildCharge())
                .And(a => _step.BuildThreeQuotaTransactionItemForDischargeWithCount(12, 10, 10))
                .When(a => _step.BuildDischargeOn())
                .Then(a=>_step.HeGetAnErrorOnChargeOfTye<InventoriesOfQuotaAreLessThanCountOfQuotaItemRequestedException>())
                .BDDfy();
        } 
        
        [Fact]
        public void if_inventories_of_two_quota_are_less_than_count_of_quota_item_requested_should_return_error()
        {
            this.Given(a => _step.BuildActualProductAndAssignSpecificProductsWithIds(10,11,12))
                .And(a => _step.BuildContractAndAssignProducts())
                .And(a => _step.BuildSegmentAndAssignProduct())
                .And(a => _step.BuildSaleAgentAndSpecifySegmentAndContract())
                .And(a => _step.BuildThreeQuotaTransactionItemForChargeWithCount(10,10,10))
                .And(a => _step.BuildCharge())
                .And(a => _step.BuildThreeQuotaTransactionItemForDischargeWithCount(12, 20, 10))
                .When(a => _step.BuildDischargeOn())
                .Then(a=>_step.HeGetAnErrorOnChargeOfTye<InventoriesOfQuotaAreLessThanCountOfQuotaItemRequestedException>())
                .BDDfy();
        }
        
       
    }
}
