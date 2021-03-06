﻿using System.Collections.Generic;
using System.Linq;
using Framework.Application;
using Framework.Core.Claims;
using Framework.Core.Events;
using Framework.Core.Utilities;
using Sales.Application.QuotaTransactions.Exceptions;
using Sales.Application.QuotaTransactions.Mappers;
using Sales.Domain.Contracts.Model.QuotaTransactions;
using Sales.Domain.Model.Contracts;
using Sales.Domain.Model.Quotas;
using Sales.Domain.Model.QuotaTransactions;
using Sales.Domain.Model.QuotaTransactions.Factories;
using Sales.Domain.Model.SaleProducts.SaleActualProducts;
using Sales.Domain.Model.SaleProducts.SaleSpecificProducts;
using Sales.Domain.Model.SalesAgents;
using Sales.Domain.Model.Segments;
using Sales.Domain.Services.QuotaTransactions.Charges;
using Sales.Gateway.Facade.Contracts.QuotaTransactions.Commands;
using Sales.Gateway.Facade.Contracts.QuotaTransactions.Commands.Charges;

namespace Sales.Application.QuotaTransactions.Charges
{
    public class ChargeQuotaTransactionCommandHandler : ICommandHandler<ChargeWithExcelFileCommand>
                                                      , ICommandHandler<ChargeCommand>
    {
        private readonly ISegmentRepository _segmentRepository;
        private readonly IChargeTransactionDomainService _chargeTransactionDomainService;
        private readonly ISaleActualProductRepository _saleActualProductRepository;
        private readonly ISaleSpecificProductRepository _saleSpecificProductRepository;
        private readonly IQuotaTransactionRepository _quotaTransactionRepository;
        private readonly ISaleAgentRepository _saleAgentRepository;
        private readonly IQuotaRepository _quotaRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IClock _clock;
        private readonly IClaimHelper _claimHelper;

        public ChargeQuotaTransactionCommandHandler(ISegmentRepository segmentRepository,
            IClock clock, IClaimHelper claimHelper, ISaleActualProductRepository saleActualProductRepository, ISaleSpecificProductRepository saleSpecificProductRepository, IQuotaTransactionRepository quotaTransactionRepository, ISaleAgentRepository saleAgentRepository, IQuotaRepository quotaRepository, IChargeTransactionDomainService chargeTransactionDomainService, IContractRepository contractRepository, IEventPublisher eventPublisher)
        {
            _segmentRepository = segmentRepository;
            _chargeTransactionDomainService = chargeTransactionDomainService;
            _saleActualProductRepository = saleActualProductRepository;
            _saleSpecificProductRepository = saleSpecificProductRepository;
            _quotaRepository = quotaRepository;
            _saleAgentRepository = saleAgentRepository;
            _quotaTransactionRepository = quotaTransactionRepository;
            _contractRepository = contractRepository;
            _eventPublisher = eventPublisher;
            _clock = clock;
            _claimHelper = claimHelper;
        }

        public void Handle(ChargeWithExcelFileCommand command)
        {
            GuardAgainstDuplicate(command);
            var groupedByChargeCommand = GetGroupedByChargeCommand(command);
            var quotasTransactionsInfoCommand = QuotaTransactionMapper.MapToQuotasTransactionsInfoCommand(groupedByChargeCommand);
            var specificProductsInfo = GetSaleSpecificProducts(quotasTransactionsInfoCommand);
            var saleActualProducts = GetSaleActualProducts(specificProductsInfo);
            var segments = GetSegments(saleActualProducts);
            var salesAgentIds = quotasTransactionsInfoCommand.Select(x => x.SaleAgentId).Distinct().ToList();
            var salesAgents = _saleAgentRepository.GetAllBy(salesAgentIds);
            var quotas = _quotaRepository.GetAllBy(salesAgentIds);
            var contracts = GetAllContractsBy(salesAgents);
            Charge(quotasTransactionsInfoCommand, specificProductsInfo, saleActualProducts, segments, salesAgents, quotas, contracts);
        }

        private void GuardAgainstDuplicate(ChargeWithExcelFileCommand command)
        {
            var productQuotaAssign = command.CumulativeQuotaTransactionCommands.GroupBy(x => new
            {
                x.SaleAgentId,
                x.ItemCode
            }).SelectMany(x => x.Where(z => x.Count() > 1)).Distinct().ToList();

            if (productQuotaAssign.Count() > 1)
            {
                var messages = BuildExceptionMessages(productQuotaAssign);
                throw new DuplicateProductQuotaItemAssignedToSaleAgentException(messages.Separate());
            }
        }
        private static List<string> BuildExceptionMessages(List<CumulativeQuotaTransactionCommand> quotaTransactionsCommand)
        {
            var messages = new List<string>();
            foreach (var quotaTransactionCommand in quotaTransactionsCommand)
            {
                string message = $"SaleAgentId:{quotaTransactionCommand.SaleAgentId}- ItemCode:{quotaTransactionCommand.ItemCode}- Count:{quotaTransactionCommand.Count}";
                messages.Add(message);
            }

            return messages;
        }
        private static List<CumulativeQuotaTransactionCommand> GetGroupedByChargeCommand(ChargeWithExcelFileCommand command)
        {
            var groupedByChargeCommand = command.CumulativeQuotaTransactionCommands.GroupBy(x => new
            {
                x.SaleAgentId,
                x.ItemCode
            }).Select(gcs => new CumulativeQuotaTransactionCommand(gcs.Key.SaleAgentId, gcs.Select(x => x.ItemCode).FirstOrDefault(), gcs.Sum(x => x.Count))).ToList();
            return groupedByChargeCommand;
        }

        private List<SaleSpecificProduct> GetSaleSpecificProducts(List<QuotaTransactionInfoCommand> chargesCommand)
        {
            var productItemsCode = chargesCommand.SelectMany(x => x.QuotasInfoCommand.Select(z => z.ItemCods)).ToList();
            return _saleSpecificProductRepository.GetAllBy(productItemsCode);
        }
        private List<SaleActualProduct> GetSaleActualProducts(List<SaleSpecificProduct> saleSpecificProducts)
        {
            var saleActualProductIds = saleSpecificProducts.Select(z => z.SaleActualProductId.DbId).ToList();
            return _saleActualProductRepository.GetAllBy(saleActualProductIds);
        }
        private List<Segment> GetSegments(List<SaleActualProduct> saleActualProducts)
        {
            var saleActualProductIds = saleActualProducts.Select(x => x.Id.DbId).ToList();
            return _segmentRepository.GetAllBy(saleActualProductIds);
        }
        private List<Contract> GetAllContractsBy(List<SaleAgent> salesAgents)
        {
            var contractIds = salesAgents.SelectMany(x => x.SaleAgentSegmentInfos).Select(z => z.ContractId).Distinct().ToList();
            return _contractRepository.GetAllContractsThatContains(contractIds);
        }
        private void Charge(List<QuotaTransactionInfoCommand> chargesCommand,
            List<SaleSpecificProduct> saleSpecificProducts, List<SaleActualProduct> saleActualProducts, List<Segment> segments, List<SaleAgent> salesAgents,
            List<Quota> quotas, List<Contract> contracts)
        {
            foreach (var chargeCommand in chargesCommand)
            {
                var id = _quotaTransactionRepository.GetNextId();
                var quotasInfoCommand = chargeCommand.QuotasInfoCommand.ToList();
                var saleAgent = salesAgents.FirstOrDefault(x => x.Id.DbId == chargeCommand.SaleAgentId);
                var quotaTransactionItems = QuotaTransactionMapper.MapToQuotaTransactionItems(saleSpecificProducts, saleActualProducts, segments, quotasInfoCommand, saleAgent, contracts);
                var saleAgentInfo = QuotaTransactionMapper.MapToSaleAgentInfo(salesAgents, chargeCommand);
                var quota = quotas.FirstOrDefault(x => x.SaleAgentId.DbId == chargeCommand.SaleAgentId);
                var charge = Factory.BuildCharge(id, _clock, saleAgentInfo, quota.Id, Reason.Excel, quotaTransactionItems, _chargeTransactionDomainService, _claimHelper, _eventPublisher);
                _quotaTransactionRepository.Create(charge);
            }
        }

        public void Handle(ChargeCommand command)
        {
            var saleSpecificProducts = GetSaleSpecificProducts(command.QuotaInfosCommand);
            var saleActualProducts = GetSaleActualProducts(saleSpecificProducts);
            var segments = GetSegments(saleActualProducts);
            var saleAgentId = new SaleAgentId(command.SaleAgentId);
            var saleAgent = _saleAgentRepository.Get<SaleAgent>(saleAgentId);
            var quota = _quotaRepository.GetBy(saleAgentId);
            var contracts = GetAllContractsBy(saleAgent);
            Charge(command.QuotaInfosCommand, saleSpecificProducts, saleActualProducts, segments, saleAgent, quota, contracts);
        }

        private List<SaleSpecificProduct> GetSaleSpecificProducts(List<QuotaInfoCommand> quotasInfoCommand)
        {
            var productItemsCode = quotasInfoCommand.Select(x => x.ItemCods).ToList();
            return _saleSpecificProductRepository.GetAllBy(productItemsCode);
        }

        private List<Contract> GetAllContractsBy(SaleAgent salesAgent)
        {
            var contractIds = salesAgent.SaleAgentSegmentInfos.Select(z => z.ContractId).Distinct().ToList();
            return _contractRepository.GetAllContractsThatContains(contractIds);
        }

        private void Charge(List<QuotaInfoCommand> quotasInfoCommand, List<SaleSpecificProduct> saleSpecificProducts, List<SaleActualProduct> saleActualProducts, List<Segment> segments, SaleAgent saleAgent, Quota quota, List<Contract> contracts)
        {
            var id = _quotaTransactionRepository.GetNextId();
            var quotaTransactionItems = QuotaTransactionMapper.MapToQuotaTransactionItems(saleSpecificProducts, saleActualProducts, segments, quotasInfoCommand, saleAgent, contracts);
            var saleAgentInfo = QuotaTransactionMapper.MapToSaleAgentInfo(saleAgent);
            var charge = Factory.BuildCharge(id, _clock, saleAgentInfo, quota.Id, Reason.Ui, quotaTransactionItems, _chargeTransactionDomainService, _claimHelper, _eventPublisher);
            _quotaTransactionRepository.Create(charge);
        }

        public void Handle(ChargeBaseOnDraftOrderForSaleAgentRejectedCommand command)
        {
            var saleSpecificProducts = GetSaleSpecificProducts(command.QuotaInfosCommand);
            var saleActualProducts = GetSaleActualProducts(saleSpecificProducts);
            var segments = GetSegments(saleActualProducts);
            var saleAgent = GetSaleAgent(command);
            var quota = _quotaRepository.GetBy(saleAgent.Id);
            var contracts = GetAllContractsBy(saleAgent);
            ChargeBaseOnSaleDraftOrderRejected(command.QuotaInfosCommand, saleSpecificProducts, saleActualProducts, segments, saleAgent, quota, Reason.SaleAgentDraftOrderRejected, contracts);
        }
        private SaleAgent GetSaleAgent(ChargeBaseOnDraftOrderForSaleAgentRejectedCommand command)
        {
            if (!string.IsNullOrEmpty(command.SocialSecurityNumber))
                return _saleAgentRepository.GetBySaleAgentIndividualBy(command.SocialSecurityNumber);
            return _saleAgentRepository.GetSaleAgentLegalBy(command.EconomicCode);
        }
        private void ChargeBaseOnSaleDraftOrderRejected(List<QuotaInfoCommand> quotasInfoCommand, List<SaleSpecificProduct> saleSpecificProducts, List<SaleActualProduct> saleActualProducts, List<Segment> segments, SaleAgent saleAgent, Quota quota, Reason reason, List<Contract> contracts)
        {
            var id = _quotaTransactionRepository.GetNextId();
            var quotaTransactionItems = QuotaTransactionMapper.MapToQuotaTransactionItems(saleSpecificProducts, saleActualProducts, segments, quotasInfoCommand, saleAgent, contracts);
            var saleAgentInfo = QuotaTransactionMapper.MapToSaleAgentInfo(saleAgent);
            var charge = Factory.BuildChargeBaseOnSaleDraftOrderRejected(id, _clock, saleAgentInfo, quota.Id, reason, quotaTransactionItems, _claimHelper, _eventPublisher);
            _quotaTransactionRepository.Create(charge);
        }
    }
}