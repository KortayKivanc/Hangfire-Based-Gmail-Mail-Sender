
namespace HangfireAutomata.Module
{
	internal class Options
	{
		public string Name { get; set; }
		public Action Selected { get; set; }

		public Options(string name, Action selected)
		{
			Name = name;
			Selected = selected;
		}
	}
}
