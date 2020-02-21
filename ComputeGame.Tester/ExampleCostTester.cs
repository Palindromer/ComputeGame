using ComputeGame.MathExampleHandler;
using Xunit;

namespace ComputeGame.Tester
{
	public class ExampleCostTester
	{
		private ExampleCoster _exampleCoster;

		public ExampleCostTester()
		{
			_exampleCoster = new ExampleCoster();
		}

		[Theory]
		[InlineData(400, 945, OperationKind.Multiplication, 5)]
		[InlineData(407, 521, OperationKind.Multiplication, 11)]
		[InlineData(647, 400, OperationKind.Addition, 1)]
		[InlineData(3647, 208400, OperationKind.Addition, 4)]
		[InlineData(26444, 44, OperationKind.Division, 3)]
		[InlineData(13545, 45, OperationKind.Division, 3)]
		[InlineData(45, 3, OperationKind.Multiplication, 3)]
		[InlineData(813, 803, OperationKind.Multiplication, 9)]
		[InlineData(650400, 2439, OperationKind.Addition, 1)]
		[InlineData(813, 8, OperationKind.Multiplication, 5)]
		[InlineData(614, 570, OperationKind.Addition, 2)]
		[InlineData(573, 9, OperationKind.Multiplication, 5)]
		[InlineData(5380, 5157, OperationKind.Subtraction, 4)]
		[InlineData(573, 3, OperationKind.Multiplication, 4)]
		[InlineData(2234, 1719, OperationKind.Subtraction, 5)]
		[InlineData(538047, 573, OperationKind.Division, 23)]
		public void StepsCountTest(int a, int b, OperationKind op, int stepsCount)
		{
			var steps = _exampleCoster.CalculateStepsCount(a, b, op);
			Assert.Equal(stepsCount, steps);
		}

		[Theory]
		[InlineData(538047, 573, OperationKind.Division, 23)]
		public void StepsCountTest2(int a, int b, OperationKind op, int stepsCount)
		{
			var steps = _exampleCoster.CalculateStepsCount(a, b, op);
			Assert.Equal(stepsCount, steps);
		}
	}
}
