using Hangfire;

namespace HangfireAutomata.Services
{
	internal class AddReccuringJob
	{
		public static void reccuringJob(string messageSubject, string messageContext, string mailtoWHO,
			string mailfromWHOM, string mailPass, bool shouldLog, string cron)
		{
			RecurringJob.AddOrUpdate("sendDailyMail",
						() => Services.MailConstructor.sendMail(messageSubject, messageContext, mailtoWHO, mailfromWHOM, mailPass, shouldLog),
						cron);
		}

	}
}
