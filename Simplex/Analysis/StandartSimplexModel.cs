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
    internal class StandartSimplexModel:SimplexModel
    {
        Subject m_PhaseOneObjective = new Subject() { RightHandValue = 0, Equality= EquailtyType.Equals };
        TestMessage m_testMessage;

        int m_currentPhase = 2; //let us assume default is phase II
        double[] m_ObjectiveMatrix = null;
        double[,] m_ConstarintMatrix = null;
        double[,] m_RightHandMatrix = null;
        double[] m_PhaseOneObjectiveMatrix = null;
        VariableType[] m_vartypes = null;

        internal StandartSimplexModel(SimplexModel basemodel)
        {
            base.GoalType = basemodel.GoalType;
            base.ObjectiveFunction = basemodel.ObjectiveFunction;
            base.Subjects = basemodel.Subjects;
            m_testMessage = this.CheckBFS();
        }
        internal Subject PhaseObjectiveFunction
        {
            get { return m_PhaseOneObjective; }
            set { m_PhaseOneObjective = value; }
        }

        internal bool IsTwoPhase
        {
            get { return m_testMessage.Exception != null; }
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
        internal double[] PhaseOneObjectiveMatrix
        {
            get { return m_PhaseOneObjectiveMatrix; }
            set { m_PhaseOneObjectiveMatrix = value; }
        }
        internal VariableType[] VarTypes
        {
            get { return m_vartypes; }
            set { m_vartypes = value; }
        }

        internal int CurrentPhase
        {
            get { return m_currentPhase; }
            set { m_currentPhase = value; }
        }
    }
}
