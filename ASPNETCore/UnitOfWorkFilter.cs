using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Transactions;

namespace ASPNETCore;

public class UnitOfWorkFilter : IAsyncActionFilter
{
    private static UnitOfWorkAttribute? GetUnitOfWorkAttribute(ActionDescriptor actionDescriptor)
    {
        var controllerActionDescriptor = actionDescriptor as ControllerActionDescriptor;
        if (controllerActionDescriptor == null)
        {
            return null;
        }
        //try to get UnitOfWorkAttribute from controller,
        //if there is no UnitOfWorkAttribute on controller,
        //try to get UnitOfWorkAttribute from action
        var unitOfWorkAttribute = controllerActionDescriptor.ControllerTypeInfo
            .GetCustomAttribute<UnitOfWorkAttribute>();
        return unitOfWorkAttribute ?? controllerActionDescriptor.MethodInfo
                .GetCustomAttribute<UnitOfWorkAttribute>();
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var unitOfWorkAttribute = GetUnitOfWorkAttribute(context.ActionDescriptor);
        if (unitOfWorkAttribute == null)
        {
            await next();
            return;
        }
        using TransactionScope transactionScope = new(TransactionScopeAsyncFlowOption.Enabled);
        List<DbContext> dbContexts = new();
        foreach (var dbContextType in unitOfWorkAttribute.DbContextTypes)
        {
            //用HttpContext的RequestServices
            //确保获取的是和请求相关的Scope实例
            var sp = context.HttpContext.RequestServices;
            DbContext dbContext = (DbContext)sp.GetRequiredService(dbContextType);
            dbContexts.Add(dbContext);
        }
        var result = await next();
        if (result.Exception == null)
        {
            foreach (var dbContext in dbContexts)
            {
                await dbContext.SaveChangesAsync();
            }
            transactionScope.Complete();
        }
    }
}