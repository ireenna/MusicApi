
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicBot.Models
{
    public class Commands
    {
        public string GetTopTracks(User item)
        {
            item.Link = $"http://ws.audioscrobbler.com/2.0/?method=chart.gettoptracks&api_key=afa703fe5d7edbe8f8a0aa92a414ec51&format=json";
            return item.Link;
        }
        public string GetArtistInfo(User item)
        {
            item.Link = $"http://ws.audioscrobbler.com/2.0/?method=artist.getinfo&artist={item.Artist}&api_key=afa703fe5d7edbe8f8a0aa92a414ec51&format=json";
            return item.Link;
        }
        public string GetTrack(User item)
        {
            if (item.Artist != null)
            {
                item.Link = $"http://ws.audioscrobbler.com/2.0/?method=track.search&track={item.Name}&artist={item.Artist}&api_key=afa703fe5d7edbe8f8a0aa92a414ec51&format=json";
                return item.Link;
            }
            else
            {
                item.Link = $"http://ws.audioscrobbler.com/2.0/?method=track.search&track={item.Name}&api_key=afa703fe5d7edbe8f8a0aa92a414ec51&format=json";
                return item.Link;
            }
        }
        //public string GetTopArtistTracks(User item)
        //{
        //    item.Link = $"http://ws.audioscrobbler.com/2.0/?method=artist.gettoptracks&artist={item.Artist}&api_key=afa703fe5d7edbe8f8a0aa92a414ec51&format=json";
        //    return item.Link;
        //}
    }
}
