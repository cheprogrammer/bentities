using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BEntities
{
    /// <summary>
    /// Represents class for Templates
    /// Templates should be used for building typical entities.
    /// Every entity template should have attribute EntityTemplateAttribute with unique name of attribute
    /// </summary>
    public abstract class EntityTemplate
    {
        public string Name { get; internal set; }

        internal void InitializeInternal()
        {
            EntityTemplateAttribute entityTemplateAttribute = GetType().GetCustomAttribute<EntityTemplateAttribute>();

            if (entityTemplateAttribute == null)
                throw new Exception($"Entity template of type {GetType().Name} does not have attribute {nameof(EntityTemplateAttribute)}");

            Name = entityTemplateAttribute.Name;
        }

        /// <summary>
        /// Performs bulding of passed entity
        /// </summary>
        /// <param name="entity">Source entity which should be built</param>
        /// <param name="args">List of arguments which might be used during the building procedure</param>
        public abstract void BuildEntity(Entity entity, params object[] args);
    }
}
