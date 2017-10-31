using System;
using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace CityInfo.API.Services
{
    public class LocalMailService : IMailService
    {
        private readonly string _mailTo;
        private readonly string _mailFrom;

	    public LocalMailService(IOptions<AppOptions> options)
	    {
		    _mailTo = options.Value.MailSettings.MailToAddress;
		    _mailFrom = options.Value.MailSettings.MailFromAddress;
	    }

        public void Send(string subject, string message)
        {
            Debug.WriteLine($"Mail from {_mailFrom} to {_mailTo}, {GetType().FullName}");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine($"Message: {message}");
			Debug.WriteLine($"GetHashCode: {GetHashCode()}");
        }

    }
}
