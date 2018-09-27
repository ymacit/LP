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

namespace Simplex.Analysis
{
    internal class StandartSimplexModel: SimplexModelDecorator
    {
        double[] m_ObjectiveMatrix = null;
        double[,] m_ConstarintMatrix = null;
        double[,] m_RightHandMatrix = null;
        double[] m_ArtificialObjectiveMatrix = null;
        internal StandartSimplexModel(ISimplexModel basemodel) :base(basemodel)
        {

        }

        internal double[] ObjectiveMatrix
        {
            get { return m_ObjectiveMatrix; }
            set { m_ObjectiveMatrix = value; }
        }
        internal double[,] ConstarintMatrix
        {
            get { return m_ConstarintMatrix; }
            set { m_ConstarintMatrix = value; }
        }
        internal double[,] RightHandMatrix
        {
            get { return m_RightHandMatrix; }
            set { m_RightHandMatrix = value; }
        }
        internal double[] ArtificialObjectiveMatrix
        {
            get { return m_ArtificialObjectiveMatrix; }
            set { m_ArtificialObjectiveMatrix = value; }
        }
    }
}
