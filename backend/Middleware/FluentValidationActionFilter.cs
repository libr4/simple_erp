using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Backend.Middleware
{
    /// <summary>
    /// Action filter that executes FluentValidation validators for action arguments.
    /// This ensures query/body/route-bound complex types are validated before controller execution.
    /// </summary>
    public class FluentValidationActionFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public FluentValidationActionFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var kv in context.ActionArguments)
            {
                var argValue = kv.Value;
                if (argValue == null) continue;

                var argType = argValue.GetType();
                var validatorType = typeof(IValidator<>).MakeGenericType(argType);
                var validator = _serviceProvider.GetService(validatorType) as IValidator;
                if (validator == null) continue;

                var validationContextType = typeof(ValidationContext<>).MakeGenericType(argType);
                var validationContext = Activator.CreateInstance(validationContextType, argValue);

                // Call ValidateAsync via dynamic to avoid reflection boilerplate
                var validateAsyncMethod = validator.GetType().GetMethod("ValidateAsync", new[] { typeof(ValidationContext<>).MakeGenericType(argType), typeof(System.Threading.CancellationToken) });
                object resultObj;
                if (validateAsyncMethod != null)
                {
                    var task = (System.Threading.Tasks.Task)validateAsyncMethod.Invoke(validator, new[] { validationContext, default(System.Threading.CancellationToken) });
                    await task.ConfigureAwait(false);
                    var resultProperty = task.GetType().GetProperty("Result");
                    resultObj = resultProperty.GetValue(task);
                }
                else
                {
                    // Fallback to synchronous Validate
                    var validateMethod = validator.GetType().GetMethod("Validate", new[] { typeof(ValidationContext<>).MakeGenericType(argType) });
                    resultObj = validateMethod.Invoke(validator, new[] { validationContext });
                }

                // resultObj is FluentValidation.Results.ValidationResult
                var failuresProp = resultObj.GetType().GetProperty("Errors");
                var failures = failuresProp.GetValue(resultObj) as System.Collections.IEnumerable;
                foreach (var f in failures)
                {
                    var propName = f.GetType().GetProperty("PropertyName").GetValue(f) as string ?? string.Empty;
                    var error = f.GetType().GetProperty("ErrorMessage").GetValue(f) as string;
                    context.ModelState.AddModelError(propName, error);
                }
            }

            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
                return;
            }

            await next();
        }
    }
}
