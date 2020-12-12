using Sales.Domain.Model.QuotaTransactions.Exceptions;
using Sales.Domain.Tests.Unit.Models.QuotaTransactions.Charges.Steps;
using TestStack.BDDfy;
using Xunit;

namespace Sales.Domain.Tests.Unit.Models.QuotaTransactions.Charges
{
    public class ChargeTests
    {
        private readonly ChargeSteps _step;

        public ChargeTests()
        {
            _step = new ChargeSteps();
        }

        [Fact]
        public void should_be_constructed_properly_build_base_on_sale_draft_order_rejected()
        {
            this.Given(a => _step.BuildActualProductAndAssignSpecificProduct())
                .And(a => _step.BuildContractAndAssignProduct())
                .And(a => _step.BuildSegmentAndAssignProduct())
                .And(a => _step.BuildSaleAgentAndSpecifySegmentAndContract())
                .And(a => _step.BuildQuotaTransactionItemForCharge())
                .When(a => _step.BuildChargeBaseOnSaleDraftOrderRejected())
                .Then(a => _step.ConstructedProperlyOfCharge())
                .BDDfy();
        }

        [Fact]
        public void should_be_constructed_properly()
        {
            this.Given(a => _step.BuildActualProductAndAssignSpecificProduct())
                .And(a => _step.BuildContractAndAssignProduct())
                .And(a => _step.BuildSegmentAndAssignProduct())
                .And(a => _step.BuildSaleAgentAndSpecifySegmentAndContract())
                .And(a => _step.BuildQuotaTransactionItemForCharge())
                .When(a => _step.BuildCharge())
                .Then(a => _step.ConstructedProperlyOfCharge())
                .BDDfy();
        }

        [Fact]
        public void if_logical_balance_of_items_are_less_than_count_of_quota_items_requested_should_return_error()
        {
            this.Given(a => _step.BuildActualProductAndAssignSpecificProduct())
                .And(a => _step.BuildContractAndAssignProduct())
                .And(a => _step.BuildSegmentAndAssignProduct())
                .And(a => _step.BuildSaleAgentAndSpecifySegmentAndContract())
                .And(a => _step.BuildInventoryStoreWithCountOfLogicalBalance(6))
                .And(a => _step.BuildQuotaTransactionItemRequestedWithCount(10))
                .When(a => _step.BuildChargeOn())
                .Then(a => _step.HeGetAnErrorOnChargeOfTye<LogicalBalanceOfItemsAreLessThanCountOfQuotaItemRequestedException>())
                .BDDfy();
        }

        [Fact]
        public void if_actual_product_items_is_not_assign_to_any_segment_should_return_error()
        {
            this.Given(a => _step.BuildActualProductAndAssignSpecificProduct())
                .And(a => _step.BuildContractAndAssignProduct())
                .And(a => _step.BuildSegmentWithoutAssignActualProduct())
                .And(a => _step.BuildSaleAgentAndSpecifySegmentAndContract())
                .When(a => _step.BuildQuotaTransactionItem())
                .Then(a => _step.HeGetAnErrorOfTye<ThisProductQuotaItemIsNotAssignToAnySegmentException>())
                .BDDfy();
        }

        [Fact]
        public void if_product_items_is_not_assign_to_any_saleAgentSegment_should_return_error()
        {
            this.Given(a => _step.BuildActualProductAndAssignSpecificProduct())
                .And(a => _step.BuildContractAndAssignProduct())
                .And(a => _step.BuildSegmentAndAssignProduct())
                .And(a => _step.BuildSaleAgentAndSpecifyContractWithoutSegment())
                .When(a => _step.BuildQuotaTransactionItem())
                .Then(a => _step.HeGetAnErrorOfTye<ThisProductQuotaItemIsNotAssignToAnySaleAgentSegmentException>())
                .BDDfy();
        }

        [Fact]
        public void if_saleAgent_at_this_segment_does_not_have_any_contract_should_return_error()
        {
            this.Given(a => _step.BuildActualProductAndAssignSpecificProduct())
                .And(a => _step.BuildContractAndAssignProduct())
                .And(a => _step.BuildSegmentAndAssignProduct())
                .And(a => _step.BuildSaleAgentAndSpecifySegmentWithoutContract())
                .When(a => _step.BuildQuotaTransactionItem())
                .Then(a => _step.HeGetAnErrorOfTye<SaleAgentThatAtThisSegmentDoesNotHaveAnyContractException>())
                .BDDfy();
        }

        [Fact]
        public void if_specific_product_is_not_assigned_to_this_contract_attached_to_this_segment_related_to_saleAgent_should_return_error()
        {
            this.Given(a => _step.BuildActualProductAndAssignSpecificProduct())
                .And(a => _step.BuildContractAndDoNotAssignSpecificProduct())
                .And(a => _step.BuildSegmentAndAssignProduct())
                .And(a => _step.BuildSaleAgentAndSpecifySegmentAndContract())
                .When(a => _step.BuildQuotaTransactionItem())
                .Then(a => _step.HeGetAnErrorOfTye<ProductIsNotAssignedToThisContractAttachedToThisSegmentRelatedToSaleAgentException>())
                .BDDfy();
        }
        
        [Fact]
        public void if_there_is_no_price_for_this_product_in_this_contract_should_return_error()
        {
            this.Given(a => _step.BuildActualProductAndAssignSpecificProduct())
                .And(a => _step.BuildContractAndAssignProductWithoutPrice())
                .And(a => _step.BuildSegmentAndAssignProduct())
                .And(a => _step.BuildSaleAgentAndSpecifySegmentAndContract())
                .When(a => _step.BuildQuotaTransactionItem())
                .Then(a => _step.HeGetAnErrorOfTye<ThereIsNoPriceForThisProductInThisContractException>())
                .BDDfy();
        }

        //[Fact]
        //public void if_product_items_is_not_assign_to_any_segment_should_return_error_2()
        //{
        //    this.Given(a => _step.BuildSegmentWithoutAssignActualProduct())
        //        .When(a => _step.BuildQuotaTransactionItem())
        //        .Then(a => _step.ItGetAnErrorOfTye<ThisProductQuotaItemIsNotAssignToAnySegmentException>())
        //        .BDDfy();
        //}
    }
}
