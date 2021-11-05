﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace ALS_BillingAppointmentAPI.DB
{
    public partial class TbSBillingAppointmentReportData
    {
        public Guid Id { get; set; }
        public string InvoiceId { get; set; }
        public string InvoiceDeliveryType { get; set; }
        public string AnalysisReportDeliveryType { get; set; }
        public string InvoiceDeliveryLaboratory { get; set; }
        public string AnalysisReportDeliveryLaboratory { get; set; }
        public string QuoteCode { get; set; }
        public string CreditTerm { get; set; }
        public string InvoiceDeliveryClientCode { get; set; }
        public string AnalysisReportDeliverToClientCode { get; set; }
        public string AnalysisReportDeliverToClientName { get; set; }
        public string InvoiceDeliveryClientName { get; set; }
        public string InvoiceDeliverToPersonName { get; set; }
        public string InvoiceDeliverToPersonTelephone { get; set; }
        public string InvoiceDeliverToPersonMobile { get; set; }
        public string AnalysisReportDeliverToPersonName { get; set; }
        public string AnalysisReportDeliverToPersonTelephone { get; set; }
        public string AnalysisReportDeliverToPersonMobile { get; set; }
        public string InvoiceDeliverToOffice { get; set; }
        public string InvoiceDeliverToAddress1 { get; set; }
        public string InvoiceDeliverToAddress2 { get; set; }
        public string InvoiceDeliverToAddress3 { get; set; }
        public string InvoiceDeliverToCity { get; set; }
        public string InvoiceDeliverToState { get; set; }
        public string InvoiceDeliverToZip { get; set; }
        public string InvoiceDeliverToLocation { get; set; }
        public string AnalysisReportDeliverToOffice { get; set; }
        public string AnalysisReportDeliverToAddress1 { get; set; }
        public string AnalysisReportDeliverToAddress2 { get; set; }
        public string AnalysisReportDeliverToAddress3 { get; set; }
        public string AnalysisReportDeliverToCity { get; set; }
        public string AnalysisReportDeliverToState { get; set; }
        public string AnalysisReportDeliverToZip { get; set; }
        public string AnalysisReportDeliverToLocation { get; set; }
        public string InvoiceDeliveryNotes { get; set; }
        public string InvoiceDeliveryProcess { get; set; }
        public DateTime? InvoiceIssuedDate { get; set; }
        public DateTime? InvoiceDateOverride { get; set; }
        public string InvoiceNumbersIncludedInSearch { get; set; }
        public string TotalInvoiceAmountIncludingVat { get; set; }
        public string ReportToPersonName { get; set; }
        public string ReportToAddress1 { get; set; }
        public string ReportToAddress2 { get; set; }
        public string ReportToAddress3 { get; set; }
        public string ReportToCity { get; set; }
        public string ReportToState { get; set; }
        public string ReportToZip { get; set; }
        public string ReportToLocation { get; set; }
        public string BillToClientCode { get; set; }
        public string BillToClientName { get; set; }
        public string BillToContactPerson { get; set; }
        public string PaymentDeliveryProcess { get; set; }
        public string PaymentDeliveryNotes { get; set; }
        public string ReportLocation { get; set; }
        public string GenerateBillingAppointmentPerInvoice { get; set; }
        public string InvoicePaid { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CeateBy { get; set; }
    }
}