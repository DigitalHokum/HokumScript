using System.Threading.Tasks;

namespace HokumScript.Script.Nodes
{
    public class StringNode : AstTreeNode
    {
        public readonly AstTreeNode Node;

        public StringNode(AstTreeNode node)
        {
            Node = node;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope)
        {
            dynamic value = await Node.Evaluate(scope);
            string str = $"{value}";
            return new DynamicReturnValue(str);
        }
    }
}
