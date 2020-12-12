
using Framework.Core.Exceptions;
using Sales.Core.ExceptionCodes;

namespace Sales.Application.QuotaTransactions.Exceptions
{
    public class InvalidProductQuotaException : BusinessException
    {
        public InvalidProductQuotaException(string itemCode)
            :base(SalesExceptionCodes.InvalidProductQuotaException
                , $"{SalesExceptionMessages.InvalidProductQuotaException} \n ItemCode: {itemCode}")
        {
        }
    }
}