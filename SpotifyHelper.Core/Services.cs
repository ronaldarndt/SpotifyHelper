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
        const int OFFSET = 25;

        var request = new PlaylistGetItemsRequest(PlaylistGetItemsRequest.AdditionalTypes.Track)
        {
            Limit = OFFSET,
            Offset = 0
        };
        request.Fields.Add("items(track(id, type)),total");

        Paging<PlaylistTrack<IPlayableItem>>? page = null;

        do
        {
            page = await m_client.Playlists.GetItems(playlistId, request);

            if (await m_client.Paginate(page).Any(x => x.Track is FullTrack item && item.Id == track.Id))
            {
                return true;
            }

            request.Offset += OFFSET;
        } while (request.Offset < page.Total);

        return false;
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
