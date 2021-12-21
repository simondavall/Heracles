using System;
using Heracles.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Heracles.Web.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private readonly ILogger<ImportController> _logger;
        private readonly IImportProgressService _progressService;


        public ImportController(IImportProgressService progressService, ILogger<ImportController> logger)
        {
            _progressService = progressService;
            _logger = logger;
        }
        
        [HttpGet]
        public string UpdateProgress(string processId)
        {
            _logger.LogDebug($"Called UpdateProgress with processId: {processId}");
            if (!Guid.TryParse(processId, out var processGuid))
            {
                throw new ArgumentException("processId must be a valid Guid.");
            }

            var returnValue = _progressService.GetImportProgress(processGuid);
            _logger.LogDebug($"Value: {returnValue} returned from UpdateProgress");

            var jResult = new JObject
            {
                ["value"] = returnValue
            };
            return jResult.ToString(Formatting.None);
        }
    }
}