using System;
using System.Collections.Generic;
using Vsts.Domain.Contract.Dto;
using Vsts.Domain.Contract.Enum;

namespace Vsts.Domain.Contract.Response
{
    [Serializable]
    public class WorkItemQueryResponse
    {
        public string QueryType { get; set; }
        public QueryResultType QueryResultType { get; set; }
        public DateTime AsOf { get; set; }
        public List<WorkItemColumn> Columns { get; set; }
        public List<WorkItem> WorkItems { get; set; }

        public class WorkItemColumn
        {
            public string ReferenceName { get; set; }
            public string Name { get; set; }
            public string Url { get; set; }
        }
    }
}