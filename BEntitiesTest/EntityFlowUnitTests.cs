using System;
using BEntities;
using BEntities.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
