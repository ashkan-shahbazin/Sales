using Sales.Persistence.NH.Tests.Integration.QuotasTransactions.Charges.Steps;
using TestStack.BDDfy;
using Xunit;

namespace Sales.Persistence.NH.Tests.Integration.QuotasTransactions.Charges
{
    public class ChargeRepositoryTests
    {
        private readonly ChargeSteps _step;

        public ChargeRepositoryTests()
        {
            _step = new ChargeSteps();
        }

        [Fact]
        public void should_save_charge()
        {
            this.Given(a => _step.SaveActualProductAndAssignSpecificProduct())
                .And(a => _step.SaveContractAndAssignProduct())
                .And(a => _step.SaveSegmentAndAssignProduct())
                .And(a => _step.SaveSaleAgentAndSpecifySegmentAndContract())
                .And(a => _step.SaveQuotaForSaleAgent())
                .And(a => _step.BuildQuotaTransactionItemForCharge())
                .When(a => _step.SaveCharge())
                .Then(a => _step.SavedProperlyOfCharge())
                .BDDfy();
        }
        
        [Fact]
        public void should_save_charge_build_base_on_sale_draft_order_rejected()
        {
            this.Given(a => _step.SaveActualProductAndAssignSpecificProduct())
                .And(a => _step.SaveContractAndAssignProduct())
                .And(a => _step.SaveSegmentAndAssignProduct())
                .And(a => _step.SaveSaleAgentAndSpecifySegmentAndContract())
                .And(a => _step.SaveQuotaForSaleAgent())
                .And(a => _step.BuildQuotaTransactionItemForCharge())
                .When(a => _step.SaveChargeBuildBaseOnSaleDraftOrderRejected())
                .Then(a => _step.SavedProperlyOfCharge())
                .BDDfy();
        }
    }
}
