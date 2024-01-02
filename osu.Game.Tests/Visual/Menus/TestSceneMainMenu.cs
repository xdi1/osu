// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using NUnit.Framework;
using osu.Framework.Graphics.Containers;
using osu.Framework.Testing;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Screens.Menu;
using osuTK.Input;

namespace osu.Game.Tests.Visual.Menus
{
    public partial class TestSceneMainMenu : OsuGameTestScene
    {
        private SystemTitle systemTitle => Game.ChildrenOfType<SystemTitle>().Single();

        [Test]
        public void TestSystemTitle()
        {
            AddStep("set system title", () => systemTitle.Current.Value = new APISystemTitle
            {
                Image = @"https://assets.ppy.sh/main-menu/project-loved-2@2x.png",
                Url = @"https://osu.ppy.sh/home/news/2023-12-21-project-loved-december-2023",
            });
            AddAssert("system title not visible", () => systemTitle.State.Value, () => Is.EqualTo(Visibility.Hidden));
            AddStep("enter menu", () => InputManager.Key(Key.Enter));
            AddUntilStep("system title visible", () => systemTitle.State.Value, () => Is.EqualTo(Visibility.Visible));
            AddStep("set another title", () => systemTitle.Current.Value = new APISystemTitle
            {
                Image = @"https://assets.ppy.sh/main-menu/wf2023-vote@2x.png",
                Url = @"https://osu.ppy.sh/community/contests/189",
            });
            AddStep("set title with nonexistent image", () => systemTitle.Current.Value = new APISystemTitle
            {
                Image = @"https://test.invalid/@2x", // .invalid TLD reserved by https://datatracker.ietf.org/doc/html/rfc2606#section-2
                Url = @"https://osu.ppy.sh/community/contests/189",
            });
            AddStep("unset system title", () => systemTitle.Current.Value = null);
        }
    }
}
