using static SpotifyHelper.UI.HotKeyManager;

namespace SpotifyHelper.UI;

public record ConfigModel(string ClientId, Keys Key, KeyModifiers KeyModifiers);

