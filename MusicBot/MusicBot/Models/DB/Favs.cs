using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicBot.Models.DB
{
    public class DBFavs
    {
        public string id { get; set; }
        public List <SearchTrackMain> jsresults = new List<SearchTrackMain>();
    }
}
