using System;
using System.IO;
using System.Threading;
using NAudio.Wave;
using ComputeGame.Properties;
using NAudio.Wave.SampleProviders;

namespace ComputeGame
{
    class AudioManager
    {
        public Sound BackGround;
        public Sound StartGame;
        public Sound EndGame;
        public Sound TrueAnswer;
        public AudioManager()
        {
            BackGround = new Sound(Resources.BackGround);
            StartGame = new Sound(Resources.StartGame);
            EndGame = new Sound(Resources.EndGame);
            TrueAnswer = new Sound(Resources.TrueAnswer);

            EndGame.PlaybackStopped += (sender, args) =>{BackGround.PlayLoop();};
            StartGame.PlaybackStopped += (sender, args) =>{BackGround.PlayLoop();};
        }

        public float MusicVolume
        {
            //get { return BackGround.Volume; }
            set { BackGround.Volume = value; }
        }

        private bool _isSoundMute;
        public bool IsSoundMute
        {
            get { return _isSoundMute; }
            set
            {
                _isSoundMute = value;
                StartGame.Mute = value;
                EndGame.Mute = value;
                TrueAnswer.Mute = value;
            }
        }
    }

    class Sound : WaveOut
    {
        private readonly WaveStream _mp3Reader;
        public bool Mute;
        public Sound(byte[] resource)
            : base()
        {
            _mp3Reader = new Mp3FileReader(new MemoryStream(resource));
            Init(_mp3Reader);
            PlaybackStopped += (sender, args) => {_mp3Reader.Position = 0; };
        }

        public void MyPlay()
        {
            if (!Mute)
            {
                Play();
            }
        }


        public void PlayLoop()
        {
            MyPlay();
            PlaybackStopped += OnPlaybackStopped;
        }


        public void StopLoop()
        {
            new Thread(() =>
            {
                _mp3Reader.Position = 0;
                PlaybackStopped -= OnPlaybackStopped;
                Stop();
            }).Start();
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs stoppedEventArgs)
        {
            MyPlay();
        }
    }
}
