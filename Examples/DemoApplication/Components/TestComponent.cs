using BEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Components
{
	public class TestComponent : BaseComponent
	{
		public int Variable1 { get; set; }

	    public string Variable2 { get; set; } = "Hello";
	}
}
