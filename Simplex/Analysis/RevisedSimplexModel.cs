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
    internal class RevisedSimplexModel : SimplexModelDecorator
    {
        Matrix m_PhaseOneBasisMatrix = null;
        Matrix m_PhaseOneNonBasisMatrix = null;
        Matrix m_PhaseOneBasisRightHandMatrix = null;
        Matrix m_PhaseOneBasisObjectiveMatrix = null;
        Matrix m_PhaseOneNonBasisObjectiveMatrix = null;

        Matrix m_BasisMatrix = null;
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

        internal Matrix PhaseOneBasisMatrix
        {
            get { return m_PhaseOneBasisMatrix; }
            set { m_PhaseOneBasisMatrix = value; }
        }

        internal Matrix PhaseOneNonBasisMatrix
        {
            get { return m_PhaseOneNonBasisMatrix; }
            set { m_PhaseOneNonBasisMatrix = value; }
        }

        internal Matrix PhaseOneBasisRightHandMatrix
        {
            get { return m_PhaseOneBasisRightHandMatrix; }
            set { m_PhaseOneBasisRightHandMatrix = value; }
        }

        internal Matrix PhaseOneBasisObjectiveMatrix
        {
            get { return m_PhaseOneBasisObjectiveMatrix; }
            set { m_PhaseOneBasisObjectiveMatrix = value; }
        }

        internal Matrix PhaseNonOneBasisObjectiveMatrix
        {
            get { return m_PhaseOneNonBasisObjectiveMatrix; }
            set { m_PhaseOneNonBasisObjectiveMatrix = value; }
        }
    }
}
