using Izenda.BI.Framework.CustomConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using Izenda.BI.Framework.Models;
using System.ComponentModel.Composition;
using Izenda.BI.Framework.Models.ReportDesigner;
using Izenda.BI.Framework.Utility;
using Izenda.BI.Framework.Models.Contexts; //For Referencing User Context

namespace CustomAdhocReports
{
    [Export(typeof(IAdHocExtension))]
    class CustomAdhocReports : DefaultAdHocExtension
    {
        /// <summary>
        /// Adds custom formats for a specified data type.
        /// </summary>
        public override List<DataFormat> LoadCustomDataFormat()
        {
            return CustomDataFormat.LoadCustomDataFormat();
        }

        /// <summary>
        /// Customizes the report content on the fly before it is executed.
        /// </summary>
        /// <param name="reportDefinition">The report definition.</param>
        public override ReportDefinition OnPreExecute(ReportDefinition reportDefinition)
        {
            return CustomReportDefinition.OnPreExecute(reportDefinition);
        }

        /// <summary>
        /// Sets custom filters which are hidden to the user of the interface.
        /// </summary>
        /// <param name="param">The hidden filter parameter.</param>
        public override ReportFilterSetting SetHiddenFilters(SetHiddenFilterParam param)
        {
            var filterSetting = new ReportFilterSetting() { FilterFields = new List<ReportFilterField>() };

            this.AddHiddenFilter(param, filterSetting, "ShipCountry", new List<string> { "USA", "Germany" });
            this.AddHiddenFilter(param, filterSetting, "ProductID", new List<string> { "5" });

            return filterSetting;
        }

        /// <summary>
        /// Adds a hidden filter based on field name and values passed
        /// </summary>
        /// <param name="param">The hidden filter parameter.</param>
        /// <param name="filterSetting">The report filter setting.</param>
        /// <param name="filterFieldName">The filter field names.</param>
        /// <param name="values">The values.</param>
        /// <returns>The report filter setting.</returns>
        private ReportFilterSetting AddHiddenFilter(SetHiddenFilterParam param, ReportFilterSetting filterSetting, string filterFieldName, List<string> values)
        {
            Action<QuerySource, QuerySourceField, Guid, Relationship> addHiddenFilters = (querySource, field, operatorId, rel) =>
            {
                var value = string.Join(";#", values);
                var filterPosition = filterSetting.FilterFields.Count + 1;

                var filter = new ReportFilterField
                {
                    Alias = $"{field.Name}{filterPosition}",
                    QuerySourceId = querySource.Id,
                    SourceDataObjectName = querySource.Name,
                    QuerySourceType = querySource.Type,
                    QuerySourceFieldId = field.Id,
                    SourceFieldName = field.Name,
                    DataType = field.DataType,
                    Position = filterPosition,
                    OperatorId = operatorId,
                    Value = value,
                    RelationshipId = rel?.Id,
                    IsParameter = false,
                    ReportFieldAlias = null
                };

                filterSetting.FilterFields.Add(filter);
            };

            // Scan thru the query sources/fields that are involved in the report
            foreach (var querySource in param.QuerySources
                .Where(x => x.QuerySourceFields.Any(y => y.Name.Equals(filterFieldName, StringComparison.OrdinalIgnoreCase))))
            {
                // Pick the relationships that joins the query source as primary source
                // Setting the join ensure the proper table is assigned when using join alias in the UI
                var rels = param.ReportDefinition.ReportRelationship.Where(x => x.JoinQuerySourceId == querySource.Id);

                // Count the relationships that the filter query source is foreign query source
                var foreignRelCounts = param.ReportDefinition.ReportRelationship.Where(x => x.ForeignQuerySourceId == querySource.Id).Count();

                // Find actual filter field in query source
                var field = querySource.QuerySourceFields.FirstOrDefault(x => x.Name.Equals(filterFieldName, StringComparison.OrdinalIgnoreCase));

                // Get the filter operator GUID
                var equalOperator = Izenda.BI.Framework.Enums.FilterOperator.FilterOperator.EqualsManualEntry.GetUid();

                // In case there is no relationship that the query source is joined as primary
                if (!rels.Any())
                {
                    // Just add hidden filter with null relationship
                    addHiddenFilters(querySource, field, equalOperator, null);
                }
                else
                {
                    if (foreignRelCounts > 0)
                    {
                        addHiddenFilters(querySource, field, equalOperator, null);
                    }

                    foreach (var rel in rels)
                    {
                        // Loop thru all relationships that the query source is joined as primary and add the hidden field associated with each relationship
                        addHiddenFilters(querySource, field, equalOperator, rel);
                    }
                }
            }

            return filterSetting;
        }
    }
}
