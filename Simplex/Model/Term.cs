using System;
using System.Collections.Generic;
using System.Text;
using Simplex.Enums;

namespace Simplex.Model
{
    [Serializable]
    public class Term
    {
        TermCore m_TermCore = null;
        double m_FactorValue = 0;
        int m_index = -1;

        public TermCore Core
        {
            get { return m_TermCore; }
            set { m_TermCore = value; }
        }


        public VariableType VarType
        {
            get { return m_TermCore.VarType; }
            set { m_TermCore.VarType = value; }
        }

        public double Factor
        {
            get { return m_FactorValue; }
            set { m_FactorValue = value; }
        }

        public String Vector
        {
            get { return m_TermCore.Vector; }
            set { m_TermCore.Vector = value; }
        }

        public int Index
        {
            get { return m_index; }
            set { m_index = value; }
        }

        public bool Basic
        {
            get { return m_TermCore.Basic; }
            set { m_TermCore.Basic = value; }
        }
    }
}
