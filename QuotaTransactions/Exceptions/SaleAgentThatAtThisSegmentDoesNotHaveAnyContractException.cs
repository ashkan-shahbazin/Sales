
using Framework.Core.Exceptions;
using Sales.Core.ExceptionCodes;

namespace Sales.Domain.Model.QuotaTransactions.Exceptions
{
    public class SaleAgentThatAtThisSegmentDoesNotHaveAnyContractException : BusinessException
    {
        public SaleAgentThatAtThisSegmentDoesNotHaveAnyContractException(long saleAgentId,long segmentId,string segmentName)
            :base(SalesExceptionCodes.SaleAgentThatAtThisSegmentDoesNotHaveAnyContractException
                , $"{SalesExceptionMessages.SaleAgentThatAtThisSegmentDoesNotHaveAnyContractException} \n SaleAgentId: {saleAgentId} \n SegmentId: {segmentId} \n SegmentName: {segmentName}")
        {
        }
    }
}