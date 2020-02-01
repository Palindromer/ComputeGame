using System;
using System.Collections.Generic;


namespace ComputeGame
{
	public class ExampleManager
	{
		private ExampleCoster _exampleCoster;
		readonly List<Example>[] _exampleStorage;

		public ExampleManager()
		{
			_exampleCoster = new ExampleCoster();
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
			//ex.Cost = (int)(Math.Pow(cost * 10 + _rnd.NextDouble() * 2, 1.18) * 10);
			return ex;
		}

		private static readonly Random Rnd = new Random();

		public ExampleCoster ExampleCoster => _exampleCoster;

		public Example GetNewExample()
		{
			return GenerateNewExample(2, 999);
		}

		private Example GenerateNewExample(int from, int to)
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

			var randomOpKind = GetRandomOperation();

			if (randomOpKind == OperationKind.Division)
			{
				a = a * b;
			}

			var example = a + randomOpKind.GetSign() + b;
			var result = randomOpKind.GetFunc().Invoke(a, b);
			var cost = _exampleCoster.GetCost(a, b, randomOpKind);

			return new Example(example, result, cost);
		}

		private OperationKind GetRandomOperation()
		{
			var randomOpKind = (OperationKind)Rnd.Next(Enum.GetNames(typeof(OperationKind)).Length);
			return randomOpKind;
		}
	}
}
