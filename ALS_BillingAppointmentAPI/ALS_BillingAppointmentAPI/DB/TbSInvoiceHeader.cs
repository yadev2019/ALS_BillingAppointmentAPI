﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

#nullable disable

namespace ALS_BillingAppointmentAPI.DB
{
    public partial class TbSInvoiceHeader
    {
        public Guid Id { get; set; }
        public string InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public string ClientCode { get; set; }
        public string InvoiceIssuedDate { get; set; }
        public string InvoiceAmount { get; set; }
        public string QuoteCode { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
    }
}