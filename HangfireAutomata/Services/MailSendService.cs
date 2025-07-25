using Hangfire;

namespace HangfireAutomata.Services
{
	internal class MailSendService
	{
		public static void ScheduleAMail(string messageSubject, string messageContext, string mailtoWHO,
			string mailfromWHOM, string mailPass,bool shouldLog, TimeSpan ts)
		{

			BackgroundJob.Schedule(() =>
				MailConstructor.sendMail(messageSubject, messageContext, mailtoWHO, mailfromWHOM, mailPass, shouldLog), ts);

		}
	}
}
