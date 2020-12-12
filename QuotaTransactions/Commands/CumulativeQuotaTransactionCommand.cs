namespace Sales.Gateway.Facade.Contracts.QuotaTransactions.Commands
{
    public class CumulativeQuotaTransactionCommand
    {
        public long SaleAgentId { get; set; }
        public string ItemCode { get; set; }
        public int Count { get; set; }

        public CumulativeQuotaTransactionCommand(long saleAgentId, string itemCode, int count)
        {
            SaleAgentId = saleAgentId;
            ItemCode = itemCode;
            Count = count;
        }
    }
}