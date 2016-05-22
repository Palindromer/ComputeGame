using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace ComputeGame
{
    public class UsersData
    {
        private readonly string _usersDataPath;

        public UsersData()
        {
            string programStorage = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/ImproveYourMind";
            
            _usersDataPath = programStorage + @"/Users.dat";

            if (!Directory.Exists(programStorage))
            {
                Directory.CreateDirectory(programStorage);
            }
        }


        public void SaveUsersData(List<User> users)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream fs = new FileStream(_usersDataPath, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, users);
            }
        }

        public List<User> GetUsersData()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(_usersDataPath, FileMode.OpenOrCreate))
            {
                if (fs.Length == 0)
                    return new List<User>();
                return (List<User>)formatter.Deserialize(fs);
            }
        }
    }

    [Serializable]
    public class User
    {
        public string Name;
        public int TotalScore;
        public List<GameResult> GameResults = new List<GameResult>();

        public User() : this("") { }
        public User(string name)
        {
            TotalScore = 0;
            Name = name;
        }

        public void AddGameResult(int score, string mode)
        {
            AddGameResult(DateTime.Now, score, mode);
        }

        public void AddGameResult(DateTime date, int score, string mode)
        {
            TotalScore += score;
            if (GameResults.Count == 0)
            {
                GameResults.Add(new GameResult(0, date, score, mode, 0));
            }
            else
            {
                int sessionNumber = GameResults.Last().SessionNumber + 1;
                int dayNumber = (int)(date - GameResults[0].Date).TotalDays;
                GameResults.Add(new GameResult(dayNumber, date, score, mode, sessionNumber));
            }
        }
    }



    [Serializable]
    public class GameResult
    {
        public DateTime Date { get; set; }
        public int Score { get; set; }
        public string Mode { get; set; }
        public int DayNumber { get; set; }
        public int SessionNumber { get; set; }

        public GameResult(int dayNumber, DateTime date, int score, string mode, int sessionNumber)
        {
            DayNumber = dayNumber;
            Date = date;
            Score = score;
            Mode = mode;
            SessionNumber = sessionNumber;
        }

        public override string ToString()
        {
            return "Mode: " + Mode + ".\nScore: " + Score +
                   ".\n\nDate: " + Date.ToShortDateString() + ".\nTime: " + Date.ToShortTimeString() +
                   ".\n\nDay: " + (DayNumber + 1) + ".\nSession: " + (SessionNumber + 1) + ".";
        }
    }
}