using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ExampleMudSystem.DAL;
using ExampleMudSystem.BLL;

namespace ExampleMudSystem
{
    public static class ExampleExtension
    {
        public static void AddBackendDependencies(this IServiceCollection services,
                                                    Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<HogWildContext>(options);

            services.AddTransient<WorkingVersionService>(serviceProvider =>
            {
                var context = serviceProvider.GetService<HogWildContext>();

                return new WorkingVersionService(context);
            });

            services.AddTransient<CustomerService>(serviceProvider =>
            {
                var context = serviceProvider.GetService<HogWildContext>();

                return new CustomerService(context);
            });

			services.AddTransient<CustomerService>(serviceProvider =>
			{
				var context = serviceProvider.GetService<HogWildContext>();

				return new CustomerService(context);
			});

			services.AddTransient<CategoryLookupService>(serviceProvider =>
			{
				var context = serviceProvider.GetService<HogWildContext>();

				return new CategoryLookupService(context);
			});

			services.AddTransient<PartService>(serviceProvider =>
			{
				var context = serviceProvider.GetService<HogWildContext>();

				return new PartService(context);
			});

			services.AddTransient<InvoiceService>(serviceProvider =>
			{
				var context = serviceProvider.GetService<HogWildContext>();

				return new InvoiceService(context);
			});
		}
    }
}
