using System;

namespace Combinators
{
    public static class CombinatorExtension
    {
        public static T2 Thrush<T1, T2>(this T1 x, Func<T1, T2> f) => f(x);
    }
    
    public static class Combinators
    {
        public static T Identity<T>(T x) => x;
        
        public static Func<dynamic, T> Constant<T>(T x) => _ => x;

        public static Func<T1, T2> Apply<T1, T2>(Func<T1, T2> f) => f;

        public static Func<Func<T1, T2>, T2> Thrush<T1, T2>(T1 x) => f => f(x);

        public static Func<T1, T2> Duplication<T1, T2>(Func<T1, Func<T1, T2>> f) => x => f(x)(x);

        public static Func<T2, Func<T1, T3>> Flip<T1, T2, T3>(Func<T1, Func<T2, T3>> f) => y => x => f(x)(y);

        public static Func<Func<T1, T2>, Func<T1, T3>> Compose<T1, T2, T3>(Func<T2, T3> f) => g => x => f(g(x));
        
        public static Func<Func<T2, T3>, Func<T1, T3>> Pipe<T1, T2, T3>(Func<T1, T2> f) => g => x => g(f(x));

        public static Func<Func<T1, T2>, Func<T1, T3>> Substitution<T1, T2, T3>(Func<T1, Func<T2, T3>> f) => g => x => f(x)(g(x));
        
        public static Func<Func<T2, T1>, Func<T2, T3>> Chain<T1, T2, T3>(Func<T1, Func<T2, T3>> f) => g => x => f(g(x))(x);

        public static Func<Func<T1, T2>, Func<Func<T1, T3>, Func<T1, T4>>> Converge<T1, T2, T3, T4>(Func<T2, Func<T3, T4>> f) => g => h => x => f(g(x))(h(x));

        public static Func<Func<T1, T2>, Func<T1, Func<T1, T3>>> Psi<T1, T2, T3>(Func<T2, Func<T2, T3>> f) => g => x => y => f(g(x))(g(y));
    }
}