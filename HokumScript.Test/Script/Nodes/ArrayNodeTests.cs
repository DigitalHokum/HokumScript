using System.Collections.Generic;
using System.Threading.Tasks;
using HokumScript.Script;
using Xunit;
using Xunit.Abstractions;

namespace HokumScript.Test.Script.Nodes
{
    public class ArrayNodeTests
    {
        private readonly ITestOutputHelper output;

        public ArrayNodeTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task TestArrayInitialization()
        {
            ScriptTree scriptTree = new ScriptTree("array = [0, 9, 200, 30];");
            Scope scope = new Scope();
            await scriptTree.Evaluate(scope);
            var array = scope.Get<List<int>>("array");
            Assert.Equal(4, array.Count);
            Assert.Equal(0, array[0]);
            Assert.Equal(9, array[1]);
            Assert.Equal(200, array[2]);
            Assert.Equal(30, array[3]);
        }
    }
}
