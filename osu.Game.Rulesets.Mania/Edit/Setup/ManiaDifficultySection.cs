// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Localisation;
using osu.Game.Resources.Localisation.Web;
using osu.Game.Screens.Edit;
using osu.Game.Screens.Edit.Setup;

namespace osu.Game.Rulesets.Mania.Edit.Setup
{
    public partial class ManiaDifficultySection : SetupSection
    {
        public override LocalisableString Title => EditorSetupStrings.DifficultyHeader;

        private LabelledSliderBar<float> keyCountSlider { get; set; } = null!;
        private LabelledSliderBar<float> overallDifficultySlider { get; set; } = null!;
        protected LabelledSliderBar<float> starRatingSlider { get; private set; } = null!;

        [Resolved]
        private Editor? editor { get; set; }

        [Resolved]
        private IEditorChangeHandler? changeHandler { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                keyCountSlider = new LabelledSliderBar<float>
                {
                    Label = BeatmapsetsStrings.ShowStatsCsMania,
                    FixedLabelWidth = LABEL_WIDTH,
                    Description = "The number of columns in the beatmap",
                    Current = new BindableFloat(Beatmap.Difficulty.CircleSize)
                    {
                        Default = BeatmapDifficulty.DEFAULT_DIFFICULTY,
                        MinValue = 0,
                        MaxValue = 10,
                        Precision = 1,
                    }
                },
                starRatingSlider = new LabelledSliderBar<float>
                {
                    Label = BeatmapsetsStrings.ShowStatsStars,
                    FixedLabelWidth = LABEL_WIDTH,
                    Description = "",
                    Current = new BindableFloat((float)Beatmap.BeatmapInfo.StarRating)
                    {
                        Default = 1,
                        MinValue = 0,
                        MaxValue = 15,
                        Precision = 1f,
                    }
                },
                overallDifficultySlider = new LabelledSliderBar<float>
                {
                    Label = BeatmapsetsStrings.ShowStatsAccuracy,
                    FixedLabelWidth = LABEL_WIDTH,
                    Description = EditorSetupStrings.OverallDifficultyDescription,
                    Current = new BindableFloat(Beatmap.Difficulty.OverallDifficulty)
                    {
                        Default = BeatmapDifficulty.DEFAULT_DIFFICULTY,
                        MinValue = 0,
                        MaxValue = 10,
                        Precision = 0.1f,
                    }
                },
                
            };

            keyCountSlider.Current.BindValueChanged(updateKeyCount);
            overallDifficultySlider.Current.BindValueChanged(_ => updateValues());
            starRatingSlider.Current.BindValueChanged(_ => updateValues());
        }

        private bool updatingKeyCount;

        private void updateKeyCount(ValueChangedEvent<float> keyCount)
        {
            if (updatingKeyCount) return;

            updateValues();

            if (editor == null) return;

            updatingKeyCount = true;

            editor.Reload().ContinueWith(t =>
            {
                if (!t.GetResultSafely())
                {
                    Schedule(() =>
                    {
                        changeHandler!.RestoreState(-1);
                        Beatmap.Difficulty.CircleSize = keyCountSlider.Current.Value = keyCount.OldValue;
                        updatingKeyCount = false;
                    });
                }
                else
                {
                    updatingKeyCount = false;
                }
            });
        }

        private void updateValues()
        {
            // for now, update these on commit rather than making BeatmapMetadata bindables.
            // after switching database engines we can reconsider if switching to bindables is a good direction.
            Beatmap.Difficulty.CircleSize = keyCountSlider.Current.Value;
            Beatmap.Difficulty.OverallDifficulty = overallDifficultySlider.Current.Value;
            Beatmap.BeatmapInfo.StarRating = starRatingSlider.Current.Value;

            Beatmap.UpdateAllHitObjects();
            Beatmap.SaveState();
        }
    }
}
