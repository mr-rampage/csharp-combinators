using FsCheck;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Combinators.Test
{
    [TestClass]
    public class CombinatorsTest
    {
        [TestMethod]
        public void TestIdentity()
        {
            Prop
                .ForAll<object>(input => input == input.Thrush(Combinator.Identity))
                .QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void TestConstant()
        {
            static object SideEffect(dynamic input) => input;
            
            Prop
                .ForAll<object>(input => Combinator.Constant(true)(SideEffect(input)))
                .QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void TestApply()
        {
            Prop
                .ForAll<NonNull<object>>(input => 
                    input.GetType() == input.Thrush(Combinator.Apply((object x) => x.GetType()))
                ).QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void TestThrush()
        {
            Prop
                .ForAll<object>(input => 
                    input == Combinator.Thrush<object, object>(input)(Combinator.Identity)
                ).QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void TestDuplication()
        {
            Prop
                .ForAll<NonNull<object>>(input =>
                    input + input.ToString() ==
                    input.Thrush(Combinator.Duplication<object, string>(a => b => a.ToString() + b))
                ).QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void TestFlip()
        {
            var tee = Combinator.Flip<object, string, object>(Combinator.Constant);
            Prop
                .ForAll<NonNull<object>>(input =>
                {
                    var sideEffect = Combinator.Thrush<object, string>(input);
                    return input.Equals(input.Thrush(tee(sideEffect(x => x.ToString()))));
                }).QuickCheckThrowOnFailure();
        }
    }
}