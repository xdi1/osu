﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using System.Collections.Generic;
using osu.Framework.Configuration;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Scoring
{
    public abstract class ScoreProcessor
    {
        /// <summary>
        /// Invoked when the ScoreProcessor is in a failed state.
        /// Return true if the fail was permitted.
        /// </summary>
        public event Func<bool> Failed;

        /// <summary>
        /// Invoked when a new judgement has occurred. This occurs after the judgement has been processed by the <see cref="ScoreProcessor"/>.
        /// </summary>
        public event Action<Judgement> NewJudgement;

        /// <summary>
        /// The current total score.
        /// </summary>
        public readonly BindableDouble TotalScore = new BindableDouble { MinValue = 0 };

        /// <summary>
        /// The current accuracy.
        /// </summary>
        public readonly BindableDouble Accuracy = new BindableDouble(1) { MinValue = 0, MaxValue = 1 };

        /// <summary>
        /// The current health.
        /// </summary>
        public readonly BindableDouble Health = new BindableDouble { MinValue = 0, MaxValue = 1 };

        /// <summary>
        /// The current combo.
        /// </summary>
        public readonly BindableInt Combo = new BindableInt();

        /// <summary>
        /// THe highest combo achieved by this score.
        /// </summary>
        public readonly BindableInt HighestCombo = new BindableInt();

        /// <summary>
        /// Whether the score is in a failed state.
        /// </summary>
        public virtual bool HasFailed => Health.Value == Health.MinValue;

        /// <summary>
        /// Whether this ScoreProcessor has already triggered the failed state.
        /// </summary>
        private bool alreadyFailed;

        protected ScoreProcessor()
        {
            Combo.ValueChanged += delegate { HighestCombo.Value = Math.Max(HighestCombo.Value, Combo.Value); };

            Reset();
        }

        private ScoreRank rankFrom(double acc)
        {
            if (acc == 1)
                return ScoreRank.X;
            if (acc > 0.95)
                return ScoreRank.S;
            if (acc > 0.9)
                return ScoreRank.A;
            if (acc > 0.8)
                return ScoreRank.B;
            if (acc > 0.7)
                return ScoreRank.C;
            return ScoreRank.D;
        }

        /// <summary>
        /// Resets this ScoreProcessor to a default state.
        /// </summary>
        protected virtual void Reset()
        {
            TotalScore.Value = 0;
            Accuracy.Value = 1;
            Health.Value = 1;
            Combo.Value = 0;
            HighestCombo.Value = 0;

            alreadyFailed = false;
        }

        /// <summary>
        /// Checks if the score is in a failed state and notifies subscribers.
        /// <para>
        /// This can only ever notify subscribers once.
        /// </para>
        /// </summary>
        protected void UpdateFailed()
        {
            if (alreadyFailed || !HasFailed)
                return;

            if (Failed?.Invoke() != false)
                alreadyFailed = true;
        }

        /// <summary>
        /// Notifies subscribers of <see cref="NewJudgement"/> that a new judgement has occurred.
        /// </summary>
        /// <param name="judgement">The judgement to notify subscribers of.</param>
        protected void NotifyNewJudgement(Judgement judgement)
        {
            NewJudgement?.Invoke(judgement);
        }

        /// <summary>
        /// Retrieve a score populated with data for the current play this processor is responsible for.
        /// </summary>
        public virtual void PopulateScore(Score score)
        {
            score.TotalScore = TotalScore;
            score.Combo = Combo;
            score.MaxCombo = HighestCombo;
            score.Accuracy = Accuracy;
            score.Rank = rankFrom(Accuracy);
            score.Date = DateTimeOffset.Now;
            score.Health = Health;
        }
    }

    public abstract class ScoreProcessor<TObject> : ScoreProcessor
        where TObject : HitObject
    {
        private const double max_score = 1000000;

        /// <summary>
        /// All judgements held by this ScoreProcessor.
        /// </summary>
        protected readonly List<Judgement> Judgements = new List<Judgement>();

        protected virtual double ComboPortion => 0.5f;
        protected virtual double AccuracyPortion => 0.5f;

        protected int MaxHits { get; private set; }
        protected int Hits { get; private set; }

        private double maxComboScore;
        private double comboScore;

        protected ScoreProcessor()
        {
        }

        protected ScoreProcessor(RulesetContainer<TObject> rulesetContainer)
        {
            Judgements.Capacity = rulesetContainer.Beatmap.HitObjects.Count;

            rulesetContainer.OnJudgement += AddJudgement;

            ComputeTargets(rulesetContainer.Beatmap);

            maxComboScore = comboScore;
            MaxHits = Hits;

            Reset();
        }

        /// <summary>
        /// Computes target scoring values for this ScoreProcessor. This is equivalent to performing an auto-play of the score to find the values.
        /// </summary>
        /// <param name="beatmap">The Beatmap containing the objects that will be judged by this ScoreProcessor.</param>
        protected virtual void ComputeTargets(Beatmap<TObject> beatmap) { }

        /// <summary>
        /// Adds a judgement to this ScoreProcessor.
        /// </summary>
        /// <param name="judgement">The judgement to add.</param>
        protected void AddJudgement(Judgement judgement)
        {
            if (judgement.AffectsCombo)
            {
                switch (judgement.Result)
                {
                    case HitResult.Miss:
                        Combo.Value = 0;
                        break;
                    default:
                        Combo.Value++;
                        break;
                }
            }

            Judgements.Add(judgement);
            OnNewJudgement(judgement);
            NotifyNewJudgement(judgement);

            UpdateFailed();
        }

        protected virtual void OnNewJudgement(Judgement judgement)
        {
            double bonusScore = 0;

            if (judgement.AffectsCombo)
            {
                switch (judgement.Result)
                {
                    case HitResult.None:
                        break;
                    case HitResult.Miss:
                        Combo.Value = 0;
                        break;
                    default:
                        Combo.Value++;
                        break;
                }

                comboScore += judgement.NumericResult;
            }
            else if (judgement.IsHit)
                bonusScore += judgement.NumericResult;

            if (judgement.AffectsAccuracy && judgement.IsHit)
                Hits++;

            TotalScore.Value =
                max_score * (ComboPortion * comboScore / maxComboScore
                                + AccuracyPortion * Hits / MaxHits)
                + bonusScore;
        }

        protected override void Reset()
        {
            base.Reset();

            Judgements.Clear();

            Hits = 0;
            comboScore = 0;
        }
    }
}
