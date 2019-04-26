using Caliburn.Micro;
using Keeper.Models;
using Microsoft.Win32;
using WMPLib;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Keeper.ViewModels
{
	class ShellViewModel :Screen
    {
		private BindableCollection<Music> _musicsItemSource = new BindableCollection<Music> ();
		WindowsMediaPlayer player = new  WindowsMediaPlayer();
		int index = 1;
		public BindableCollection<Music> MusicsItemSource
		{
			get { return _musicsItemSource; }
			set { _musicsItemSource = value; NotifyOfPropertyChange(() => MusicsItemSource); }
		}


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
					tempTime = tagFile.Properties.Duration.Seconds > 10 ? tagFile.Properties.Duration.Seconds.ToString() : "0" + tagFile.Properties.Duration.Seconds;
					musics.Duration = tagFile.Properties.Duration.Minutes + ":" + tempTime;
					MusicsItemSource.Add(musics);
					index++;
				}
			}
		}

		public void MusicPlay(Music music)
		{
			var url = music.URL;
			if (url == null)
				return;
			player.controls.stop();
			player.URL = string.Empty;
			player.URL = url;
			player.controls.play();
		}


	}
}
