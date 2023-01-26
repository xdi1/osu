// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Globalization;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;
using osu.Framework.Utils;
using osu.Game.Utils;

namespace osu.Game.Graphics.UserInterface
{
    public abstract partial class OsuSliderBar<T> : SliderBar<T>, IHasTooltip
        where T : struct, IEquatable<T>, IComparable<T>, IConvertible
    {
        private Sample sample = null!;

        private double lastSampleTime;
        private T lastSampleValue;

        public bool PlaySamplesOnAdjust { get; set; } = true;

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            sample = audio.Samples.Get(@"UI/notch-tick");
        }

        public virtual LocalisableString TooltipText { get; private set; }

        /// <summary>
        /// Maximum number of decimal digits to be displayed in the tooltip.
        /// </summary>
        private const int max_decimal_digits = 5;

        /// <summary>
        /// Whether to format the tooltip as a percentage or the actual value.
        /// </summary>
        public bool DisplayAsPercentage { get; set; }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            CurrentNumber.BindValueChanged(current => TooltipText = getTooltipText(current.NewValue), true);
        }

        protected override void OnUserChange(T value)
        {
            base.OnUserChange(value);

            playSample(value);

            TooltipText = getTooltipText(value);
        }

        private void playSample(T value)
        {
            if (!PlaySamplesOnAdjust)
                return;

            if (Clock == null || Clock.CurrentTime - lastSampleTime <= 30)
                return;

            if (value.Equals(lastSampleValue))
                return;

            lastSampleValue = value;
            lastSampleTime = Clock.CurrentTime;

            var channel = sample.GetChannel();

            channel.Frequency.Value = 0.99f + RNG.NextDouble(0.02f) + NormalizedValue * 0.2f;

            // intentionally pitched down, even when hitting max.
            if (NormalizedValue == 0 || NormalizedValue == 1)
                channel.Frequency.Value -= 0.5f;

            channel.Play();
        }

        private LocalisableString getTooltipText(T value)
        {
            if (CurrentNumber.IsInteger)
                return value.ToInt32(NumberFormatInfo.InvariantInfo).ToString("N0");

            double floatValue = value.ToDouble(NumberFormatInfo.InvariantInfo);

            if (DisplayAsPercentage)
                return floatValue.ToString("0%");

            decimal decimalPrecision = normalise(CurrentNumber.Precision.ToDecimal(NumberFormatInfo.InvariantInfo), max_decimal_digits);

            // Find the number of significant digits (we could have less than 5 after normalize())
            int significantDigits = FormatUtils.FindPrecision(decimalPrecision);

            return floatValue.ToString($"N{significantDigits}");
        }

        /// <summary>
        /// Removes all non-significant digits, keeping at most a requested number of decimal digits.
        /// </summary>
        /// <param name="d">The decimal to normalize.</param>
        /// <param name="sd">The maximum number of decimal digits to keep. The final result may have fewer decimal digits than this value.</param>
        /// <returns>The normalised decimal.</returns>
        private decimal normalise(decimal d, int sd)
            => decimal.Parse(Math.Round(d, sd).ToString(string.Concat("0.", new string('#', sd)), CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
    }
}
