using Mlie;
using UnityEngine;
using Verse;

namespace MountainMiner;

[StaticConstructorOnStartup]
internal class MountainMinerMod : Mod
{
    private static string currentVersion;

    /// <summary>
    ///     The private settings
    /// </summary>
    private readonly MountainMinerSettings settings;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="content"></param>
    public MountainMinerMod(ModContentPack content) : base(content)
    {
        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
        settings = GetSettings<MountainMinerSettings>();
    }

    /// <summary>
    ///     The title for the mod-settings
    /// </summary>
    /// <returns></returns>
    public override string SettingsCategory()
    {
        return "Mountain Miner";
    }

    /// <summary>
    ///     The settings-window
    ///     For more info: https://rimworldwiki.com/wiki/Modding_Tutorials/ModSettings
    /// </summary>
    /// <param name="rect"></param>
    public override void DoSettingsWindowContents(Rect rect)
    {
        var listingStandard = new Listing_Standard();
        listingStandard.Begin(rect);
        listingStandard.Gap();
        listingStandard.Label("MM.TimeBetween".Translate(settings.ChunkMultiplier), -1,
            "MM.TimeBetweenTooltip".Translate());
        settings.ChunkMultiplier = Widgets.HorizontalSlider(listingStandard.GetRect(20),
            settings.ChunkMultiplier,
            0.1f, 10f, false, "MM.SpawnTime".Translate(), null, null, 0.1f);
        if (currentVersion != null)
        {
            listingStandard.Gap();
            GUI.contentColor = Color.gray;
            listingStandard.Label("MM.CurrentModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listingStandard.End();
        settings.Write();
    }
}