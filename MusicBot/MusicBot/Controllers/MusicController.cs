using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MusicBot.Models;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using MusicBot.Models.DB;

namespace MusicBot.Controllers
{
    [Route("api/[controller]")]
    public class MusicController:Controller
    {
        private const string path = @"DB.json";
        Commands commands = new Commands();

        [HttpPost]
        [Route("GetTopTracks")]
        public async Task<ObjectResult> FindTopTracks([FromBody] User item)
        {
            string link = commands.GetTopTracks(item);

            string response;

            using var client = new HttpClient();
            response = await client.GetStringAsync(link);

            User useritem = JsonConvert.DeserializeObject<User>(response);

            ClassTracks toptracksinfo = JsonConvert.DeserializeObject<ClassTracks>(response);

            for (int i = toptracksinfo.tracks.track.Count-1; i > 9; i--)
            {
                toptracksinfo.tracks.track.RemoveAt(i);
            }

            return new ObjectResult(toptracksinfo);
        }
        [HttpPost]
        [Route("GetArtistInfo")]
        public async Task<ObjectResult> FindArtistInfo([FromBody] User item)
        {
            string link = commands.GetArtistInfo(item);

            string response;

            using var client = new HttpClient();
            response = await client.GetStringAsync(link);

            ClassListArtistInfo artistinfo = JsonConvert.DeserializeObject<ClassListArtistInfo>(response);

            return new ObjectResult(artistinfo);
        }
        [HttpPost]
        [Route("GetTrack")]
        public async Task<ObjectResult> FindTrack([FromBody] User item)
        {
            string link = commands.GetTrack(item);

            string response;

            using var client = new HttpClient();
            response = await client.GetStringAsync(link);

            User useritem = JsonConvert.DeserializeObject<User>(response);

            ClassResults searchtrack = JsonConvert.DeserializeObject<ClassResults>(response);

            for (int i = searchtrack.results.trackmatches.track.Count - 1; i > 9; i--)
            {
                searchtrack.results.trackmatches.track.RemoveAt(i);
            }

            return new ObjectResult(searchtrack);
        }

        [HttpPost]
        [Route("AddFavorite")]
        public async Task<ObjectResult> AddFavorite([FromBody] User item)
        {
                string link = commands.GetTrack(item);

                string response;

                using var client = new HttpClient();
                response = await client.GetStringAsync(link);

                User useritem = JsonConvert.DeserializeObject<User>(response);

                ClassResults searchtrack = JsonConvert.DeserializeObject<ClassResults>(response);

                if (searchtrack.results.trackmatches.track.Count == 0)
                {
                    return new ObjectResult("It's bad");
                }

                for (int i = 0; i < searchtrack.results.trackmatches.track.Count - 1; i++)
                {
                //...
                if (searchtrack.results.trackmatches.track[i].name== item.Name && searchtrack.results.trackmatches.track[i].artist == item.Artist)
                {
                    SearchTrackMain dbtrack = searchtrack.results.trackmatches.track[i];
                    searchtrack.results.trackmatches.track.Clear();
                    searchtrack.results.trackmatches.track.Add(dbtrack);

                    DBFavs dbfavs = new DBFavs();
                    dbfavs.id = item.Id;
                    dbfavs.jsresults.Add(searchtrack.results.trackmatches.track[i]);


                    DBListofFavs dBUserItemsDB = new DBListofFavs();
                    dBUserItemsDB.dbresults.Add(dbfavs);
                    DBListofFavs dBUserItemsDBFind = JsonConvert.DeserializeObject<DBListofFavs>(System.IO.File.ReadAllText(path));

                    bool contains = false;
                    int indx = 0;

                    for (int j = 0; j < dBUserItemsDBFind.dbresults.Count; j++)
                    {
                        if (dBUserItemsDBFind.dbresults[j].id == item.Id)
                        {
                            indx = j;
                            contains = true;
                            break;
                        }
                    }

                    if (contains == false)
                    {
                        dBUserItemsDBFind.dbresults.Add(dbfavs);
                        System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(dBUserItemsDBFind));
                    }
                    else
                    {
                        bool TrackContains = false;
                        List<SearchTrackMain> userItemadding = dBUserItemsDBFind.dbresults[indx].jsresults;
                        dBUserItemsDBFind.dbresults[indx].jsresults = userItemadding;
                        for (int k = 0; k < dBUserItemsDBFind.dbresults[indx].jsresults.Count; k++)
                        {
                            if (dBUserItemsDBFind.dbresults[indx].jsresults[k].name == dbtrack.name & dBUserItemsDBFind.dbresults[indx].jsresults[k].artist == dbtrack.artist)
                            {
                                TrackContains = true;
                            }
                        }

                        if (TrackContains == false)
                        {
                            dBUserItemsDBFind.dbresults[indx].jsresults.Add(dbtrack);
                            System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(dBUserItemsDBFind));
                            break;
                        }
                        else
                        {
                            return (new ObjectResult("BAD"));//уже есть в списке
                        }
                    }
                    break;
                }
                }
            return new ObjectResult("OK");
        }
        [HttpPost]
        [Route("GetFavorite")]
        public async Task<ObjectResult> GetFavorite([FromBody] User item)
        {
            DBListofFavs dBUserItemsDBFind = JsonConvert.DeserializeObject<DBListofFavs>(System.IO.File.ReadAllText(path));

            bool contains = false;
            int indx = 0;

            for (int i = 0; i < dBUserItemsDBFind.dbresults.Count; i++)
            {
                if (dBUserItemsDBFind.dbresults[i].id == item.Id)
                {
                    indx = i;
                    contains = true;
                    break;
                }
            }

            if (contains == false)
            {
                return new ObjectResult("BAD");
            }
            else
            {
                return new ObjectResult(dBUserItemsDBFind.dbresults[indx].jsresults);
            }
        }
        [HttpPut]
        [Route("DeleteFavorite")]
        public async Task<ObjectResult> DeleteFavorite([FromBody] User item)
        {

            Models.DB.DBListofFavs dBUserItemsDBFind = JsonConvert.DeserializeObject<Models.DB.DBListofFavs>(System.IO.File.ReadAllText(path));

            bool contains = false;
            int indx = 0;

            for (int j = 0; j < dBUserItemsDBFind.dbresults.Count; j++)
            {
                if (dBUserItemsDBFind.dbresults[j].id == item.Id)
                {
                    indx = j;
                    contains = true;
                    break;
                }
            }

            if (contains == false)
            {
                return (new ObjectResult("BAD"));
            }
            else
            {
                bool TrackContains = false;
                List<SearchTrackMain> userItemadding = dBUserItemsDBFind.dbresults[indx].jsresults;
                dBUserItemsDBFind.dbresults[indx].jsresults = (userItemadding);
                int IndexOfTrack=0;

                for (int k = 0; k < dBUserItemsDBFind.dbresults[indx].jsresults.Count; k++)
                {
                    if (dBUserItemsDBFind.dbresults[indx].jsresults[k].name == item.Name)
                    {
                        IndexOfTrack = k;
                        TrackContains = true;
                    }
                }

                if (TrackContains == true)
                {
                    dBUserItemsDBFind.dbresults[indx].jsresults.RemoveAt(IndexOfTrack);
                    System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(dBUserItemsDBFind));
                    return (new ObjectResult("OK"));
                }
                else
                {
                    return (new ObjectResult("BAD"));
                }
            }
        
        }
    }
}

