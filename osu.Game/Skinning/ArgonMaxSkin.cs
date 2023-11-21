// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using JetBrains.Annotations;
using osu.Framework.Audio.Sample;
using osu.Game.Audio;
using osu.Game.Extensions;
using osu.Game.IO;

namespace osu.Game.Skinning
{
    public class ArgonMaxSkin : ArgonSkin
    {
        public new static SkinInfo CreateInfo() => new SkinInfo
        {
            ID = Skinning.SkinInfo.ARGON_MAX_SKIN,
            Name = "osu! \"argon\" max (2022)",
            Creator = "team osu!",
            Protected = true,
            InstantiationInfo = typeof(ArgonMaxSkin).GetInvariantInstantiationInfo()
        };

        public override ISample? GetSample(ISampleInfo sampleInfo)
        {
            foreach (string lookup in sampleInfo.LookupNames)
            {
                string remappedLookup = lookup.Replace(@"Gameplay/", @"Gameplay/ArgonMax/");

                var sample = Samples?.Get(remappedLookup) ?? Resources.AudioManager?.Samples.Get(remappedLookup);
                if (sample != null)
                    return sample;
            }

            return null;
        }

        public ArgonMaxSkin(IStorageResourceProvider resources)
            : this(CreateInfo(), resources)
        {
        }

        [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
        public ArgonMaxSkin(SkinInfo skin, IStorageResourceProvider resources)
            : base(skin, resources)
        {
        }
    }
}
