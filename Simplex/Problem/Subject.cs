using System;
using System.Collections.Generic;
using System.Text;
using Simplex.Enums;

namespace Simplex.Problem
{
    [Serializable]
    public class Subject :Clause
    {
        EquailtyType m_EqualityType = EquailtyType.Equals;
        double m_RightHandValue = 0;

        public EquailtyType Equality
        {
            get { return m_EqualityType; }
            set { m_EqualityType = value; }
        }

        public double RightHandValue
        {
            get { return m_RightHandValue; }
            set { m_RightHandValue = value; }
        }

    }
}
