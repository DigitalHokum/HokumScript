using System;
using System.Threading.Tasks;

namespace HokumScript.Script.Nodes
{
    public class ScopeMemberNode : AstTreeNode, IScopeMemberNode {
        public readonly AstTreeNode Scope;
        public readonly AstTreeNode Name;

        public ScopeMemberNode(
            AstTreeNode scope,
            AstTreeNode name
        )
        {
            Scope = scope;
            Name = name;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope) {
            
            DynamicReturnValue parent = await Scope.Evaluate(scope);

            if (parent.IsNull())
            {
                Console.WriteLine("ERROR ERROR BEEP BOOP");
                return new DynamicReturnValue(null);
            }
            string name = await GetName(scope);
            Type t = parent.GetValue<Scope>().GetType(name);
            return new DynamicReturnValue(parent.GetValue<Scope>().Get(name));
        }

        public async Task<string> GetName(Scope scope)
        {
            return (await Name.Evaluate(scope)).GetValue<string>();
        }

        public async Task<Scope> GetScope(Scope scope)
        {
            return (await Scope.Evaluate(scope)).GetValue<Scope>();
        }
    }
}
