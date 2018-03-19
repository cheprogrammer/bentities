using System;

namespace BEntities
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ComponentSystemAttribute : Attribute
    {
        public  SystemProcessingType SystemProcessingType { get; }

        public int Order { get; } = 0;

        public Type[] Components { get; }

        public ComponentSystemAttribute(SystemProcessingType type, int order, params Type[] components)
        {
            SystemProcessingType = type;
            Order = order;
            Components = components;
        }
    }
}
