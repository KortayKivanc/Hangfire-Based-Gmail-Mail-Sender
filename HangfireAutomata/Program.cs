using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.SqlServer;
using HangfireAutomata.Module;
using System.Globalization;
using System.Net.Mail;

using Owin;

//Bu projede bir console uygulaması geliştireceksin.
//Uygulamanın amacı, arka planda görev çalıştırmak için Hangfire’ı kullanmak olacak.
//Bu görevler belirli bir düzenle, zamanlı olarak veya kullanıcı isteğiyle çalıştırılabilecek.
//Uygulama çalıştığında, belirli işleri sıraya alacak ve Hangfire bu işleri arka planda gerçekleştirecek.
//Uygulama bir console ekranı üzerinden yönetilecek. Console ekranında basit bir menü olacak.
//=== Görev Takip Sistemi ===================================================================================
//1. Tek Seferlik Görev Ekle
//2. Zamanlı (Recurring) Görev Ekle
//3. Gecikmeli Görev Ekle
//4. Hatalı Görev Ekle
//5. Log Görevi Ekle
//6. Çıkış	

namespace HangfireAutomata
{
    internal class Program
    {
		public static List<Options> options_mainScreen;

		static void Main(string[] args)
        {

			bool exitStatus = false;


			//SQL Server kullanmak için aşağıdaki yorumu kullanın.
			GlobalConfiguration.Configuration.UseSqlServerStorage("Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=true");
			//Local SQL Server kullanmak için aşağıdaki yorumu kullanın
			//GlobalConfiguration.Configuration.UseMemoryStorage();

			using (var server = new BackgroundJobServer())
			{
				Console.WriteLine("Hangfire Server started. Press any key to exit...");
				Thread.Sleep(500);
				options_mainScreen = new List<Options>
			{
				new Options("Tek Seferlik Görev Ekle", () => sendMailTo(TimeSpan.Zero)),
				new Options("Zamanlı (Recurring) Görev Ekle", () => addReccuringJob()),
				new Options("Zamanlı (Recurring) Görev Kaldır", () => removeReccuringJob()),
				new Options("Gecikmeli Görev Ekle", () => jobSchedule()),
				new Options("Çıkış", () => exitStatus = true),
			};
				int index = 0;

				WriteMenu(options_mainScreen, options_mainScreen[index]);
				ConsoleKeyInfo pressedKey;
				do
				{
					pressedKey = Console.ReadKey();

					if (pressedKey.Key == ConsoleKey.DownArrow)
					{
						if (index + 1 < options_mainScreen.Count)
						{
							index++;
							WriteMenu(options_mainScreen, options_mainScreen[index]);
							Console.WriteLine(index);
						}
					}
					if (pressedKey.Key == ConsoleKey.UpArrow)
					{
						if (index - 1 >= 0)
						{
							index--;
							WriteMenu(options_mainScreen, options_mainScreen[index]);
							Console.WriteLine(index);
						}
					}
					if (pressedKey.Key == ConsoleKey.Enter)
					{
						options_mainScreen[index].Selected.Invoke();
						index = 0;
					}
					if (pressedKey.Key == ConsoleKey.B)
					{
						WriteMenu(options_mainScreen, options_mainScreen[0]);
						index = 0;	
					}


				} while (exitStatus == false);

				Console.Clear();
				Console.WriteLine("Press any key to close the app..   ");
				Console.ReadKey();
			}		
		}

