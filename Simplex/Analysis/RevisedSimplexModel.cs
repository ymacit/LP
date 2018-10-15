//***************************
//Sınıf Adı : RevisedtSimplexModel 
//Dosya Adı : RevisedtSimplexModel.cs 
//Tanım : Temel simplex modelin üzerinde faz 1 için geçici modeli barındırır.
//****************************

using System;
using System.Collections.Generic;
using System.Text;
using Simplex.Model;
using Simplex.Enums;
using Simplex.Helper;


namespace Simplex.Analysis
{
    [Serializable]
    internal class RevisedSimplexModel : SimplexModelDecorator
    {
        Matrix m_PhaseBasisObjectiveMatrix = null;
        Matrix m_PhaseNonBasisObjectiveMatrix = null;

        Matrix m_BasisMatrix = null;
        Matrix m_BasisInverseMatrix = null;
        Matrix m_NonBasisMatrix = null;
        Matrix m_BasisRightHandMatrix = null;
        Matrix m_BasisObjectiveMatrix = null;
        Matrix m_BasisNonObjectiveMatrix = null;
        double m_ObjectiveCost;
        internal RevisedSimplexModel(ISimplexModel basemodel) : base(basemodel)
        {
        }

        internal double ObjectiveCost
        {
            get { return m_ObjectiveCost; }
            set { m_ObjectiveCost = value; }
        }
        internal Matrix BasisMatrix
        {
            get { return m_BasisMatrix; }
            set { m_BasisMatrix = value; }
        }

        internal Matrix BasisInverseMatrix
        {
            get { return m_BasisInverseMatrix; }
            set { m_BasisInverseMatrix = value; }
        }

        internal Matrix NonBasisMatrix
        {
            get { return m_NonBasisMatrix; }
            set { m_NonBasisMatrix = value; }
        }

        internal Matrix BasisRightHandMatrix
        {
            get { return m_BasisRightHandMatrix; }
            set { m_BasisRightHandMatrix = value; }
        }

        internal Matrix BasisObjectiveMatrix
        {
            get { return m_BasisObjectiveMatrix; }
            set { m_BasisObjectiveMatrix = value; }
        }
        internal Matrix BasisNonObjectiveMatrix
        {
            get { return m_BasisNonObjectiveMatrix; }
            set { m_BasisNonObjectiveMatrix = value; }
        }


        internal Matrix PhaseBasisObjectiveMatrix
        {
            get { return m_PhaseBasisObjectiveMatrix; }
            set { m_PhaseBasisObjectiveMatrix = value; }
        }

        internal Matrix PhaseNonBasisObjectiveMatrix
        {
            get { return m_PhaseNonBasisObjectiveMatrix; }
            set { m_PhaseNonBasisObjectiveMatrix = value; }
        }
    }
}
