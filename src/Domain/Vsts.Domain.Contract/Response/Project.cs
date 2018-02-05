﻿namespace Vsts.Domain.Contract.Response
{
    public class Project
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string State { get; set; }
        public string Revision { get; set; }
        public string Visibility { get; set; }
    }
}