		static void sendMailTo(TimeSpan ts)
		{
			Console.Clear();

			Console.WriteLine("Görev tamamlandığında log dosyası oluşturulsun istiyor musunuz? ");
			Console.WriteLine(">Evet  Hayır");
			ConsoleKeyInfo pressedKey;
			bool shouldLog = true;
			while (true)
			{
				pressedKey = Console.ReadKey();
				if (pressedKey.Key == ConsoleKey.RightArrow)
				{
					shouldLog = false;
					Console.Clear();
					Console.WriteLine("Görev tamamlandığında log dosyası oluşturulsun istiyor musunuz?");
					Console.WriteLine(" Evet >Hayır");
				}
				if (pressedKey.Key == ConsoleKey.LeftArrow)
				{
					shouldLog = true;
					Console.Clear();
					Console.WriteLine("Görev tamamlandığında log dosyası oluşturulsun istiyor musunuz? ");
					Console.WriteLine(">Evet  Hayır");
					
				}
				if (pressedKey.Key == ConsoleKey.Enter)
				{
					Console.Clear();
					
					
					break;
				}
			}
			Console.WriteLine("Hangi mail adresinden göndericeğinizi giriniz:  ");
			string mailfromWHOM = Console.ReadLine();

			Console.WriteLine("Mail adresinizin **uygulama şifresini** giriniz (mailin kendi şifresi değil!):  ");
			string mailPass = Console.ReadLine();

			Console.WriteLine("Hangi mail adresine göndericeğinizi giriniz:  ");
			string mailtoWHO = Console.ReadLine();




			Console.WriteLine("Göndermek İstediğiniz Mesajın Konusunu Giriniz:  ");
			string messageSubject = Console.ReadLine();
			Console.WriteLine("Göndermek İstediğiniz Mesajı Giriniz:  ");
			string messageContext = Console.ReadLine();


			Services.MailSendService.ScheduleAMail(messageSubject, messageContext, mailtoWHO,
			mailfromWHOM, mailPass, shouldLog, ts);

			Console.WriteLine("Geri dönmek için B tuşuna basınız..");
			while (Console.ReadKey(true).Key != ConsoleKey.B) { }

		}
		static void jobSchedule()
		{
			Console.Clear();
			while (true)
			{	
				Console.WriteLine("Mesajın Tarihini seçin (DD/MM/YYYY/HH:mm): ");
				string taskToDate = Console.ReadLine();

				try
				{
					DateTime dt = DateTime.ParseExact(taskToDate, "dd/MM/yyyy/HH:mm", CultureInfo.InvariantCulture);
					TimeSpan remTime = dt - DateTime.Now;
					if (remTime > TimeSpan.Zero)
					{
						sendMailTo(remTime);
						break;
					}
					else { Console.WriteLine("lütfen geçerli bir zaman giriniz!"); }
				}
				catch (FormatException)
				{
					Console.WriteLine("lütfen geçerli bir zaman giriniz!");
				}
			} 
		}
		
		static void removeReccuringJob()
		{
			Console.Clear();
			RecurringJob.RemoveIfExists("sendDailyMail");
			WriteMenu(options_mainScreen, options_mainScreen[0]);
		}
		
		static void addReccuringJob()
		{
			Console.Clear();
			Console.WriteLine("Hangi mail adresinden göndericeğinizi giriniz:  ");
			string mailfromWHOM = Console.ReadLine();

			Console.WriteLine("Mail adresinizin şifresini veya uygulama şifresini giriniz:  ");
			string mailPass = Console.ReadLine();

			Console.WriteLine("Hangi mail adresine göndericeğinizi giriniz:  ");
			string mailtoWHO = Console.ReadLine();
			Console.WriteLine("Göndermek İstediğiniz Mesajın Konusunu Giriniz:  ");
			string messageSubject = Console.ReadLine();
			Console.WriteLine("Göndermek İstediğiniz Mesajı Giriniz:  ");
			string messageContext = Console.ReadLine();

			while (true)
			{
				try
				{
					Console.Clear();

					Console.WriteLine("Görev tamamlandığında log dosyası oluşturulsun istiyor musunuz? ");
					Console.WriteLine(">Evet  Hayır");
					ConsoleKeyInfo pressedKey;
					bool shouldLog = true;
					while (true)
					{
						pressedKey = Console.ReadKey();
						if (pressedKey.Key == ConsoleKey.RightArrow)
						{
							shouldLog = false;
							Console.Clear();
							Console.WriteLine("Görev tamamlandığında log dosyası oluşturulsun istiyor musunuz?");
							Console.WriteLine(" Evet >Hayır");
						}
						if (pressedKey.Key == ConsoleKey.LeftArrow)
						{
							shouldLog = true;
							Console.Clear();
							Console.WriteLine("Görev tamamlandığında log dosyası oluşturulsun istiyor musunuz? ");
							Console.WriteLine(">Evet  Hayır");

						}
						if (pressedKey.Key == ConsoleKey.Enter)
						{
							Console.Clear();


							break;
						}
					}
					Console.WriteLine("Mesajın Tarihini seçin (HH:mm): ");
					string taskToDate = Console.ReadLine();
					DateTime dt = DateTime.ParseExact(taskToDate, "HH:mm", CultureInfo.InvariantCulture);
					int hour = dt.Hour;
					int minute = dt.Minute;
					string cron = $"{minute} {hour} * * *";
					Services.AddReccuringJob.reccuringJob(messageSubject, messageContext,  mailtoWHO,
			 mailfromWHOM,  mailPass, shouldLog,  cron);	
					Console.WriteLine($"Her gün {hour:D2}:{minute:D2}'da çalışacak.");
					Console.WriteLine("Geri dönmek için B tuşuna basınız..");
					break;
				} catch (FormatException) { Console.WriteLine("a"); }
			}
		}
		static void WriteMenu(List<Options> options, Options selectedOption)
		{
			Console.Clear();

			foreach (Options option in options)
			{
				if (option == selectedOption)
				{
					Console.Write("> ");
				}
				else
				{
					Console.Write(" ");
				}

				Console.WriteLine(option.Name);
			}
		}
	}
}
