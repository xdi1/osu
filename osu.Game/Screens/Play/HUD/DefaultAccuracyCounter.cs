// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Extensions.LocalisationExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Configuration;
using osu.Game.Localisation.SkinComponents;
using osu.Game.Resources.Localisation.Web;
using osu.Game.Skinning;
using osuTK;

namespace osu.Game.Screens.Play.HUD
{
    public partial class DefaultAccuracyCounter : GameplayAccuracyCounter, ISerialisableDrawable
    {
        protected override double RollingDuration => 0;

        [SettingSource("Wireframe opacity", "Controls the opacity of the wireframes behind the digits.")]
        public BindableFloat WireframeOpacity { get; } = new BindableFloat(0.25f)
        {
            Precision = 0.01f,
            MinValue = 0,
            MaxValue = 1,
        };

        [SettingSource(typeof(SkinnableComponentStrings), nameof(SkinnableComponentStrings.ShowLabel), nameof(SkinnableComponentStrings.ShowLabelDescription))]
        public Bindable<bool> ShowLabel { get; } = new BindableBool(true);

        public bool UsesFixedAnchor { get; set; }

        protected override IHasText CreateText() => new DefaultAccuracyTextComponent
        {
            WireframeOpacity = { BindTarget = WireframeOpacity },
            ShowLabel = { BindTarget = ShowLabel },
        };

        private partial class DefaultAccuracyTextComponent : CompositeDrawable, IHasText
        {
            private readonly DefaultCounterTextComponent wholePart;
            private readonly DefaultCounterTextComponent fractionPart;
            private readonly DefaultCounterTextComponent percentText;

            public IBindable<float> WireframeOpacity { get; } = new BindableFloat();

            public Bindable<bool> ShowLabel { get; } = new BindableBool();

            public LocalisableString Text
            {
                get => wholePart.Text;
                set
                {
                    string[] split = value.ToString().Replace("%", string.Empty).Split(".");

                    wholePart.Text = split[0];
                    fractionPart.Text = "." + split[1];
                }
            }

            public DefaultAccuracyTextComponent()
            {
                AutoSizeAxes = Axes.Both;

                InternalChild = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            AutoSizeAxes = Axes.Both,
                            Child = wholePart = new DefaultCounterTextComponent(Anchor.TopRight, BeatmapsetsStrings.ShowScoreboardHeadersAccuracy.ToUpper())
                            {
                                ShowLabel = { BindTarget = ShowLabel },
                            }
                        },
                        fractionPart = new DefaultCounterTextComponent(Anchor.TopLeft),
                        percentText = new DefaultCounterTextComponent(Anchor.TopLeft)
                        {
                            Text = @"%",
                        },
                    }
                };
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                ShowLabel.BindValueChanged(s =>
                {
                    //fractionPart.Margin = new MarginPadding { Top = 0 }; // +4 to account for the extra spaces above the digits.
                    percentText.Margin = new MarginPadding { Top = s.NewValue ? 12f : 0 };
                }, true);
            }
        }
    }
}
