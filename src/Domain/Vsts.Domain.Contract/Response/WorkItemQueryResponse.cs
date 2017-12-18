using System;
using System.Collections.Generic;

namespace Vsts.Domain.Contract.Response
{
    [Serializable]
    public class WorkItemQueryResponse : QueryResponse
    {
        public WorkItemQueryResponse()
        {
            SetQueryResultType();
        }

        public List<WorkItemColumn> Columns { get; set; }
        public List<WorkItem> WorkItems { get; set; }

        public override void SetQueryResultType()
        {
            this.QueryResultType = Enum.QueryResultType.WorkItem.ToString();
        }
    }

    public class WorkItemColumn
    {
        public string ReferenceName { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
