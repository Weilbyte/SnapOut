namespace SnapOut
{
    using Verse;
    using Multiplayer.API;

    [StaticConstructorOnStartup]
    public static class SnapOutMultiplayer
    {
        static SnapOutMultiplayer()
        {
            if (!MP.enabled) return;
            MP.RegisterAll();
        }
    }

}
