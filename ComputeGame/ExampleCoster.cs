using System;
using System.Collections.Generic;
using static ComputeGame.ExampleGenerator;

namespace ComputeGame
{
	/// <summary>
	/// Клас для вираховування ціни прикладу
	/// </summary>
	public class ExampleCoster
	{
		private static readonly Dictionary<OperationKind, Func<int, int, int>> OperationFuncCosters = new Dictionary<OperationKind, Func<int, int, int>>
		{
			{OperationKind.Addition, Addition },
			{OperationKind.Subtraction, Subtraction },
			{OperationKind.Multiplication, Multiplication },
			{OperationKind.Division, Division },
		};

		public int CalculateCost(int a, int b, OperationKind opKind)
		{
			var cost = OperationFuncCosters[opKind].Invoke(a, b);
			cost = (int)Math.Round(Math.Pow(cost, 1.4));
			return cost;
		}

		/// <summary>Вираховування цінності для чисел під час додавання</summary>
		/// <param name="a">Перший доданок</param>
		/// <param name="b">Другий доданок</param>
		/// <returns>Повертає цінність прикладу</returns>
		private static int Addition(int a, int b)
		{
			//визначаємо розрядності чисел
			int rankA = a.ToString().Length;
			int rankB = b.ToString().Length;


			int rank = (int)Math.Pow(10, Math.Max(rankA, rankB));

			int cost = 0;
			int keepInMind = 0;

			for (int i = 1; i < rank; i *= 10)
			{
				if (keepInMind == 1) cost++;

				int digitA = (a / i) % 10;
				int digitB = (b / i) % 10;
				cost += SimpleAddition(digitA, digitB, ref keepInMind);
			}

			return cost <= 0 ? 1 : cost;
		}

		/// <summary>Вираховування цінності для цифр під час додавання</summary>
		/// <param name="keepInMind">Один або нуль. Вказує, чи потрібно додавати одиничку</param>
		private static int SimpleAddition(int a, int b, ref int keepInMind)
		{
			// додати ці доданки нічого не варто, адже сума дорівнює ненульовому доданку (або 0)
			if (a == 0 || b == 0)
				return keepInMind;

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
		private static int Subtraction(int a, int b)
		{
			//вважатимемо, що a завжди більше, ніж b

			//визначаємо розрядності чисел
			int rankA = a.ToString().Length;
			int rankB = b.ToString().Length;
			int differenceRank = (a - b).ToString().Length;

			int cost = (differenceRank == rankB && rankB < rankA) ? -1 : 0;

			int rank = (int)Math.Pow(10, (rankA));
			int keepInMind = 0;

			for (int i = 1; i < rank; i *= 10)
			{
				int digitA = (a / i) % 10;
				int digitB = (b / i) % 10;
				cost += SimpleSubtraction(digitA, digitB, ref keepInMind);
			}

			return cost <= 0 ? 1 : cost;
		}

		private static int SimpleSubtraction(int a, int b, ref int keepInMind)
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


		private static int Multiplication(int a, int b)
		{
			// Робимо так, щоб а за будь-яких умов було більшим
			if (b > a)
			{
				int foo = a;
				a = b;
				b = foo;
			}

			if (b == 1) return 1;


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

					cost += SimpleMultiplication(digitA, digitB, ref keepInMind);
				}
				summands.Add(digitB * a * i);
			}


			if (bLength > 1)
			{
				cost += Addition(summands[1], summands[0]);
				for (int i = 2; i < bLength; i++)
				{
					// Сума попередніх двох доданків.
					int sum = summands[i - 1] + summands[i - 2];

					cost += Addition(sum, summands[1]);
				}
				//int Result = sum;
			}
			return cost;
		}

		private static int SimpleMultiplication(int a, int b, ref int keepInMind)
		{
			int ab = a * b;
			if (ab == 0)
			{
				keepInMind = 0;
				return 0;
			}

			if (ab + keepInMind < 10)
			{
				keepInMind = 0;
				// if(a==0 and b == 0) return 0
			}
			else
			{
				keepInMind = (ab) / 10;
			}

			return 1;
		}


		/// <param name="a">dividend</param>
		/// <param name="b">divisor</param>
		private static int Division(int a, int b)
		{
			while (b % 10 == 0)
			{
				b /= 10;
			}
			if (b == 1) return 1;


			//a від початку повинно бути більшим за b
			string quotient = (a / b).ToString();
			int qLength = quotient.Length;

			int rankA = (int)Math.Pow(10, a.ToString().Length);
			int cost = 0;

			for (int i = 0; i < qLength; i++)
			{
				// -48 для того, щоб перевести чар в інт
				int digitQ = quotient[i] - 48;
				cost += Multiplication(b, digitQ);

				int product = digitQ * b * (int)Math.Pow(10, qLength - i - 1);
				cost += Subtraction(a, product);
				a -= product;
			}


			return cost;
		}
	}
}
