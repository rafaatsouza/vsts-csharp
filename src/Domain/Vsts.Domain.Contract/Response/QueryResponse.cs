using System;
using System.Runtime.Serialization;

namespace Vsts.Domain.Contract.Response
{
    public abstract class QueryResponse
    {
        public string QueryType { get; set; }
        public string QueryResultType { get; set; }
        public DateTime AsOf { get; set; }

        public abstract void SetQueryResultType();

        #region Private Methods

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            SetQueryResultType();
        }

        [OnSerialized]
        private void OnSerialized(StreamingContext context)
        {
            SetQueryResultType();
        }

        #endregion       
    }
}
