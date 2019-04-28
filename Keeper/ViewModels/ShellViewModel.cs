using Caliburn.Micro;
using Keeper.Models;
using Microsoft.Win32;
using WMPLib;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using System.Windows;
using MaterialDesignThemes.Wpf;
using FluentScheduler;
using Keeper.MyHelpers;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Threading;
using System.Windows.Input;
using AudioSwitcher.AudioApi.CoreAudio;

namespace Keeper.ViewModels
{
	class ShellViewModel : Screen
	{
		#region Field
		private BindableCollection<Music> _musicsItemSource = new BindableCollection<Music>();
		private BindableCollection<PackIconKind> _packIcon = new BindableCollection<PackIconKind>() { PackIconKind.Pause, PackIconKind.RotateLeft, PackIconKind.ShuffleDisabled };
		int currentMusic = 1;
		WindowsMediaPlayer player = new WindowsMediaPlayer();
		int index = 1;
		bool isPaused = false;
		bool isShuffled = false;
		private int _selectedMusic;
		private string _song;
		private string _singer;
		private int m,
					s;
		int randomshuffle = 0;
		private List<int> randNumbers = null;
		private string _songDurationView;
		private int _currentMusicPosition;
		private int _maxTimeMusic = 10;
		private bool replayMusic = false;
		private FluentScheduler.Registry registry;
		#endregion
		#region Properties
		public BindableCollection<Music> MusicsItemSource
		{
			get { return _musicsItemSource; }
			set { _musicsItemSource = value; NotifyOfPropertyChange(() => MusicsItemSource); }
		}

		public BindableCollection<PackIconKind> PackIcon
		{
			get { return _packIcon; }
			set { _packIcon = value; NotifyOfPropertyChange(() => PackIcon); }
		}


		public int SelectedMusic
		{
			get { return _selectedMusic; }
			set { _selectedMusic = value; NotifyOfPropertyChange(() => SelectedMusic); }
		}

		public string Song
		{
			get { return _song; }
			set { _song = value; NotifyOfPropertyChange(() => Song); }
		}



		public string Singer
		{
			get { return _singer; }
			set { _singer = value; NotifyOfPropertyChange(() => Singer); }
		}
		public string SongDurationView
		{
			get { return _songDurationView; }
			set { _songDurationView = value; NotifyOfPropertyChange(() => SongDurationView); }
		}
		public int CurrentMusicPosition
		{
			get { return _currentMusicPosition; }
			set
			{
				_currentMusicPosition = value;
				NotifyOfPropertyChange(() => CurrentMusicPosition);
			}
		}

		public int MaxTimeMusic
		{
			get { return _maxTimeMusic; }
			set
			{
				_maxTimeMusic = value;
				NotifyOfPropertyChange(() => MaxTimeMusic);
			}
		}

		#endregion
		public  void MusicTimer_Tick()
		{
			s--;
			if (s == 0)
			{
				m--;
				s = 59;
			}
				if (m >= 10 && s >= 10)
					SongDurationView = m.ToString() + " : " + s.ToString();
				if (m >= 10 && s <= 10)
					SongDurationView = m.ToString() + " : " + "0" + s.ToString();
				if (m < 10 && s > 10)
					SongDurationView = "0" + m.ToString() + " : " + s.ToString();
				if (m < 10 && s < 10)
					SongDurationView = "0" + m.ToString() + " : " + "0" + s.ToString();
			if (m < 0)
			{
				SongDurationView = "00:00";
				if (replayMusic)
					PlayCurrentMusic();
				else
					NextSong();
			}
			CurrentMusicPosition++;
		}

		public void MusicDurationPlace(int userSetCurrentPosition)
		{

			CurrentMusicPosition = userSetCurrentPosition;
			int temp = MaxTimeMusic - CurrentMusicPosition;
			m = MathHelper.GetMinutes(temp);
			s = MathHelper.GetSeconds(temp);
			player.controls.currentPosition = CurrentMusicPosition;

		}

