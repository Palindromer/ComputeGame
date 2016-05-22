namespace ComputeGame
{
    public class Example
    {
        public string Result { get; set; }
        public int Count { get; set; }
        public string Content { get; set; }
        public int Cost { get; set; }

        public Example(string content, int result, int cost)
        {
            Result = result.ToString();
            Content = content;
            Cost = cost;
        }
    }
}
