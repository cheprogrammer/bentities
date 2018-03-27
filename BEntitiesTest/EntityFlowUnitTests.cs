using System;
using BEntities;
using BEntities.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BEntitiesTest
{
    [TestClass]
    public class EntityFlowUnitTests
    {
        public EntityFlowUnitTests()
        {
			ECSManagerFactory.ScanAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        }

        [TestMethod]
        public void EntityCreationAndRetrievingBasicComponent()
        {
            ECSManager manager = ECSManagerFactory.CreateECSManager();

            Entity newEntity = manager.CreateEntity();

            Assert.IsNotNull(newEntity.GetComponent<Transform2DComponent>());
            Assert.IsNotNull(newEntity.Transform);
        }


		[TestMethod]
		public void EntityCreationOfMultipleEntities()
		{
			ECSManager manager = ECSManagerFactory.CreateECSManager();

			List<Entity> entities = new List<Entity>(128);

			for(int i = 0; i < 128; i++)
			{
				Entity newEntity = manager.CreateEntity();
				entities.Add(newEntity);				
			}

			manager.Update(new GameTime());

			for (int i = 0; i < 128; i++)
			{
				Entity newEntity = manager.CreateEntity();
				entities.Add(newEntity);

				Entity entityForDestroy = entities[0];
				entities.RemoveAt(0);
				entityForDestroy.Destroy();

				manager.Update(new GameTime());
			}
		}
	}
}
