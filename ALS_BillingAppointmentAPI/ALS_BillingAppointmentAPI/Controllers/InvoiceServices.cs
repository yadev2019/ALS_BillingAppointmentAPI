using ALS_BillingAppointmentAPI.DB;
using ALS_BillingAppointmentAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
                //var getDate = DateTime.Today.ToShortDateString();
                ////var endDate = DateTime.Today.ToShortDateString();
                //var startDate = DateTime.Today.Year + "-" + DateTime.Today.Month + "-" + DateTime.Today.Day;
                //var invoiceNumber = (dynamic)null;
                //var workorderCode = (dynamic)null;

                var url = baseUrl + "/Default.GetInvoiceHeaders(startDate='2021-07-01',endDate='2021-07-16',invoiceNumber='',workorderCode='')";
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

                      inv = JsonConvert.DeserializeObject<InvoiceHeaderModel>(jsonString.Result);

                      List<TbSInvoiceHeader> entityInvoiceHeader = new List<TbSInvoiceHeader>();
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
                                  InvoiceIssuedDate = item.InvoiceIssuedDate.ToString(),
                                  InvoiceAmount = item.InvoiceAmount.ToString(),
                                  QuoteCode = item.QuoteCode,
                                  CreateDate = DateTime.Now,
                                  CreateBy = "K2Admin",
                              });
                              db.TbSInvoiceHeader.Add(add);
                          }
                      }
                      db.SaveChanges();

                  });
                task.Wait();

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
                List<TbSInvoiceHeader> invoiceHeader = new List<TbSInvoiceHeader>();
                invoiceHeader = db.TbSInvoiceHeader.ToList();

                //var urlGetDetail = baseUrl + $"/Default.GetBillingAppointmentReportData(invoiceNumbersList=";
                var urlGetDetail = baseUrl + $"/Default.GetBillingAppointmentReportData(invoiceNumbersList='1111275')";

                if (invoiceHeader != null && invoiceHeader.Count > 0)
                {
                    foreach(var item in invoiceHeader)
                    {
                        //urlGetDetail += $"{item.InvoiceId})";


                        var credentialsCache = new CredentialCache
                        {
                        {new Uri(urlGetDetail), "NTLM", new NetworkCredential(
                            userName,Password
                        )}
                        };
                        var handler = new HttpClientHandler { Credentials = credentialsCache };
                        var client = new HttpClient(handler);
                        var res = await client.GetAsync(urlGetDetail);

                        InvoiceDetailModel inv = new InvoiceDetailModel();
                        var task = client.GetAsync(urlGetDetail)
                          .ContinueWith((taskwithresponse) =>
                          {
                              var response = taskwithresponse.Result;
                              var jsonString = response.Content.ReadAsStringAsync();
                              jsonString.Wait();
                              inv = JsonConvert.DeserializeObject<InvoiceDetailModel>(jsonString.Result);

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
                                          InvoiceDeliveryLaboratory = (item.InvoiceDeliveryLaboratory == null ? "" : item.InvoiceDeliveryLaboratory.ToString()),
                                          AnalysisReportDeliveryLaboratory = item.AnalysisReportDeliveryLaboratory,
                                          QuoteCode = (item.QuoteCode == null ? "" : item.QuoteCode.ToString()),
                                          CreditTerm = (item.CreditTerm == null ? "" : item.CreditTerm.ToString()),
                                          InvoiceDeliveryClientCode = item.InvoiceDeliveryClientCode,
                                          AnalysisReportDeliverToClientCode = item.AnalysisReportDeliverToClientCode,
                                          AnalysisReportDeliverToClientName = item.AnalysisReportDeliverToClientName,
                                          InvoiceDeliveryClientName = item.InvoiceDeliveryClientName,
                                          InvoiceDeliverToPersonName = item.InvoiceDeliverToPersonName,
                                          InvoiceDeliverToPersonTelephone = item.InvoiceDeliverToPersonTelephone,
                                          InvoiceDeliverToPersonMobile = item.InvoiceDeliverToPersonMobile,
                                          AnalysisReportDeliverToPersonName = item.AnalysisReportDeliverToPersonName,
                                          AnalysisReportDeliverToPersonTelephone = item.AnalysisReportDeliverToPersonTelephone,
                                          AnalysisReportDeliverToPersonMobile = (item.AnalysisReportDeliverToPersonMobile == null ? "" : item.AnalysisReportDeliverToPersonMobile.ToString()),
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
                                          InvoiceDeliveryNotes = (item.InvoiceDeliveryNotes == null ? "" : item.InvoiceDeliveryNotes.ToString()),
                                          InvoiceDeliveryProcess = (item.InvoiceDeliveryProcess == null ? "" : item.InvoiceDeliveryProcess.ToString()),
                                          InvoiceIssuedDate = (item.InvoiceIssuedDate == null ? "" : item.InvoiceIssuedDate.ToString()),
                                          InvoiceDateOverride = (item.InvoiceDateOverride == null ? "" : item.InvoiceDateOverride.ToString()),
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
                                          PaymentDeliveryProcess = (item.PaymentDeliveryProcess == null ? "" : item.PaymentDeliveryProcess.ToString()),
                                          PaymentDeliveryNotes = (item.PaymentDeliveryNotes == null ? "" : item.PaymentDeliveryNotes.ToString()),
                                          ReportLocation = (item.ReportLocation == null ? "" : item.ReportLocation.ToString()),
                                          GenerateBillingAppointmentPerInvoice = (item.GenerateBillingAppointmentPerInvoice == null ? "" : item.GenerateBillingAppointmentPerInvoice.ToString()),
                                          InvoicePaid = (item.InvoicePaid == null ? "" : item.InvoicePaid.ToString()),
                                      });
                                      //db.TbSBillingAppointmentReportData.Add(add);
                                  }
                              }
                              //db.SaveChanges();
                          });
                          task.Wait();
                    }
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

        [HttpGet("GenerateBA")]
        public async Task<IActionResult> GenerateBA()
        {
            try
            {
                List<TbSInvoiceHeader> invoiceHeader = new List<TbSInvoiceHeader>();
                invoiceHeader = db.TbSInvoiceHeader.ToList();

                //var urlGetDetail = baseUrl + $"/Default.GetBillingAppointmentReportData(invoiceNumbersList=";
                var urlGetDetail = baseUrl + $"/Default.GetBillingAppointmentReportData(invoiceNumbersList='1111275')";

                if (invoiceHeader != null && invoiceHeader.Count > 0)
                {
                    foreach (var item in invoiceHeader)
                    {
                        //urlGetDetail += $"{item.InvoiceId})";


                        var credentialsCache = new CredentialCache
                        {
                        {new Uri(urlGetDetail), "NTLM", new NetworkCredential(
                            userName,Password
                        )}
                        };
                        var handler = new HttpClientHandler { Credentials = credentialsCache };
                        var client = new HttpClient(handler);
                        var res = await client.GetAsync(urlGetDetail);

                        InvoiceDetailModel inv = new InvoiceDetailModel();
                        var task = client.GetAsync(urlGetDetail)
                          .ContinueWith((taskwithresponse) =>
                          {
                              var response = taskwithresponse.Result;
                              var jsonString = response.Content.ReadAsStringAsync();
                              jsonString.Wait();
                              inv = JsonConvert.DeserializeObject<InvoiceDetailModel>(jsonString.Result);

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
                                          InvoiceDeliveryLaboratory = (item.InvoiceDeliveryLaboratory == null ? "" : item.InvoiceDeliveryLaboratory.ToString()),
                                          AnalysisReportDeliveryLaboratory = item.AnalysisReportDeliveryLaboratory,
                                          QuoteCode = (item.QuoteCode == null ? "" : item.QuoteCode.ToString()),
                                          CreditTerm = (item.CreditTerm == null ? "" : item.CreditTerm.ToString()),
                                          InvoiceDeliveryClientCode = item.InvoiceDeliveryClientCode,
                                          AnalysisReportDeliverToClientCode = item.AnalysisReportDeliverToClientCode,
                                          AnalysisReportDeliverToClientName = item.AnalysisReportDeliverToClientName,
                                          InvoiceDeliveryClientName = item.InvoiceDeliveryClientName,
                                          InvoiceDeliverToPersonName = item.InvoiceDeliverToPersonName,
                                          InvoiceDeliverToPersonTelephone = item.InvoiceDeliverToPersonTelephone,
                                          InvoiceDeliverToPersonMobile = item.InvoiceDeliverToPersonMobile,
                                          AnalysisReportDeliverToPersonName = item.AnalysisReportDeliverToPersonName,
                                          AnalysisReportDeliverToPersonTelephone = item.AnalysisReportDeliverToPersonTelephone,
                                          AnalysisReportDeliverToPersonMobile = (item.AnalysisReportDeliverToPersonMobile == null ? "" : item.AnalysisReportDeliverToPersonMobile.ToString()),
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
                                          InvoiceDeliveryNotes = (item.InvoiceDeliveryNotes == null ? "" : item.InvoiceDeliveryNotes.ToString()),
                                          InvoiceDeliveryProcess = (item.InvoiceDeliveryProcess == null ? "" : item.InvoiceDeliveryProcess.ToString()),
                                          InvoiceIssuedDate = (item.InvoiceIssuedDate == null ? "" : item.InvoiceIssuedDate.ToString()),
                                          InvoiceDateOverride = (item.InvoiceDateOverride == null ? "" : item.InvoiceDateOverride.ToString()),
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
                                          PaymentDeliveryProcess = (item.PaymentDeliveryProcess == null ? "" : item.PaymentDeliveryProcess.ToString()),
                                          PaymentDeliveryNotes = (item.PaymentDeliveryNotes == null ? "" : item.PaymentDeliveryNotes.ToString()),
                                          ReportLocation = (item.ReportLocation == null ? "" : item.ReportLocation.ToString()),
                                          GenerateBillingAppointmentPerInvoice = (item.GenerateBillingAppointmentPerInvoice == null ? "" : item.GenerateBillingAppointmentPerInvoice.ToString()),
                                          InvoicePaid = (item.InvoicePaid == null ? "" : item.InvoicePaid.ToString()),
                                      });
                                      //db.TbSBillingAppointmentReportData.Add(add);
                                  }
                              }
                              //db.SaveChanges();
                          });
                        task.Wait();
                    }
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
