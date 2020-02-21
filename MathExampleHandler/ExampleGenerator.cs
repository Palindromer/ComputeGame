using System;
using System.Collections.Generic;


namespace ComputeGame.MathExampleHandler
{
	public class ExampleGenerator
	{
		private ExampleCoster _exampleCoster;
		private DifficultyLevelDeterminer _difficultyLevelDeterminer;

		private static readonly Random Rnd = new Random();

		private readonly Dictionary<DifficultyLevel, Stack<Example>> _examples = new Dictionary<DifficultyLevel, Stack<Example>>();

		public ExampleGenerator()
		{
			_exampleCoster = new ExampleCoster();
			_difficultyLevelDeterminer = new DifficultyLevelDeterminer();

			foreach (DifficultyLevel level in Enum.GetValues(typeof(DifficultyLevel)))
			{
				_examples[level] = new Stack<Example>();
			}
		}

		public Example GenerateByDifficulty(DifficultyLevel level)
		{
			while (_examples[level].Count == 0)
			{
				var newExample = GenerateNewExample();
				var newExampleLevel = _difficultyLevelDeterminer.DetermineLevel(newExample.Points);

				if (newExampleLevel.HasValue)
				{
					_examples[newExampleLevel.Value].Push(newExample);
				}
			}

			return _examples[level].Pop();
		}

		private Example GenerateNewExample()
		{
			return GenerateNewExample(2, 999);
		}

		private Example GenerateNewExample(int from, int to)
		{
			// First number.
			int a = Rnd.Next(from, to);
			// Second number.
			int b = Rnd.Next(from, to);

			// Уникаємо прикладу, результат якого є від'ємним числом.
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
			var points = _exampleCoster.CalculatePoints(a, b, randomOpKind);

			return new Example(example, result, points);
		}

		private OperationKind GetRandomOperation()
		{
			var randomOpKind = (OperationKind)Rnd.Next(Enum.GetNames(typeof(OperationKind)).Length);
			return randomOpKind;
		}
	}
}
