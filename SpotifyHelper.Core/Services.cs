using SpotifyAPI.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyHelper.Core
{
    public class Services
    {
        public record PlaylistDTO
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

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

        public async Task<IEnumerable<PlaylistDTO>> GetPlaylists()
        {
            var requestResult = await _client.PaginateAll(await _client.Playlists.CurrentUsers());

            return requestResult
                .Select(x => new PlaylistDTO() with { Id = x.Id, Name = x.Name });
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
                if (await ExistsAsync(playlistId, track))
                {
                    return AddCurrentlyPlayingToPlaylistResponse.TrackAlreadyExists;
                }

                var currentlyPlaying = new List<string>() { track.Uri };

                await _client.Playlists.AddItems(playlistId, new PlaylistAddItemsRequest(currentlyPlaying));
            }

            return AddCurrentlyPlayingToPlaylistResponse.Ok;
        }

        private async Task<bool> ExistsAsync(string playlistId, FullTrack track)
        {
            int? offset = 0;

            while (offset.HasValue)
            {
                var request = new PlaylistGetItemsRequest(PlaylistGetItemsRequest.AdditionalTypes.Track)
                {
                    Limit = 35,
                    Offset = offset
                };

                request.Fields.Add("items(track(id, type))");

                var tracks = await _client.PaginateAll(await _client.Playlists.GetItems(playlistId, request));

                if (tracks.Any(item => ((FullTrack)item.Track).Id == track.Id))
                {
                    return true;
                }

                offset = tracks.Count > 0
                    ? offset + tracks.Count
                    : null;
            }

            return false;
        }
    }
}
