using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HokumScript.Script;
using Xunit;
using Xunit.Abstractions;

namespace HokumScript.Test
{
    public class FunctionTest
    {
        public string MyTestFunction(string whatToReturn)
        {
            Console.WriteLine(whatToReturn);
            return whatToReturn;
        }
        
        public async Task<string> MyTestAsyncFunction(string whatToReturn)
        {
            Console.WriteLine(whatToReturn);
            string value = await Task.Run(() => whatToReturn);
            return value;
        }
        
        public string MyTestFunctionWithMultipleArguments(string whatToReturn, string cheese)
        {
            return $"{whatToReturn} {cheese}";
        }
    }
    
    public class ScriptingTest
    {
        private readonly ITestOutputHelper output;

        public ScriptingTest(ITestOutputHelper output)
        {
            this.output = output;
        }
        
        [Fact]
        public void TestTokenize()
        {
            ScriptParser parser = new ScriptParser();
            List<ScriptToken> tokens = parser.Tokenize("1 + 5;something = false;callFunc();indice[0];block {};");
            Assert.Equal(26, tokens.Count);
        }
        
        [Fact]
        public async Task TestNumberLiteral()
        {
            ScriptTree scriptTree = new ScriptTree("5.52");
            DynamicReturnValue value = await scriptTree.Evaluate(new Scope());
            Assert.Equal(5.52f, value.GetValue<float>());
            
            scriptTree = new ScriptTree("5");
            value = await scriptTree.Evaluate(new Scope());
            Assert.Equal(5, value.GetValue<int>());
        }

        [Fact]
        public async Task TestBooleanLiteral()
        {
            ScriptTree scriptTree = new ScriptTree("true");
            DynamicReturnValue value = await scriptTree.Evaluate(new Scope());
            Assert.Equal(true, value.GetValue<bool>());
            
            scriptTree = new ScriptTree("false");
            value = await scriptTree.Evaluate(new Scope());
            Assert.Equal(false, value.GetValue<bool>());
        }
        
        [Fact]
        public async Task TestStringLiteral()
        {
            ScriptTree scriptTree = new ScriptTree("'testing'");
            DynamicReturnValue value = await scriptTree.Evaluate(new Scope());
            Assert.Equal("testing", value.GetValue<string>());
            
            scriptTree = new ScriptTree("\"testing double quotes\"");
            value = await scriptTree.Evaluate(new Scope());
            Assert.Equal("testing double quotes", value.GetValue<string>());
        }
        
        [Fact]
        public async Task TestRootScopeNode()
        {
            ScriptTree scriptTree = new ScriptTree("foo");
            Scope scope = new Scope();
            scope.Set("foo", 5);
            DynamicReturnValue value = await scriptTree.Evaluate(scope);
            Assert.Equal(5, value.GetValue<int>());
            
            scriptTree = new ScriptTree("foo");
            scope.Set("foo", false);
            value = await scriptTree.Evaluate(scope);
            Assert.Equal(false, value.GetValue<bool>());
        }
        
        [Fact]
        public async Task TestScopeNode()
        {
            ScriptTree scriptTree = new ScriptTree("foo.bar;");
            Scope scope = new Scope();
            Scope innerScope = new Scope();
            scope.Set("foo", innerScope);
            innerScope.Set("bar", 5);
            DynamicReturnValue value = await scriptTree.Evaluate(scope);
            Console.WriteLine($"result {value.GetValue<int>()}");
            Assert.Equal(5, value.GetValue<int>());
            
            innerScope.Set("bar", false);
            value = await scriptTree.Evaluate(scope);
            Assert.Equal(false, value.GetValue<bool>());
            
            scriptTree = new ScriptTree("foo.baz.nitch");
            Scope lastScope = new Scope();
            lastScope.Set("nitch", 101);
            innerScope.Set("baz", lastScope);
            value = await scriptTree.Evaluate(scope);
            Assert.Equal(101, value.GetValue<int>());
        }
        
        [Fact]
        public async Task TestScopeAssignmentNode()
        {
            ScriptTree scriptTree = new ScriptTree("foo = 150");
            Scope scope = new Scope();
            await scriptTree.Evaluate(scope);
            Assert.Equal(150, scope.Get("foo"));
            
            scriptTree = new ScriptTree("bar = 1.5;bar;");
            DynamicReturnValue value = await scriptTree.Evaluate(scope);
            Assert.Equal(1.5f, value.GetValue<float>());
            
            scope = new Scope();
            scriptTree = new ScriptTree("foo.bar = 7.5;foo.bar;");
            Scope innerScope = new Scope();
            scope.Set("foo", innerScope);
            value = await scriptTree.Evaluate(scope);
            Assert.Equal(7.5f, value.GetValue<float>());
        }

