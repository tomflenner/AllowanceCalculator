using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AllowanceCalculator.Function
{
    public static class AllowanceCalculator
    {
        [FunctionName("AllowanceCalculator")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string fixedAllowanceQueryParam = req.Query["fixedAllowance"];
            string geographicAllowanceQueryParam = req.Query["geographicAllowance"];
            string housingSupportQueryParam = req.Query["housingSupport"];

            if (string.IsNullOrEmpty(fixedAllowanceQueryParam))
                return new BadRequestObjectResult("Missing the query parameter fixedAllowance in HTTP GET Request");

            if (string.IsNullOrEmpty(geographicAllowanceQueryParam))
                return new BadRequestObjectResult("Missing the query parameter geographicAllowance in HTTP GET Request");

            decimal fixedAllowance, geographicAllowance;

            if (!decimal.TryParse(fixedAllowanceQueryParam, out fixedAllowance))
                return new ObjectResult(value: "Error while parsing fixedAllowanceQueryParam to decimal.") { StatusCode = StatusCodes.Status500InternalServerError };

            if (!decimal.TryParse(geographicAllowanceQueryParam, out geographicAllowance))
                return new ObjectResult(value: "Error while parsing geographicAllowanceQueryParam to decimal.") { StatusCode = StatusCodes.Status500InternalServerError };

            if (string.IsNullOrEmpty(housingSupportQueryParam))
            {
                return new OkObjectResult(new
                {
                    fixedAllowance = fixedAllowance,
                    geographicalAllowance = geographicAllowance,
                    totalAllowance = fixedAllowance + geographicAllowance,
                });
            }
            else
            {
                decimal housingSupport;

                if (!decimal.TryParse(housingSupportQueryParam, out housingSupport))
                    return new ObjectResult(value: "Error while parsing housingSupportQueryParam to decimal.") { StatusCode = StatusCodes.Status500InternalServerError };

                return new OkObjectResult(new
                {
                    fixedAllowance = fixedAllowance,
                    geographicalAllowance = geographicAllowance,
                    depressionAllowance = geographicAllowance * (decimal)0.2,
                    housingSupport = housingSupport,
                    totalAllowance = fixedAllowance + geographicAllowance * (decimal)0.8 + housingSupport,
                });
            }
        }
    }
}
