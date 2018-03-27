using BEntities;
using DemoProject.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Systems
{
	[ComponentSystem(SystemProcessingType.Update, 1, typeof(TestComponent))]
	public class TestComponentSystem : BaseComponentProcessingSystem
	{
		protected override void ProcessComponent(GameTime gameTime, BaseComponent component)
		{
			// do something with component
			TestComponent testComponent = (TestComponent)component;

			testComponent.Variable1 += 0;
			testComponent.Variable2 += "";
		}
	}
}
