using System.Collections.Generic;
using System.Linq;
using Sales.Application.QuotaTransactions.Exceptions;
using Sales.Domain.Model.Contracts;
using Sales.Domain.Model.QuotaTransactions;
using Sales.Domain.Model.QuotaTransactions.Factories;
using Sales.Domain.Model.QuotaTransactions.QuotaTransactionItems;
using Sales.Domain.Model.SaleProducts.SaleActualProducts;
using Sales.Domain.Model.SaleProducts.SaleSpecificProducts;
using Sales.Domain.Model.SalesAgents;
using Sales.Domain.Model.Segments;
using Sales.Gateway.Facade.Contracts.QuotaTransactions.Commands;

namespace Sales.Application.QuotaTransactions.Mappers
{
    public static class QuotaTransactionMapper
    {
        public static List<QuotaTransactionInfoCommand> MapToQuotasTransactionsInfoCommand(List<CumulativeQuotaTransactionCommand> groupedByChargeCommand)
        {
            var saleAgentIds = groupedByChargeCommand.Select(x => x.SaleAgentId).Distinct().ToList();
            var quotaTransactionsInfoCommand = new List<QuotaTransactionInfoCommand>();
            foreach (var saleAgentId in saleAgentIds)
            {
                var allChargeInfoAboutSpecificSaleAgents =
                    groupedByChargeCommand.Where(x => x.SaleAgentId == saleAgentId).ToList();

                var quotasInfoCommand = new List<QuotaInfoCommand>();
                foreach (var allChargeInfoAboutSpecificSaleAgent in allChargeInfoAboutSpecificSaleAgents)
                {
                    var quotaInfoCommand = new QuotaInfoCommand(allChargeInfoAboutSpecificSaleAgent.ItemCode,
                        allChargeInfoAboutSpecificSaleAgent.Count);
                    quotasInfoCommand.Add(quotaInfoCommand);
                }

                var chargeCommand = new QuotaTransactionInfoCommand(saleAgentId, quotasInfoCommand);
                quotaTransactionsInfoCommand.Add(chargeCommand);
            }

            return quotaTransactionsInfoCommand;
        }

        public static List<QuotaTransactionItem> MapToQuotaTransactionItems(List<SaleSpecificProduct> saleSpecificProducts, List<SaleActualProduct> saleActualProducts, List<Segment> segments, List<QuotaInfoCommand> quotasInfoCommand, SaleAgent saleAgent, List<Contract> contracts)
        {
            var quotaTransactionItems = new List<QuotaTransactionItem>();
            foreach (var quotaInfoCommand in quotasInfoCommand)
            {
                var saleSpecificProduct = saleSpecificProducts.FirstOrDefault(x => x.ItemCode == quotaInfoCommand.ItemCods);
                var saleActualProduct = saleActualProducts.FirstOrDefault(x => x.Id.DbId == saleSpecificProduct.SaleActualProductId.DbId);

                GuardAgainstInvalidProductQuota(saleSpecificProduct, quotaInfoCommand);
                var quotaTransactionItem = Factory.CreateQuotaTransactionItem(saleSpecificProduct.SaleActualProductId.DbId, saleSpecificProduct.Id.DbId,
                      saleSpecificProduct.SpecificProductName.Name, saleSpecificProduct.SpecificProductName.PersianName, saleSpecificProduct.ItemCode, saleActualProduct.BrandInfo.BrandId, saleActualProduct.BrandInfo
                    .BrandName, segments, saleAgent, quotaInfoCommand.Count, contracts);
                quotaTransactionItems.Add(quotaTransactionItem);
            }

            return quotaTransactionItems;
        }

        private static void GuardAgainstInvalidProductQuota(SaleSpecificProduct saleSpecificProduct, QuotaInfoCommand quotaInfoCommand)
        {
            if (saleSpecificProduct == null)
                throw new InvalidProductQuotaException(quotaInfoCommand.ItemCods);
        }

        public static SaleAgentInfo MapToSaleAgentInfo(List<SaleAgent> salesAgents, QuotaTransactionInfoCommand command)
        {
            var saleAgent = salesAgents.FirstOrDefault(x => x.Id.DbId == command.SaleAgentId);
            if (saleAgent.GetType() == typeof(SaleAgentIndividual))
            {
                var saleAgentIndividual = saleAgent as SaleAgentIndividual;
                return Factory.CreateSaleAgentIndividualInfo(saleAgentIndividual.Id, saleAgentIndividual.FirstName,
                    saleAgentIndividual.LastName, saleAgentIndividual.SocialSecurityNumber);
            }

            var saleAgentLegal = saleAgent as SaleAgentLegal;
            return Factory.CreateSaleAgentLegalInfo(saleAgentLegal.Id, saleAgentLegal.CompanyName,
                saleAgentLegal.CompanyNationalId, saleAgentLegal.EconomicCode);
        }
        public static SaleAgentInfo MapToSaleAgentInfo(SaleAgent saleAgent)
        {
            if (saleAgent.GetType() == typeof(SaleAgentIndividual))
            {
                var saleAgentIndividual = saleAgent as SaleAgentIndividual;
                return Factory.CreateSaleAgentIndividualInfo(saleAgentIndividual.Id, saleAgentIndividual.FirstName,
                    saleAgentIndividual.LastName, saleAgentIndividual.SocialSecurityNumber);
            }

            var saleAgentLegal = saleAgent as SaleAgentLegal;
            return Factory.CreateSaleAgentLegalInfo(saleAgentLegal.Id, saleAgentLegal.CompanyName,
                saleAgentLegal.CompanyNationalId, saleAgentLegal.EconomicCode);
        }
    }
}
