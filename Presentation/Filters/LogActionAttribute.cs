using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookDemo.Presentation.Filters
{
    /// <summary>
    /// Logs information about incoming HTTP requests before and after action execution.
    /// 
    /// This filter captures:
    /// - Controller and action names
    /// - HTTP method and request path
    /// - Response status code
    /// - Trace identifier (for request tracking)
    /// - Parameter types (only before execution)
    /// 
    /// NOTE:
    /// This is intended for structured logging (e.g. Serilog, NLog).
    /// </summary>
    public class LogActionAttribute : ActionFilterAttribute
    {
        private readonly ILogger<LogActionAttribute> _logger;

        /// <summary>
        /// Constructor with dependency injection for ILogger.
        /// </summary>
        public LogActionAttribute(ILogger<LogActionAttribute> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Executes BEFORE the action method runs.
        /// 
        /// At this stage:
        /// - Request parameters are available
        /// - Response is not finalized yet
        /// </summary>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var logDetails = BuildLogDetails(context);

            // Structured logging: object is serialized automatically
            _logger.LogInformation("Action executing: {@LogDetails}", logDetails);
        }

        /// <summary>
        /// Executes AFTER the action method has finished.
        /// 
        /// At this stage:
        /// - Response status code is available
        /// - Action result has been produced
        /// </summary>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var logDetails = BuildLogDetails(context);

            _logger.LogInformation("Action executed: {@LogDetails}", logDetails);
        }

        /// <summary>
        /// Builds a LogDetails object from the current filter context.
        /// 
        /// This method is static because:
        /// - It does not depend on instance fields (_logger etc.)
        /// - It is a pure helper method (input -> output)
        /// 
        /// It extracts:
        /// - Route data (controller, action)
        /// - HTTP metadata
        /// - Response status
        /// - Optional parameter type information
        /// </summary>
        private static LogDetails BuildLogDetails(FilterContext context)
        {
            var routeData = context.RouteData;

            // Only available BEFORE execution (ActionExecutingContext)
            var parameterTypes = context is ActionExecutingContext actionExecutingContext
                ? BuildParameterTypes(actionExecutingContext)
                : null;

            return new LogDetails(
                Controller: routeData.Values["controller"]?.ToString(),
                Action: routeData.Values["action"]?.ToString(),
                HttpMethod: context.HttpContext.Request.Method,
                Path: context.HttpContext.Request.Path,
                StatusCode: context.HttpContext.Response.StatusCode,
                TraceId: context.HttpContext.TraceIdentifier,
                ParameterTypes: parameterTypes
            );
        }

        /// <summary>
        /// Extracts parameter names and their types from the action arguments.
        /// 
        /// Example output:
        /// "id:Int32, name:String"
        /// 
        /// IMPORTANT:
        /// We log only types (not values) to avoid exposing sensitive data.
        /// </summary>
        private static string? BuildParameterTypes(ActionExecutingContext context)
        {
            if (!context.ActionArguments.Any())
                return null;

            return string.Join(", ",
                context.ActionArguments.Select(argument =>
                    $"{argument.Key}: {argument.Value?.GetType().Name ?? "null"}"));
        }
    }
}