        [Fact]
        public async Task TestArithmeticNode()
        {
            ScriptTree scriptTree = new ScriptTree("foo = 150 + 5.0");
            Scope scope = new Scope();
            await scriptTree.Evaluate(scope);
            Assert.Equal(155.0f, scope.Get("foo"));

            scriptTree = new ScriptTree("foo = 150 + 5");
            scope = new Scope();
            await scriptTree.Evaluate(scope);
            Assert.Equal(155, scope.Get("foo"));
            
            scriptTree = new ScriptTree("foo = 150 - 5.0");
            scope = new Scope();
            await scriptTree.Evaluate(scope);
            Assert.Equal(145f, scope.Get("foo"));

            scriptTree = new ScriptTree("foo = 150 - 5");
            scope = new Scope();
            await scriptTree.Evaluate(scope);
            Assert.Equal(145, scope.Get("foo"));
            
            scriptTree = new ScriptTree("foo = 150 * 5.0");
            scope = new Scope();
            await scriptTree.Evaluate(scope);
            Assert.Equal(750f, scope.Get("foo"));
            
            scriptTree = new ScriptTree("foo = 150 * 5");
            scope = new Scope();
            await scriptTree.Evaluate(scope);
            Assert.Equal(750, scope.Get("foo"));
            
            scriptTree = new ScriptTree("foo = 150 / 5.0");
            scope = new Scope();
            await scriptTree.Evaluate(scope);
            Assert.Equal(30f, scope.Get("foo"));
            
            scriptTree = new ScriptTree("foo = 150 / 5");
            scope = new Scope();
            await scriptTree.Evaluate(scope);
            Assert.Equal(30, scope.Get("foo"));
        }
        
        [Fact]
        public async Task TestIfStatementNode()
        {
            ScriptTree scriptTree = new ScriptTree(@"
            if (false) {
                result = false;
            } else if (1 > 2) {
                result = 'impossibru!';
            } else if (2 > 1) {
                result = true;
            } else {
                result = 'not right';
            }
            ");
            Scope scope = new Scope();
            await scriptTree.Evaluate(scope);
            Console.WriteLine($"Result it Type {scope.GetType("result")}");
            Assert.Equal(true, scope.Get("result"));
        }
        
        [Fact]
        public async Task TestForStatementNode()
        {
            Scope scope = new Scope();
            Scope innerScope = new Scope();
            List<float> loopTest = new List<float>();
            for (float i = 1.0f; i <= 9.0f; i++)
            {
                loopTest.Add(i);
            }
            scope.Set("loopTest", loopTest);
            scope.Set("test", innerScope);
            
            ScriptTree scriptTree = new ScriptTree(@"
                test.foo = 1.0;
                foo = 1.0;
                for (var of loopTest) {
                    test.foo += var;
                    foo = var;
                }
            ");
           
            await scriptTree.Evaluate(scope);
            Assert.Equal(46.0f, innerScope.Get("foo"));
            Assert.Equal(9.0f, scope.Get("foo"));
        }

        [Fact]
        public async Task TestFunctionCallNode()
        {
            Function function = new Function(typeof(FunctionTest), "MyTestFunction");
            ScriptTree scriptTree = new ScriptTree(@"test_function(words)");
            Scope scope = new Scope();
            string words = "pancakes are tasty.";
            scope.Set("words", words);
            scope.Set("test_function", function);
            DynamicReturnValue r = await scriptTree.Evaluate(scope);
            Assert.Equal(words, r.GetValue<string>());
        }
        
        [Fact]
        public async Task TestFunctionMultipleArgumentsCallNode()
        {
            Function function = new Function(typeof(FunctionTest), "MyTestFunctionWithMultipleArguments");
            ScriptTree scriptTree = new ScriptTree(@"test_function(words, 'Cheese is also rather tasty.')");
            Scope scope = new Scope();
            string words = "pancakes are tasty.";
            scope.Set("words", words);
            scope.Set("test_function", function);
            DynamicReturnValue r = await scriptTree.Evaluate(scope);
            Assert.Equal($"{words} Cheese is also rather tasty.", r.GetValue<string>());
        }
        
        [Fact]
        public async Task TestAsyncFunctionCallNode()
        {
            Function function = new Function(typeof(FunctionTest), "MyTestAsyncFunction");
            ScriptTree scriptTree = new ScriptTree(@"test_function(words)");
            Scope scope = new Scope();
            string words = "pancakes are super tasty.";
            scope.Set("words", words);
            scope.Set("test_function", function);
            DynamicReturnValue r = await scriptTree.Evaluate(scope);
            Assert.Equal(words, r.GetValue<string>());
        }
    }
}
