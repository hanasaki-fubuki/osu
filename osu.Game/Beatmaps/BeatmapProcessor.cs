﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Game.Rulesets.Objects.Types;

namespace osu.Game.Beatmaps
{
    /// <summary>
    /// Provides functionality to alter a <see cref="IBeatmap"/> after it has been converted.
    /// </summary>
    public class BeatmapProcessor : IBeatmapProcessor
    {
        public IBeatmap Beatmap { get; }

        public BeatmapProcessor(IBeatmap beatmap)
        {
            Beatmap = beatmap;
        }

        public virtual void PreProcess()
        {
            IHasComboInformation lastObj = null;

            bool isFirst = true;

            foreach (var obj in Beatmap.HitObjects.OfType<IHasComboInformation>())
            {
                if (isFirst)
                {
                    obj.NewCombo = true;

                    // first hitobject should always be marked as a new combo for sanity.
                    isFirst = false;
                }

                if (obj.NewCombo)
                {
                    obj.IndexInCurrentCombo = 0;
                    obj.ComboIndex = (lastObj?.ComboIndex ?? 0) + 1;

                    if (lastObj != null)
                        lastObj.LastInCombo = true;
                }
                else if (lastObj != null)
                {
                    obj.IndexInCurrentCombo = lastObj.IndexInCurrentCombo + 1;
                    obj.ComboIndex = lastObj.ComboIndex;
                }

                if (obj is IHasLegacyBeatmapComboOffset legacyObj)
                {
                    var lastLegacyObj = (IHasLegacyBeatmapComboOffset)lastObj;

                    if (obj.NewCombo)
                        legacyObj.LegacyBeatmapComboIndex = (lastLegacyObj?.LegacyBeatmapComboIndex ?? 0) + legacyObj.LegacyBeatmapComboOffset + 1;
                    else if (lastLegacyObj != null)
                        legacyObj.LegacyBeatmapComboIndex = lastLegacyObj.LegacyBeatmapComboIndex;
                }

                lastObj = obj;
            }
        }

        public virtual void PostProcess()
        {
        }
    }
}
