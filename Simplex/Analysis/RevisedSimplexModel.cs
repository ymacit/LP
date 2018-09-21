//***************************
//Sınıf Adı : RevisedtSimplexModel 
//Dosya Adı : RevisedtSimplexModel.cs 
//Tanım : Temel simplex modelin üzerinde faz 1 için geçici modeli barındırır.
//****************************

using System;
using System.Collections.Generic;
using System.Text;
using Simplex.Problem;
using Simplex.Enums;
using Simplex.Helper;

namespace Simplex.Analysis
{
    public class RevisedSimplexModel : StandartSimplexModel
    {
        Matrix m_PhaseOneBasisMatrix = null;
        Matrix m_PhaseOneBasisInverse = null;
        Matrix m_PhaseOneNonBasisMatrix = null;
        Matrix m_PhaseOneBasisRightHandMatrix = null;
        Matrix m_PhaseOneBasisObjectiveMatrix = null;

        Matrix m_BasisMatrix = null;
        Matrix m_BasisInverse = null;
        Matrix m_NonBasisMatrix = null;
        Matrix m_BasisRightHandMatrix = null;
        Matrix m_BasisObjectiveMatrix = null;

        public RevisedSimplexModel(StandartSimplexModel basemodel):base(basemodel)
        {
            ConstarintMatrix = basemodel.ConstarintMatrix;
            RightHandMatrix = basemodel.RightHandMatrix;
            ObjectiveMatrix = basemodel.ObjectiveMatrix;
            PhaseOneObjectiveMatrix = basemodel.PhaseOneObjectiveMatrix;
            VarTypes = basemodel.VarTypes;
        }

        public Matrix BasisMatrix
        {
            get { return m_BasisMatrix; }
            set { m_BasisMatrix = value; }
        }
        public Matrix BasisInverse
        {
            get { return m_BasisInverse; }
            set { m_BasisInverse = value; }
        }

        public Matrix NonBasisMatrix
        {
            get { return m_NonBasisMatrix; }
            set { m_NonBasisMatrix = value; }
        }

        public Matrix BasisRightHandMatrix
        {
            get { return m_BasisRightHandMatrix; }
            set { m_BasisRightHandMatrix = value; }
        }

        public Matrix BasisObjectiveMatrix
        {
            get { return m_BasisObjectiveMatrix; }
            set { m_BasisObjectiveMatrix = value; }
        }

        public Matrix PhaseOneBasisMatrix
        {
            get { return m_PhaseOneBasisMatrix; }
            set { m_PhaseOneBasisMatrix = value; }
        }
        public Matrix PhaseOneBasisInverse
        {
            get { return m_PhaseOneBasisInverse; }
            set { m_PhaseOneBasisInverse = value; }
        }

        public Matrix PhaseOneNonBasisMatrix
        {
            get { return m_PhaseOneNonBasisMatrix; }
            set { m_PhaseOneNonBasisMatrix = value; }
        }

        public Matrix PhaseOneBasisRightHandMatrix
        {
            get { return m_PhaseOneBasisRightHandMatrix; }
            set { m_PhaseOneBasisRightHandMatrix = value; }
        }

        public Matrix PhaseOneBasisObjectiveMatrix
        {
            get { return m_PhaseOneBasisObjectiveMatrix; }
            set { m_PhaseOneBasisObjectiveMatrix = value; }
        }

    }
}
