//***************************
//Sınıf Adı : PhaseOneSimplexModel 
//Dosya Adı : PhaseOneSimplexModel.cs 
//Tanım : Temel simplex modelin üzerinde faz 1 için geçici modeli barındırır.
//****************************

using System;
using System.Collections.Generic;
using System.Text;
using Simplex.Enums;
using Simplex.Model;
using Simplex.Helper;

namespace Simplex.Analysis
{
    internal class StandartSimplexModel: SimplexModelDecorator
    {
        Matrix m_ObjectiveMatrix = null;
        Matrix m_ConstarintMatrix = null;
        Matrix m_RightHandMatrix = null;
        Matrix m_ArtificialObjectiveMatrix = null;
        internal StandartSimplexModel(ISimplexModel basemodel) :base(basemodel)
        {

        }

        internal Matrix ObjectiveMatrix
        {
            get { return m_ObjectiveMatrix; }
            set { m_ObjectiveMatrix = value; }
        }
        internal Matrix ConstarintMatrix
        {
            get { return m_ConstarintMatrix; }
            set { m_ConstarintMatrix = value; }
        }
        internal Matrix RightHandMatrix
        {
            get { return m_RightHandMatrix; }
            set { m_RightHandMatrix = value; }
        }
        internal Matrix ArtificialObjectiveMatrix
        {
            get { return m_ArtificialObjectiveMatrix; }
            set { m_ArtificialObjectiveMatrix = value; }
        }
    }
}
