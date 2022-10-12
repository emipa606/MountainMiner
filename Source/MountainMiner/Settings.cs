using Verse;

namespace MountainMiner;

/// <summary>
///     Definition of the settings for the mod
/// </summary>
internal class MountainMinerSettings : ModSettings
{
    public float ChunkMultiplier = 1f;

    /// <summary>
    ///     Saving and loading the values
    /// </summary>
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref ChunkMultiplier, "ChunkMultiplier", 1f);
    }
}