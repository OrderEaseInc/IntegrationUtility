using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LinkGreen.Applications.Common.Model
{
    [DataContract]
    public class AllNotesToAndFromCompanyRemoteResult
    {
        [DataMember]
        public List<NoteViewModel> OurNotes { get; set; }
        [DataMember]
        public List<NoteViewModel> TheirNotes { get; set; }

    }
}