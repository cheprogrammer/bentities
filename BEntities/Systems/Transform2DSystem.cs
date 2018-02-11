using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.Systems
{
    [ComponentSystem(SystemProcessingType.Update, 0, typeof(Transform2DSystem))]
    public class Transform2DSystem : BaseComponentSystem
    {

    }
}
