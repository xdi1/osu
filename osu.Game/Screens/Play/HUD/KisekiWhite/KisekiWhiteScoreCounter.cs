// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Extensions.Color4Extensions;
using osu.Game.Graphics;
using osu.Game.Skinning;

namespace osu.Game.Screens.Play.HUD
{
    public partial class KisekiWhiteScoreCounter : GameplayScoreCounter, ISerialisableDrawable
    {
        protected override double RollingDuration => 0;
        public KisekiWhiteScoreCounter()
        {
            Anchor = Anchor.TopCentre;
            Origin = Anchor.TopCentre;
        }

        public bool UsesFixedAnchor { get; set; }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            Colour = ColourInfo.GradientVertical(Color4Extensions.FromHex(@"015eea"), Color4Extensions.FromHex(@"00c0fa"));
        }
    }
}
