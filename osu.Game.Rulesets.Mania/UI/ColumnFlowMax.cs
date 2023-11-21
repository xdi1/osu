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
    public partial class ColumnFlowMax<TContent> : CompositeDrawable
        where TContent : Drawable
    {
        /// <summary>
        /// All contents added to this <see cref="ColumnFlow{TContent}"/>.
        /// </summary>
        public IReadOnlyList<TContent> Content =>
            columnsST.Children.Take(1)
                .Concat(columnsFX.Children.Take(1))
                .Concat(columns.Children)
                .Concat(columnsFX.Children.Skip(1))
                .Concat(columnsST.Children.Skip(1))
                .Select(c => c.Count == 0 ? null : (TContent)c.Child)
                .ToList();

        private readonly FillFlowContainer<Container> columns;
        private readonly FillFlowContainer<Container> columnsFX;
        private readonly FillFlowContainer<Container> columnsST;
        private readonly StageDefinition stageDefinition;
        private readonly int columnCount;


        public ColumnFlowMax(StageDefinition stageDefinition)
        {
            this.stageDefinition = stageDefinition;

            columnCount = stageDefinition.Columns;

            AutoSizeAxes = Axes.X;

            Masking = true;

            InternalChildren = new Drawable[]
            {
                columnsST = new FillFlowContainer<Container>
                {
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    Direction = FillDirection.Horizontal,
                },
                columnsFX = new FillFlowContainer<Container>
                {
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    Direction = FillDirection.Horizontal,
                },
                columns = new FillFlowContainer<Container>
                {
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    Direction = FillDirection.Horizontal,
                }
            };
            if (stageDefinition.Columns >= 6)
            {
                for (int i = 0; i < 2; i++)
                    columnsST.Add(new Container { RelativeSizeAxes = Axes.Y });
                for (int i = 0; i < 2; i++)
                    columnsFX.Add(new Container { RelativeSizeAxes = Axes.Y });
                for (int i = 0; i < stageDefinition.Columns-4; i++)
                    columns.Add(new Container { RelativeSizeAxes = Axes.Y });
            }
            else
            {
                for (int i = 0; i < stageDefinition.Columns; i++)
                    columns.Add(new Container { RelativeSizeAxes = Axes.Y });
            }
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
            if (columnCount >= 6) {
                for (int i = 0; i < stageDefinition.Columns; i++)
                {
                    float spacing = currentSkin.GetConfig<ManiaSkinConfigurationLookup, float>(
                                                new ManiaSkinConfigurationLookup(LegacyManiaSkinConfigurationLookups.ColumnSpacing, i))
                                            ?.Value ?? Stage.COLUMN_SPACING;
                    float? width = currentSkin.GetConfig<ManiaSkinConfigurationLookup, float>(
                                                new ManiaSkinConfigurationLookup(LegacyManiaSkinConfigurationLookups.ColumnWidth, i))
                                            ?.Value;
                    if (i == 0)
                    {
                        columnsST[0].Margin = new MarginPadding { Left = spacing };
                        if (width == null)
                            columnsST[0].Width = stageDefinition.IsSpecialColumn(i) ? Column.SPECIAL_COLUMN_WIDTH : Column.COLUMN_WIDTH;
                        else
                            columnsST[0].Width = width.Value;
                    }
                    else if (i == columnCount - 1)
                    {
                        columnsST[1].Margin = new MarginPadding { Left = spacing };
                        if (width == null)
                            columnsST[1].Width = stageDefinition.IsSpecialColumn(i) ? Column.SPECIAL_COLUMN_WIDTH : Column.COLUMN_WIDTH;
                        else
                            columnsST[1].Width = width.Value;
                    }
                    else if (i == 1)
                    {
                        columnsFX[0].Margin = new MarginPadding { Left = spacing };
                        if (width == null)
                            columnsFX[0].Width = stageDefinition.IsSpecialColumn(i) ? Column.SPECIAL_COLUMN_WIDTH : Column.COLUMN_WIDTH;
                        else
                            columnsFX[0].Width = width.Value;
                    }
                    else if (i == columnCount - 2)
                    {
                        columnsFX[1].Margin = new MarginPadding { Left = spacing };
                        if (width == null)
                            columnsFX[1].Width = stageDefinition.IsSpecialColumn(i) ? Column.SPECIAL_COLUMN_WIDTH : Column.COLUMN_WIDTH;
                        else
                            columnsFX[1].Width = width.Value;
                    }
                    else
                    {
                        columns[i-2].Margin = new MarginPadding { Left = spacing };
                        if (width == null)
                            columns[i-2].Width = stageDefinition.IsSpecialColumn(i) ? Column.SPECIAL_COLUMN_WIDTH : Column.COLUMN_WIDTH;
                        else
                            columns[i-2].Width = width.Value;
                    }
                }  
            }
            else
            {
                for (int i = 0; i < stageDefinition.Columns; i++)
                {
                    if (i > 0)
                    {
                        float spacing = currentSkin.GetConfig<ManiaSkinConfigurationLookup, float>(
                                                    new ManiaSkinConfigurationLookup(LegacyManiaSkinConfigurationLookups.ColumnSpacing, i - 1))
                                                ?.Value ?? Stage.COLUMN_SPACING;

                        columns[i].Margin = new MarginPadding { Left = spacing };
                    }

                    float? width = currentSkin.GetConfig<ManiaSkinConfigurationLookup, float>(
                                                new ManiaSkinConfigurationLookup(LegacyManiaSkinConfigurationLookups.ColumnWidth, i))
                                            ?.Value;

                    if (width == null)
                        //only used by default skin (legacy skins get defaults set in LegacyManiaSkinConfiguration)
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
        public void SetContentForColumn(int column, TContent content)
        {
            int columnCount = stageDefinition.Columns;
            if (columnCount >= 6)
            {
                if (column == 0) 
                {
                    columnsST[0].Child = content;
                }
                else if (column == columnCount - 1)
                {
                    columnsST[1].Child = content;
                }
                else if (column == 1)
                {
                    columnsFX[0].Child = content;
                }
                else if (column == columnCount - 2)
                {
                    columnsFX[1].Child = content;
                }
                else
                {
                    columns[column - 2].Child = content;
                }
            }
            /*else if (columnCount >= 6)
            {
                if (column == 0)
                {
                    columnsFX[0].Child = content;
                }
                else if (column == columnCount - 1)
                {
                    columnsFX[1].Child = content;
                }
                else
                {
                    columns[column - 1].Child = content;
                }
            }*/
            else
            {
                columns[column].Child = content;
            }
        }

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
