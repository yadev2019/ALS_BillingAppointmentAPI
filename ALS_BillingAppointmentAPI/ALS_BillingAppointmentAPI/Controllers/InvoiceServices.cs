using ALS_BillingAppointmentAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private readonly IConfiguration _configuration;
        public static string baseUrl;
        public static string userName;
        public static string Password;

        public InvoiceServices(IConfiguration configuration)
        {
            _configuration = configuration;
            baseUrl = _configuration["AppSettings:BaseUrl"];
            userName = _configuration["AppSettings:Username"];
            Password = _configuration["AppSettings:Password"];
        }

        [HttpGet("GetInvoiceHeader")]
        public async Task<IActionResult> GetInvoiceHeader()
        {
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
            var url = baseUrl + "/Default.GetBillingAppointmentReportData(invoiceNumbersList='1111275')";
            var credentialsCache = new CredentialCache
            {
                {new Uri(url), "NTLM", new NetworkCredential(
                    userName,Password
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
