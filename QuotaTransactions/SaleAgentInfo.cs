using Framework.Domain;
using Sales.Domain.Model.SalesAgents;

namespace Sales.Domain.Model.QuotaTransactions
{
    public class SaleAgentInfo : ValueObjectBase
    {
        public SaleAgentId SaleAgentId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string SocialSecurityNumber { get; private set; }
        public string CompanyName { get; private set; }
        public string CompanyNationalId { get; private set; }
        public string EconomicCode { get; private set; }

        private SaleAgentInfo(string firstName, string lastName, string socialSecurityNumber, SaleAgentId saleAgentId)
        {
            SaleAgentId = saleAgentId;
            FirstName = firstName;
            LastName = lastName;
            SocialSecurityNumber = socialSecurityNumber;
        }

        public static SaleAgentInfo BuildForIndividual(SaleAgentId saleAgentId, string firstName, string lastName, string socialSecurityNumber)
        {
            return new SaleAgentInfo(firstName, lastName, socialSecurityNumber, saleAgentId);
        }
        private SaleAgentInfo(SaleAgentId saleAgentId, string companyName, string companyNationalId, string economicCode)
        {
            SaleAgentId = saleAgentId;
            CompanyName = companyName;
            CompanyNationalId = companyNationalId;
            EconomicCode = economicCode;
        }

        public static SaleAgentInfo BuildForLegal(SaleAgentId saleAgentId, string companyName, string companyNationalId, string economicCode)
        {
            return new SaleAgentInfo(saleAgentId, companyName, companyNationalId, economicCode);
        }
        protected SaleAgentInfo() { }
    }
}