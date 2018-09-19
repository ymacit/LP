using System;
using System.Collections.Generic;
using System.Text;
using Simplex.Enums;

namespace Simplex.Problem
{
    [Serializable]
    public class Term
    {
        VariableType m_VariableType = VariableType.Original;

        double m_FactorValue = 0;
        string m_label = string.Empty;
        int m_index = -1;
        
        public VariableType VarType
        {
            get { return m_VariableType; }
            set { m_VariableType = value; }
        }

        public double Factor
        {
            get { return m_FactorValue; }
            set { m_FactorValue = value; }
        }

        public String Vector
        {
            get { return m_label; }
            set { m_label = value; }
        }

        public int Index
        {
            get { return m_index; }
            set { m_index = value; }
        }

        public bool isConstant
        {
            get
            {
                if (m_label != null && m_label.Trim().Length > 0)
                    return false;
                else
                    return true;
            }
        }
    }
}
