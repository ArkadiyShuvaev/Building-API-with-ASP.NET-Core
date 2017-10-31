using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace CityInfo.API.Services
{
    public class CloudMailService : IMailService
    {
        private readonly ILogger<CloudMailService> _logger;
		private readonly string _mailTo = Startup.Configuration["mailSettings:mailToAddress"];
	    private readonly string _mailFrom = Startup.Configuration["mailSettings:mailFromAddress"];


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