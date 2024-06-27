// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Mania.Beatmaps;
using osu.Game.Rulesets.Mania.Skinning.Legacy;
using osu.Game.Rulesets.Mania.UI;
using osu.Game.Skinning;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Mania.Skinning.KisekiWhite
{
    public partial class KisekiWhiteStageBackground : CompositeDrawable
    {
        private Drawable background = null!;

        public KisekiWhiteStageBackground()
        {
            RelativeSizeAxes = Axes.Both;
            //AutoSizeAxes = Axes.X;
        }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            InternalChildren = new[]
            {
                background = new Sprite
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Texture = textures.Get(@"Skin/KisekiWhite/Background")
                },
                
            };

            
        }

        protected override void Update()
        {
            base.Update();

            if (background?.Height > 0)
                background.Scale = new Vector2(DrawHeight / background.Height);

        }

    }
}

