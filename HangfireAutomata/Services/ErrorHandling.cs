

namespace HangfireAutomata.Services
{
	internal class ErrorHandling
	{

		const string logFolder = "..\\..\\LogFiles\\";
		public static void errorHandling(Exception e)
		{
			Console.WriteLine(e);
			string logText = e.ToString();
			Directory.CreateDirectory(logFolder);

			string fileName = DateTime.Now.ToString("crash-yyyy-MM-dd_HH-mm-ss") + ".txt";
			File.WriteAllText(Path.Combine(logFolder, fileName), logText);
		}
	}
}
