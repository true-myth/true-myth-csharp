using System;
using System.Collections.Generic;

namespace TrueMyth
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            DemoStatic();
            DemoSome();
            DemoNothing();
            DemoMatching();
        }

        private static int Length(string s) => s.Length;
        static int Double(int n) => n * 2;

        private static void DemoStatic()
        {
            var someNumber = Maybe.Of(42);
            
            var doubled = Maybe.Map(Double, someNumber);
            Console.WriteLine(Maybe.UnsafelyUnwrap(doubled));
        }

        private static void DemoMatching()
        {
            var something = Maybe.Of("a string");
            var nothing = Maybe.Nothing<string>();

            PrintValue(something);
            PrintValue(nothing);
        }

        private static void PrintValue<TValue>(IMaybe<TValue> something)
        {
            switch (something)
            {
                case Just<TValue> s:
                    Console.WriteLine("Some({0})", s.UnsafelyUnwrap());
                    break;
                case Nothing<TValue> _:
                    Console.WriteLine("`Nothing` to see here");
                    break;
                default:
                    throw new Exception("This should be impossible.");
            }
        }

        public static void DemoSome()
        {
            var someString = Maybe.Of("waffles");

            var mappedToLength = someString.Map(Length);
            Console.WriteLine(mappedToLength.UnsafelyUnwrap());
            Console.WriteLine(mappedToLength.UnwrapOr(int.MinValue));
        }

        public static void DemoNothing()
        {
            var nothing = Maybe.Of<string>(null);
            var mappedNothing = nothing.Map(Length);
            
            // GROSS, DO NOT WANT
            try
            {
                mappedNothing.UnsafelyUnwrap();
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            var anotherNothing = new Nothing<string>();
            try
            {
                Console.WriteLine(anotherNothing.UnsafelyUnwrap());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
            // Much nicer
            Console.WriteLine(anotherNothing.UnwrapOr(String.Empty));
        }
    }
}