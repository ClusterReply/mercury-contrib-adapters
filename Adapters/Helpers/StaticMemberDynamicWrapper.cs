using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.Helpers
{
    public class StaticMembersDynamicWrapper : DynamicObject
    {
        private Type type;
        public StaticMembersDynamicWrapper(Type type) { this.type = type; }

        // Handle static properties
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            PropertyInfo prop = type.GetProperty(binder.Name, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
            if (prop == null)
            {
                result = null;
                return false;
            }

            result = prop.GetValue(null, null);
            return true;
        }

        // Handle static methods
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            MethodInfo method = type.GetMethod(binder.Name, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
            if (method == null)
            {
                result = null;
                return false;
            }

            result = method.Invoke(null, args);
            return true;
        }
    }
}
