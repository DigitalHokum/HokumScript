using System.Threading.Tasks;

namespace HokumScript
{
    public interface AstTreeNode {
        Task<DynamicReturnValue> Evaluate(Scope scope);
    }

    public interface IScopeMemberNode
    {
        Task<string> GetName(Scope scope);
        Task<Scope> GetScope(Scope scope);
        Task<DynamicReturnValue> Evaluate(Scope scope);
    }
}
