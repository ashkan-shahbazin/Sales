namespace Sales.Gateway.Facade.Contracts.QuotaTransactions.Commands
{
    public class QuotaInfoCommand
    {
        public string ItemCods { get; set; }
        public int Count { get; set; }

        public QuotaInfoCommand(string itemCods, int count)
        {
            ItemCods = itemCods;
            Count = count;
        }
    }
}