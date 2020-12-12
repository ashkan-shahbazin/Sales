
using Framework.Core.Exceptions;
using Sales.Core.ExceptionCodes;

namespace Sales.Domain.Model.QuotaTransactions.Exceptions
{
    public class ThereIsNoPriceForThisProductInThisContractException : BusinessException
    {
        public ThereIsNoPriceForThisProductInThisContractException(string itemCode, long contractId)
        :base(SalesExceptionCodes.ThereIsNoPriceForThisProductInThisContractException,
            $"{SalesExceptionMessages.ThereIsNoPriceForThisProductInThisContractException} \n ItemCode: {itemCode}\n ContractId: {contractId}")
        {
        }
    }
}