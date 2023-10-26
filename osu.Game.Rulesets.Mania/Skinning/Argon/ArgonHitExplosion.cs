// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Mania.UI;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Mania.Skinning.Argon
{
    public partial class ArgonHitExplosion : CompositeDrawable, IHitExplosion
    {
        public override bool RemoveWhenNotAlive => true;

        [Resolved]
        private Column column { get; set; } = null!;

        private readonly IBindable<ScrollingDirection> direction = new Bindable<ScrollingDirection>();

        private Container largeFaint = null!;

        private Bindable<Color4> accentColour = null!;

        public ArgonHitExplosion()
        {
            Origin = Anchor.Centre;

            RelativeSizeAxes = Axes.X;
            Height = ArgonNotePiece.NOTE_HEIGHT;
        }

        private RingExplosion? ringExplosion;

        [BackgroundDependencyLoader]
        private void load(IScrollingInfo scrollingInfo)
        {
            
            InternalChildren = new Drawable[]
            {
                largeFaint = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Height = ArgonNotePiece.NOTE_ACCENT_RATIO,
                    Masking = true,
                    CornerRadius = ArgonNotePiece.CORNER_RADIUS,
                    Blending = BlendingParameters.Additive,
                    Child = new Box
                    {
                        Colour = Color4.White,
                        RelativeSizeAxes = Axes.Both,
                    },
                },
                ringExplosion = new RingExplosion()
                {
                    Colour = Color4.White,
                }
            };

            direction.BindTo(scrollingInfo.Direction);
            direction.BindValueChanged(onDirectionChanged, true);

            accentColour = column.AccentColour.GetBoundCopy();
            accentColour.BindValueChanged(colour =>
            {
                largeFaint.Colour = Interpolation.ValueAt(0.8f, colour.NewValue, Color4.White, 0, 1);

                largeFaint.EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Glow,
                    Colour = colour.NewValue,
                    Roundness = 40,
                    Radius = 60,
                };
            }, true);
        }

        private void onDirectionChanged(ValueChangedEvent<ScrollingDirection> direction)
        {
            if (direction.NewValue == ScrollingDirection.Up)
            {
                Anchor = Anchor.TopCentre;
                largeFaint.Anchor = Anchor.TopCentre;
                largeFaint.Origin = Anchor.TopCentre;
                Y = ArgonNotePiece.NOTE_HEIGHT / 2;
            }
            else
            {
                Anchor = Anchor.BottomCentre;
                largeFaint.Anchor = Anchor.BottomCentre;
                largeFaint.Origin = Anchor.BottomCentre;
                Y = -ArgonNotePiece.NOTE_HEIGHT / 2;
            }
        }

        public void Animate(JudgementResult result)
        {
            this.FadeOutFromOne(PoolableHitExplosion.DURATION, Easing.Out);
            ringExplosion?.PlayAnimation();
        }

        private partial class RingExplosion : CompositeDrawable
        {
            private readonly float travel = 150;

            public RingExplosion()
            {
                const float thickness = 4;

                const float small_size = 9;
                const float large_size = 14;

                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;

                Blending = BlendingParameters.Additive;

                int countSmall = 0;
                int countLarge = 0;

                countSmall = 4;
                countLarge = 4;

                for (int i = 0; i < countSmall; i++)
                    AddInternal(new RingPiece(thickness) { Size = new Vector2(small_size) });

                for (int i = 0; i < countLarge; i++)
                    AddInternal(new RingPiece(thickness) { Size = new Vector2(large_size) });
            }

            public void PlayAnimation()
            {
                foreach (var c in InternalChildren)
                {
                    const float start_position_ratio = 0.3f;

                    float direction = RNG.NextSingle(0, 360);
                    float distance = RNG.NextSingle(travel / 2, travel);

                    c.MoveTo(new Vector2(
                        MathF.Cos(direction) * distance * start_position_ratio,
                        MathF.Sin(direction) * distance * start_position_ratio
                    ));

                    c.MoveTo(new Vector2(
                        MathF.Cos(direction) * distance,
                        MathF.Sin(direction) * distance
                    ), 600, Easing.OutQuint);
                }

                this.FadeOutFromOne(1000, Easing.OutQuint);
            }

            public partial class RingPiece : CircularContainer
            {
                public RingPiece(float thickness = 9)
                {
                    Anchor = Anchor.Centre;
                    Origin = Anchor.Centre;

                    Masking = true;
                    BorderThickness = thickness;
                    BorderColour = Color4.White;

                    Child = new Box
                    {
                        AlwaysPresent = true,
                        Alpha = 0,
                        RelativeSizeAxes = Axes.Both
                    };
                }
            }
        }
    }
}
