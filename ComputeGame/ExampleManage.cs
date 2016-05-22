using System;
using System.Collections.Generic;


namespace ComputeGame
{
    public class ExampleManage : ExampleCost
    {
        readonly List<Example>[] _exampleStorage;

        public ExampleManage()
        {
            _exampleStorage = new List<Example>[85];
            for (int i = 0; i < _exampleStorage.Length; i++)
            {
                _exampleStorage[i] = new List<Example>();
            }
        }

        private readonly Random _rnd = new Random();
        public Example GetExampleByCost(int cost)
        {
            Example ex;
            if (_exampleStorage[cost].Count != 0)
            {
                ex = _exampleStorage[cost][0];
                _exampleStorage[cost].RemoveAt(0);
            }
            else
            {
                while (true)
                {
                    ex = GetNewExample();
                    int nominalCost = ex.Cost;
                    if (nominalCost == cost)
                        break;

                    _exampleStorage[nominalCost].Add(ex);
                }
            }
            ex.Cost = (int) (Math.Pow(cost * 10 + _rnd.NextDouble() * 2, 1.18) * 10);
            return ex;
        }


        private static readonly Array Operations = Enum.GetValues(typeof(Operation));
        private enum Operation
        {
            Addition,
            Subtraction,
            Multiplication,
            Division
        }

        static readonly Random Rnd = new Random();

        public static Example GetNewExample()
        {
            return GetNewExample(2, 666);
        }


        private static Example GetNewExample(int from, int to)
        {
            // first number
            int a = Rnd.Next(from, to);
            // second number
            int b = Rnd.Next(from, to);

            // Уникаємо прикладу, результат якого є від'ємним числом
            if (a < b)
            {
                int foo = a;
                a = b;
                b = foo;
            }

            string example = null;
            int result = 0;
            int cost = 0;


            Operation randomBOp = (Operation)Operations.GetValue(Rnd.Next(4));
            switch (randomBOp)
            {
                case Operation.Addition:
                    example = a + "+" + b;
                    result = a + b;
                    cost = Addition(a, b);
                    break;
                case Operation.Subtraction:
                    result = a - b;
                    example = a + "-" + b;
                    cost = Subtraction(a, b);
                    break;
                case Operation.Multiplication:
                    example = a + "×" + b;
                    result = a * b;
                    cost = Multiplication(a, b);

                    break;
                case Operation.Division:
                    a = a * b;
                    result = a / b;
                    example = a + "÷" + b;
                    cost = Division(a, b);
                    break;
            }

           // int additional = (a.ToString() + result).Length * 7;

            return new Example(example, result, cost /*+ Rnd.Next(-28, 28) + additional*/);
        }
    }




}
