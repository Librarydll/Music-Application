using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Keeper.ViewModels
{
    class ShellViewModel :Screen
    {
		private BindableCollection<string> _musicsItemSource = new BindableCollection<string> ();

		public BindableCollection<string> MusicsItemSource
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
			OpenFileDialog open = new OpenFileDialog
			{
				Title = "Select Music",
				Filter = "Music |*.mp3"
			};

			if(open.ShowDialog()==true)
			{
				foreach (var mus in open.FileNames)
				{
					MusicsItemSource.Add(Path.GetFileNameWithoutExtension(mus));
				}
			}


		}
	}
}
