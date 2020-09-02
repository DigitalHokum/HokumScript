using System.Collections.Generic;
using System.Threading.Tasks;

namespace HokumScript.Script.Nodes
{
    public class BlockNode : AstTreeNode
    {
        public readonly List<AstTreeNode> Statements;

        public BlockNode(List<AstTreeNode> statements)
        {
            Statements = statements;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope) {
            DynamicReturnValue returnValue = new DynamicReturnValue(null);
            
            for (int i = 0; i < Statements.Count; i++) {
                returnValue = await Statements[i].Evaluate(scope);
            }

            return returnValue;
        }
    }
}
