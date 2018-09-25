//***************************
//Sınıf Adı : PhaseOneSimplexModel 
//Dosya Adı : PhaseOneSimplexModel.cs 
//Tanım : Temel simplex modelin üzerinde faz 1 için geçici modeli barındırır.
//****************************

using System;
using System.Collections.Generic;
using System.Text;
using Simplex.Problem;
using Simplex.Enums;

namespace Simplex.Analysis
{
    public class StandartSimplexModel:SimplexModel
    {
        Subject m_PhaseOneObjective = new Subject() { RightHandValue = 0, Equality= EquailtyType.Equals };
        TestMessage m_testMessage;

        int m_currentPhase = 2; //let us assume default is phase II
        double[] m_ObjectiveMatrix = null;
        double[,] m_ConstarintMatrix = null;
        double[,] m_RightHandMatrix = null;
        double[] m_PhaseOneObjectiveMatrix = null;
        VariableType[] m_vartypes = null;

        public StandartSimplexModel(SimplexModel basemodel)
        {
            base.GoalType = basemodel.GoalType;
            base.ObjectiveFunction = basemodel.ObjectiveFunction;
            base.Subjects = basemodel.Subjects;
            m_testMessage = this.CheckBFS();
        }
        public Subject PhaseObjectiveFunction
        {
            get { return m_PhaseOneObjective; }
            set { m_PhaseOneObjective = value; }
        }

        public bool IsTwoPhase
        {
            get { return m_testMessage.Exception != null; }
        }

        public double[] ObjectiveMatrix
        {
            get { return m_ObjectiveMatrix; }
            set { m_ObjectiveMatrix = value; }
        }
        public double[,] ConstarintMatrix
        {
            get { return m_ConstarintMatrix; }
            set { m_ConstarintMatrix = value; }
        }
        public double[,] RightHandMatrix
        {
            get { return m_RightHandMatrix; }
            set { m_RightHandMatrix = value; }
        }
        public double[] PhaseOneObjectiveMatrix
        {
            get { return m_PhaseOneObjectiveMatrix; }
            set { m_PhaseOneObjectiveMatrix = value; }
        }
        public VariableType[] VarTypes
        {
            get { return m_vartypes; }
            set { m_vartypes = value; }
        }

        public int CurrentPhase
        {
            get { return m_currentPhase; }
            set { m_currentPhase = value; }
        }
    }
}
