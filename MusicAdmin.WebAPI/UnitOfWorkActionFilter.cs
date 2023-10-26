using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using MusicInfrastructure;
using System.Reflection;

namespace MusicAdmin.WebAPI
{
    public class UnitOfWorkFilter : IAsyncActionFilter
    {
        //private readonly IMediator mediator;
        //public UnitOfWorkFilter(IMediator mediator)
        //{
        //    this.mediator = mediator;
        //}

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            var result = await next();
            if (result.Exception != null)
            {
                return;
            }

            //MethodInfo是ControllerActionDescriptor独有的，所以需要显式转换
            var actionDesc = context.ActionDescriptor as ControllerActionDescriptor;
            if (actionDesc == null)
            {
                return;
            }
            //获取控制器上的attribute，是否有UnitOfWorkAttribute
            var uowAttr = actionDesc.ControllerTypeInfo.GetCustomAttribute<UnitOfWorkAttribute>();
            if (uowAttr == null)
            {
                return;
            }
            foreach (var dbCtxType in uowAttr.DbConextTypes)
            {
                //通过DI获得Dbcontext的实例
                var dbCtx = context.HttpContext.RequestServices.GetService(dbCtxType) as MusicDBContext;
                if (dbCtx != null)
                {

                    //var domainEntities = dbCtx.ChangeTracker
                    //.Entries<IDomainEvents>()
                    //.Where(x => x.Entity.GetDomainEvents().Any());

                    //var domainEvents = domainEntities
                    //    .SelectMany(x => x.Entity.GetDomainEvents())
                    //    .ToList();//加ToList()是为立即加载，否则会延迟执行，到foreach的时候已经被ClearDomainEvents()了

                    //domainEntities.ToList()
                    //    .ForEach(entity => entity.Entity.ClearDomainEvents());
                    //foreach (var domainEvent in domainEvents)
                    //{
                    //    await mediator.Publish(domainEvent);
                    //}

                    await dbCtx.SaveChangesAsync();
                }
            }
        }
    }
}
