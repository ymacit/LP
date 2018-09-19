using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simplex.Problem;
using Simplex.Enums;
using Simplex.Analysis;

namespace MsTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SimplexModel_Copy_Test()
        {
            SimplexModel simplex = CreateSimplexModel2();
            SimplexModel simplex2 = simplex.DeepCopy();
            simplex2.GoalType = ObjectiveType.Maximum;
            simplex2.Subjects[0].Terms[0].Vector = "X1 Copy";
            var result = simplex.Subjects[0].Terms[0].Vector == simplex2.Subjects[0].Terms[0].Vector;
            Assert.IsFalse(result, "copy is not verified");          
        }

        [TestMethod]
        public void SimplexModel_BFS_Test()
        {
            SimplexModel simplex = CreateSimplexModel1();
            TestMessage testMessage = simplex.CheckBFS();
            Assert.IsNull(testMessage.Exception, testMessage.Message);
        }

        [TestMethod]
        public void StandartSimplexModel_Test()
        {
            SimplexModel simplex = CreateSimplexModel2();
            //simplex.PrintMatrix();
            SimplexModel standartModel = simplex.DeepCopy();
            standartModel.ConvertStandardModel();
            //standartModel.PrintMatrix();
            StandartSimplexModel phasemodel = new StandartSimplexModel(standartModel);
            phasemodel.CreatePhaseOneObjective();
            phasemodel.PhaseOnePrintMatrix();
            Matrix tmp_matrix = phasemodel.GetFullMatrix();
            System.Diagnostics.Debug.WriteLine( tmp_matrix.ToString());
            Assert.IsNull(standartModel, "success");
        }

        [TestMethod]
        public void SolveSimplexModel1_Test()
        {
            SimplexModel simplex = CreateSimplexModel3();
            //simplex.PrintMatrix();
            SimplexModel standartModel = simplex.DeepCopy();
            //standartModel.PrintMatrix();
            StandartSimplexModel phasemodel = new StandartSimplexModel(standartModel);
            Solver tmp_solver = new Solver();
            Solution tmp_solution = tmp_solver.Solve(phasemodel);
            //            System.Diagnostics.Debug.WriteLine(tmp_matrix.ToString());
            if (tmp_solution.Quality == SolutionQuality.Optimal)
            {
                System.Diagnostics.Debug.WriteLine("*************************");
                System.Diagnostics.Debug.WriteLine("***      Solution");
                foreach (ResultTerm term in tmp_solution.Results)
                {
                    string tmp_sign = string.Empty;
                    System.Diagnostics.Debug.WriteLine("*** Variable | " + term.VarType.ToString() + " | " + term.Vector +  " = " + term.Value.ToString("F5") + " | ***" );
                }
                System.Diagnostics.Debug.WriteLine("*************************");
            }
            Assert.AreEqual(SolutionQuality.Optimal, tmp_solution.Quality, "success");
        }

        private SimplexModel CreateSimplexModel1()
        {
            SimplexModel simplex = new SimplexModel();

            simplex.GoalType = ObjectiveType.Minumum;
            simplex.ObjectiveFunction = new Subject() { Index = 0, RowLabel = "RO" };
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 2, VarType = VariableType.Original, Vector = "X1" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 3, VarType = VariableType.Original, Vector = "X2" });

            Subject subject1 = new Subject() { Index = 1, RowLabel = "R1", Equality = EquailtyType.LessEquals, RightHandValue = 4 };
            subject1.Terms.Add(new Term() { Factor = 0.5, VarType = VariableType.Original, Vector = "X1" });
            subject1.Terms.Add(new Term() { Factor = 0.25, VarType = VariableType.Original, Vector = "X2" });

            Subject subject2 = new Subject() { Index = 2, RowLabel = "R2", Equality = EquailtyType.GreaterEquals, RightHandValue = 20 };
            subject2.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X1" });
            subject2.Terms.Add(new Term() { Factor = 3, VarType = VariableType.Original, Vector = "X2" });

            Subject subject3 = new Subject() { Index = 3, RowLabel = "R3", Equality = EquailtyType.Equals, RightHandValue = 10 };
            subject3.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X1" });
            subject3.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X2" });

            simplex.Subjects=new System.Collections.Generic.List<Subject>() {subject1, subject2, subject3};

            return simplex;
        }

        private SimplexModel CreateSimplexModel2()
        {
            SimplexModel simplex = new SimplexModel();

            simplex.GoalType = ObjectiveType.Minumum;
            simplex.ObjectiveFunction = new Subject() { Index = 0, RowLabel = "RO" };
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 4, VarType = VariableType.Original, Vector = "X1" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X2" });

            Subject subject1 = new Subject() { Index = 1, RowLabel = "R1", Equality = EquailtyType.Equals, RightHandValue = 3 };
            subject1.Terms.Add(new Term() { Factor = 3, VarType = VariableType.Original, Vector = "X1" });
            subject1.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X2" });

            Subject subject2 = new Subject() { Index = 2, RowLabel = "R2", Equality = EquailtyType.GreaterEquals, RightHandValue = 6 };
            subject2.Terms.Add(new Term() { Factor = 4, VarType = VariableType.Original, Vector = "X1" });
            subject2.Terms.Add(new Term() { Factor = 3, VarType = VariableType.Original, Vector = "X2" });

            Subject subject3 = new Subject() { Index = 3, RowLabel = "R3", Equality = EquailtyType.LessEquals, RightHandValue = 4 };
            subject3.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X1" });
            subject3.Terms.Add(new Term() { Factor = 2, VarType = VariableType.Original, Vector = "X2" });

            simplex.Subjects = new System.Collections.Generic.List<Subject>() { subject1, subject2, subject3 };

            return simplex;
        }

        private SimplexModel CreateSimplexModel3()
        {
            SimplexModel simplex = new SimplexModel();

            simplex.GoalType = ObjectiveType.Minumum;
            simplex.ObjectiveFunction = new Subject() { Index = 0, RowLabel = "RO" };
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 0.35, VarType = VariableType.Original, Vector = "X1" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 0.35, VarType = VariableType.Original, Vector = "X2" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 0.20, VarType = VariableType.Original, Vector = "X3" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 0.10, VarType = VariableType.Original, Vector = "X4" });

            Subject subject1 = new Subject() { Index = 1, RowLabel = "R1", Equality = EquailtyType.GreaterEquals, RightHandValue = 82 };
            subject1.Terms.Add(new Term() { Factor = 0.35, VarType = VariableType.Original, Vector = "X1" });
            subject1.Terms.Add(new Term() { Factor = 0.35, VarType = VariableType.Original, Vector = "X2" });
            subject1.Terms.Add(new Term() { Factor = 0.20, VarType = VariableType.Original, Vector = "X3" });
            subject1.Terms.Add(new Term() { Factor = 0.10, VarType = VariableType.Original, Vector = "X4" });

            Subject subject2 = new Subject() { Index = 2, RowLabel = "R2", Equality = EquailtyType.GreaterEquals, RightHandValue = 75 };
            subject2.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X1" });

            Subject subject3 = new Subject() { Index = 3, RowLabel = "R3", Equality = EquailtyType.GreaterEquals, RightHandValue = 75 };
            subject3.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X2" });


            Subject subject4 = new Subject() { Index = 4, RowLabel = "R4", Equality = EquailtyType.GreaterEquals, RightHandValue = 72 };
            subject4.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X3" });


            Subject subject5 = new Subject() { Index = 5, RowLabel = "R5", Equality = EquailtyType.GreaterEquals, RightHandValue = 70 };
            subject5.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X4" });


            Subject subject6 = new Subject() { Index = 6, RowLabel = "R6", Equality = EquailtyType.LessEquals, RightHandValue = 85 };
            subject6.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X1" });


            Subject subject7 = new Subject() { Index = 7, RowLabel = "R7", Equality = EquailtyType.LessEquals, RightHandValue = 85 };
            subject7.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X2" });


            Subject subject8 = new Subject() { Index = 8, RowLabel = "R8", Equality = EquailtyType.LessEquals, RightHandValue = 90 };
            subject8.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X3" });


            Subject subject9 = new Subject() { Index = 9, RowLabel = "R9", Equality = EquailtyType.LessEquals, RightHandValue = 88 };
            subject9.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X4" });

            //simplex.Subjects = new System.Collections.Generic.List<Subject>() { subject2, subject3, subject4, subject5, subject6, subject7, subject8, subject9 };
            simplex.Subjects = new System.Collections.Generic.List<Subject>() { subject1, subject2, subject3, subject4, subject5, subject6, subject7, subject8, subject9 };

            return simplex;
        }
    }
}
