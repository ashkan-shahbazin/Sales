using Framework.Core.Exceptions;
using Sales.Core.ExceptionCodes;

namespace Sales.Domain.Model.QuotaTransactions.Exceptions
{
    public class InventoriesOfQuotaAreLessThanCountOfQuotaItemRequestedException : BusinessException
    {
        public InventoriesOfQuotaAreLessThanCountOfQuotaItemRequestedException(string message)
            : base(SalesExceptionCodes.InventoriesOfQuotaAreLessThanCountOfQuotaItemRequestedException
                , $"{SalesExceptionMessages.InventoriesOfQuotaAreLessThanCountOfQuotaItemRequestedException} \n" +
                  $" {message}")
        {
        }
    }
}