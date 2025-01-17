using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finacle.Infrastructure.Services
{
    public class OtpDb : DbContext
    {
        public OtpDb(DbContextOptions<OtpDb> options) : base(options)
        { }

        public DbSet<CustomerOtp> CustomerOtps => Set<CustomerOtp>();
    }

    public class CustomerOtp
    {
        public int Id { get; set; }
        public string MobileNumber { get; set; }
        public string OTP { get; set; }
    }
}
