﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace ALS_BillingAppointmentAPI.DB
{
    public partial class VCbBillingAppointment
    {
        public Guid? CbPackageId { get; set; }
        public Guid CbId { get; set; }
        public string CbNo { get; set; }
        public Guid? CbInvoiceId { get; set; }
        public string CbInvoiceNo { get; set; }
        public string CbInvoiceCustCode { get; set; }
        public string CbQuoteNo { get; set; }
        public string CbBillToCompany { get; set; }
        public string CbInvoiceToAddress { get; set; }
        public string CbReportsToAddress { get; set; }
        public string DiffAddress { get; set; }
        public string CbInvoiceNote { get; set; }
        public string CbDeliveryType { get; set; }
        public string DeliveryName { get; set; }
        public string CbStatusCode { get; set; }
        public string StatusName { get; set; }
        public string CbCreateBy { get; set; }
        public DateTime? CbCreateDate { get; set; }
        public string CbUpdateBy { get; set; }
        public DateTime? CbUpdateDate { get; set; }
    }
}