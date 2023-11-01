using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Sufficit.Telephony.JsSIP
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJsSIP(this IServiceCollection services)
        {
            IConfiguration configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

            // Definindo o local da configuração global
            // Importante ser dessa forma para o sistema acompanhar as mudanças no arquivo de configuração em tempo real 
            services.Configure<JsSIPOptions>(configuration.GetSection(JsSIPOptions.SECTIONNAME));

            // Incluindo serviço de softphone em javascript
            services.AddTransient<JsSIPSessions>();
            services.AddScoped<JsSIPService>();

            return services;
        }
    }
}
