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
            EntityComponentSystemManager.ScanAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        }

        [TestMethod]
        public void EntityCreationAndRetrievingBasicComponent()
        {
            EntityComponentSystemManager manager = new EntityComponentSystemManager();
            manager.Initialize();

            Entity newEntity = manager.CreateEntity();

            Assert.IsNotNull(newEntity.GetComponent<Transform2DComponent>());
            Assert.IsNotNull(newEntity.Transform);
        }
    }
}
