using System.Threading.Tasks;

namespace HokumScript.Script.Nodes
{
    public class LiteralNode<T> : AstTreeNode
    {
        public readonly T Value;

        public LiteralNode(
            T value
        )
        {
            Value = value;
        }

        public virtual async Task<DynamicReturnValue> Evaluate(Scope scope)
        {
            T value = await Task.Run(() => Value);
            return new DynamicReturnValue(value);
        }
    }
}
