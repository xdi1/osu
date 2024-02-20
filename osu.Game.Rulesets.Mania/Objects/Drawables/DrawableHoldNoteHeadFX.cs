// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Mania.Objects.Drawables
{
    /// <summary>
    /// The head of a <see cref="DrawableHoldNote"/>.
    /// </summary>
    public partial class DrawableHoldNoteHeadFX : DrawableHoldNoteHead
    {
        protected override ManiaSkinComponents Component => ManiaSkinComponents.HoldNoteHeadFX;
    }
}
