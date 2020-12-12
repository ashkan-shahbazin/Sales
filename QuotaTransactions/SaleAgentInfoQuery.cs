using Framework.Domain;

namespace Sales.Query.Model.Model.QuotaTransactions
{
    public class SaleAgentInfoQuery
    {
        public long SaleAgentId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string SocialSecurityNumber { get; private set; }
        public string CompanyName { get; private set; }
        public string CompanyNationalId { get; private set; }
        public string EconomicCode { get; private set; }

        public SaleAgentInfoQuery(long saleAgentId, string firstName, string lastName,string socialSecurityNumber, string companyName, string companyNationalId, string economicCode)
        {
            SaleAgentId = saleAgentId;
            FirstName = firstName;
            LastName = lastName;
            SocialSecurityNumber = socialSecurityNumber;
            CompanyName = companyName;
            CompanyNationalId = companyNationalId;
            EconomicCode = economicCode;
        }

        protected SaleAgentInfoQuery() { }
    }
}