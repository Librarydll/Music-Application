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

namespace Keeper.ViewModels
{
	class ShellViewModel :Screen
    {
		#region Field
		private BindableCollection<Music> _musicsItemSource = new BindableCollection<Music> ();
		private MaterialDesignThemes.Wpf.PackIconKind _packIcon = MaterialDesignThemes.Wpf.PackIconKind.Pause;
		int currentMusic = 1;
		WindowsMediaPlayer player = new  WindowsMediaPlayer();
		int index = 1;
		bool isPaused = false;
		private int _selectedMusic;
		private string _song;
		private string _singer;

		#endregion
		#region Properties
		public BindableCollection<Music> MusicsItemSource
		{
			get { return _musicsItemSource; }
			set { _musicsItemSource = value; NotifyOfPropertyChange(() => MusicsItemSource); }
		}

		public MaterialDesignThemes.Wpf.PackIconKind PackIcon
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

		#endregion




		public void	CloseApplication()
		{
			this.TryClose();
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

			if(open.ShowDialog()==true)
			{
				foreach (var mus in open.FileNames)
				{
					string tempTime = null;
					musics = new Music();
					TagLib.File tagFile = TagLib.File.Create(mus);
					musics.Id = index;
					musics.Singer = tagFile.Tag.FirstPerformer;
					musics.SongName = !string.IsNullOrWhiteSpace(tagFile.Tag.Title)? tagFile.Tag.Title : Path.GetFileNameWithoutExtension(mus);
					musics.URL = mus;
					if(tagFile.Tag.Pictures.Length>=1)
					{
						var bin = tagFile.Tag.Pictures[0].Data.Data;
						var a  = System.Drawing.Image.FromStream(new MemoryStream(bin)).GetThumbnailImage(100, 100, null, IntPtr.Zero);
					}
					tempTime = tagFile.Properties.Duration.Seconds > 10 ? tagFile.Properties.Duration.Seconds.ToString() : "0" + tagFile.Properties.Duration.Seconds;
					musics.Duration = tagFile.Properties.Duration.Minutes + ":" + tempTime;
					MusicsItemSource.Add(musics);
					index++;
				}
			}
		}
		#region MainOperation
		public void MusicPlay(Music music)
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
			Song = music.SongName;
			Singer = music.Singer;
			currentMusic = music.Id;
			SelectedMusic = currentMusic-1;
		}
		public void PreviousSong()
		{
			if (currentMusic == 1)
				return;
			var tempCurrentMusic = MusicsItemSource.Where(i => i.Id == currentMusic-1).FirstOrDefault();
			var currentUrl = tempCurrentMusic.URL;
			if (string.IsNullOrWhiteSpace(currentUrl)||currentMusic==1)
				return;
			player.controls.stop();
			player.URL = string.Empty;
			player.URL = currentUrl;
			player.controls.play();
			Song = tempCurrentMusic.SongName;
			Singer = tempCurrentMusic.Singer;
			currentMusic = currentMusic-1;
			SelectedMusic = currentMusic-1;
		}
		public void NextSong()
		{
			if (currentMusic + 1 >= MusicsItemSource.Count)
				return;
			var tempCurrentMusic = MusicsItemSource.Where(i => i.Id == currentMusic - 1).FirstOrDefault();
			var currentUrl = tempCurrentMusic.URL;
			if (string.IsNullOrWhiteSpace(currentUrl))
				return;
			player.controls.stop();
			player.URL = string.Empty;
			player.URL = currentUrl;
			player.controls.play();
			Song = tempCurrentMusic.SongName;
			Singer = tempCurrentMusic.Singer;
			currentMusic = currentMusic + 1;
			SelectedMusic = currentMusic-1;
		}
		#endregion
		public void PauseButton()
		{
			if (string.IsNullOrWhiteSpace(player.URL))
				return;
			if (!isPaused)
			{
				PackIcon = MaterialDesignThemes.Wpf.PackIconKind.Play;
				player.controls.pause();
				isPaused = true;
			}
			else
			{
				PackIcon = MaterialDesignThemes.Wpf.PackIconKind.Pause;
				player.controls.play();
				isPaused = false;
			}
		}

	}
}
