using UnityEngine;
using Verse;

namespace MountainMiner
{
    [StaticConstructorOnStartup]
    internal class MountainMinerMod : Mod
    {
        /// <summary>
        ///     The private settings
        /// </summary>
        private MountainMinerSettings settings;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="content"></param>
        public MountainMinerMod(ModContentPack content) : base(content)
        {
        }

        /// <summary>
        ///     The instance-settings for the mod
        /// </summary>
        private MountainMinerSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = GetSettings<MountainMinerSettings>();
                }

                return settings;
            }
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
            var listing_Standard = new Listing_Standard();
            listing_Standard.Begin(rect);
            listing_Standard.Gap();
            listing_Standard.Label("Time between chunks multiplier: " + Settings.ChunkMultiplier, -1,
                "The value for how long time between chunks are spawned by the miner is multiplied with this value");
            Settings.ChunkMultiplier = Widgets.HorizontalSlider(listing_Standard.GetRect(20), Settings.ChunkMultiplier,
                0.1f, 10f, false, "Chunk spawntime multiplier", null, null, 0.1f);
            listing_Standard.End();
            Settings.Write();
        }
    }
}