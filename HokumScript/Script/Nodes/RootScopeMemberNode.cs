using System.Threading.Tasks;

namespace HokumScript.Script.Nodes
{
    public class RootScopeMemberNode : AstTreeNode, IScopeMemberNode {
        public readonly AstTreeNode Name;

        public RootScopeMemberNode(
            AstTreeNode name
        )
        {
            Name = name;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope)
        {
            string key = await GetName(scope);
            return new DynamicReturnValue(scope.Get(key));
        }

        public async Task<string> GetName(Scope scope)
        {
            return (await Name.Evaluate(scope)).GetValue<string>();
        }

        public async Task<Scope> GetScope(Scope scope)
        {
            return await Task.Run(() => scope);
        }
    }
}
