using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
	public class AppOptions
    {
	    public EmailOptions MailSettings { get; set; }
	    public ConnectionDbStrings ConnectionDStrings { get; set; }

	    public class ConnectionDbStrings
	    {
		    public string CityInfoConnectionString { get; set; }
	    }
	    public class EmailOptions
	    {
		    public string MailToAddress { get; set; }
		    public string MailFromAddress { get; set; }
		    public int AttemptCount { get; set; }
	    }
	}

	
}
