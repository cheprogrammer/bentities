using System;

namespace BEntities
{
    /// <summary>
    /// Attribute for every entity template. Defines the name of this template
    /// </summary>
    public class EntityTemplateAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of template
        /// </summary>
        public string Name { get; set; }
    }
}
