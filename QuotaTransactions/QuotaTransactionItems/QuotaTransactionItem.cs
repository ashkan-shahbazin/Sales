using System.Collections.Generic;
using System.Linq;
using Framework.Domain;
using Sales.Domain.Model.Contracts;
using Sales.Domain.Model.Contracts.ProductsInfo;
using Sales.Domain.Model.QuotaTransactions.Exceptions;
using Sales.Domain.Model.SalesAgents;
using Sales.Domain.Model.Segments;

namespace Sales.Domain.Model.QuotaTransactions.QuotaTransactionItems
{
    public class QuotaTransactionItem : ValueObjectBase
    {
        public long ProductId { get; private set; }
        public string ProductName { get; private set; }
        public string PersianProductName { get; private set; }
        public string ItemCode { get; private set; }
        public long BrandId { get; private set; }
        public string BrandName { get; private set; }
        public long SegmentId { get; private set; }
        public string SegmentName { get; private set; }
        public int Count { get; private set; }
        public decimal Price { get; private set; }

        public QuotaTransactionItem(long actualProductId, long productId, string productName, string persianProductName, string itemCode, long brandId, string brandName, List<Segment> segments, SaleAgent saleAgent, int count, List<Contract> contracts)
        {
            var segment = segments.FirstOrDefault(x => x.ProductInfos.Any(z => z.ActualProductId == actualProductId));
            GuardAgainstProductQuotaItemThatIsNotAssignToAny(segment, productId, persianProductName, itemCode);
            var saleAgentSegmentInfo = saleAgent.SaleAgentSegmentInfos.FirstOrDefault(x => x.SegmentId == segment.Id.DbId);
            GuardAgainstProductQuotaItemIsNotAssignToAnySaleAgentSegment(saleAgentSegmentInfo, productId, persianProductName, itemCode);
            var contract = contracts.FirstOrDefault(x => x.Id.DbId == saleAgentSegmentInfo.ContractId);
            GuardAgainstSaleAgentThatAtThisSegmentDoesNotHaveAnyContract(saleAgent, segment, contract);
            var contractProductInfo = contract.ContractProductsInfo.FirstOrDefault(x => x.SpecificProductId == productId);
            GuardAgainstProductIsNotAssignedToThisContractAttachedToThisSegmentRelatedToSaleAgent(contractProductInfo, itemCode, contract, segment, saleAgent);
            GuardAgainstThereIsNoPriceForThisProductInThisContract(contractProductInfo, itemCode, contract);
            ProductId = productId;
            ProductName = productName;
            PersianProductName = persianProductName;
            ItemCode = itemCode;
            BrandId = brandId;
            BrandName = brandName;
            SegmentId = segment.Id.DbId;
            SegmentName = segment.Name;
            Count = count;
            Price = contractProductInfo.Price.Value;
        }

        private void GuardAgainstProductQuotaItemThatIsNotAssignToAny(Segment segment, long productId, string persianProductName, string itemCode)
        {
            if (segment == null)
                throw new ThisProductQuotaItemIsNotAssignToAnySegmentException(productId, persianProductName, itemCode);
        }
        private void GuardAgainstProductQuotaItemIsNotAssignToAnySaleAgentSegment(SaleAgentSegmentInfo saleAgentSegmentInfo, long productId, string persianProductName, string itemCode)
        {
            if (saleAgentSegmentInfo == null)
                throw new ThisProductQuotaItemIsNotAssignToAnySaleAgentSegmentException(productId, persianProductName, itemCode);
        }

        private static void GuardAgainstSaleAgentThatAtThisSegmentDoesNotHaveAnyContract(SaleAgent saleAgent, Segment segment, Contract contract)
        {
            if (contract == null)
                throw new SaleAgentThatAtThisSegmentDoesNotHaveAnyContractException(saleAgent.Id.DbId, segment.Id.DbId, segment.Name);
        }
        private void GuardAgainstProductIsNotAssignedToThisContractAttachedToThisSegmentRelatedToSaleAgent(
            ContractProductInfo contractProductInfo, string itemCode, Contract contract, Segment segment, SaleAgent saleAgent)
        {
            if (contractProductInfo == null)
            {
                var saleAgentName = GetSaleAgentNameFrom(saleAgent);
                throw new ProductIsNotAssignedToThisContractAttachedToThisSegmentRelatedToSaleAgentException(itemCode,
                    contract, segment, saleAgent.Id.DbId, saleAgentName);
            }
        }

        private static string GetSaleAgentNameFrom(SaleAgent saleAgent)
        {
            if (saleAgent.GetType() == typeof(SaleAgentIndividual))
            {
                var saleAgentIndividual = saleAgent as SaleAgentIndividual;
                return $"{saleAgentIndividual.FirstName} {saleAgentIndividual.LastName}";
            }

            var saleAgentLegal = saleAgent as SaleAgentLegal;
            return saleAgentLegal.CompanyName;
        }

        private void GuardAgainstThereIsNoPriceForThisProductInThisContract(ContractProductInfo contractProductInfo, string itemCode
            , Contract contract)
        {
            if (!contractProductInfo.Price.HasValue)
                throw new ThereIsNoPriceForThisProductInThisContractException(itemCode, contract.Id.DbId);
        }
        protected QuotaTransactionItem() { }
    }
}