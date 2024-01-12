// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mania.Beatmaps;
using osu.Game.Rulesets.Scoring;
using osu.Game.Skinning;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Mania.Skinning.Argon
{
    public class ManiaArgonFXSkinTransformer : SkinTransformer
    {
        private readonly ManiaBeatmap beatmap;

        public ManiaArgonFXSkinTransformer(ISkin skin, IBeatmap beatmap)
            : base(skin)
        {
            this.beatmap = (ManiaBeatmap)beatmap;
        }

        public override Drawable? GetDrawableComponent(ISkinComponentLookup lookup)
        {
            switch (lookup)
            {
                case GameplaySkinComponentLookup<HitResult> resultComponent:
                    // This should eventually be moved to a skin setting, when supported.
                    if (Skin is ArgonProSkin && resultComponent.Component >= HitResult.Great)
                        return Drawable.Empty();

                    return new ArgonJudgementPiece(resultComponent.Component);

                case ManiaSkinComponentLookup maniaComponent:
                    // TODO: Once everything is finalised, consider throwing UnsupportedSkinComponentException on missing entries.
                    switch (maniaComponent.Component)
                    {
                        case ManiaSkinComponents.StageBackground:
                            return new ArgonStageBackground();

                        case ManiaSkinComponents.ColumnBackground:
                            return new ArgonColumnBackground();

                        case ManiaSkinComponents.HoldNoteBody:
                            return new ArgonHoldBodyPiece();

                        case ManiaSkinComponents.HoldNoteTail:
                            return new ArgonHoldNoteTailPiece();

                        case ManiaSkinComponents.HoldNoteHead:
                            return new ArgonHoldNoteHeadPiece();

                        case ManiaSkinComponents.Note:
                            return new ArgonNotePiece();

                        case ManiaSkinComponents.HitTarget:
                            return new ArgonHitTarget();

                        case ManiaSkinComponents.KeyArea:
                            return new ArgonKeyArea();

                        case ManiaSkinComponents.HitExplosion:
                            return new ArgonHitExplosion();
                    }

                    break;
            }

            return base.GetDrawableComponent(lookup);
        }

        private static readonly Color4 colour_special_column = new Color4(169, 106, 255, 255);

        private const int total_colours = 7;

        private static readonly Color4 colour_yellow = new Color4(255, 197, 40, 255);
        private static readonly Color4 colour_orange = new Color4(255, 173, 5, 255);
        private static readonly Color4 colour_pink = new Color4(226, 0, 125, 255);
        private static readonly Color4 colour_purple = new Color4(203, 60, 236, 255);
        private static readonly Color4 colour_cyan = new Color4(68, 186, 254, 255);
        private static readonly Color4 colour_green = new Color4(100, 192, 92, 255);
        private static readonly Color4 colour_white = new Color4(255, 255, 255, 255);

        public override IBindable<TValue>? GetConfig<TLookup, TValue>(TLookup lookup)
        {
            if (lookup is ManiaSkinConfigurationLookup maniaLookup)
            {
                int columnIndex = maniaLookup.ColumnIndex ?? 0;
                var stage = beatmap.GetStageForColumnIndex(columnIndex);
                int columnCount = stage.Columns;

                switch (maniaLookup.Lookup)
                {
                    case LegacyManiaSkinConfigurationLookups.ColumnSpacing:
                        if (columnCount == 6)
                        {
                            switch(columnIndex)
                            {
                                case 0:
                                case 1:
                                    return SkinUtils.As<TValue>(new Bindable<float>(0));
                                case 2:
                                    return SkinUtils.As<TValue>(new Bindable<float>(93));
                                case 4:
                                    return SkinUtils.As<TValue>(new Bindable<float>(188));
                                default:
                                    return SkinUtils.As<TValue>(new Bindable<float>(2));
                            }
                        } else if (columnCount == 7)
                        {
                            switch(columnIndex)
                            {
                                case 0:
                                case 1:
                                    return SkinUtils.As<TValue>(new Bindable<float>(0));
                                case 2:
                                    return SkinUtils.As<TValue>(new Bindable<float>(74));
                                case 5:
                                    return SkinUtils.As<TValue>(new Bindable<float>(226));
                                default:
                                    return SkinUtils.As<TValue>(new Bindable<float>(2));
                            }
                        } else if (columnCount == 8)
                        {
                            switch(columnIndex)
                            {
                                case 0:
                                case 1:
                                    return SkinUtils.As<TValue>(new Bindable<float>(0));
                                case 2:
                                    return SkinUtils.As<TValue>(new Bindable<float>(62));
                                case 6:
                                    return SkinUtils.As<TValue>(new Bindable<float>(250));
                                default:
                                    return SkinUtils.As<TValue>(new Bindable<float>(2));
                            }
                        } else {
                            switch(columnIndex)
                            {
                                case 0:
                                    return SkinUtils.As<TValue>(new Bindable<float>(0));
                                default:
                                    return SkinUtils.As<TValue>(new Bindable<float>(2));
                            }
                        }
                    case LegacyManiaSkinConfigurationLookups.StagePaddingBottom:
                        return SkinUtils.As<TValue>(new Bindable<float>(60));
                    case LegacyManiaSkinConfigurationLookups.StagePaddingTop:
                        return SkinUtils.As<TValue>(new Bindable<float>(30));
                    case LegacyManiaSkinConfigurationLookups.ColumnWidth:
                        if (columnCount >= 6 | columnCount <= 8) 
                        {
                            if (columnIndex == 0 || (columnIndex == columnCount - 1))
                            {
                                if (columnCount <= 4)
                                {
                                    return SkinUtils.As<TValue>(new Bindable<float>(stage.IsSpecialColumn(columnIndex) ? 120 : 91));
                                }
                                else if (columnCount == 5)
                                {
                                    return SkinUtils.As<TValue>(new Bindable<float>(stage.IsSpecialColumn(columnIndex) ? 74 : 72));
                                }
                                return SkinUtils.As<TValue>(new Bindable<float>(184));
                            }
                            else
                            {
                                if (columnCount == 6 || columnCount == 4)
                                {
                                    return SkinUtils.As<TValue>(new Bindable<float>(stage.IsSpecialColumn(columnIndex) ? 120 : 91));
                                }
                                else if (columnCount == 7 || columnCount == 5)
                                {
                                    return SkinUtils.As<TValue>(new Bindable<float>(stage.IsSpecialColumn(columnIndex) ? 74 : 72));
                                }
                                else 
                                {
                                    return SkinUtils.As<TValue>(new Bindable<float>(stage.IsSpecialColumn(columnIndex) ? 60 : 60));
                                }
                            }
                        } 
                        else
                        {
                            return SkinUtils.As<TValue>(new Bindable<float>(stage.IsSpecialColumn(columnIndex) ? 120 : 60));
                        }
                        
                    case LegacyManiaSkinConfigurationLookups.ColumnBackgroundColour:

                        var colour = getColourForLayout(columnIndex, stage);

                        return SkinUtils.As<TValue>(new Bindable<Color4>(colour));
                }
            }

            return base.GetConfig<TLookup, TValue>(lookup);
        }

        private Color4 getColourForLayout(int columnIndex, StageDefinition stage)
        {
            // Account for cases like dual-stage (assume that all stages have the same column count for now).
            columnIndex %= stage.Columns;

            // For now, these are defined per column count as per https://user-images.githubusercontent.com/50823728/218038463-b450f46c-ef21-4551-b133-f866be59970c.png
            // See https://github.com/ppy/osu/discussions/21996 for discussion.
            switch (stage.Columns)
            {
                case 1:
                    return colour_yellow;

                case 2:
                    switch (columnIndex)
                    {
                        case 0: return colour_white;

                        case 1: return colour_cyan;

                        default: throw new ArgumentOutOfRangeException();
                    }

                case 3:
                    switch (columnIndex)
                    {
                        case 0: return colour_white;

                        case 1: return colour_cyan;

                        case 2: return colour_white;

                        default: throw new ArgumentOutOfRangeException();
                    }

                case 4:
                    switch (columnIndex)
                    {
                        case 0: return colour_white;

                        case 1: return colour_cyan;

                        case 2: return colour_cyan;

                        case 3: return colour_white;

                        default: throw new ArgumentOutOfRangeException();
                    }

                case 5:
                    switch (columnIndex)
                    {
                        case 0: return colour_white;

                        case 1: return colour_cyan;

                        case 2: return colour_white;

                        case 3: return colour_cyan;

                        case 4: return colour_white;

                        default: throw new ArgumentOutOfRangeException();
                    }

                case 6:
                    switch (columnIndex)
                    {
                        case 0: return colour_pink;

                        case 1: return colour_white;

                        case 2: return colour_cyan;

                        case 3: return colour_cyan;

                        case 4: return colour_white;

                        case 5: return colour_pink;

                        default: throw new ArgumentOutOfRangeException();
                    }

                case 7:
                    switch (columnIndex)
                    {
                        case 0: return colour_pink;

                        case 1: return colour_white;

                        case 2: return colour_cyan;

                        case 3: return colour_white;

                        case 4: return colour_cyan;

                        case 5: return colour_white;

                        case 6: return colour_pink;

                        default: throw new ArgumentOutOfRangeException();
                    }

                case 8:
                    switch (columnIndex)
                    {
                        case 0: return colour_pink;

                        case 1: return colour_white;

                        case 2: return colour_cyan;

                        case 3: return colour_white;

                        case 4: return colour_white;

                        case 5: return colour_cyan;

                        case 6: return colour_white;

                        case 7: return colour_pink;

                        default: throw new ArgumentOutOfRangeException();
                    }

                case 9:
                    switch (columnIndex)
                    {
                        case 0: return colour_green;

                        case 1: return colour_pink;

                        case 2: return colour_white;

                        case 3: return colour_cyan;

                        case 4: return colour_white;

                        case 5: return colour_cyan;

                        case 6: return colour_white;

                        case 7: return colour_pink;

                        case 8: return colour_green;

                        default: throw new ArgumentOutOfRangeException();
                    }

                case 10:
                    switch (columnIndex)
                    {
                        case 0: return colour_green;

                        case 1: return colour_pink;

                        case 2: return colour_white;

                        case 3: return colour_cyan;

                        case 4: return colour_white;

                        case 5: return colour_white;

                        case 6: return colour_cyan;

                        case 7: return colour_white;

                        case 8: return colour_pink;

                        case 9: return colour_green;

                        default: throw new ArgumentOutOfRangeException();
                    }
            }

            // fallback for unhandled scenarios

            if (stage.IsSpecialColumn(columnIndex))
                return colour_special_column;

            switch (columnIndex % total_colours)
            {
                case 0: return colour_yellow;

                case 1: return colour_white;

                case 2: return colour_pink;

                case 3: return colour_purple;

                case 4: return colour_cyan;

                case 5: return colour_green;

                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
