using ALS_BillingAppointmentAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ALS_BillingAppointmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceServices : ControllerBase
    {
        [HttpGet("GetInvoiceHeader")]
        public async Task<IActionResult> GetInvoiceHeader()
        {
            var url = "http://webdata.uatangel.corp.alsglobal.org/odata/Invoices/Default.GetInvoiceHeaders(startDate='2021-07-01',endDate='2021-07-16',invoiceNumber='',workorderCode='')";
            var credentialsCache = new CredentialCache
            {
                {new Uri(url), "NTLM", new NetworkCredential(
                    "athit.khaokaew","K8[ZdKR3" //,"http://webdata.uatangel.corp.alsglobal.org"
                )}
            };
            var handler = new HttpClientHandler { Credentials = credentialsCache };
            var client = new HttpClient(handler);
            var res = await client.GetAsync(url);

            InvoiceHeaderModel inv = new InvoiceHeaderModel();
            var task = client.GetAsync(url)
              .ContinueWith((taskwithresponse) =>
              {
                  var response = taskwithresponse.Result;
                  var jsonString = response.Content.ReadAsStringAsync();
                  jsonString.Wait();

                  inv = JsonConvert.DeserializeObject<InvoiceHeaderModel>(jsonString.Result);

              });
            task.Wait();

            return Ok(inv);
        }

        [HttpGet("GetInvoiceDetail")]
        public async Task<IActionResult> GetInvoiceDetail()
        {
            //var baseUrl = "http://webdata.uatangel.corp.alsglobal.org";
            var url = "http://webdata.uatangel.corp.alsglobal.org/odata/Invoices/Default.GetBillingAppointmentReportData(invoiceNumbersList='1111275')";
            var credentialsCache = new CredentialCache
            {
                {new Uri(url), "NTLM", new NetworkCredential(
                    "athit.khaokaew","K8[ZdKR3" //,"http://webdata.uatangel.corp.alsglobal.org"
                )}
            };
            var handler = new HttpClientHandler { Credentials = credentialsCache };
            var client = new HttpClient(handler);
            var res = await client.GetAsync(url);

            InvoiceDetailModel inv = new InvoiceDetailModel();
            var task = client.GetAsync(url)
              .ContinueWith((taskwithresponse) =>
              {
                  var response = taskwithresponse.Result;
                  var jsonString = response.Content.ReadAsStringAsync();
                  jsonString.Wait();

                  inv = JsonConvert.DeserializeObject<InvoiceDetailModel>(jsonString.Result);

              });
            task.Wait();

            return Ok(inv);
        }


    }
}
