using Izenda.BI.Framework.CustomConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Composition;
using Izenda.BI.Framework.Models.ReportDesigner;
using Izenda.BI.Framework.Models.Contexts; // UserContext reference
using Izenda.BI.Framework.Models.Common;
using Dapper;

namespace CustomAdhocReports
{
    [Export(typeof(IAdHocExtension))]
    class CustomAdhocReports : DefaultAdHocExtension
    {
        /// <summary>
        /// Pre-loads the filter data as tree for any filter operator designed to support tree lookup values
        /// </summary>
        /// <param name="filterField">The filter field info of current tree filter.</param>
        /// <param name="filterSetting">The filter settings of report containing all filter field values and data.</param>
        /// <param name="handled">Determines whether the tree data was pre-loaded. If yes, the OnLoadFilterDataTree is ignored.</param>
        /// <returns>List of tree data</returns>
        public override List<ValueTreeNode> OnPreLoadFilterDataTree(ReportFilterField filterField, ReportFilterSetting filterSetting, out bool handled)
        {
            // Load filter tree data if filter field is OrgId
            handled = false;
            if (!filterField.Alias.Equals("OrgId", StringComparison.OrdinalIgnoreCase))
                return null;

            var connectionString = "server=localhost;database=Retail;user id=sa;password=izenda;";
            var query = "SELECT OrgId, OrgName, ParentOrgId FROM dbo.OrgSample ORDER BY ParentOrgId ASC";

            // SQL query execution
            IEnumerable<dynamic> result = null;
            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                result = connection.Query(query);
            }

            if (result == null)
                return null;

            // Set root node based on NULL parent id or defaults
            ValueTreeNode root;
            var rootItem = result.FirstOrDefault(r => r.ParentOrgId == null);
            if (rootItem != null)
            {
                result = result.Where(r => r.ParentOrgId != null);
                root = new ValueTreeNode
                {
                    Parent = null,
                    Text = Convert.ToString(rootItem.OrgName),
                    Value = "[All]",
                    Nodes = new List<ValueTreeNode>()
                };
            }
            else
            {
                root = new ValueTreeNode
                {
                    Parent = null,
                    Text = "Root",
                    Value = "[All]",
                    Nodes = new List<ValueTreeNode>()
                };
            }

            // Group result based on parent id
            var groupedResult = result.ToLookup(x => x.ParentOrgId);
            // Recurse through grouped orgs and build tree
            var orgTree = GenerateTreeNodes(root, groupedResult);

            handled = true;
            return new List<ValueTreeNode> { orgTree };
        }

        /// <summary>
        /// Generates the tree nodes
        /// </summary>
        /// <param name="parent">The parent node</param>
        /// <param name="nodes">The full list of nodes available</param>
        /// <returns>The parent node populated with children nodes</returns>
        private ValueTreeNode GenerateTreeNodes(ValueTreeNode parent, ILookup<dynamic, dynamic> nodes)
        {
            var parentOrgId = int.TryParse(parent.Value, out int orgId) ? orgId : 1;
            var childNodes = nodes[parentOrgId].Select(n => new ValueTreeNode
            {
                Text = Convert.ToString(n.OrgName),
                Value = Convert.ToString(n.OrgId),
                Nodes = new List<ValueTreeNode>()
            });

            foreach (var node in childNodes)
            {
                parent.Nodes.Add(GenerateTreeNodes(node, nodes));
            }

            parent.NumOfChilds = parent.Nodes.Count();
            return parent;
        }

        /// <summary>
        /// Sets custom filters which are hidden to the user of the interface.
        /// </summary>
        public override ReportFilterSetting SetHiddenFilters(SetHiddenFilterParam param)
        {
            return base.SetHiddenFilters(param);
        }
    }
}
