using System;
using System.Diagnostics;

namespace CityInfo.API.Services
{
    public class LocalMailService : IMailService
    {
        private string _mailTo = "admin@company.com";
        private string _mailFrom = "noreply@company.com";

        public void Send(string subject, string message)
        {
            Debug.WriteLine($"Mail from {_mailFrom} to {_mailTo}, {GetType().FullName}");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine($"Message: {message}");
        }

    }
}
