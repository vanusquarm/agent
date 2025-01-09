using Microsoft.EntityFrameworkCore;

namespace Notification.Entities;

public class CustomerOtp
{
    public int Id { get; set; }
    public string MobileNumber { get; set; }
    public string OTP { get; set; }
}