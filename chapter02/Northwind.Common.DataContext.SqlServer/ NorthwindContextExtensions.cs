using Microsoft.EntityFrameworkCore; //UseSqlServer
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options; //IServiceCollection

namespace Packt.Shared
{
    public static class NorthwindContextExtensions
    {
        ///<summary>
        ///Adds NorthwindContext to the specified IServiceCollection.Uses the SqlServer database
        ///</summary>
        ///<param name="services"></param>
        //////<param name="connectionString">Set to override the default</param>
        ///<returns>An Iservicecollection that can be used to add more services.</returns>

        public static IServiceCollection AddNorthwindContext(this IServiceCollection services,
            string connectionString = "Data Source = .; Initial Catalog = Northwind;" +
                "Integrated Security = true; MultipleActiveResultsets=true;Encrypt=true=false")
        {
            services.AddDbContext<NorthwindContext>(options =>
            {
                options.UseSqlServer(connectionString);
                options.LogTo(Console.WriteLine, new[] {
                    Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuting});
            });
            return services;
        }

    }
}