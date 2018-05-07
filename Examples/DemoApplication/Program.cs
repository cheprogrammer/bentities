using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEntities;
using BEntities.Components;
using DemoProject.Components;
using Microsoft.Xna.Framework;

namespace DemoApplication
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
            Transform2DComponent component = firstEntity.GetComponentOrDefault<Transform2DComponent>();
            component.Position += new Vector2(10, 10);

            // attach a new component and change its values
            TestComponent newComponent = firstEntity.AttachComponent<TestComponent>();
            newComponent.Variable1 += 10;

            GameTime testGameTime = new GameTime(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));

            // here is the call of Update()
            // All routine, connected to creation of entities, attaching of components and etc. is performed within Update()
            manager.Update(testGameTime);

            // it is better to call Draw, isn't it?
            manager.Draw(testGameTime);

            // here is the one more call of Update()
            // Now all components are registered and can be processed in Systems
            manager.Update(testGameTime);

            // Now it is time to shutdown ECS manager - lets perform unloading of content
            manager.UnloadContent();
        }
    }
}
