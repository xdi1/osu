// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Resources.Localisation.Web;
using osu.Game.Localisation;

namespace osu.Game.Screens.Edit.Setup
{
    public partial class DifficultySection : SetupSection
    {
        protected LabelledSliderBar<float> CircleSizeSlider { get; private set; } = null!;
        protected LabelledSliderBar<float> HealthDrainSlider { get; private set; } = null!;
        protected LabelledSliderBar<float> ApproachRateSlider { get; private set; } = null!;
        protected LabelledSliderBar<float> OverallDifficultySlider { get; private set; } = null!;
        protected LabelledSliderBar<double> BaseVelocitySlider { get; private set; } = null!;
        protected LabelledSliderBar<double> TickRateSlider { get; private set; } = null!;
        protected LabelledSliderBar<float> StarRatingSlider { get; private set; } = null!;

        public override LocalisableString Title => EditorSetupStrings.DifficultyHeader;

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                CircleSizeSlider = new LabelledSliderBar<float>
                {
                    Label = BeatmapsetsStrings.ShowStatsCs,
                    FixedLabelWidth = LABEL_WIDTH,
                    Description = EditorSetupStrings.CircleSizeDescription,
                    Current = new BindableFloat(Beatmap.Difficulty.CircleSize)
                    {
                        Default = BeatmapDifficulty.DEFAULT_DIFFICULTY,
                        MinValue = 0,
                        MaxValue = 10,
                        Precision = 0.1f,
                    }
                },
                StarRatingSlider = new LabelledSliderBar<float>
                {
                    Label = BeatmapsetsStrings.ShowStatsStars,
                    FixedLabelWidth = LABEL_WIDTH,
                    Description = "",
                    Current = new BindableFloat(Beatmap.Difficulty.OverallDifficulty)
                    {
                        Default = 1,
                        MinValue = 0,
                        MaxValue = 15,
                        Precision = 1f,
                    }
                },
            };

            foreach (var item in Children.OfType<LabelledSliderBar<float>>())
                item.Current.ValueChanged += _ => updateValues();

            foreach (var item in Children.OfType<LabelledSliderBar<double>>())
                item.Current.ValueChanged += _ => updateValues();
        }

        private void updateValues()
        {
            // for now, update these on commit rather than making BeatmapMetadata bindables.
            // after switching database engines we can reconsider if switching to bindables is a good direction.
            Beatmap.Difficulty.CircleSize = CircleSizeSlider.Current.Value;
            Beatmap.Difficulty.OverallDifficulty = StarRatingSlider.Current.Value;


            Beatmap.UpdateAllHitObjects();
            Beatmap.SaveState();
        }
    }
}
