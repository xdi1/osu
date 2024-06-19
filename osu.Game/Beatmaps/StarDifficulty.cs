// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using JetBrains.Annotations;
using osu.Framework.Utils;
using osu.Game.Rulesets.Difficulty;

namespace osu.Game.Beatmaps
{
    public readonly struct StarDifficulty
    {
        /// <summary>
        /// The star difficulty rating for the given beatmap.
        /// </summary>
        public readonly double Stars;

        /// <summary>
        /// The maximum combo achievable on the given beatmap.
        /// </summary>
        public readonly int MaxCombo;

        public readonly string difficultyName = "";

        /// <summary>
        /// The difficulty attributes computed for the given beatmap.
        /// Might not be available if the star difficulty is associated with a beatmap that's not locally available.
        /// </summary>
        [CanBeNull]
        public readonly DifficultyAttributes Attributes;

        /// <summary>
        /// Creates a <see cref="StarDifficulty"/> structure based on <see cref="DifficultyAttributes"/> computed
        /// by a <see cref="DifficultyCalculator"/>.
        /// </summary>
        public StarDifficulty([NotNull] DifficultyAttributes attributes)
        {
            Stars = double.IsFinite(attributes.StarRating) ? attributes.StarRating : 0;
            MaxCombo = attributes.MaxCombo;
            Attributes = attributes;
            // Todo: Add more members (BeatmapInfo.DifficultyRating? Attributes? Etc...)
        }

        public StarDifficulty(string difficulty, [NotNull] DifficultyAttributes attributes)
        {
            Stars = double.IsFinite(attributes.StarRating) ? attributes.StarRating : 0;
            MaxCombo = attributes.MaxCombo;
            Attributes = attributes;
            // Todo: Add more members (BeatmapInfo.DifficultyRating? Attributes? Etc...)
        }

        /// <summary>
        /// Creates a <see cref="StarDifficulty"/> structure with a pre-populated star difficulty and max combo
        /// in scenarios where computing <see cref="DifficultyAttributes"/> is not feasible (i.e. when working with online sources).
        /// </summary>
        public StarDifficulty(double starDifficulty, int maxCombo)
        {
            Stars = double.IsFinite(starDifficulty) ? starDifficulty : 0;
            MaxCombo = maxCombo;
            Attributes = null;
        }

        /// <summary>
        /// Creates a <see cref="StarDifficulty"/> structure with a pre-populated star difficulty and max combo
        /// in scenarios where computing <see cref="DifficultyAttributes"/> is not feasible (i.e. when working with online sources).
        /// </summary>
        public StarDifficulty(string difficulty, double starDifficulty, int maxCombo)
        {
            Stars = double.IsFinite(starDifficulty) ? starDifficulty : 0;
            difficultyName = difficulty;
            MaxCombo = maxCombo;
            Attributes = null;
        }

        public DifficultyRating DifficultyRating => GetDifficultyRating(Stars);

        /// <summary>
        /// Retrieves the <see cref="DifficultyRating"/> that describes a star rating.
        /// </summary>
        /// <remarks>
        /// For more information, see: https://osu.ppy.sh/help/wiki/Difficulties
        /// </remarks>
        /// <param name="starRating">The star rating.</param>
        /// <returns>The <see cref="DifficultyRating"/> that best describes <paramref name="starRating"/>.</returns>
        public static DifficultyRating GetDifficultyRating(double starRating)
        {
            if (Precision.AlmostBigger(starRating, 15.0, 0.005))
                return DifficultyRating.ExpertPlus;

            if (Precision.AlmostBigger(starRating, 12.0, 0.005))
                return DifficultyRating.Expert;

            if (Precision.AlmostBigger(starRating, 9.0, 0.005))
                return DifficultyRating.Insane;

            if (Precision.AlmostBigger(starRating, 6.0, 0.005))
                return DifficultyRating.Hard;

            if (Precision.AlmostBigger(starRating, 3.0, 0.005))
                return DifficultyRating.Normal;

            return DifficultyRating.Easy;
        }
    }
}
