using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEntities.Components;
using Microsoft.Xna.Framework;

namespace BEntities
{
    public class BaseComponent
    {
        /// <summary>
        /// Gets the source entity
        /// </summary>
        internal Entity Entity { get; set; }

        public Transform2DComponent Transform => Entity.Transform;

        public bool MarkedToBeRemoved { get; internal set; } = false;

        internal BaseComponent()
        {
            
        }

        public virtual void Reset()
        {
            
        }

        public T GetComponent<T>() where T: BaseComponent
        {
            return Entity.GetComponent<T>();
        }
    }
}
