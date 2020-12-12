using System;
using Framework.Core.Exceptions;
using Sales.Core.ExceptionCodes;

namespace Sales.Domain.Model.QuotaTransactions.Exceptions
{
    public class ThisProductQuotaItemIsNotAssignToAnySegmentException : BusinessException
    {
        public ThisProductQuotaItemIsNotAssignToAnySegmentException(long productId,string persianProductName,string itemCode) 
            : base(SalesExceptionCodes.ThisProductQuotaItemIsNotAssignToAnySegmentException
                , $"{SalesExceptionMessages.ThisProductQuotaItemIsNotAssignToAnySegmentException} \n ProductId: {productId}\n Name: {persianProductName}\nItemCode: {itemCode}")
        {
        }
    }
}