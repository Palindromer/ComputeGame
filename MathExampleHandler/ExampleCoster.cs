using System;
using System.Collections.Generic;

namespace ComputeGame.MathExampleHandler
{
	/// <summary>
	/// Клас для вираховування ціни прикладу
	/// </summary>
	public class ExampleCoster
	{
		private static readonly Dictionary<OperationKind, Func<int, int, int>> OperationFuncCosters = new Dictionary<OperationKind, Func<int, int, int>>
		{
			{OperationKind.Addition, AdditionCost },
			{OperationKind.Subtraction, SubtractionCost },
			{OperationKind.Multiplication, MultiplicationCost },
			{OperationKind.Division, DivisionCost },
		};

		public int CalculatePoints(int a, int b, OperationKind opKind)
		{
			var cost = OperationFuncCosters[opKind].Invoke(a, b);
			cost = (int)Math.Round(Math.Pow(cost, 1.4));
			return cost;
		}

		public int CalculateStepsCount(int a, int b, OperationKind opKind)
		{
			var cost = OperationFuncCosters[opKind].Invoke(a, b);
			return cost;
		}

		/// <summary>Вираховування цінності для чисел під час додавання</summary>
		/// <param name="a">Перший доданок</param>
		/// <param name="b">Другий доданок</param>
		/// <returns>Повертає цінність прикладу</returns>
		private static int AdditionCost(int a, int b)
		{
			if (b > a)
			{
				(a, b) = (b, a);
			}

			//визначаємо розрядності чисел
			int rankA = a.ToString().Length;
			int rankB = b.ToString().Length;

			int rank = (int)Math.Pow(10, rankA);

			int cost = 0;
			int keepInMind = 0;

			for (int i = 1; i < rank; i *= 10)
			{
				if (keepInMind == 1) cost++;

				int digitA = (a / i) % 10;
				int digitB = (b / i) % 10;
				cost += SimpleAdditionCost(digitA, digitB, ref keepInMind);
			}

			return cost;
		}

		/// <summary>Вираховування цінності для цифр під час додавання</summary>
		/// <param name="keepInMind">Один або нуль. Вказує, чи потрібно додавати одиничку</param>
		private static int SimpleAdditionCost(int a, int b, ref int keepInMind)
		{
			// додати ці доданки нічого не варто, адже сума дорівнює ненульовому доданку (або 0)
			if (a == 0 || b == 0)
			{
				keepInMind = 0;
				return 0;
			}

			if (a + b + keepInMind >= 10)
			{
				keepInMind = 1;
			}
			else
			{
				keepInMind = 0;
			}
			return 1;
		}


		/// <summary>Вираховування цінності для чисел під час віднімання</summary>
		/// <param name="a">minuend (Зменшуване)</param>
		/// <param name="b">subtrahend (Від'ємник)</param>
		/// <returns>Повертає цінність прикладу</returns>
		private static int SubtractionCost(int a, int b)
		{
			//вважатимемо, що a завжди більше, ніж b

			//визначаємо розрядності чисел
			int aLength = a.ToString().Length;
			int bLength = b.ToString().Length;
			int differenceRank = (a - b).ToString().Length;

			int cost = (differenceRank == bLength && bLength < aLength) ? -1 : 0;

			int rank = (int)Math.Pow(10, aLength);
			int keepInMind = 0;

			for (int i = 1; i < rank; i *= 10)
			{
				int digitA = (a / i) % 10;
				int digitB = (b / i) % 10;
				cost += SimpleSubtractionCost(digitA, digitB, ref keepInMind);
			}

			return cost;
		}

		private static int SimpleSubtractionCost(int a, int b, ref int keepInMind)
		{
			if (b == 0)
			{
				keepInMind = (a - keepInMind < 0) ? 1 : 0;
				return 0;
			}

			b = b + keepInMind;

			if (a == b)
			{
				keepInMind = 0;
				return 0;
			}

			if (b == 0)
			{
				return 0;
				//return keepInMind; // можна додати 0.5, коли a == b 
			}

			if (a - b < 0)
			{
				keepInMind = 1;
				return 2;
			}

			keepInMind = 0;
			return 1;
		}


		private static int MultiplicationCost(int a, int b)
		{
			// Робимо так, щоб а за будь-яких умов було більшим
			if (b > a)
			{
				(a, b) = (b, a);
			}


			if (b == 1)
			{
				// Maybe we have to return 0?
				return 0;
			}


			int bLength = b.ToString().Length;

			// Визначаємо розрядності чисел. rankA завжди більше rankB.
			int rankA = (int)Math.Pow(10, a.ToString().Length);
			int rankB = (int)Math.Pow(10, bLength);

			int cost = 0;


			List<int> summands = new List<int>(bLength);
			for (int i = 1; i < rankB; i *= 10)
			{
				int digitB = (b / i) % 10;
				int keepInMind = 0;
				for (int j = 1; j < rankA; j *= 10)
				{
					if (keepInMind > 0) cost++;

					int digitA = (a / j) % 10;

					var smc = SimpleMultiplicationCost(digitA, digitB, ref keepInMind);
					cost += smc;
				}
				summands.Add(digitB * a * i);
			}


			if (bLength > 1)
			{
				cost += AdditionCost(summands[1], summands[0]);
				for (int i = 2; i < bLength; i++)
				{
					// Сума попередніх двох доданків.
					int sum = summands[i - 1] + summands[i - 2];

					cost += AdditionCost(sum, summands[i]);
				}
				//int Result = sum;
			}
			return cost;
		}

		private static int SimpleMultiplicationCost(int a, int b, ref int keepInMind)
		{
			int ab = a * b;
			if (ab == 0)
			{
				keepInMind = 0;
				return 0;
			}

			var result = ab + keepInMind;
			if (result < 10)
			{
				keepInMind = 0;
				// if(a==0 and b == 0) return 0
			}
			else
			{
				keepInMind = (result) / 10;
			}

			return 1;
		}


		/// <param name="a">dividend</param>
		/// <param name="b">divisor</param>
		private static int DivisionCost(int a, int b)
		{
			while (b % 10 == 0)
			{
				b /= 10;
			}
			if (b == 1) return 1;


			// WARN: a від початку повинно бути більшим за b.
			string quotient = (a / b).ToString();
			int qLength = quotient.Length;

			int aLength = a.ToString().Length;
			int rankA = (int)Math.Pow(10, aLength);

			int cost = 0;

			for (var i = 0; i < qLength; i++)
			{
				// -48 для того, щоб перевести чар в інт.
				var digitQ = (int)char.GetNumericValue(quotient[i]);

				cost += MultiplicationCost(b, digitQ);

				int product = digitQ * b * (int)Math.Pow(10, qLength - i - 1);
				cost += SubtractionCost(a, product);
				a -= product;
			}

			return cost;
		}
	}
}
