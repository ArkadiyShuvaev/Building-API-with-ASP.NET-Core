using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace CityInfo.API.Services
{
    public class CloudMailService : IMailService
    {
        private readonly ILogger<CloudMailService> _logger;
        private string _mailTo = "admin@company.com";
        private string _mailFrom = "noreply@company.com";


        public CloudMailService(ILogger<CloudMailService> logger)
        {
            _logger = logger;
        }
        public void Send(string subject, string message)
        {
            _logger.LogInformation($"Mail from {_mailFrom} to {_mailTo}, {GetType().FullName}");
            _logger.LogInformation($"Subject: {subject}");
            _logger.LogInformation($"Message: {message}");
        }
    }
}