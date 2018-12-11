using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AspNetCoreWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : Controller
    {
        private readonly ILogger<TestController> _logger;
        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        private IDisposable _disposableControllerProp;
        private IDisposable _disposableActionProp;
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];

            _disposableControllerProp = LogContext.PushProperty("Controller", controllerName);
            _disposableActionProp = LogContext.PushProperty("Action", actionName);

            _logger.LogInformation("Starting action.");

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            _logger.LogInformation("Ending action.");

            _disposableActionProp.Dispose();
            _disposableControllerProp.Dispose();
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            try
            {
                _logger.LogInformation("Custom outer log message before.");

                await Task.Delay(1);

                using (LogContext.PushProperty("CustomLogPropKey", "customLogPropValue"))
                {
                    _logger.LogInformation("Custom inner log message.");

                    await Task.Delay(1);

                    return new[] {"value1", "value2"};
                }
            }
            finally
            {
                _logger.LogInformation("Custom outer log message after.");
            }
        }
        
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
        
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
        
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}