using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineAbit2013.Models
{
    public class ObrazProgramInEntrySmallEntity
    {
        public string Name { get; set; }
        public bool HasProfileInObrazProgramInEntry { get; set; }
    }

    public class PriorityChangerApplication
    {
        public Guid ApplicationId { get; set; }
        public Guid CommitId { get; set; }
        public string CommitName { get; set; }
        public List<KeyValuePair<Guid, ObrazProgramInEntrySmallEntity>> lstObrazPrograms { get; set; }
    }

    public class PriorityChangerProfile
    {
        public Guid ApplicationId { get; set; }
        public Guid CommitId { get; set; }
        public string CommitName { get; set; }
        public string ObrazProgramName { get; set; }
        public List<KeyValuePair<Guid, string>> lstProfiles { get; set; }
    }
}