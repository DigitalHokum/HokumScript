using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace HokumScript
{
    public class FunctionArguments : Scope
    {
        
    }

    public class Function
    {
        public readonly Type ObjectType;
        public readonly object Instance;
        public readonly string MethodName;

        public Function(object instance, string methodName)
        {
            ObjectType = instance.GetType();
            Instance = instance;
            MethodName = methodName;
        }
        
        public Function(Type type, string methodName)
        {
            ObjectType = type;
            Instance = Activator.CreateInstance(type);
            MethodName = methodName;
        }
        
        public bool IsAsyncMethod()
        {
            MethodInfo method = ObjectType.GetMethod(MethodName);
            Type attType = typeof(AsyncStateMachineAttribute);
            // Null is returned if the attribute isn't present for the method. 
            var attrib = (AsyncStateMachineAttribute)method.GetCustomAttribute(attType);

            return (attrib != null);
        }

        public dynamic Invoke(object[] arguments)
        {
            MethodInfo m = ObjectType.GetMethod(MethodName);
            return m.Invoke(Instance, arguments);
        }
        
        public async Task<dynamic> AsyncInvoke(object[] arguments)
        {
            MethodInfo m = ObjectType.GetMethod(MethodName);
            Task task = (Task) m.Invoke(Instance, arguments);
            
            await task.ConfigureAwait(false);

            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty.GetValue(task);
        }
    }
}
