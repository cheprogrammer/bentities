using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities
{
    /// <summary>
    /// Represents class for creation of Templates
    /// </summary>
    public abstract class BaseTemplate
    {
        public abstract void BuildEntity(Entity entity, params object[] args);
    }
}
