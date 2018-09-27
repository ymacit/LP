﻿using System;
using System.Collections.Generic;
using System.Text;
using Simplex.Enums;
using Simplex.Model;

namespace Simplex.Analysis
{
    internal class SimplexModelDecorator:ISimplexModel
    {
        protected ISimplexModel m_decoratedModel = null;
        Subject m_PhaseOneObjective = new Subject() { RightHandValue = 0, Equality = EquailtyType.Equals };
        TestMessage m_testMessage;
        int m_currentPhase = 2; //let us assume default is phase II
        VariableType[] m_vartypes = null;


        public SimplexModelDecorator(ISimplexModel decoratedModel)
        {
            m_decoratedModel = decoratedModel;
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
        public ObjectiveType GoalType { get => m_decoratedModel.GoalType; set => m_decoratedModel.GoalType = value; }
        public Subject ObjectiveFunction { get => m_decoratedModel.ObjectiveFunction; set => m_decoratedModel.ObjectiveFunction = value; }
        public List<Subject> Subjects { get => m_decoratedModel.Subjects; set => m_decoratedModel.Subjects = value; }
    }
}
