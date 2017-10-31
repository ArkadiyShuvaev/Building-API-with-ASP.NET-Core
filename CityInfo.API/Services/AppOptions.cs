using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{

	public class EmailOptions
	{
		public string MailToAddress { get; set; }
		public string MailFromAddress { get; set; }
		public int AttemptCount { get; set; }
	}
    public class AppOptions
    {
	    public EmailOptions MailSettings { get; set; }

    }
}
