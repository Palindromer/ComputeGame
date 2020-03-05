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
using ComputeGame.MathExampleHandler;

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

		readonly AudioManager _audioManage = new AudioManager();
		private readonly ExampleGenerator _exampleManager = new ExampleGenerator();

		private DifficultyLevel? _currentDifficultyLevel = null;

		public MainWindow()
		{
			InitializeComponent();

			UsersData ud = new UsersData();
			_users = ud.GetUsersData();

			if (_users.Count != 0)
			{
				UsersListComboBox.Text = _users.Last().Name;
				AddUsersToListBox();
			}

			_dispatcherTimer = new DispatcherTimer();
			_dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1000);
			_dispatcherTimer.Tick += dispatcherTimer_Tick;


			//_audioManage.BackGround.PlayLoop();
			_audioManage.BackGround.Mute = true;
			_audioManage.StartGame.Mute = true;
			_audioManage.MusicVolume = 0.3f;
		}


		private void AddUsersToListBox()
		{
			for (int i = _users.Count - 1; i >= 0; i--)
			{
				UsersListBox.Items.Add(_users[i].Name);
				UsersSetListBox.Items.Add(_users[i].Name);
				UsersListComboBox.Items.Add(_users[i].Name);
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

		private void ChangeExample()
		{
			LastExample = _exampleManager.GenerateByDifficulty(_currentDifficultyLevel.Value);
		}

		private void Answer_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			if (Answer.Text == LastExample.Result)
			{
				_audioManage.TrueAnswer.MyPlay();
				HistoryGrid.Items.Add(_lastExample);
				Score += LastExample.Points;
				ChangeExample();
			}
		}

		private void SkipButton_Click(object sender, RoutedEventArgs e)
		{
			HistoryGrid.Items.Add(_lastExample);

			Score -= LastExample.Points / 2;
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


		private void StartFlow()
		{
			if (UsersListComboBox.Text == "")
			{
				MessageBox.Show("Enter your name!");
				return;
			}
			_currentUser = GetUserByName(UsersListComboBox.Text);
			if (_currentUser == null)
			{
				_currentUser = new User(UsersListComboBox.Text);
				_users.Add(_currentUser);
				UsersListBox.Items.Insert(0, _currentUser.Name);
				UsersSetListBox.Items.Insert(0, _currentUser.Name);
				UsersListComboBox.Items.Insert(0, _currentUser.Name);
			}

			UsersListBox.SelectedItem = UsersListComboBox.Text;
			UsersListComboBox.SelectedItem = UsersListComboBox.Text;

			if (!_currentDifficultyLevel.HasValue)
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
			_currentUser.AddGameResult(Score, _currentDifficultyLevel.ToString());

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
			var isDigit = (keyCode >= 34 && keyCode <= 43) || (keyCode >= 74 && keyCode <= 83);
			if (!isDigit)
			{
				e.Handled = true;
			}
		}

		// Подія вибору однієї з точок на графіку
		private void LineSeries_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var ls = sender as LineSeries;
			if (ls.SelectedItem != null)
			{
				var gameResult = (GameResult)(ls.SelectedItem);
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

			if (UsersListBox.SelectedItems.Count == 0)
			{
				return;
			}

			var selectedUserName = UsersListBox.SelectedItems[0] as string;

			var selectedUser = GetUserByName(selectedUserName);
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


		private void ToggleButton_Checked(object sender, RoutedEventArgs e)
		{
			var difficultyLevelButtons = new Dictionary<ToggleButton, DifficultyLevel>
			{
				{ VeryEasyToggleButton, DifficultyLevel.VeryEasy },
				{ EasyToggleButton, DifficultyLevel.Easy },
				{ MediumToggleButton, DifficultyLevel.Medium },
				{ HardToggleButton, DifficultyLevel.Hard },
				{ VeryHardToggleButton, DifficultyLevel.VeryHard },
			};

			foreach (var pair in difficultyLevelButtons)
			{
				if (pair.Key == (ToggleButton)sender)
				{
					_currentDifficultyLevel = pair.Value;
				}
				else
				{
					pair.Key.IsChecked = false;
				}
			}
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