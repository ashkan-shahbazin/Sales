using System;
using Framework.Core.Exceptions;
using Sales.Core.ExceptionCodes;
using Sales.Domain.Model.Contracts;
using Sales.Domain.Model.Segments;

namespace Sales.Domain.Model.QuotaTransactions.Exceptions
{
    public class ProductIsNotAssignedToThisContractAttachedToThisSegmentRelatedToSaleAgentException : BusinessException
    {
        public ProductIsNotAssignedToThisContractAttachedToThisSegmentRelatedToSaleAgentException(string itemCode, Contract contract,Segment segment,long saleAgentId, string saleAgentName)
        :base(SalesExceptionCodes.ProductIsNotAssignedToThisContractAttachedToThisSegmentRelatedToSaleAgentException,
            $"{SalesExceptionMessages.ProductIsNotAssignedToThisContractAttachedToThisSegmentRelatedToSaleAgentException} \n itemCode: {itemCode} \n contractId: {contract.Id.DbId} \n  ContractTitle: {contract.Title} \n SegmentId: {segment.Id.DbId} \n SegmentName: {segment.Name} \n SegmentAgentId: {saleAgentId} \n SaleAgentName: {saleAgentName}")
        {
        }
    }
}