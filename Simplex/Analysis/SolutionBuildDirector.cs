using System;
using System.Collections.Generic;
using System.Text;
using Simplex.Enums;
using Simplex.Model;

namespace Simplex.Analysis
{
    public class SolutionBuildDirector
    {
        private ISolutionBuilder m_builder;
        private SolverType m_type;
        private SimplexModel m_model;

        public SolutionBuildDirector(SolverType solverType, SimplexModel model)
        {
            m_model = model;
            m_type = solverType;
            m_builder = getBuilder(m_type);
        }
        public ISolutionBuilder SolutionBuilder
        {
            get { return m_builder; }
        }

        public void Construct()
        {
            m_builder.setStandartModel(m_model);
            m_builder.setPhase();
            m_builder.setMatrices();
        }

        //private static ISolutionBuilder getBuilder(SolverType solverType, SimplexModel model)
        //{
        //    Type tmp_builderType = BuilderList[solverType];
        //    object[] tmp_args = new object[] { model };
        //    return Activator.CreateInstance(tmp_builderType, tmp_args) as ISolutionBuilder;
        //}

        private static ISolutionBuilder getBuilder(SolverType solverType)
        {
            Type tmp_builderType = BuilderList[solverType];
            return Activator.CreateInstance(tmp_builderType) as ISolutionBuilder;
        }

        private static Dictionary<SolverType, Type> BuilderList = FillSolverTypes();

        private static Dictionary<SolverType, Type> FillSolverTypes()
        {
            Dictionary<SolverType, Type> _builderList = new Dictionary<SolverType, Type>();
            _builderList.Add(SolverType.Regular, typeof(PrimalSolutionBuilder));
            _builderList.Add(SolverType.Revised, typeof(RevisedSolutionBuilder));
            return _builderList;
        }
    }
}
