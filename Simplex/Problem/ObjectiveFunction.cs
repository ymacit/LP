using System;
using System.Collections.Generic;
using System.Text;
using Solver.Enums;

namespace Solver
{
    public class ObjectiveFunction :Clause
    {
        ObjectiveType m_ObjectiveType = ObjectiveType.Minumum;

        ObjectiveType Objective
        {
            get { return m_ObjectiveType; }
            set { m_ObjectiveType = value; }
        }
    }
}
