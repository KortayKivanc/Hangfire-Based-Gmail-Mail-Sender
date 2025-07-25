using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace HangfireAutomata.Services
{
	internal class MailConstructor
	{

		const string logFolder = "..\\..\\LogFiles\\";
		public static void sendMail(string messageSubject, string messageContext, string mailtoWHO,
			string mailfromWHOM, string mailPass, bool shouldLog = true)
		{
			MailMessage message = new MailMessage();
			message.From = new MailAddress(mailfromWHOM);
			message.To.Add(mailtoWHO);

			message.Subject = messageSubject;

			message.Body = messageContext;
			SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
			client.Credentials = new System.Net.NetworkCredential(mailfromWHOM, mailPass);
			client.EnableSsl = true;
			try
			{
				client.Send(message);
				if (shouldLog == true)
				{


					string logText = $@"Mesajınız başarıyla gönderilmiştir.

Konu: {messageSubject}

Tarih: {DateTime.Now}
";

					Directory.CreateDirectory(logFolder);

					string fileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt";
					File.WriteAllText(Path.Combine(logFolder, fileName), logText);
				}
			}
			catch (Exception e) { ErrorHandling.errorHandling(e); }

		}
	}
}
