// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace ALS_BillingAppointmentAPI.DB
{
    public partial class VBillingAppointment
    {
        public Guid? PackageId { get; set; }
        public Guid BaId { get; set; }
        public string BaNo { get; set; }
        public Guid? InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceCustCode { get; set; }
        public string QuoteNo { get; set; }
        public string BillToCompany { get; set; }
        public string InvoiceToAddress { get; set; }
        public string ReportsToAddress { get; set; }
        public string DiffAddress { get; set; }
        public string InvoiceNote { get; set; }
        public string DeliveryType { get; set; }
        public string DeliveryName { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public string CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}