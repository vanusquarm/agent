using System.Security.Principal;

namespace Finacle.Infrastructure.Notifications;

public class OtpClient
{
    private readonly OtpDb _customerDb;
    private readonly SmsClient _smsClient;
    private readonly EmailClient _emailClient;

    public OtpClient(OtpDb OtpDb, SmsClient smsClient, EmailClient emailClient)
    {
        _customerDb = OtpDb;
        _smsClient = smsClient;
        _emailClient = emailClient;
    }

    public async Task<OtpReturn?> SendOtp(string mobileNumber)
    {
        var to = mobileNumber.StartsWith('0') ? "233" + mobileNumber.TrimStart('0') : mobileNumber;
        var randomNumber = new Random().Next(1000, 9999).ToString();
        var findCustomer = _customerDb.CustomerOtps.FirstOrDefault(item => item.MobileNumber.Equals(mobileNumber));

        if (findCustomer == null)
        {
            _customerDb.CustomerOtps.Add(new CustomerOtp
            {
                MobileNumber = mobileNumber,
                OTP = randomNumber
            });

            await _customerDb.SaveChangesAsync();
        }
        else
        {
            findCustomer!.OTP = randomNumber;
            _customerDb.CustomerOtps.Update(findCustomer);
            await _customerDb.SaveChangesAsync();
        }

        var response = await _smsClient.SendSms(to, $"737 Monitoring Portal OTP: {randomNumber}. Please don't share");
        if (!response.IsSuccessful)
        {
            throw new Exception("Failed to send otp");
        }

        return new OtpReturn
        {
            MobileNumber = mobileNumber,
            Otp = randomNumber
        };
    }

    public async Task<OtpReturn?> SendMailOtp(string emailAddress)
    {
        var to = emailAddress;
        var randomNumber = new Random().Next(10000000, 99999999).ToString();
        var findCustomer = _customerDb.CustomerOtps.FirstOrDefault(item => item.MobileNumber.Equals(emailAddress));

        if (findCustomer == null)
        {
            _customerDb.CustomerOtps.Add(new CustomerOtp
            {
                MobileNumber = emailAddress,
                OTP = randomNumber
            });

            await _customerDb.SaveChangesAsync();
        }
        else
        {
            findCustomer!.OTP = randomNumber;
            _customerDb.CustomerOtps.Update(findCustomer);
            await _customerDb.SaveChangesAsync();
        }

        _emailClient.SendEmail(to, randomNumber, "Verify your Identity");

        return new OtpReturn
        {
            MobileNumber = emailAddress,
            Otp = randomNumber
        };
    }

    public bool VerifyOtp(string mobileNumber, string otp)
    {

        var found = _customerDb.CustomerOtps.FirstOrDefault(item =>
            item.MobileNumber.Equals(mobileNumber));

        if (found != null)
        {
            return found.OTP.Equals(otp);
        }

        return false;
    }

    public void DeleteOtp(string mobileNumber)
    {
        var found = _customerDb.CustomerOtps.FirstOrDefault(item =>
            item.MobileNumber.Equals(mobileNumber));

        _customerDb.CustomerOtps.Remove(found);
    }

    public class OtpReturn
    {
        public string MobileNumber { get; set; }
        public string Otp { get; set; }
    }
}