using Sales.Persistence.NH.Tests.Integration.QuotasTransactions.Discharges.Steps;
using TestStack.BDDfy;
using Xunit;

namespace Sales.Persistence.NH.Tests.Integration.QuotasTransactions.Discharges
{
    public class DischargeRepositoryTests
    {
        private readonly DischargeSteps _step;

        public DischargeRepositoryTests()
        {
            _step = new DischargeSteps();
        }

        [Fact]
        public void should_save_Discharge()
        {
            this.Given(a => _step.SaveActualProductAndAssignSpecificProduct())
                .And(a => _step.SaveContractAndAssignProduct())
                .And(a => _step.SaveSegmentAndAssignProduct())
                .And(a => _step.SaveSaleAgentAndSpecifySegmentAndContract())
                .And(a => _step.SaveQuotaForSaleAgent())
                .And(a => _step.BuildQuotaTransactionItemForCharge())
                .And(a => _step.SaveCharge())
                .And(a => _step.BuildQuotaTransactionItemForDischarge())
                .When(a => _step.SaveDischarge())
                .Then(a => _step.SavedProperlyOfDischarge())
                .BDDfy();
        }
        
        [Fact]
        public void should_save_Discharge_base_on_sale_draft_order_confirmed()
        {
            this.Given(a => _step.SaveActualProductAndAssignSpecificProduct())
                .And(a => _step.SaveContractAndAssignProduct())
                .And(a => _step.SaveSegmentAndAssignProduct())
                .And(a => _step.SaveSaleAgentAndSpecifySegmentAndContract())
                .And(a => _step.SaveQuotaForSaleAgent())
                .And(a => _step.BuildQuotaTransactionItemForCharge())
                .And(a => _step.SaveCharge())
                .And(a => _step.BuildQuotaTransactionItemForDischarge())
                .When(a => _step.SaveDischargeBuildBaseOnSaleDraftOrderConfirmedDischarge())
                .Then(a => _step.SavedProperlyOfDischarge())
                .BDDfy();
        }
    }
}
