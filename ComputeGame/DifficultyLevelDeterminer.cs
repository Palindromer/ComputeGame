using System;
using System.Collections.Generic;


namespace ComputeGame
{
	public class DifficultyLevelDeterminer
	{
		private readonly Dictionary<DifficultyLevel, Tuple<int, int>> LevelBounds = new Dictionary<DifficultyLevel, Tuple<int, int>>
		{
			{DifficultyLevel.VeryEasy, Tuple.Create(1, 4)},
			{DifficultyLevel.Easy, Tuple.Create(4, 7)},
			{DifficultyLevel.Medium, Tuple.Create(7, 10)},
			{DifficultyLevel.Hard, Tuple.Create(10, 16)},
			{DifficultyLevel.VeryHard, Tuple.Create(16, 24)},
		};

		public DifficultyLevel? DetermineLevel(int cost)
		{
			foreach (var levelBound in LevelBounds)
			{
				var bounds = levelBound.Value;

				var min = Math.Pow(bounds.Item1, 1.4);
				var max = Math.Pow(bounds.Item2, 1.4);

				if (cost >= min && cost < max)
				{
					return levelBound.Key;
				}
			}

			return null;
		}
	}
}
