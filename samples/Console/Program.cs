using System;
using TrueMyth;

namespace TrueMythConsole
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            DemoStatic();
            DemoSome();
            DemoNothing();
            DemoMatching();
            DemoOk();
            DemoErr();
        }

        private static int Length(string s) => s.Length;
        private static int Double(int n) => n * 2;

        private static void DemoSome()
        {
            var someString = new Just<string>("waffles");

            var mappedToLength = someString.Select(Length);
            Console.WriteLine(mappedToLength.UnsafelyUnwrap());
            Console.WriteLine(mappedToLength.UnwrapOr(int.MinValue));
        }

        private static void DemoNothing()
        {
            var nothing = new Nothing<string>();
            var mappedNothing = nothing.Select(Length);
            
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
            Console.WriteLine(anotherNothing.UnwrapOr(string.Empty));
        }

        private static void DemoStatic()
        {
            var someNumber = Maybe.Of(42);
            var doubledJust = Maybe.Select(someNumber, Double);
            Console.WriteLine(Maybe.UnsafelyUnwrap(doubledJust));

            var aNothing = Maybe.Of<string>(null);
            var nothingLength = Maybe.Select(aNothing, Length);
            Console.WriteLine(Maybe.UnsafelyUnwrap(nothingLength));
        }

        private static void DemoMatching()
        {
            var something = Maybe.Of("a string");
            var nothing = Maybe.Nothing<string>();

            PrintMaybeValue(something);
            PrintMaybeValue(nothing);
        }

        private static void PrintMaybeValue<TValue>(IMaybe<TValue> maybe)
        {
            switch (maybe)
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

        private static void DemoOk()
        {
            var anOk = Result.Ok<string, int>("heyo!");
            var lengthOrDoubled = anOk.Select(Length).Select(Double);
            Console.WriteLine(lengthOrDoubled.ToString());
        }

        private static void DemoErr()
        {
            var anErr = Result.Err<string, int>(42);
            var lengthOrDoubled = anErr.Select(Length).SelectErr(Double);
            Console.WriteLine(lengthOrDoubled.ToString());
        }
        
        private static void DemoResultMatch() {}

        private static void PrintResultValue<TValue, TError>(IResult<TValue, TError> result)
        {
            switch (result)
            {
                case Ok<TValue, TError> ok:
                    Console.WriteLine("Ok({0})", ok.UnsafelyUnwrap());
                    break;
                case Err<TValue, TError> err:
                    Console.WriteLine("This was `Err({0})`", err.UnsafelyUnwrapErr());
                    break;
                default:
                    throw new Exception("This should be impossible.");
            }
        }
    }
}