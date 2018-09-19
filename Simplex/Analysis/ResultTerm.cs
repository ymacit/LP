using System;
using System.Collections.Generic;
using System.Text;
using Simplex.Enums;

namespace Simplex.Analysis
{
    [Serializable]
    public class ResultTerm
    {
        VariableType m_VariableType = VariableType.Original;

        double m_VectorValue = 0;
        string m_label = string.Empty;

        public VariableType VarType
        {
            get { return m_VariableType; }
            set { m_VariableType = value; }
        }

        public double Value
        {
            get { return m_VectorValue; }
            set { m_VectorValue = value; }
        }

        public String Vector
        {
            get { return m_label; }
            set { m_label = value; }
        }
    }
}
