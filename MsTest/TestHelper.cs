using System;
using System.Collections.Generic;
using System.Text;
using Simplex.Problem;
using Simplex.Enums;
using Simplex.Analysis;
using Simplex.Helper;

namespace MsTest
{
    internal class TestHelper
    {
        internal static SimplexModel CreateSimplexModel1()
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

            simplex.Subjects = new System.Collections.Generic.List<Subject>() { subject1, subject2, subject3 };

            return simplex;
        }

        internal static SimplexModel CreateSimplexModel2()
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


        internal static SimplexModel CreateSimplexModel3()
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
