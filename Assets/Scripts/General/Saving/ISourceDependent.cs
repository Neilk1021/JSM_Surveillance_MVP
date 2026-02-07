using System;
using System.Collections.Generic;
using JSM.Surveillance.Game;

namespace JSM.Surviellance.Saving
{
    public interface ISourceDependent
    {
        void RehydrateSourceReferences(Dictionary<Guid, Source> sourceDict);
    }
}