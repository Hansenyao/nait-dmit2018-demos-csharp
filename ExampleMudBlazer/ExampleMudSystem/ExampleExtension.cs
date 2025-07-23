using ExampleMudSystem.BLL;
using ExampleMudSystem.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ExampleMudSystem;

public static class ExampleMudExtersion
{
    public static void AddBackendDependencies(this IServiceCollection services,
        Action<DbContextOptionsBuilder> options)
    {
        services.AddDbContext<OLTPDMIT2018Context>(options);

        services.AddTransient<WorkingVersionService>((ServiceProvider) =>
        {
            var context = ServiceProvider.GetService<OLTPDMIT2018Context>();

            return new WorkingVersionService(context);
        });

        services.AddTransient<CustomerService>((ServiceProvider) =>
        {
            var context = ServiceProvider.GetService<OLTPDMIT2018Context>();

            return new CustomerService(context);
        });

        services.AddTransient<CategoryLookupService>((ServiceProvider) =>
        {
            var context = ServiceProvider.GetService<OLTPDMIT2018Context>();

            return new CategoryLookupService(context);
        });
    }

}
