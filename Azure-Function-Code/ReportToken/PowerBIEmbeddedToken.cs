using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ReportToken.Services;
using ReportToken.Models;

namespace ReportToken
{
    public static class PowerBIEmbeddedToken
    {

        [FunctionName("PowerBIEmbeddedToken")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("Power BI Report Token Requested");
                // string reportId = "0c94ccca-55f8-4cd8-95c4-2ce136ec5e22";

                string reportId = req.Query["reportid"];

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                reportId = reportId ?? data?.reportId;

                log.LogInformation("reportId: " + reportId);

                if (!String.IsNullOrWhiteSpace(reportId))
                {
                    string username = null;
                    string roles = null;


                    IEmbedService m_embedService = new EmbedService();
                    var embedResult = await m_embedService.EmbedReport(username, roles, reportId);

                    if (embedResult)
                    {
                        ResultJson resultJson = new ResultJson()
                        {
                            EmbedToken = m_embedService.EmbedConfig.EmbedToken.Token,
                            EmbedUrl = m_embedService.EmbedConfig.EmbedUrl,
                            ReportId = reportId
                        };

                        string resultString = JsonConvert.SerializeObject(resultJson);

                        return (ActionResult)new OkObjectResult(resultString);
                    }
                    else
                    {
                        return (ActionResult)new BadRequestObjectResult("Could not generate a token.");
                    }
                }
                else
                {
                    return (ActionResult)new BadRequestObjectResult("Report Id is empty or null.");
                }
            }
            catch (Exception ex)
            {
                log.LogInformation("Exception: " + ex.ToString());
                return (ActionResult)new BadRequestObjectResult("An Exception has occurred.");
            }
        } // Run

    } // class

} // namespace
