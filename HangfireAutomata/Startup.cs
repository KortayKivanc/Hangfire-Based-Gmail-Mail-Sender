using Hangfire;

using Hangfire.Owin;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(HangfireAutomata.Startup))]
namespace HangfireAutomata
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			GlobalConfiguration.Configuration
				.UseSqlServerStorage("your_connection_string_here");

			app.UseHangfireDashboard();
			app.UseHangfireServer();
		}
	}
}
