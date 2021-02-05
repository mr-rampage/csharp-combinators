using System;
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
                .ForAll<object>(input => input == input.Thrush(Combinators.Identity))
                .QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void TestConstant()
        {
            static object SideEffect(dynamic input) => input;
            
            Prop
                .ForAll<object>(input => Combinators.Constant(true)(SideEffect(input)))
                .QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void TestApply()
        {
            Prop
                .ForAll<NonNull<object>>(input => 
                    input.GetType() == input.Thrush(Combinators.Apply((object x) => x.GetType()))
                ).QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void TestThrush()
        {
            Prop
                .ForAll<object>(input => 
                    input == Combinators.Thrush<object, object>(input)(Combinators.Identity)
                ).QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void TestDuplication()
        {
            Prop
                .ForAll<NonNull<object>>(input =>
                    input + input.ToString() ==
                    input.Thrush(Combinators.Duplication<object, string>(a => b => a.ToString() + b))
                ).QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void TestFlip()
        {
            static string SideEffect(dynamic input) => input.ToString();
            var tee = Combinators.Flip<object, string, object>(Combinators.Constant);
            Prop
                .ForAll<NonNull<object>>(input => 
                    input.Equals(input.Thrush(tee(SideEffect(input)))))
                .QuickCheckThrowOnFailure();
        }
    }
}