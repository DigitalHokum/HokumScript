using System.Threading.Tasks;

namespace HokumScript.Script.Nodes
{
    public class ConditionalNode : AstTreeNode
    {
        public readonly AstTreeNode Condition;
        public readonly BlockNode Block;

        public ConditionalNode(AstTreeNode condition, BlockNode block)
        {
            Condition = condition;
            Block = block;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope) {
            DynamicReturnValue returnValue = await this.Condition.Evaluate(scope);
            if (!returnValue.IsNull()) {
                DynamicReturnValue two = await this.Block.Evaluate(scope);
                return two;
            }
            return returnValue;
        }
    }
}
