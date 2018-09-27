using System;
using System.Collections.Generic;
using System.Text;
using Simplex.Model;

namespace Simplex.Analysis
{
    public interface ISolutionBuilder
    {
        void setStandartModel(SimplexModel model);
        void setPhase();
        void setMatrices();
        Solution getResult();
    }
}
