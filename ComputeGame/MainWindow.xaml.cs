using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

namespace ComputeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _exampleCount;


        private readonly List<User> _users;
        private User _currentUser;

        readonly AudioManage _audioManage = new AudioManage();
        public MainWindow()
        {
            InitializeComponent();

            UsersData ud = new UsersData();
            _users = ud.GetUsersData();

            if (_users.Count != 0)
            {
                UserNameTB.Text = _users.Last().Name;
                AddUsersToListBox();
            }

            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1000);
            _dispatcherTimer.Tick += dispatcherTimer_Tick;


            _audioManage.BackGround.PlayLoop();
            _audioManage.MusicVolume = 0.75f;

        }


        private void AddUsersToListBox()
        {
            for (int i = _users.Count - 1; i >= 0; i--)
            {
                UsersListBox.Items.Add(_users[i].Name);
                UsersSetListBox.Items.Add(_users[i].Name);
            }
            UsersListBox.SelectedIndex = 0;
        }


        private int _score;

        public int Score
        {
            get { return _score; }
            set
            {
                if (value < 0)
                    value = 0;
                _score = value;
                ScoreLabel.Content = value;
            }
        }

        private Example _lastExample;

        public Example LastExample
        {
            get { return _lastExample; }
            set
            {
                Answer.Text = "";
                _lastExample = value;
                ExampleTB.Content = value.Content;
                LastExample.Count = _exampleCount++;
            }
        }

        private readonly ExampleManage _em = new ExampleManage();
        readonly Random _rnd = new Random();

        private void ChangeExample()
        {
            int index = _rnd.Next(_modesList.Count);
            int min = _modesList[index][0];
            int max = _modesList[index][1];
            LastExample = _em.GetExampleByCost(_rnd.Next(min, max));
        }

        private void Answer_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (Answer.Text == LastExample.Result)
            {
                _audioManage.TrueAnswer.MyPlay();
                HistoryGrid.Items.Add(_lastExample);
                Score += LastExample.Cost;
                ChangeExample();
            }
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            HistoryGrid.Items.Add(_lastExample);

            Score -= LastExample.Cost / 2;
            ChangeExample();

            Answer.Focus();
        }

        private int _totalSeconds = 300;
        private readonly DispatcherTimer _dispatcherTimer;

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartFlow();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msb = MessageBox.Show("Are you sure?", "Inspection", MessageBoxButton.YesNo);
            if (msb == MessageBoxResult.Yes)
            {
                StopFlow();
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            string seconds = (_totalSeconds % 60).ToString();
            if (seconds.Length == 1)
                seconds = "0" + seconds;
            TimerBlock.Content = _totalSeconds / 60 + ":" + seconds;
            _totalSeconds--;

            if (_totalSeconds < 0)
            {
                StopFlow();
                WriteProgress();
            }
        }

        private string _currentMode;
        private List<int[]> _modesList;
        private void StartFlow()
        {
            if (UserNameTB.Text == "")
            {
                MessageBox.Show("Enter your name!");
                return;
            }
            _currentUser = GetUserByName(UserNameTB.Text);
            if (_currentUser == null)
            {
                _currentUser = new User(UserNameTB.Text);
                _users.Add(_currentUser);
                UsersListBox.Items.Insert(0, _currentUser.Name);
                UsersSetListBox.Items.Insert(0, _currentUser.Name);
            }

            UsersListBox.SelectedItem = UserNameTB.Text;

            if (_modesList == null || _modesList.Count == 0)
            {
                MessageBox.Show("Please. Choose your mode!");
                return;
            }


            _audioManage.EndGame.Stop();
            _audioManage.BackGround.StopLoop();
            _audioManage.StartGame.MyPlay();

            StartTab.Width = 0;
            FlowTab.Width = 100;

            ChangeExample();

            Score = 0;

            FlowTab.Focus();

            Answer.Text = "";

            dispatcherTimer_Tick(new object(), EventArgs.Empty);
            _dispatcherTimer.Start();
        }


        private void StopFlow()
        {
            _audioManage.StartGame.Stop();
            _audioManage.BackGround.StopLoop();
            _audioManage.EndGame.MyPlay();

            _dispatcherTimer.Stop();
            MessageBox.Show("Flow is over! Your score: " + Score);

            StartTab.Width = 100;
            FlowTab.Width = 0;

            _totalSeconds = 300;
            StartTab.Focus();
        }

        private User GetUserByName(string userName)
        {
            for (int i = 0; i < _users.Count; i++)
            {
                if (_users[i].Name == userName)
                    return _users[i];
            }
            return null;
        }

        private void WriteProgress()
        {
            _currentUser.AddGameResult(Score, _currentMode);

            if (_users.Last() != _currentUser)
            {
                _users.Remove(_currentUser);
                _users.Add(_currentUser);
            }

            new UsersData().SaveUsersData(_users);
            MyChart.DataContext = null;
            MyChart.DataContext = _currentUser.GameResults;

            MyChart.Title = _currentUser.Name + " (Total: " + _currentUser.TotalScore + ")";
        }

        private void FlowTab_KeyDown(object sender, KeyEventArgs e)
        {
            Answer.Focus();
            int keyCode = (int)e.Key;

            // До вводу допускаємо лише цифри
            if (keyCode > 83 || keyCode < 34 || (keyCode > 43 && keyCode < 74))
            {
                e.Handled = true;
            }
        }

        // Подія вибору однієї з точок на графіку
        private void LineSeries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LineSeries ls = sender as LineSeries;
            if (ls.SelectedItem != null)
            {
                GameResult gameResult = (GameResult)(ls.SelectedItem);
                ResultDescription.Text = gameResult.ToString();
            }
        }

        private void UsersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_users.Count == 0)
            {
                MyChart.DataContext = null;
                MyChart.Title = "";
                return;
            }

            string selectedUserName;
            if (UsersListBox.SelectedItems.Count == 0)
            {
                return;
            }
            else
            {
                selectedUserName = UsersListBox.SelectedItems[0] as string;
            }

            User selectedUser = GetUserByName(selectedUserName);
            MyChart.DataContext = selectedUser.GameResults;
            MyChart.Title = selectedUserName + " (Total: " + selectedUser.TotalScore + ")";
        }

        private void sessionsRB_Checked(object sender, RoutedEventArgs e)
        {
            LineSeries.IndependentValuePath = "SessionNumber";
        }

        private void daysRB_Checked(object sender, RoutedEventArgs e)
        {
            LineSeries.IndependentValuePath = "DayNumber";
        }

        private readonly int[][] _modesRegions = new int[5][];
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            switch (((ToggleButton)sender).Content.ToString())
            {
                case "Very easy":
                    _modesRegions[0] = new[] { 1, 3 };
                    break;
                case "Easy":
                    _modesRegions[1] = new[] { 3, 5 };
                    break;
                case "Medium":
                    _modesRegions[2] = new[] { 5, 8 };
                    break;
                case "Hard":
                    _modesRegions[3] = new[] { 8, 16 };
                    break;
                case "Very hard":
                    _modesRegions[4] = new[] { 16, 24 };
                    break;
            }
            SpecifyMode();
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            switch (((ToggleButton)sender).Content.ToString())
            {
                case "Very easy":
                    _modesRegions[0] = null;
                    break;
                case "Easy":
                    _modesRegions[1] = null;
                    break;
                case "Medium":
                    _modesRegions[2] = null;
                    break;
                case "Hard":
                    _modesRegions[3] = null;
                    break;
                case "Very hard":
                    _modesRegions[4] = null;
                    break;
            }
            SpecifyMode();
        }

        private void SpecifyMode()
        {
            _modesList = new List<int[]>();

            _currentMode = "";
            for (int i = 0; i < _modesRegions.Length; i++)
            {
                if (_modesRegions[i] == null)
                {
                    _currentMode += "0";
                }
                else
                {
                    _currentMode += "1";
                    _modesList.Add(_modesRegions[i]);
                }
            }
            ModeNumberTB.Text = _currentMode;
        }



        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < UsersListBox.Items.Count; i++)
            {
                string userItem = (string)UsersListBox.SelectedItem;
                if (_currentUser != null && userItem == _currentUser.Name)
                {
                    UsersListBox.SelectedItem = userItem;
                    break;
                }
            }
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            TabControl.SelectedItem = SettingsTabItem;
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            HistoryGrid.Items.Clear();
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int newValue = (int)e.NewValue;
            if (VolumeSliderTB == null)
                return;
            VolumeSliderTB.Text = newValue.ToString();
            _audioManage.MusicVolume = (float)newValue / 100;
        }


        private void MusicButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            string imageSource;
            _audioManage.BackGround.Mute = !_audioManage.BackGround.Mute;
            if (_audioManage.BackGround.Mute)
            {
                imageSource = @"pack://application:,,,/ComputeGame;component/Resources/Mute.png";
                _audioManage.BackGround.StopLoop();
            }
            else
            {
                imageSource = @"pack://application:,,,/ComputeGame;component/Resources/Unmute.png";
                _audioManage.BackGround.PlayLoop();
            }
            MusicButton.Source = new BitmapImage(new Uri(imageSource));
        }

        private void SoundsButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _audioManage.IsSoundMute = !_audioManage.IsSoundMute;
            var imageSource = _audioManage.IsSoundMute ? @"pack://application:,,,/ComputeGame;component/Resources/Mute.png" : @"pack://application:,,,/ComputeGame;component/Resources/Unmute.png";
            SoundsButton.Source = new BitmapImage(new Uri(imageSource));
        }



        private void RemoveUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersSetListBox.SelectedItems.Count == 0)
            {
                MessageBox.Show("Choose user!");
                return;
            }

            string userName = UsersSetListBox.SelectedItems[0] as string;
            MessageBoxResult mbr = MessageBox.Show("  Remove " + userName + "?", "", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (mbr == MessageBoxResult.Yes)
            {
                for (int i = 0; i < _users.Count; i++)
                {
                    if (_users[i].Name == userName)
                    {
                        _users.RemoveAt(i);
                        UsersSetListBox.Items.Clear();
                        UsersListBox.Items.Clear();
                        AddUsersToListBox();
                        new UsersData().SaveUsersData(_users);
                    }
                }
            }
        }
    }
}