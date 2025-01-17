using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Finacle.Infrastructure.Notifications
{

    public static class Extensions
    {
        public static IServiceCollection AddNotification(this IServiceCollection services)
        {

            // In-memory Database for OTPs
            services.AddDbContext<OtpDb>(opt => opt.UseInMemoryDatabase("CustomerOtps"));
            services.AddScoped<SmsClient>();
            services.AddScoped<EmailClient>();
            services.AddScoped<OtpClient>();


            return services;
        }

    }

}
