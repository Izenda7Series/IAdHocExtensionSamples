using Izenda.BI.Framework.CustomConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Composition;
using Izenda.BI.Framework.Models.ReportDesigner;
using Izenda.BI.Framework.Models.Contexts; // UserContext reference
using Izenda.BI.Framework.Models.Common;
using Izenda.BI.Framework.Components.QueryExpressionTree;
using Izenda.BI.Framework.Utility;
using Izenda.BI.Framework.Components.QueryExpressionTree.Operator;
using Framework = Izenda.BI.Framework;
using Izenda.BI.Framework.Constants;

namespace CustomAdhocReports
{
    [Export(typeof(IAdHocExtension))]
    class CustomAdhocReports : DefaultAdHocExtension
    {
        /// <summary>
        /// Source field name for Contains/Like Tree filter
        /// </summary>
        private const string FieldNameForContainsTreeFilter = "media_right_1";

        /// <summary>
        /// Called when [executing].
        /// </summary>
        /// <param name="queryTree">The query tree.</param>
        /// <returns></returns>
        public override QueryTree OnExecuting(QueryTree queryTree)
        {
            var logic = string.Empty;
            var newFilters = new List<ReportFilterField>();
            var selectionOperator = QueryTreeUtil.FindNodeType<SelectionOperator>(queryTree.Root);
            if (selectionOperator == null || !selectionOperator.FilterFields.Any(f => IsTreeFilterWithValues(f)))
                return queryTree;

            foreach (var filterField in selectionOperator.FilterFields.OrderBy(f => f.Position))
            {
                var previousPosition = newFilters.LastOrDefault()?.Position ?? 0;
                if (IsTreeFilterWithValues(filterField))
                {
                    var values = FlattenTreeValues(filterField).ToArray();
                    var valuesCount = values.Count();
                    for (var i = 0; i < valuesCount; i++)
                    {
                        var position = previousPosition + 1 + i;
                        newFilters.Add(new ReportFilterField(filterField)
                        {
                            Position = position,
                            OperatorId = Framework.Enums.FilterOperator.FilterOperator.Like.GetUid(),
                            OperatorName = Framework.Enums.FilterOperator.FilterOperator.Like.GetDisplayName(),
                            Value = values[i]
                        });

                        if (i == 0) // first value in tree filter
                        {
                            if (valuesCount == 1) // only one value in tree filter
                            {
                                if (string.IsNullOrEmpty(logic)) // first filter
                                    logic = $"{position}";
                                else
                                    logic = $"{logic} AND {position}";
                            }
                            else if (string.IsNullOrEmpty(logic)) // first filter and value
                            {
                                logic = $"({position}";
                            }
                            else // after first filter, but first value in tree filter
                            {
                                logic = $"{logic} AND ({position}";
                            }
                        }
                        else if (i == valuesCount - 1) // last value in tree filter
                            logic = $"{logic} OR {position})";
                        else // after first value in tree filter
                            logic = $"{logic} OR {position}";
                    }
                }
                else
                {
                    var position = previousPosition + 1;
                    filterField.Position = position;
                    newFilters.Add(filterField);

                    if (string.IsNullOrEmpty(logic)) // first filter
                        logic = $"{position}";
                    else // after first filter
                        logic = $"{logic} AND {position}";
                }
            }

            selectionOperator.FilterFields = newFilters;
            selectionOperator.Logic = logic;
            return queryTree;
        }

        /// <summary>
        /// Indicates whether a filter field is a tree filter with values from the expected source field 
        /// </summary>
        /// <param name="filterField">The filter field</param>
        /// <returns>True if the filter parameter is a tree filter with values from the expected source field</returns>
        private static bool IsTreeFilterWithValues(ReportFilterField filterField)
        {
            var equalsTreeOperatorId = Framework.Enums.FilterOperator.FilterOperator.EqualsTree.GetUid();
            var equalsTreeOperatorName = Framework.Enums.FilterOperator.FilterOperator.EqualsTree.GetDisplayDescription();

            return filterField.SourceFieldName == FieldNameForContainsTreeFilter
                && filterField.OperatorId == equalsTreeOperatorId && filterField.OperatorName == equalsTreeOperatorName
                && !string.IsNullOrWhiteSpace(filterField.Value) && !filterField.Value.Equals("[ALL]", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Flattens the tree filter's value into the collection of values
        /// </summary>
        /// <param name="filterField">The filter field</param>
        /// <returns>Collection of filter values</returns>
        private static IEnumerable<string> FlattenTreeValues(ReportFilterField filterField)
        {
            var result = new List<string>();
            if (!string.IsNullOrWhiteSpace(filterField?.Value))
            {
                foreach (var value in filterField.Value.Split(new[] { IzendaKey.ValueSeparator }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var leaf = value.Split(new[] { FilterGenerator.BackSlashSeperator }, StringSplitOptions.RemoveEmptyEntries).Last();
                    result.Add(leaf);
                }
            }

            return result;
        }

        /// <summary>
        /// Call to pre-load the filter data as tree for any that is designed to support tree lookup values.
        /// </summary>
        /// <param name="filterField">The filter field info of current tree filter</param>
        /// <param name="filterSetting">The filter settings of report. It contains all filter field values and data</param>
        /// <param name="handled">Indicate whether it already handles the tree data. If yes, the OnLoadFilterDataTree is ignored.</param>
        /// <returns></returns>
        public override List<ValueTreeNode> OnPreLoadFilterDataTree(ReportFilterField filterField, ReportFilterSetting filterSetting, out bool handled)
        {
            handled = false;
            var result = new List<ValueTreeNode>();
            if (filterField.SourceFieldName == FieldNameForContainsTreeFilter)
            {
                handled = true;
                var rootNode = new ValueTreeNode { Text = "All Media", Value = "All Media" };
                rootNode.Nodes = new List<ValueTreeNode>
                {
                    new ValueTreeNode { Text = "Airlines", Value = "Airlines" },
                    new ValueTreeNode { Text = "Hotels", Value = "Hotels" },
                    new ValueTreeNode { Text = "Non-Theatrical / Public Video", Value = "Non-Theatrical / Public Video" }
                };

                result.Add(rootNode);
            }
            else if (filterField.SourceFieldName == "media_right_2")
            {
                handled = true;
                var rootNode = new ValueTreeNode { Text = "All Media", Value = "All Media" };
                rootNode.Nodes = new List<ValueTreeNode>
                {
                    new ValueTreeNode { Text = "Airlines", Value = "Airlines" },
                    new ValueTreeNode { Text = "Hotels", Value = "Hotels" },
                    new ValueTreeNode { Text = "Non-Theatrical / Public Video", Value = "Non-Theatrical / Public Video" }
                };

                result.Add(rootNode);
            }
            else if (filterField.SourceFieldName == "ShipCity")
            {
                handled = true;
                var rootNode = new ValueTreeNode { Text = "USA", Value = "USA" };
                rootNode.Nodes = new List<ValueTreeNode>
                {
                    new ValueTreeNode { Text = "Albuquerque", Value = "Albuquerque" },
                    new ValueTreeNode { Text = "Seattle", Value = "Seattle" },
                    new ValueTreeNode { Text = "Portland", Value = "Portland" }
                };

                result.Add(rootNode);
            }

            return result;
        }
    }
}