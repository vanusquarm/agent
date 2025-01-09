using Microsoft.EntityFrameworkCore;

namespace Notification.Entities;

public class OtpDb : DbContext
{
    public OtpDb(DbContextOptions<OtpDb> options) : base(options)
    { }

    public DbSet<CustomerOtp> CustomerOtps => Set<CustomerOtp>();
}