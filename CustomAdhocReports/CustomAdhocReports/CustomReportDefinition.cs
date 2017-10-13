using System;
using System.Linq;
using Izenda.BI.Framework.Models.ReportDesigner;
using Izenda.BI.Framework.Constants;

namespace CustomAdhocReports
{
    /// <summary>
    /// The custom report definition class.
    /// </summary>
    static class CustomReportDefinition
    {
        /// <summary>
        /// Customizes the report definition before it is executed.
        /// </summary>
        /// <param name="reportDefinition">The report definition to be customized.</param>
        /// <returns>Returns the customized report definition.</returns>
        public static ReportDefinition OnPreExecute(ReportDefinition reportDefinition)
        {
            // Updates a filter's alias and changes the operator to filter on.
            const string filterToBeCustomized = "CustomerID";
            foreach (var filter in reportDefinition.ReportFilter.FilterFields.Where(f => f.Alias == filterToBeCustomized))
            {
                filter.Alias = "Customized-Id";
                filter.OperatorId = Guid.Parse("5CE630BC-6615-42C4-B11E-1D09C651EAAE");
                filter.OperatorName = "Equals (Checkbox)";
            }

            // Updates a column's name/alias
            const string columnToBeCustomized = "ContactName";
            foreach (var reportPart in reportDefinition.ReportPart.Select(f => f.ReportPartContent))
            {
                if (reportPart.GetAllFields().Any(f => f.FieldNameAlias == columnToBeCustomized))
                {
                    var filteredFieldsWithColumn = reportPart.GetAllFields().Where(f => f.FieldNameAlias == columnToBeCustomized).ToList();
                    filteredFieldsWithColumn.ForEach(f => f.FieldNameAlias = "Customized-Name");
                }
            }

            // Removes all report parts that are a map
            if (reportDefinition.ReportPart.Any(x => x.ReportPartContent.Type == ReportPartContentType.Map))
            {
                var filteredReportPart = reportDefinition.ReportPart.Where(x => x.ReportPartContent.Type != ReportPartContentType.Map).ToList();
                reportDefinition.ReportPart = filteredReportPart;
            }

            return reportDefinition;
        }
    }
}
