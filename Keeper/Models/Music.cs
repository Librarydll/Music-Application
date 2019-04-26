using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keeper.Models
{
    public class Music
    {
		public int Id { get; set; }
		public string Singer { get; set; }
		public string SongName { get; set; }
		public string Duration { get; set; }
		public string URL { get; set; }
		public string SingerAndSong { get => $"{Singer } {SongName}"; }
	}
}
