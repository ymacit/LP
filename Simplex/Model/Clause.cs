using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simplex.Enums;

namespace Simplex.Model
{
    [Serializable]
    public class Clause
    {
        List<Term> m_List = new List<Term>();
        int m_index = 0;
        string m_RowLabel = string.Empty;
        public List<Term> Terms
        {
            get { return m_List; }
        }

        public bool IsVectorContained(string vector)
        {
            return m_List.Any(vec => vec.Vector == vector);
        }

        public void AddTerm(double Factor, VariableType VarType, string Vector)
        {
            m_List.Add(new Term() { Factor = Factor, Core = new TermCore() { VarType = VarType, Vector = Vector } });
        }
        public int Index
        {
            get { return m_index; }
            set { m_index = value; }
        }

        public string RowLabel
        {
            get { return m_RowLabel; }
            set { m_RowLabel = value; }
        }

    }
}
