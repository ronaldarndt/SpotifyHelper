using SpotifyAPI.Web;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotifyHelper.Core
{
    public class Services
    {
        public enum AddCurrentlyPlayingToPlaylistResponse : byte
        {
            Ok,
            PlaylistNotFound,
            TrackAlreadyExists
        }

        private readonly SpotifyClient _client;

        public Services(SpotifyClient client)
        {
            _client = client;
        }

        public async Task<IList<SimplePlaylist>> GetPlaylists()
        {
            return await _client.PaginateAll(await _client.Playlists.CurrentUsers());
        }

        public async Task<AddCurrentlyPlayingToPlaylistResponse> AddCurrentlyPlayingToPlaylist(string playlistId)
        {
            if (string.IsNullOrEmpty(playlistId))
            {
                return AddCurrentlyPlayingToPlaylistResponse.PlaylistNotFound;
            }

            var playlistRequest = new PlaylistGetRequest();
            playlistRequest.Fields.Add("id");

            try
            {
                await _client.Playlists.Get(playlistId, playlistRequest);
            }
            catch (APIException)
            {
                return AddCurrentlyPlayingToPlaylistResponse.PlaylistNotFound;
            }

            var playback = await _client.Player.GetCurrentPlayback();

            if (playback != null && playback.IsPlaying && playback.Item is FullTrack track)
            {
                if (await Exists(playlistId, track))
                {
                    return AddCurrentlyPlayingToPlaylistResponse.TrackAlreadyExists;
                }

                var currentlyPlaying = new List<string>() { track.Uri };

                await _client.Playlists.AddItems(playlistId, new PlaylistAddItemsRequest(currentlyPlaying));
            }

            return AddCurrentlyPlayingToPlaylistResponse.Ok;
        }

        private async Task<bool> Exists(string playlistId, FullTrack track)
        {
            var request = new PlaylistGetItemsRequest(PlaylistGetItemsRequest.AdditionalTypes.Track);
            request.Fields.Add("items(track(id, type))");
            request.Limit = 25;

            var firstPage = await _client.Playlists.GetItems(playlistId, request);

            await foreach (var item in _client.Paginate(firstPage, new SimplePaginator()))
            {
                if (((FullTrack)item.Track).Id == track.Id)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
