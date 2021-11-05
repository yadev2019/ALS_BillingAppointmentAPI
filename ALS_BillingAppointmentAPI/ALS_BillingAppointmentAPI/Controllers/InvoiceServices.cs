using ALS_BillingAppointmentAPI.DB;
using ALS_BillingAppointmentAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


namespace ALS_BillingAppointmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceServices : ControllerBase
    {
        private readonly K2_Billing_AppointmentContext db;
        private readonly IConfiguration _configuration;
        public static string baseUrl;
        public static string userName;
        public static string Password;

        public InvoiceServices(K2_Billing_AppointmentContext db, IConfiguration configuration)
        {
            this.db = db;
            _configuration = configuration;
            baseUrl = _configuration["AppSettings:BaseUrl"];
            userName = _configuration["AppSettings:Username"];
            Password = _configuration["AppSettings:Password"];
        }

        [HttpGet("GetInvoiceHeader")]
        public async Task<IActionResult> GetInvoiceHeader()
        {
            try
            {
                //var getDate = DateTime.Today.ToString("yyyy-MM-dd"); //prod
                                
                var invoiceHeader = db.TbSInvoiceHeader.ToList();
                db.TbSInvoiceHeader.RemoveRange(invoiceHeader);
                db.SaveChanges();

                //var url = baseUrl + $"/Default.GetInvoiceHeaders(startDate='{getDate}',endDate='{getDate}',invoiceNumber='',workorderCode='')"; //prod
                var url = baseUrl + $"/Default.GetInvoiceHeaders(startDate='2021-09-01',endDate='2021-10-31',invoiceNumber='',workorderCode='')";
                var credentialsCache = new CredentialCache
            {
                {new Uri(url), "NTLM", new NetworkCredential(
                    userName,Password
                )}
            };
                var handler = new HttpClientHandler { Credentials = credentialsCache };
                var client = new HttpClient(handler);
                var res = await client.GetAsync(url);
                var result = (dynamic)null;

                InvoiceHeaderModel inv = new InvoiceHeaderModel();
                var task = client.GetAsync(url)
                  .ContinueWith((taskwithresponse) =>
                  {
                      var response = taskwithresponse.Result;
                      var jsonString = response.Content.ReadAsStringAsync();
                      jsonString.Wait();

                      var format = "dd/MM/yyyy"; // your datetime format
                      var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };

                      inv = JsonConvert.DeserializeObject<InvoiceHeaderModel>(jsonString.Result, dateTimeConverter);

                      List<TbSInvoiceHeader> entityInvoiceHeader = new List<TbSInvoiceHeader>();
                      if(inv.InoviceHD != null && inv.InoviceHD.Count > 0)
                      {
                          foreach (var item in inv.InoviceHD)
                          {
                              if (inv.InoviceHD != null && inv.InoviceHD.Count > 0)
                              {
                                  var add = (new TbSInvoiceHeader
                                  {
                                      Id = Guid.NewGuid(),
                                      InvoiceId = item.InvoiceId.ToString(),
                                      InvoiceNumber = item.InvoiceNumber,
                                      ClientCode = item.ClientCode,
                                      InvoiceIssuedDate = item.InvoiceIssuedDate,
                                      InvoiceAmount = item.InvoiceAmount.ToString(),
                                      QuoteCode = item.QuoteCode,
                                      CreateDate = DateTime.Now,
                                      CreateBy = "K2Admin",
                                  });
                                  db.TbSInvoiceHeader.Add(add);

                              }
                          }
                          db.SaveChanges();
                      }
                      

                  });
                task.Wait();

                //System.Threading.Thread.Sleep(3000);
                //var invoiceDetail = GetInvoiceDetail();

                return Ok(new BaseResponseViewModel<TbSInvoiceHeader>()
                {
                    is_error = false,
                    msg_alert = "Sucess",
                    data = null
                });
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseViewModel<TbSInvoiceHeader>()
                {
                    is_error = true,
                    msg_alert = "Fail" + ex.Message,
                    data = null
                });
            }

        }

        [HttpGet("GetInvoiceDetail")]
        public async Task<IActionResult> GetInvoiceDetail()
        {
            try
            {
                //List<TbSInvoiceHeader> invoiceHeader = new List<TbSInvoiceHeader>();
                var invoiceHeader = db.TbSInvoiceHeader.ToList();
                var invoiceDT = db.TbSBillingAppointmentReportData.ToList();
                db.TbSBillingAppointmentReportData.RemoveRange(invoiceDT);
                db.SaveChanges();

                var urlGetDetail = baseUrl + $"/Default.GetBillingAppointmentReportData(invoiceNumbersList="; // prod
               //var urlGetDetail = baseUrl + $"/Default.GetBillingAppointmentReportData(invoiceNumbersList='1111275')"; //fix test

                if (invoiceHeader != null && invoiceHeader.Count > 0)
                {
                    foreach(var item in invoiceHeader)
                    {
                        var URLexe = urlGetDetail+ "'"+ item.InvoiceId + "')"; // prod


                        var credentialsCache = new CredentialCache
                        {
                        {new Uri(URLexe), "NTLM", new NetworkCredential(
                            userName,Password
                        )}
                        };
                        var handler = new HttpClientHandler { Credentials = credentialsCache };
                        var client = new HttpClient(handler);
                        var res = await client.GetAsync(URLexe);
                        if (res.IsSuccessStatusCode == false)
                        {
                            continue;
                        }

                        InvoiceDetailModel inv = new InvoiceDetailModel();
                        var task = client.GetAsync(URLexe)
                          .ContinueWith((taskwithresponse) =>
                          {
                              var response = taskwithresponse.Result;
                              var jsonString = response.Content.ReadAsStringAsync();
                              jsonString.Wait();

                              var format = "dd/MM/yyyy"; // your datetime format
                              var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
                              inv = JsonConvert.DeserializeObject<InvoiceDetailModel>(jsonString.Result, dateTimeConverter);

                              if (inv.InvoiceDT != null && inv.InvoiceDT.Count > 0)
                              {
                                  foreach (var item in inv.InvoiceDT)
                                  {
                                      var add = (new TbSBillingAppointmentReportData
                                      {
                                          Id = Guid.NewGuid(),
                                          InvoiceId = item.InvoiceId.ToString(),
                                          InvoiceDeliveryType = item.InvoiceDeliveryType,
                                          AnalysisReportDeliveryType = item.AnalysisReportDeliveryType,
                                          InvoiceDeliveryLaboratory = ((item.InvoiceDeliveryLaboratory == null|| item.InvoiceDeliveryLaboratory == "") ? null : item.InvoiceDeliveryLaboratory.ToString()),
                                          AnalysisReportDeliveryLaboratory = item.AnalysisReportDeliveryLaboratory,
                                          QuoteCode = ((item.QuoteCode == null || item.QuoteCode == "") ? null : item.QuoteCode.ToString()),
                                          CreditTerm = (item.CreditTerm == null ? null : item.CreditTerm.ToString()),
                                          InvoiceDeliveryClientCode = item.InvoiceDeliveryClientCode,
                                          AnalysisReportDeliverToClientCode = item.AnalysisReportDeliverToClientCode,
                                          AnalysisReportDeliverToClientName = item.AnalysisReportDeliverToClientName,
                                          InvoiceDeliveryClientName = item.InvoiceDeliveryClientName,
                                          InvoiceDeliverToPersonName = item.InvoiceDeliverToPersonName,
                                          InvoiceDeliverToPersonTelephone = item.InvoiceDeliverToPersonTelephone,
                                          InvoiceDeliverToPersonMobile = item.InvoiceDeliverToPersonMobile,
                                          AnalysisReportDeliverToPersonName = item.AnalysisReportDeliverToPersonName,
                                          AnalysisReportDeliverToPersonTelephone = item.AnalysisReportDeliverToPersonTelephone,
                                          AnalysisReportDeliverToPersonMobile = ((item.AnalysisReportDeliverToPersonMobile == null || item.AnalysisReportDeliverToPersonMobile == "") ? null : item.AnalysisReportDeliverToPersonMobile.ToString()),
                                          InvoiceDeliverToOffice = item.InvoiceDeliverToOffice,
                                          InvoiceDeliverToAddress1 = item.InvoiceDeliverToAddress1,
                                          InvoiceDeliverToAddress2 = item.InvoiceDeliverToAddress2,
                                          InvoiceDeliverToAddress3 = item.InvoiceDeliverToAddress3,
                                          InvoiceDeliverToCity = item.InvoiceDeliverToCity,
                                          InvoiceDeliverToState = item.InvoiceDeliverToState,
                                          InvoiceDeliverToZip = item.InvoiceDeliverToZip,
                                          InvoiceDeliverToLocation = item.InvoiceDeliverToLocation,
                                          //InvoiceDeliverToOffice = item.InvoiceDeliverToOffice,
                                          AnalysisReportDeliverToAddress1 = item.AnalysisReportDeliverToAddress1,
                                          AnalysisReportDeliverToAddress2 = item.AnalysisReportDeliverToAddress2,
                                          AnalysisReportDeliverToAddress3 = item.AnalysisReportDeliverToAddress3,
                                          AnalysisReportDeliverToCity = item.AnalysisReportDeliverToCity,
                                          AnalysisReportDeliverToState = item.AnalysisReportDeliverToState,
                                          AnalysisReportDeliverToZip = item.AnalysisReportDeliverToZip,
                                          AnalysisReportDeliverToLocation = item.AnalysisReportDeliverToLocation,
                                          InvoiceDeliveryNotes = ((item.InvoiceDeliveryNotes == null || item.InvoiceDeliveryNotes == "") ? null : item.InvoiceDeliveryNotes.ToString()),
                                          InvoiceDeliveryProcess = ((item.InvoiceDeliveryProcess == null || item.InvoiceDeliveryProcess == "") ? null : item.InvoiceDeliveryProcess.ToString()),
                                          InvoiceIssuedDate = item.InvoiceIssuedDate,
                                          InvoiceDateOverride = item.InvoiceDateOverride,
                                          InvoiceNumbersIncludedInSearch = item.InvoiceNumbersIncludedInSearch,
                                          TotalInvoiceAmountIncludingVat = item.TotalInvoiceAmountIncludingVat.ToString(),
                                          ReportToPersonName = item.ReportToPersonName,
                                          ReportToAddress1 = item.ReportToAddress1,
                                          ReportToAddress2 = item.ReportToAddress2,
                                          ReportToAddress3 = item.ReportToAddress3,
                                          ReportToCity = item.ReportToCity,
                                          ReportToState = item.ReportToState,
                                          ReportToZip = item.ReportToZip,
                                          ReportToLocation = item.ReportToLocation,
                                          //BillToClientCode = item.BillToClientCode,
                                          //BillToClientName = item.BillToClientName,
                                          //BillToContactPerson = item.BillToContactPerson,
                                          PaymentDeliveryProcess = (item.PaymentDeliveryProcess == null ? null : item.PaymentDeliveryProcess.ToString()),
                                          PaymentDeliveryNotes = (item.PaymentDeliveryNotes == null ? null : item.PaymentDeliveryNotes.ToString()),
                                          ReportLocation = ((item.ReportLocation == null || item.ReportLocation == "") ? null : item.ReportLocation.ToString()),
                                          GenerateBillingAppointmentPerInvoice = (item.GenerateBillingAppointmentPerInvoice == null  ? null : item.GenerateBillingAppointmentPerInvoice.ToString()),
                                          InvoicePaid = (item.InvoicePaid == null  ? null : item.InvoicePaid.ToString()),
                                          CeateBy = "K2Admin",
                                          CreateDate = DateTime.Now
                                      });;;
                                      db.TbSBillingAppointmentReportData.Add(add);
                                  }
                              }
                              db.SaveChanges();
                          });
                          task.Wait();
                    }
                }

                var generateBA = GenerateBA();

                return Ok(new BaseResponseViewModel<TbSBillingAppointmentReportData>()
                {
                    is_error = false,
                    msg_alert = "Sucess",
                    data = null
                });
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseViewModel<TbSBillingAppointmentReportData>()
                {
                    is_error = true,
                    msg_alert = "Fail" + ex.Message,
                    data = null
                });
            }

        }

        [HttpGet("GenerateBA")]
        public async Task<IActionResult> GenerateBA()
        {
            try
            {
                //List<TbSInvoiceHeader> invoiceHeader = new List<TbSInvoiceHeader>();
                var invoiceHeader = db.TbSInvoiceHeader.ToList();
                //List<TbSBillingAppointmentReportData> invoiceDetail = new List<TbSBillingAppointmentReportData>();
                //var invoiceDetail = db.TbSBillingAppointmentReportData.OrderBy(x => { x.QuoteCode, x.InvoiceDeliveryClientCode, x.InvoiceDeliveryLaboratory, x.InvoiceDeliveryType, x.InvoiceIssuedDate}).ToList();
                var invoiceDetail = db.TbSBillingAppointmentReportData
                                    .OrderBy(x => x.InvoiceDeliveryClientCode)
                                    .ThenBy(x => x.QuoteCode)
                                    .ThenBy(x => x.InvoiceDeliveryLaboratory)
                                    .ThenBy(x => x.InvoiceDeliveryType)
                                    .ThenBy(x => x.InvoiceIssuedDate).ToList();

                var custCode = (dynamic)null;
                var quoteCode = (dynamic)null;
                var Lab = (dynamic)null;
                var OverrideDate = (dynamic)null;
                var deliveryType = (dynamic)null;

                //var usp = new K2_Billing_AppointmentContextProcedures(db);
                //var rsSp = await usp.usp_GetNewRunningNoAsync("");

                var baID = Guid.NewGuid();

                //if (invoiceDetail != null && invoiceDetail.Count > 0)
                //{
                //    foreach (var item in invoiceDetail)
                //    {
                //        OverrideDate = (item.InvoiceDateOverride == null ? item.InvoiceIssuedDate : item.InvoiceDateOverride);

                //        if (item.InvoiceDeliveryClientCode == custCode)
                //        {
                //            if (item.QuoteCode == quoteCode)
                //            {
                //                if (item.AnalysisReportDeliveryLaboratory == Lab)
                //                {
                //                    if (item.InvoiceIssuedDate == OverrideDate || item.InvoiceDateOverride == OverrideDate)
                //                    {
                //                        if (item.InvoiceDeliveryType == deliveryType)
                //                        {
                //                            #region // same baid
                //                            var add = new TbRInvoice();

                //                            add.InvoiceId = Guid.NewGuid();
                //                            add.BaId = baID;
                //                            add.CustCode = item.InvoiceDeliveryClientCode;
                //                            add.DeliveryAddress = item.InvoiceDeliverToAddress1;
                //                            add.DeliveryToPerson = item.InvoiceDeliverToPersonName;
                //                            add.DeliveryToTel = item.InvoiceDeliverToPersonTelephone;
                //                            add.ReportToComany = item.ReportToPersonName;
                //                            add.ReportsToAddress = item.ReportToAddress1;
                //                            add.ReportsToTel = item.AnalysisReportDeliverToPersonTelephone;
                //                            add.InvoiceIssueDate = OverrideDate;
                //                            add.InvoiceDateOverride = item.InvoiceDateOverride;
                //                            add.InvoiceNote = item.InvoiceDeliveryNotes;
                //                            add.InvoiceProcess = item.InvoiceDeliveryProcess;
                //                            add.TotalInvoiceAmountIncVat = Convert.ToDecimal(item.TotalInvoiceAmountIncludingVat);
                //                            add.CreditTerm = item.CreditTerm;
                //                            add.StatusCode = "8";
                //                            add.CreateDate = DateTime.Today;
                //                            add.CreateBy = "K2Admin";

                //                            db.TbRInvoice.Add(add);
                //                            db.SaveChanges();
                //                            #endregion
                //                        }
                //                        else
                //                        {
                //                            baID = Guid.NewGuid();
                //                            deliveryType = item.InvoiceDeliveryType;

                //                            #region 
                //                            var add = new TbRInvoice();

                //                            add.InvoiceId = Guid.NewGuid();
                //                            add.BaId = baID;
                //                            add.CustCode = item.InvoiceDeliveryClientCode;
                //                            add.DeliveryAddress = item.InvoiceDeliverToAddress1;
                //                            add.DeliveryToPerson = item.InvoiceDeliverToPersonName;
                //                            add.DeliveryToTel = item.InvoiceDeliverToPersonTelephone;
                //                            add.ReportToComany = item.ReportToPersonName;
                //                            add.ReportsToAddress = item.ReportToAddress1;
                //                            add.ReportsToTel = item.AnalysisReportDeliverToPersonTelephone;
                //                            add.InvoiceIssueDate = item.InvoiceIssuedDate;
                //                            add.InvoiceDateOverride = item.InvoiceDateOverride;
                //                            add.InvoiceNote = item.InvoiceDeliveryNotes;
                //                            add.InvoiceProcess = item.InvoiceDeliveryProcess;
                //                            add.TotalInvoiceAmountIncVat = Convert.ToDecimal(item.TotalInvoiceAmountIncludingVat);
                //                            add.CreditTerm = item.CreditTerm;
                //                            add.StatusCode = "7";
                //                            add.CreateDate = DateTime.Today;
                //                            add.CreateBy = "K2Admin";

                //                            db.TbRInvoice.Add(add);
                //                            db.SaveChanges();
                //                            #endregion
                //                        }
                //                    }
                //                    else
                //                    {
                //                        baID = Guid.NewGuid();
                //                        OverrideDate = (item.InvoiceDateOverride == null ? item.InvoiceIssuedDate : item.InvoiceDateOverride);

                //                        #region 
                //                        var add = new TbRInvoice();

                //                        add.InvoiceId = Guid.NewGuid();
                //                        add.BaId = baID;
                //                        add.CustCode = item.InvoiceDeliveryClientCode;
                //                        add.DeliveryAddress = item.InvoiceDeliverToAddress1;
                //                        add.DeliveryToPerson = item.InvoiceDeliverToPersonName;
                //                        add.DeliveryToTel = item.InvoiceDeliverToPersonTelephone;
                //                        add.ReportToComany = item.ReportToPersonName;
                //                        add.ReportsToAddress = item.ReportToAddress1;
                //                        add.ReportsToTel = item.AnalysisReportDeliverToPersonTelephone;
                //                        add.InvoiceIssueDate = item.InvoiceIssuedDate;
                //                        add.InvoiceDateOverride = item.InvoiceDateOverride;
                //                        add.InvoiceNote = item.InvoiceDeliveryNotes;
                //                        add.InvoiceProcess = item.InvoiceDeliveryProcess;
                //                        add.TotalInvoiceAmountIncVat = Convert.ToDecimal(item.TotalInvoiceAmountIncludingVat);
                //                        add.CreditTerm = item.CreditTerm;
                //                        add.StatusCode = "7";
                //                        add.CreateDate = DateTime.Today;
                //                        add.CreateBy = "K2Admin";

                //                        db.TbRInvoice.Add(add);
                //                        db.SaveChanges();
                //                        #endregion
                //                    }
                //                }
                //                else if (item.InvoiceDeliveryLaboratory != Lab)
                //                {
                //                    baID = Guid.NewGuid();
                //                    Lab = item.InvoiceDeliveryLaboratory;

                //                    #region 
                //                    var add = new TbRInvoice();

                //                    add.InvoiceId = Guid.NewGuid();
                //                    add.BaId = baID;
                //                    add.CustCode = item.InvoiceDeliveryClientCode;
                //                    add.DeliveryAddress = item.InvoiceDeliverToAddress1;
                //                    add.DeliveryToPerson = item.InvoiceDeliverToPersonName;
                //                    add.DeliveryToTel = item.InvoiceDeliverToPersonTelephone;
                //                    add.ReportToComany = item.ReportToPersonName;
                //                    add.ReportsToAddress = item.ReportToAddress1;
                //                    add.ReportsToTel = item.AnalysisReportDeliverToPersonTelephone;
                //                    add.InvoiceIssueDate = item.InvoiceIssuedDate;
                //                    add.InvoiceDateOverride = item.InvoiceDateOverride;
                //                    add.InvoiceNote = item.InvoiceDeliveryNotes;
                //                    add.InvoiceProcess = item.InvoiceDeliveryProcess;
                //                    add.TotalInvoiceAmountIncVat = Convert.ToDecimal(item.TotalInvoiceAmountIncludingVat);
                //                    add.CreditTerm = item.CreditTerm;
                //                    add.StatusCode = "7";
                //                    add.CreateDate = DateTime.Today;
                //                    add.CreateBy = "K2Admin";

                //                    db.TbRInvoice.Add(add);
                //                    db.SaveChanges();
                //                    #endregion
                //                }
                //            }
                //            else if (item.QuoteCode != quoteCode)
                //            {
                //                baID = Guid.NewGuid();
                //                quoteCode = item.QuoteCode;

                //                #region 
                //                var add = new TbRInvoice();

                //                add.InvoiceId = Guid.NewGuid();
                //                add.BaId = baID;
                //                add.CustCode = item.InvoiceDeliveryClientCode;
                //                add.DeliveryAddress = item.InvoiceDeliverToAddress1;
                //                add.DeliveryToPerson = item.InvoiceDeliverToPersonName;
                //                add.DeliveryToTel = item.InvoiceDeliverToPersonTelephone;
                //                add.ReportToComany = item.ReportToPersonName;
                //                add.ReportsToAddress = item.ReportToAddress1;
                //                add.ReportsToTel = item.AnalysisReportDeliverToPersonTelephone;
                //                add.InvoiceIssueDate = item.InvoiceIssuedDate;
                //                add.InvoiceDateOverride = item.InvoiceDateOverride;
                //                add.InvoiceNote = item.InvoiceDeliveryNotes;
                //                add.InvoiceProcess = item.InvoiceDeliveryProcess;
                //                add.TotalInvoiceAmountIncVat = Convert.ToDecimal(item.TotalInvoiceAmountIncludingVat);
                //                add.CreditTerm = item.CreditTerm;
                //                add.StatusCode = "7";
                //                add.CreateDate = DateTime.Today;
                //                add.CreateBy = "K2Admin";

                //                db.TbRInvoice.Add(add);
                //                db.SaveChanges();
                //                #endregion
                //            }
                //        }
                //        else if (item.InvoiceDeliveryClientCode != custCode)
                //        {
                //            baID = Guid.NewGuid();
                //            custCode = item.InvoiceDeliveryClientCode;

                //            #region 
                //            var add = new TbRInvoice();

                //            add.InvoiceId = Guid.NewGuid();
                //            add.BaId = baID;
                //            add.CustCode = item.InvoiceDeliveryClientCode;
                //            add.DeliveryAddress = item.InvoiceDeliverToAddress1;
                //            add.DeliveryToPerson = item.InvoiceDeliverToPersonName;
                //            add.DeliveryToTel = item.InvoiceDeliverToPersonTelephone;
                //            add.ReportToComany = item.ReportToPersonName;
                //            add.ReportsToAddress = item.ReportToAddress1;
                //            add.ReportsToTel = item.AnalysisReportDeliverToPersonTelephone;
                //            add.InvoiceIssueDate = item.InvoiceIssuedDate;
                //            add.InvoiceDateOverride = item.InvoiceDateOverride;
                //            add.InvoiceNote = item.InvoiceDeliveryNotes;
                //            add.InvoiceProcess = item.InvoiceDeliveryProcess;
                //            add.TotalInvoiceAmountIncVat = Convert.ToDecimal(item.TotalInvoiceAmountIncludingVat);
                //            add.CreditTerm = item.CreditTerm;
                //            add.StatusCode = "7";
                //            add.CreateDate = DateTime.Today;
                //            add.CreateBy = "K2Admin";

                //            db.TbRInvoice.Add(add);
                //            db.SaveChanges();
                //            #endregion //new baid
                //        }

                //        //if ((item.QuoteCode == quoteCode
                //        //    && item.InvoiceDeliveryClientCode == custCode
                //        //    && item.InvoiceDeliveryLaboratory == Lab
                //        //    && item.InvoiceDeliveryType == deliveryType) 
                //        //    && (item.InvoiceIssuedDate == OverrideDate) || (item.InvoiceDateOverride == OverrideDate)) // Same BA
                //        //{

                //        //    var add = new TbRInvoice();

                //        //    add.InvoiceId = Guid.NewGuid();
                //        //    add.BaId = baID;
                //        //    add.CustCode = item.InvoiceDeliveryClientCode;
                //        //    add.DeliveryAddress = item.InvoiceDeliverToAddress1;
                //        //    add.DeliveryToPerson = item.InvoiceDeliverToPersonName;
                //        //    add.DeliveryToTel = item.InvoiceDeliverToPersonTelephone;
                //        //    add.ReportToComany = item.ReportToPersonName;
                //        //    add.ReportsToAddress = item.ReportToAddress1;
                //        //    add.ReportsToTel = item.AnalysisReportDeliverToPersonTelephone;
                //        //    add.InvoiceIssueDate = OverrideDate;
                //        //    add.InvoiceDateOverride = item.InvoiceDateOverride;
                //        //    add.TotalInvoiceAmountIncVat = Convert.ToDecimal(item.TotalInvoiceAmountIncludingVat);
                //        //    add.CreditTerm = item.CreditTerm;
                //        //    add.StatusCode = "7";
                //        //    add.CreateDate = DateTime.Today;
                //        //    add.CreateBy = "K2Admin";

                //        //    db.TbRInvoice.Add(add);
                //        //    db.SaveChanges();
                //        //}
                //        //else if ((item.QuoteCode != quoteCode
                //        //    && item.InvoiceDeliveryClientCode != custCode
                //        //    && item.InvoiceDeliveryLaboratory != Lab
                //        //    && item.InvoiceDeliveryType != deliveryType)
                //        //    && (item.InvoiceIssuedDate == OverrideDate) || (item.InvoiceDateOverride == OverrideDate)) // New BA
                //        //{

                //        //    baID = Guid.NewGuid();
                //        //    quoteCode = item.QuoteCode;
                //        //    custCode = item.InvoiceDeliveryClientCode;
                //        //    Lab = item.InvoiceDeliveryLaboratory;
                //        //    //OverrideDate = (item.InvoiceDateOverride == null ? item.InvoiceIssuedDate : item.InvoiceDateOverride);
                //        //    deliveryType = item.InvoiceDeliveryType;

                //        //    var add = new TbRInvoice();

                //        //    add.InvoiceId = Guid.NewGuid();
                //        //    add.BaId = baID;
                //        //    add.CustCode = item.InvoiceDeliveryClientCode;
                //        //    add.DeliveryAddress = item.InvoiceDeliverToAddress1;
                //        //    add.DeliveryToPerson = item.InvoiceDeliverToPersonName;
                //        //    add.DeliveryToTel = item.InvoiceDeliverToPersonTelephone;
                //        //    add.ReportToComany = item.ReportToPersonName;
                //        //    add.ReportsToAddress = item.ReportToAddress1;
                //        //    add.ReportsToTel = item.AnalysisReportDeliverToPersonTelephone;
                //        //    add.InvoiceIssueDate = item.InvoiceIssuedDate;
                //        //    add.InvoiceDateOverride = item.InvoiceDateOverride;
                //        //    add.TotalInvoiceAmountIncVat = Convert.ToDecimal(item.TotalInvoiceAmountIncludingVat);
                //        //    add.CreditTerm = item.CreditTerm;
                //        //    add.StatusCode = "8";
                //        //    add.CreateDate = DateTime.Today;
                //        //    add.CreateBy = "K2Admin";

                //        //    db.TbRInvoice.Add(add);
                //        //    db.SaveChanges();
                //        //}


                //        ////    if (item.QuoteCode != quoteCode
                //        ////    && item.InvoiceDeliveryClientCode != custCode
                //        ////    && item.InvoiceDeliveryLaboratory != Lab
                //        ////    && item.InvoiceDateOverride != OverrideDate
                //        ////    && item.InvoiceDeliveryType != deliveryType)    // new BAID
                //        ////    //if (string.IsNullOrEmpty(item.QuoteCode) != quoteCode
                //        ////    //&& string.IsNullOrEmpty(item.InvoiceDeliveryClientCode) != custCode
                //        ////    //&& string.IsNullOrEmpty(item.InvoiceDeliveryLaboratory) != Lab
                //        ////    //&& item.InvoiceDateOverride != printDate
                //        ////    //&& string.IsNullOrEmpty(item.InvoiceDeliveryType) != deliveryType)
                //        ////{
                //        ////    baID = Guid.NewGuid();
                //        ////    quoteCode = item.QuoteCode;
                //        ////    custCode = item.InvoiceDeliveryClientCode;
                //        ////    Lab = item.InvoiceDeliveryLaboratory;
                //        ////    OverrideDate = (item.InvoiceDateOverride == null ? item.InvoiceIssuedDate : item.InvoiceDateOverride);
                //        ////    deliveryType = item.InvoiceDeliveryType;

                //        ////    var add = new TbRInvoice();

                //        ////    add.InvoiceId = Guid.NewGuid();
                //        ////    add.BaId = baID;
                //        ////    add.CustCode = item.InvoiceDeliveryClientCode;
                //        ////    add.DeliveryAddress = item.InvoiceDeliverToAddress1;
                //        ////    add.DeliveryToPerson = item.InvoiceDeliverToPersonName;
                //        ////    add.DeliveryToTel = item.InvoiceDeliverToPersonTelephone;
                //        ////    add.ReportToComany = item.ReportToPersonName;
                //        ////    add.ReportsToAddress = item.ReportToAddress1;
                //        ////    add.ReportsToTel = item.AnalysisReportDeliverToPersonTelephone;
                //        ////    add.InvoiceIssueDate = item.InvoiceIssuedDate;
                //        ////    add.InvoiceDateOverride = item.InvoiceDateOverride;
                //        ////    add.TotalInvoiceAmountIncVat = Convert.ToDecimal(item.TotalInvoiceAmountIncludingVat);
                //        ////    add.CreditTerm = item.CreditTerm;
                //        ////    add.StatusCode = "8";
                //        ////    add.CreateDate = DateTime.Today;
                //        ////    add.CreateBy = "K2Admin";

                //        ////    ////if (!string.IsNullOrEmpty(item.InvoiceDateOverride))
                //        ////    //if (item.InvoiceDateOverride != null)
                //        ////    //{
                //        ////    //    //add.InvoiceDateOverride = Convert.ToDateTime(item.InvoiceDateOverride);
                //        ////    //    add.InvoiceDateOverride = item.InvoiceDateOverride;
                //        ////    //}
                //        ////    db.TbRInvoice.Add(add);
                //        ////    db.SaveChanges();
                //        ////}

                //        ////else if (item.QuoteCode == quoteCode
                //        ////    && item.InvoiceDeliveryClientCode == custCode
                //        ////    && item.InvoiceDeliveryLaboratory == Lab
                //        ////    && item.InvoiceDateOverride == OverrideDate
                //        ////    && item.InvoiceDeliveryType == deliveryType)    // same BAID
                //        ////{
                //        ////    var printDate = (item.InvoiceDateOverride == null ? item.InvoiceIssuedDate : item.InvoiceDateOverride);

                //        ////    var add = new TbRInvoice();

                //        ////    add.InvoiceId = Guid.NewGuid();
                //        ////    add.BaId = baID;
                //        ////    add.CustCode = item.InvoiceDeliveryClientCode;
                //        ////    add.DeliveryAddress = item.InvoiceDeliverToAddress1;
                //        ////    add.DeliveryToPerson = item.InvoiceDeliverToPersonName;
                //        ////    add.DeliveryToTel = item.InvoiceDeliverToPersonTelephone;
                //        ////    add.ReportToComany = item.ReportToPersonName;
                //        ////    add.ReportsToAddress = item.ReportToAddress1;
                //        ////    add.ReportsToTel = item.AnalysisReportDeliverToPersonTelephone;
                //        ////    add.InvoiceIssueDate = printDate;
                //        ////    add.InvoiceDateOverride = item.InvoiceDateOverride;
                //        ////    add.TotalInvoiceAmountIncVat = Convert.ToDecimal(item.TotalInvoiceAmountIncludingVat);
                //        ////    add.CreditTerm = item.CreditTerm;
                //        ////    add.StatusCode = "7";
                //        ////    add.CreateDate = DateTime.Today;
                //        ////    add.CreateBy = "K2Admin";

                //        ////    ////if (!string.IsNullOrEmpty(item.InvoiceDateOverride))
                //        ////    //if (item.InvoiceDateOverride != null)
                //        ////    //{
                //        ////    //    add.InvoiceDateOverride = item.InvoiceDateOverride;
                //        ////    //}

                //        ////    db.TbRInvoice.Add(add);
                //        ////    db.SaveChanges();
                //        ////}
                //    }
                //    //db.SaveChanges();
                //}

                //var newBAID = db.TbRInvoice.Where(x => x.CreateDate == DateTime.Today).OrderBy(x => x.CreateDate).Distinct();
                //var newBAID = db.TbRInvoice.Select(x => x.BaId).Distinct().ToList();
                var newBAID = db.TbRInvoice.Where(x => x.CreateDate == DateTime.Today).OrderBy(x => x.CreateDate).Select(x => x.BaId).Distinct().ToList();
                if (newBAID != null && newBAID.Count > 0)
                {
                    foreach (var item in newBAID)
                    {
                        var result = db.TbRInvoice.Where(x => x.BaId == item.Value).FirstOrDefault();

                        var newBA_NO = new K2_Billing_AppointmentContextProcedures(db);
                        var rsSp = await newBA_NO.usp_GetNewRunningNoAsync("True",null);

                        var add = (new TbRBa
                        {
                            BaId = item.Value,
                            BaNo = newBA_NO.ToString(),
                            PackageId = Guid.NewGuid(),
                            InvoiceToComany = result.BillToCompany,
                            InvoiceToPerson = result.DeliveryToPerson,
                            InvoiceToAddress = result.DeliveryAddress,
                            InvoiceCustCode = result.CustCode,
                            InvoiceToTel = result.DeliveryToTel,
                            InvoiceNote = null,
                            ReportToComany = result.ReportToComany,
                            ReportsToAddress = result.ReportsToAddress,
                            ReportsToTel = result.ReportsToTel,
                            StatusCode = "5",
                            CreateBy = "K2Admin",
                            CreateDate = DateTime.Now
                        });
                        //db.TbRBa.Add(add);
                        //db.SaveChanges();
                    }
                    //db.SaveChanges();
                }

                var newPackageID = db.TbRBa.Where(x => x.CreateDate == DateTime.Today).OrderBy(x => x.CreateDate).Select(x => x.PackageId).Distinct().ToList();
                if (newPackageID != null)
                {
                    foreach (var item in newPackageID)
                    {
                        var result = db.TbRBa.Where(x => x.PackageId == item.Value).FirstOrDefault();

                        var newPAC_NO = new K2_Billing_AppointmentContextProcedures(db);
                        var rsSp = await newPAC_NO.usp_GetNewRunningNoAsync("False", result.DeliveryType);

                        var add = (new TbRPackage
                        {
                            PackageId = item.Value,
                            PackageNo = newPAC_NO.ToString(),
                            InvoiceToAddress = result.InvoiceToAddress,
                            InvoiceToPerson = result.InvoiceToPerson,
                            InvoiceToCustCode = result.InvoiceCustCode,
                            InvoiceToCompany = result.InvoiceToComany,
                            InvoiceDeliveryPhone = result.InvoiceToTel,
                            InvoiceDeliveryLab = result.DeliveryLab,
                            StatusCode = "2",
                            CreateBy = "K2Admin",
                            CreateDate = DateTime.Now
                        });
                        db.TbRPackage.Add(add);
                        db.SaveChanges();
                    }
                    //db.SaveChanges();
                }

                return Ok(new BaseResponseViewModel<TbSBillingAppointmentReportData>()
                {
                    is_error = false,
                    msg_alert = "Sucess",
                    data = null
                });
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseViewModel<TbSBillingAppointmentReportData>()
                {
                    is_error = true,
                    msg_alert = "Fail" + ex.Message,
                    data = null
                });
            }

        }

    }
}
