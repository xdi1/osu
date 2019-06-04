﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Platform;
using osu.Framework.Testing;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Tests.Visual
{
    public abstract class OsuTestScene : TestScene
    {
        [Cached(typeof(Bindable<WorkingBeatmap>))]
        [Cached(typeof(IBindable<WorkingBeatmap>))]
        private readonly OsuTestBeatmap beatmap = new OsuTestBeatmap(new DummyWorkingBeatmap());

        protected BindableBeatmap Beatmap => beatmap;

        [Cached]
        [Cached(typeof(IBindable<RulesetInfo>))]
        protected readonly Bindable<RulesetInfo> Ruleset = new Bindable<RulesetInfo>();

        [Cached]
        [Cached(Type = typeof(IBindable<IReadOnlyList<Mod>>))]
        protected readonly Bindable<IReadOnlyList<Mod>> Mods = new Bindable<IReadOnlyList<Mod>>(Array.Empty<Mod>());

        protected new DependencyContainer Dependencies { get; private set; }

        private readonly Lazy<Storage> localStorage;
        protected Storage LocalStorage => localStorage.Value;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            // This is the earliest we can get OsuGameBase, which is used by the dummy working beatmap to find textures
            beatmap.Default = new DummyWorkingBeatmap(parent.Get<OsuGameBase>());

            return Dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
        }

        protected OsuTestScene()
        {
            localStorage = new Lazy<Storage>(() => new NativeStorage($"{GetType().Name}-{Guid.NewGuid()}"));
        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audioManager, RulesetStore rulesets)
        {
            beatmap.SetAudioManager(audioManager);

            Ruleset.Value = rulesets.AvailableRulesets.First();
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (beatmap?.Value.TrackLoaded == true)
                beatmap.Value.Track.Stop();

            if (localStorage.IsValueCreated)
            {
                try
                {
                    localStorage.Value.DeleteDirectory(".");
                }
                catch
                {
                    // we don't really care if this fails; it will just leave folders lying around from test runs.
                }
            }
        }

        protected override ITestSceneTestRunner CreateRunner() => new OsuTestSceneTestRunner();

        public class OsuTestSceneTestRunner : OsuGameBase, ITestSceneTestRunner
        {
            private TestSceneTestRunner.TestRunner runner;

            protected override void LoadAsyncComplete()
            {
                // this has to be run here rather than LoadComplete because
                // TestScene.cs is checking the IsLoaded state (on another thread) and expects
                // the runner to be loaded at that point.
                Add(runner = new TestSceneTestRunner.TestRunner());
            }

            public void RunTestBlocking(TestScene test) => runner.RunTestBlocking(test);
        }

        private class OsuTestBeatmap : BindableBeatmap
        {
            public OsuTestBeatmap(WorkingBeatmap defaultValue)
                : base(defaultValue)
            {
            }

            public void SetAudioManager(AudioManager audioManager) => RegisterAudioManager(audioManager);

            public override BindableBeatmap GetBoundCopy()
            {
                var copy = new OsuTestBeatmap(Default);
                copy.BindTo(this);
                return copy;
            }
        }
    }
}