		public void InsertMusic()
		{
			Music musics = null;
			OpenFileDialog open = new OpenFileDialog
			{
				Title = "Select Music",
				Filter = "Music |*.mp3",
				Multiselect = true,
			};
			SaveFileDialog savePhoto = new SaveFileDialog();

			if (open.ShowDialog() == true)
			{
				foreach (var mus in open.FileNames)
				{
					string tempTime = null;
					musics = new Music();
					TagLib.File tagFile = TagLib.File.Create(mus);
					musics.Id = index;
					musics.Singer = tagFile.Tag.FirstPerformer;
					musics.SongName = !string.IsNullOrWhiteSpace(tagFile.Tag.Title) ? tagFile.Tag.Title : Path.GetFileNameWithoutExtension(mus);
					musics.URL = mus;
					musics.TimeSpanDuration = tagFile.Properties.Duration;
					//if (tagFile.Tag.Pictures.Length >= 1)
					//{
					//	var bin = tagFile.Tag.Pictures[0].Data.Data;
					//	var a = System.Drawing.Image.FromStream(new MemoryStream(bin)).GetThumbnailImage(100, 100, null, IntPtr.Zero);
					//}
					tempTime = tagFile.Properties.Duration.Seconds >= 10 ? tagFile.Properties.Duration.Seconds.ToString() : "0" + tagFile.Properties.Duration.Seconds;
					musics.Duration = tagFile.Properties.Duration.Minutes + ":" + tempTime;
					MusicsItemSource.Add(musics);
					index++;
				}
			}

				randNumbers = RandomHelper.GetRandom(MusicsItemSource.Count);

		}
		#region MainOperation
		public void MusicPlay(Music music)
		{
			MusicPlayFunction(music);
			currentMusic = music.Id;
			MaxTimeMusic =(int)music.TimeSpanDuration.TotalSeconds;
			SelectedMusic = currentMusic - 1;
			CurrentMusicPosition = 0;
			ActivateTimer();
		}
		public void PreviousSong()
		{
			if (MusicsItemSource.Count == 0)
				return;
			currentMusic--;
			if (currentMusic == 0)
			{
				currentMusic=MusicsItemSource.Count;
			}
			var tempCurrentMusic = MusicsItemSource.Where(i => i.Id == currentMusic).FirstOrDefault();

			if (isPaused)
			{
				PackIcon[0] = PackIconKind.Pause;
				isPaused = false;
			}
			MusicPlayFunction(tempCurrentMusic);
			SelectedMusic = currentMusic - 1;
			CurrentMusicPosition = 0;

		}
		public void NextSong()
		{
			if (MusicsItemSource.Count == 0)
				return;
			currentMusic++;
			if (currentMusic > MusicsItemSource.Count)
			{
				currentMusic=1;
			}
			if(isShuffled)
			{
				currentMusic = randNumbers[randomshuffle];
				randomshuffle++;
				randomshuffle = randomshuffle >= MusicsItemSource.Count ? randomshuffle = 0 : randomshuffle;
			}
			var tempCurrentMusic = MusicsItemSource.Where(i => i.Id == currentMusic).FirstOrDefault();
			if (isPaused)
			{
				PackIcon[0] = PackIconKind.Pause;
				isPaused = false;
			}
			MusicPlayFunction(tempCurrentMusic);	
			SelectedMusic = currentMusic - 1;
			CurrentMusicPosition = 0;
			MaxTimeMusic =(int) tempCurrentMusic.TimeSpanDuration.TotalSeconds;
		}
		#endregion
		public void PauseButton()
		{
			if (string.IsNullOrWhiteSpace(player.URL))
				return;
			if (!isPaused)
			{
				PackIcon[0] = PackIconKind.Play;
				player.controls.pause();
				isPaused = true;
			}
			else
			{
				PackIcon[0] = PackIconKind.Pause;
				player.controls.play();
				isPaused = false;
			}
		}
		public void ReplayCurrentMusic()
		{
			if (!replayMusic)
			{
				replayMusic = true;
				PackIcon[1] = PackIconKind.FormatRotate90;
			}
			else
			{
				PackIcon[1] = PackIconKind.RotateLeft;
				replayMusic = false;
			}

		}
		private void PlayCurrentMusic()
		{
			var url = MusicsItemSource[currentMusic-1].URL;
			if (url == null)
				return;
			currentMusic = MusicsItemSource[currentMusic-1].Id;
			player.controls.stop();
			player.URL = string.Empty;
			player.URL = url;
			player.controls.play();
			m = MusicsItemSource[currentMusic-1].TimeSpanDuration.Minutes;
			s = MusicsItemSource[currentMusic-1].TimeSpanDuration.Seconds;
			Song = MusicsItemSource[currentMusic-1].SongName;
			Singer = MusicsItemSource[currentMusic-1].Singer;
			SelectedMusic = currentMusic-1;
			CurrentMusicPosition = 0;
		}
		public void ActivateTimer()
		{
			registry = null;
			registry = new FluentScheduler.Registry();
			JobManager.Stop();
			JobManager.RemoveAllJobs();
			registry.Schedule(() =>
			{
				MusicTimer_Tick();
			}).ToRunEvery(1).Seconds();
			JobManager.Initialize(registry);
		}
		public void CloseApplication()
		{
			this.TryClose();
		}
		public void MusicPlayFunction(Music music)
		{
			if (music == null)
				return;
			var url = music.URL;
			if (url == null)
				return;
			player.controls.stop();
			player.URL = string.Empty;
			player.URL = url;
			player.controls.play();
			m = music.TimeSpanDuration.Minutes;
			s = music.TimeSpanDuration.Seconds;
			Song = music.SongName;
			Singer = music.Singer;		
		}
		public void ShuffleMusic()
		{
			if (!isShuffled)
			{
				isShuffled = true;
				PackIcon[2] = PackIconKind.ShuffleVariant;
			}
			else
			{
				PackIcon[2] = PackIconKind.ShuffleDisabled;
				isShuffled = false;
			}
		}
		public void KeyPressed(KeyEventArgs e)
		{
			if(e.Key == Key.Space)
			{
				PauseButton();
			}
			if(e.Key ==Key.MediaNextTrack)
			{
				NextSong();
			}
			if (e.Key == Key.MediaPreviousTrack)
			{
				PreviousSong();
			}
			if (e.Key == Key.MediaPlayPause)
			{
				PauseButton();
			}

		}
	}
}
