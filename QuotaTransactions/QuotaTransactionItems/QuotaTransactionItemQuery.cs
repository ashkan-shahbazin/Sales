using Framework.Domain;

namespace Sales.Query.Model.Model.QuotaTransactions.QuotaTransactionItems
{
    public class QuotaTransactionItemQuery
    {
        public long ProductId { get; private set; }
        public string ProductName { get; private set; }
        public string PersianProductName { get; private set; }
        public string ItemCode { get; private set; }
        public long BrandId { get; private set; }
        public string BrandName { get; private set; }
        public long SegmentId { get; private set; }
        public string SegmentName { get; private set; }
        public long Count { get; private set; }
        public decimal Price { get; private set; }

        public QuotaTransactionItemQuery(long productId, string productName, string persianProductName, string itemCode, long brandId, string brandName, long segmentId, string segmentName, long count, decimal price)
        {
            ProductId = productId;
            ProductName = productName;
            PersianProductName = persianProductName;
            ItemCode = itemCode;
            BrandId = brandId;
            BrandName = brandName;
            SegmentId = segmentId;
            SegmentName = segmentName;
            Count = count;
            Price = price;
        }

        protected QuotaTransactionItemQuery() { }
    }
}