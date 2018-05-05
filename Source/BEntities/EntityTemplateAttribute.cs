using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
