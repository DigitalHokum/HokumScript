using System.Collections.Generic;
using System.Threading.Tasks;

namespace HokumScript.Script.Nodes
{
    public class FunctionArgumentNode : AstTreeNode
    {
        protected List<AstTreeNode> Args;

        public FunctionArgumentNode(
            List<AstTreeNode> args
        )
        {
            Args = args;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope) {
            List<object> arguments = new List<object>();
            foreach (AstTreeNode arg in Args) {
                arguments.Add((await arg.Evaluate(scope)).Value);
            }
            return new DynamicReturnValue(arguments.ToArray());
        }
    }
}
