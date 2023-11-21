// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using JetBrains.Annotations;
using osu.Framework.Audio.Sample;
using osu.Game.Audio;
using osu.Game.Extensions;
using osu.Game.IO;

namespace osu.Game.Skinning
{
    public class ArgonFXSkin : ArgonSkin
    {
        public new static SkinInfo CreateInfo() => new SkinInfo
        {
            ID = Skinning.SkinInfo.ARGON_FX_SKIN,
            Name = "osu! \"argon\" fx",
            Creator = "1xdi",
            Protected = true,
            InstantiationInfo = typeof(ArgonFXSkin).GetInvariantInstantiationInfo()
        };

        public override ISample? GetSample(ISampleInfo sampleInfo)
        {
            foreach (string lookup in sampleInfo.LookupNames)
            {
                string remappedLookup = lookup.Replace(@"Gameplay/", @"Gameplay/ArgonFX/");

                var sample = Samples?.Get(remappedLookup) ?? Resources.AudioManager?.Samples.Get(remappedLookup);
                if (sample != null)
                    return sample;
            }

            return null;
        }

        public ArgonFXSkin(IStorageResourceProvider resources)
            : this(CreateInfo(), resources)
        {
        }

        [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
        public ArgonFXSkin(SkinInfo skin, IStorageResourceProvider resources)
            : base(skin, resources)
        {
        }
    }
}
