using Finacle.Infrastructure.Auth;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finacle.Infrastructure.Config
{
    public static class SecureConfigurationExtensions
    {
        public static void DecryptConnectionStrings(this IConfiguration configuration)
        {
            var encrypted = configuration.GetConnectionString("OracleDb");

            if (!string.IsNullOrWhiteSpace(encrypted))
            {
                var decrypted = EncryptionHelper.Decrypt(encrypted);
                configuration["ConnectionStrings:OracleDb"] = decrypted;
            }
        }
    }
}
