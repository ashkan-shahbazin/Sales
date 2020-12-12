
using Framework.Core.Exceptions;
using Sales.Core.ExceptionCodes;

namespace Sales.Domain.Model.QuotaTransactions.Exceptions
{
    public class ThisProductQuotaItemIsNotAssignToAnySaleAgentSegmentException : BusinessException
    {
        public ThisProductQuotaItemIsNotAssignToAnySaleAgentSegmentException(long productId, string persianProductName, string itemCode):
            base(SalesExceptionCodes.ThisProductQuotaItemIsNotAssignToAnySaleAgentSegmentException
                , $"{SalesExceptionMessages.ThisProductQuotaItemIsNotAssignToAnySaleAgentSegmentException}\n ProductId: {productId}\n Name: {persianProductName}\nItemCode: {itemCode}")
        {
        }
    }
}