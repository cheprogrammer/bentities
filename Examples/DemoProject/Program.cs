using BEntities;
using BEntities.Components;
using DemoProject.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject
{
	class Program
	{
		static void Main(string[] args)
		{
			// fisrt of all, perform scanning of assemblies for factory
			ECSManagerFactory.ScanAssemblies(AppDomain.CurrentDomain.GetAssemblies());

			// after that, we can create ECS Manager
			ECSManager manager = ECSManagerFactory.CreateECSManager();

			// let's create first entity!
			Entity firstEntity = manager.CreateEntity();

			// now let's get transform component and move entity a little bit
			Transform2DComponent component = firstEntity.GetComponent<Transform2DComponent>();
			component.Position += new Vector2(10, 10);

			// attach a new component and change its values
			TestComponent newComponent = firstEntity.AttachComponent<TestComponent>();
			newComponent.Variable1 += 10;
		}
	}
}
