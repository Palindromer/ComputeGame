namespace ComputeGame.MathExampleHandler
{
	public class Example
	{
		public string Result { get; set; }

		public int Count { get; set; }

		public string Content { get; set; }

		public int Points { get; set; }

		public DifficultyLevel DifficultyLevel { get; set; }

		public Example(string content, int result, int points)
		{
			Result = result.ToString();
			Content = content;
			Points = points;
		}
	}
}
