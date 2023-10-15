// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Mania.Beatmaps;
using osu.Game.Rulesets.Mania.Skinning;
using osu.Game.Skinning;
using System;

namespace osu.Game.Rulesets.Mania.UI
{
    /// <summary>
    /// A <see cref="Drawable"/> which flows its contents according to the <see cref="Column"/>s in a <see cref="Stage"/>.
    /// Content can be added to individual columns via <see cref="SetContentForColumn"/>.
    /// </summary>
    /// <typeparam name="TContent">The type of content in each column.</typeparam>
    public partial class ColumnFlow<TContent> : CompositeDrawable
        where TContent : Drawable
    {
        /// <summary>
        /// All contents added to this <see cref="ColumnFlow{TContent}"/>.
        /// </summary>
        public IReadOnlyList<TContent> Content => columns.Children.Select(c => c.Count == 0 ? null : (TContent)c.Child).ToList();

        private readonly FillFlowContainer<Container> columns;
        private readonly StageDefinition stageDefinition;

        public ColumnFlow(StageDefinition stageDefinition)
        {
            this.stageDefinition = stageDefinition;

            AutoSizeAxes = Axes.X;

            Masking = true;

            InternalChild = columns = new FillFlowContainer<Container>
            {
                RelativeSizeAxes = Axes.Y,
                AutoSizeAxes = Axes.X,
                Direction = FillDirection.Horizontal,
            };

            for (int i = 0; i < stageDefinition.Columns; i++)
                columns.Add(new Container { RelativeSizeAxes = Axes.Y });
        }

        private ISkinSource currentSkin;

        [BackgroundDependencyLoader]
        private void load(ISkinSource skin)
        {
            currentSkin = skin;

            skin.SourceChanged += onSkinChanged;
            onSkinChanged();
        }

        private void onSkinChanged()
        {
            int columnsCount = stageDefinition.Columns;
            if (columnsCount == 8) 
            {
                columns[0].Margin = new MarginPadding { Left = 738 };
                columns[1].Margin = new MarginPadding { Left = 2 };
                columns[2].Margin = new MarginPadding { Left = -370, Right = 370};
                columns[3].Margin = new MarginPadding { Left = -368, Right = 368};
                columns[4].Margin = new MarginPadding { Left = -738, Right = 738};
                columns[5].Margin = new MarginPadding { Left = -736, Right = 736};
                columns[6].Margin = new MarginPadding { Left = -734, Right = 734};
                columns[7].Margin = new MarginPadding { Left = -732, Right = 732};
                for (int i = 0; i < columnsCount; i++)
                {
                    float? width = currentSkin.GetConfig<ManiaSkinConfigurationLookup, float>(
                                                new ManiaSkinConfigurationLookup(LegacyManiaSkinConfigurationLookups.ColumnWidth, i))
                                            ?.Value;

                    if (width == null)
                        // only used by default skin (legacy skins get defaults set in LegacyManiaSkinConfiguration)
                        columns[i].Width = stageDefinition.IsSpecialColumn(i) ? Column.SPECIAL_COLUMN_WIDTH : Column.COLUMN_WIDTH;
                    else
                        columns[i].Width = width.Value;
                }
            }
            else if (columnsCount == 10)
            {
                columns[0].Margin = new MarginPadding { Left = 738 };
                columns[1].Margin = new MarginPadding { Left = 2 };
                columns[2].Margin = new MarginPadding { Left = -370, Right = 370};
                columns[3].Margin = new MarginPadding { Left = -368, Right = 368};
                columns[4].Margin = new MarginPadding { Left = -738, Right = 738};
                columns[5].Margin = new MarginPadding { Left = -736, Right = 736};
                columns[6].Margin = new MarginPadding { Left = -734, Right = 734};
                columns[7].Margin = new MarginPadding { Left = -732, Right = 732};
                columns[8].Margin = new MarginPadding { Left = -730, Right = 730};
                columns[9].Margin = new MarginPadding { Left = -728, Right = 728};
                for (int i = 0; i < columnsCount; i++)
                {
                    float? width = currentSkin.GetConfig<ManiaSkinConfigurationLookup, float>(
                                                new ManiaSkinConfigurationLookup(LegacyManiaSkinConfigurationLookups.ColumnWidth, i))
                                            ?.Value;

                    if (width == null)
                        // only used by default skin (legacy skins get defaults set in LegacyManiaSkinConfiguration)
                        columns[i].Width = stageDefinition.IsSpecialColumn(i) ? Column.SPECIAL_COLUMN_WIDTH : Column.COLUMN_WIDTH;
                    else
                        columns[i].Width = width.Value;
                }
            }
            else 
            {
                for (int i = 0; i < columnsCount; i++)
                {
                    if (i > 0)
                    {
                        float spacing = currentSkin.GetConfig<ManiaSkinConfigurationLookup, float>(
                                                    new ManiaSkinConfigurationLookup(LegacyManiaSkinConfigurationLookups.ColumnSpacing, i - 1))
                                                ?.Value ?? Stage.COLUMN_SPACING;
                        
                        columns[i].Margin = new MarginPadding { Left = spacing, Right = spacing };
                    }

                    float? width = currentSkin.GetConfig<ManiaSkinConfigurationLookup, float>(
                                                new ManiaSkinConfigurationLookup(LegacyManiaSkinConfigurationLookups.ColumnWidth, i))
                                            ?.Value;

                    if (width == null)
                        // only used by default skin (legacy skins get defaults set in LegacyManiaSkinConfiguration)
                        columns[i].Width = stageDefinition.IsSpecialColumn(i) ? Column.SPECIAL_COLUMN_WIDTH : Column.COLUMN_WIDTH;
                    else
                        columns[i].Width = width.Value;
                }
            }
            
        }

        /// <summary>
        /// Sets the content of one of the columns of this <see cref="ColumnFlow{TContent}"/>.
        /// </summary>
        /// <param name="column">The index of the column to set the content of.</param>
        /// <param name="content">The content.</param>
        public void SetContentForColumn(int column, TContent content) => columns[column].Child = content;

        public new MarginPadding Padding
        {
            get => base.Padding;
            set => base.Padding = value;
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (currentSkin != null)
                currentSkin.SourceChanged -= onSkinChanged;
        }
    }
}
