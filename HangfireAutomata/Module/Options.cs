using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
