using Framework.Core.Exceptions;
using Sales.Core.ExceptionCodes;

namespace Sales.Domain.Model.QuotaTransactions.Exceptions
{
    public class LogicalBalanceOfItemsAreLessThanCountOfQuotaItemRequestedException : BusinessException
    {
        public LogicalBalanceOfItemsAreLessThanCountOfQuotaItemRequestedException(string message)
            : base(SalesExceptionCodes.LogicalBalanceOfItemsAreLessThanCountOfQuotaItemRequestedException
                , $"{SalesExceptionMessages.LogicalBalanceOfItemsAreLessThanCountOfQuotaItemRequestedException} \n" +
                  $" {message}")
        {

        }
    }
}

   