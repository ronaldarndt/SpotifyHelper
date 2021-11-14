using SpotifyAPI.Web;
using SpotifyHelper.Core.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotifyHelper.Core;

public class Services
{
    private readonly SpotifyClient m_client;

    public Services(SpotifyClient client)
    {
        m_client = client;
    }

    public async Task<IList<SimplePlaylist>> GetPlaylists()
    {
        return await m_client.PaginateAll(await m_client.Playlists.CurrentUsers());
    }

    public async Task AddCurrentlyPlayingToPlaylist(string playlistId)
    {
        if (!await PlaylistExistsAsync(playlistId))
        {
            return;
        }

        var playback = await m_client.Player.GetCurrentPlayback();

        if (playback is null or { IsPlaying: false } || playback.Item is not FullTrack track || await TrackExistsInPlaylistAsync(playlistId, track))
        {
            return;
        }

        var currentlyPlaying = new List<string>() { track.Uri };

        await m_client.Playlists.AddItems(playlistId, new(currentlyPlaying));
    }

    private async Task<bool> TrackExistsInPlaylistAsync(string playlistId, FullTrack track)
    {
        var request = new PlaylistGetItemsRequest(PlaylistGetItemsRequest.AdditionalTypes.Track);
        request.Fields.Add("items(track(id, type))");
        request.Limit = 25;

        var firstPage = await m_client.Playlists.GetItems(playlistId, request);

        return await m_client.Paginate(firstPage, new SimplePaginator())
            .Any(x => x.Track is FullTrack item && item.Id == track.Id);
    }

    private async Task<bool> PlaylistExistsAsync(string? playlistId)
    {
        if (string.IsNullOrWhiteSpace(playlistId))
        {
            return false;
        }

        var playlistRequest = new PlaylistGetRequest();
        playlistRequest.Fields.Add("id");

        try
        {
            await m_client.Playlists.Get(playlistId, playlistRequest);

            return true;
        }
        catch (APIException)
        {
            return false;
        }
    }
}
