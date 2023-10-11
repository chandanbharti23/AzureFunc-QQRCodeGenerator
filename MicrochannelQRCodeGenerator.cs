using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QRCoder;

namespace AzureFunc_QQRCodeGenerator
{
    public static class MicrochannelQRCodeGenerator
    {
        [FunctionName("MicrochannelQRCodeGenerator")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string ReqData = req.Query["ReqData"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            ReqData ??= data?.url;
            if (string.IsNullOrEmpty(ReqData))
            {
                return new BadRequestResult();
            }

            string generator = ReqData;
            string payload = generator.ToString();

            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeAsPng = qrCode.GetGraphic(20);
            return new FileContentResult(qrCodeAsPng, "image/png");
        }
    }
}
