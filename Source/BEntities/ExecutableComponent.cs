using Microsoft.Xna.Framework;

namespace BEntities
{
    /// <summary>
    /// Generic executeable component for performing typical un-ordered operations during Update() stage of Game loop
    /// </summary>
    public abstract class ExecutableComponent : BaseComponent
    {
        /// <summary>
        /// Performs initialization during registering stage of ECS
        /// </summary>
        public virtual void Initialize()
        {

        }

        /// <summary>
        /// Performs update routine
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {

        }

        /// <summary>
        /// Performs unload of internal resources on destorying entity or detaching component
        /// </summary>
        public virtual void Unload()
        {

        }
    }
}
