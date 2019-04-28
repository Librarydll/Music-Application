using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TagLib;

namespace Keeper.Models
{
    public class Music
    {
		public int Id { get; set; }
		public string Singer { get; set; }
		public string SongName { get; set; }
		public string Duration { get; set; }
		public TimeSpan TimeSpanDuration { get; set; }
		public string URL { get; set; }
		public string AlbumUrl { get; set; }
		public string SingerAndSong { get => $"{Singer } {SongName}"; }
	}
}
