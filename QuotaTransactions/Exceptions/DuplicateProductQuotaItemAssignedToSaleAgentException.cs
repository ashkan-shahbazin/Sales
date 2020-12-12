
using Framework.Core.Exceptions;
using Sales.Core.ExceptionCodes;

namespace Sales.Application.QuotaTransactions.Exceptions
{
    public class DuplicateProductQuotaItemAssignedToSaleAgentException : BusinessException
    {
        public DuplicateProductQuotaItemAssignedToSaleAgentException(string message)
            : base(SalesExceptionCodes.DuplicateProductQuotaItemAssignedToSaleAgentException
                , $"{SalesExceptionMessages.DuplicateProductQuotaItemAssignedToSaleAgentException} \n " +
                  $" { message}")
        {
        }
    }
}