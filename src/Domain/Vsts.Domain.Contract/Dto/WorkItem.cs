﻿using Newtonsoft.Json.Linq;

namespace Vsts.Domain.Contract.Dto
{
    public class WorkItem
    {
        public int Id { get; set; }
        public int Rev { get; set; }
        public JObject Fields { get; set; }
        public string Url { get; set; }
    }
}