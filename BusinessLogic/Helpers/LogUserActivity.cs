using System;
using System.Threading.Tasks;
using BusinessLogic.Extensions;
using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;


namespace BusinessLogic.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userId = resultContext.HttpContext.User.GetUserId();
            var uow = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();
            var user = await uow.UserRepository.GetUserByIdAsync(userId);
            user.LastActive = DateTime.Now;
            await uow.Complete();
        }
    }
}