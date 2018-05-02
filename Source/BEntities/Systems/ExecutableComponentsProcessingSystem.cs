using Microsoft.Xna.Framework;

namespace BEntities.Systems
{
    [ComponentSystem(SystemProcessingType.Update, 100, typeof(ExecutableComponent))]
    public class ExecutableComponentsProcessingSystem : BaseComponentProcessingSystem
    {
        protected override void ComponentRegistered(Entity entity, BaseComponent component)
        {
            ExecutableComponent execComponent = (ExecutableComponent)component;
            execComponent.Initialize();
        }

        protected override void ComponentRemoved(Entity entity, BaseComponent component)
        {
            ExecutableComponent execComponent = (ExecutableComponent)component;
            execComponent.Unload();
        }

        protected override void ProcessComponent(GameTime gameTime, BaseComponent component)
        {
            ExecutableComponent execComponent = (ExecutableComponent) component;
            execComponent.Update(gameTime);
        }
    }
}
