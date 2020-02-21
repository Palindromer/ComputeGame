using System;
using System.Collections.Generic;

namespace ComputeGame.MathExampleHandler
{
	public static class OperationKindExtensions
	{
		private static readonly Dictionary<OperationKind, string> OperationSigns = new Dictionary<OperationKind, string>
		{
			{OperationKind.Addition, "+" },
			{OperationKind.Subtraction, "-" },
			{OperationKind.Multiplication, "×" },
			{OperationKind.Division, "÷" },
		};


		private static readonly Dictionary<OperationKind, Func<int, int, int>> OperationFuncs = new Dictionary<OperationKind, Func<int, int, int>>
		{
			{OperationKind.Addition, (int a, int b) => { return a + b; } },
			{OperationKind.Subtraction, (int a, int b) => { return a - b; } },
			{OperationKind.Multiplication, (int a, int b) => { return a * b; } },
			{OperationKind.Division, (int a, int b) => { return a / b; } },
		};

		public static string GetSign(this OperationKind opKind)
		{
			return OperationSigns[opKind];
		}

		public static Func<int, int, int> GetFunc(this OperationKind opKind)
		{
			return OperationFuncs[opKind];
		}
	}

}
