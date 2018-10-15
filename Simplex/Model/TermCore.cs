using System;
using System.Collections.Generic;
using System.Text;
using Simplex.Enums;

namespace Simplex.Model
{
    [Serializable]
    public class TermCore
    {
        VariableType m_VariableType = VariableType.Original;

        string m_label = string.Empty;
        int m_index = -1;
        bool m_isBasic = false;
        
        public VariableType VarType
        {
            get { return m_VariableType; }
            set { m_VariableType = value; }
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

        public bool Basic
        {
            get { return m_isBasic;}
            set { m_isBasic = value; }
        }
    }
}
