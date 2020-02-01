using System;


namespace ComputeGame
{
	[Flags]
	public enum DifficultyLevel
	{
		VeryEasy = 1 << 1,
		Easy = 1 << 2,
		Medium = 1 << 3,
		Hard = 1 << 4,
		VeryHard = 1 << 5
	}
}
