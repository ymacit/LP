using System;
using System.Collections.Generic;
using System.Text;
using Simplex.Model;
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

            /*
             *              Phase 1
             * +0,000 +0,000 +0,000 -0,001 -0,999 -1,001  = -0,006
                 *******Constarints*****    
            +0,000 +0,000 +1,000 -0,125 +0,125 -0,625  = 0,2490  | 2
            +0,000 +1,000 +0,000 -0,499 +0,499 -0,499  = 5,0030  | 1
            +1,000 +0,000 +0,000 +0,499 -0,499 +1,499  = 4,9970  | 0

                        Phase 2
                        *************************
            ***         Solution
            ***         Optimal Value :24,997            ***
            *** Variable | Slack      | s1 =      0,249 | ***
            *** Variable | Original   | X2 =      5,003 | ***
            *** Variable | Original   | X1 =      4,997 | ***
            *** Variable | Original   | X1 =     24,997 | ***
            *************************
            * 
            */
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

            /*
             **************************
             ***         Solution
             ***         Optimal Value :82,000            ***
             *** Variable | Excess     | e4 =      5,500 | ***
             *** Variable | Original   | X1 =     85,000 | ***
             *** Variable | Original   | X2 =     85,000 | ***
             *** Variable | Original   | X3 =     77,500 | ***
             *** Variable | Original   | X4 =     70,000 | ***
             *** Variable | Excess     | e2 =     10,000 | ***
             *** Variable | Excess     | e3 =     10,000 | ***
             *** Variable | Slack      | s3 =     12,500 | ***
             *** Variable | Slack      | s4 =     18,000 | ***
             ************************* 
            */
            return simplex;
        }

        internal static SimplexModel CreateSimplexModel4()
        {
            //source :http://www.ahmetaksoy.com.tr/endustri-muhendisligi/yoneylem-arastirmasinda-primal-simpleks-yontemi.html
            /*
             * Max Z = 3X1 + 4X2
             *Hammadde Kısıtı;
             *15X1 + 10X2 <= 150
             *İşçilik Kısıtı:
             *2.5X1 + 5X2 <= 55
             *X1,X2 >=0
            */
            SimplexModel simplex = new SimplexModel();

            simplex.GoalType = ObjectiveType.Maximum;
            simplex.ObjectiveFunction = new Subject() { Index = 0, RowLabel = "RO" };
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 3, VarType = VariableType.Original, Vector = "X1" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 4, VarType = VariableType.Original, Vector = "X2" });

            Subject subject1 = new Subject() { Index = 1, RowLabel = "R1", Equality = EquailtyType.LessEquals, RightHandValue = 150 };
            subject1.Terms.Add(new Term() { Factor = 15, VarType = VariableType.Original, Vector = "X1" });
            subject1.Terms.Add(new Term() { Factor = 10, VarType = VariableType.Original, Vector = "X2" });

            Subject subject2 = new Subject() { Index = 2, RowLabel = "R2", Equality = EquailtyType.LessEquals, RightHandValue = 55 };
            subject2.Terms.Add(new Term() { Factor = 2.5, VarType = VariableType.Original, Vector = "X1" });
            subject2.Terms.Add(new Term() { Factor = 5, VarType = VariableType.Original, Vector = "X2" });


            simplex.Subjects = new System.Collections.Generic.List<Subject>() { subject1, subject2 };

            return simplex;
        }

        internal static SimplexModel CreateSimplexModel5()
        {
            //OR_End331a.pdf two phase sample 2
            SimplexModel simplex = new SimplexModel();

            simplex.GoalType = ObjectiveType.Maximum;
            simplex.ObjectiveFunction = new Subject() { Index = 0, RowLabel = "RO" };
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 40, VarType = VariableType.Original, Vector = "X1" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 10, VarType = VariableType.Original, Vector = "X2" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 7, VarType = VariableType.Original, Vector = "X5" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 14, VarType = VariableType.Original, Vector = "X6" });

            Subject subject1 = new Subject() { Index = 1, RowLabel = "R1", Equality = EquailtyType.Equals, RightHandValue = 0 };
            subject1.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X1" });
            subject1.Terms.Add(new Term() { Factor = -1, VarType = VariableType.Original, Vector = "X2" });
            subject1.Terms.Add(new Term() { Factor = 2, VarType = VariableType.Original, Vector = "X5" });

            Subject subject2 = new Subject() { Index = 2, RowLabel = "R2", Equality = EquailtyType.Equals, RightHandValue = 0 };
            subject2.Terms.Add(new Term() { Factor = -2, VarType = VariableType.Original, Vector = "X1" });
            subject2.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X2" });
            subject2.Terms.Add(new Term() { Factor = -2, VarType = VariableType.Original, Vector = "X5" });


            Subject subject3 = new Subject() { Index = 2, RowLabel = "R3", Equality = EquailtyType.Equals, RightHandValue = 3 };
            subject3.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X1" });
            subject3.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X3" });
            subject3.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X5" });
            subject3.Terms.Add(new Term() { Factor = -1, VarType = VariableType.Original, Vector = "X6" });

            Subject subject4 = new Subject() { Index = 2, RowLabel = "R4", Equality = EquailtyType.Equals, RightHandValue = 4 };
            subject4.Terms.Add(new Term() { Factor = 2, VarType = VariableType.Original, Vector = "X2" });
            subject4.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X3" });
            subject4.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X4" });
            subject4.Terms.Add(new Term() { Factor = 2, VarType = VariableType.Original, Vector = "X5" });
            subject4.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X6" });


            simplex.Subjects = new System.Collections.Generic.List<Subject>() { subject1, subject2, subject3, subject4 };

            return simplex;
        }

        internal static SimplexModel CreateSimplexModel6()
        {
            SimplexModel simplex = new SimplexModel();

            //Maks x1 + 2x2 – x3 + x4 + 4x5 – 2x6
            simplex.GoalType = ObjectiveType.Maximum;
            simplex.ObjectiveFunction = new Subject() { Index = 0, RowLabel = "RO" };
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X1" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 2, VarType = VariableType.Original, Vector = "X2" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = -1, VarType = VariableType.Original, Vector = "X3" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X4" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 4, VarType = VariableType.Original, Vector = "X5" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = -2, VarType = VariableType.Original, Vector = "X6" });

            Subject subject1 = new Subject() { Index = 1, RowLabel = "R1", Equality = EquailtyType.LessEquals, RightHandValue = 6 };
            subject1.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X1" });
            subject1.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X2" });
            subject1.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X3" });
            subject1.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X4" });
            subject1.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X5" });
            subject1.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X6" });

            Subject subject2 = new Subject() { Index = 2, RowLabel = "R2", Equality = EquailtyType.LessEquals, RightHandValue = 4 };
            subject2.Terms.Add(new Term() { Factor = 2, VarType = VariableType.Original, Vector = "X1" });
            subject2.Terms.Add(new Term() { Factor = -1, VarType = VariableType.Original, Vector = "X2" });
            subject2.Terms.Add(new Term() { Factor = -2, VarType = VariableType.Original, Vector = "X3" });
            subject2.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X4" });


            Subject subject3 = new Subject() { Index = 2, RowLabel = "R3", Equality = EquailtyType.LessEquals, RightHandValue = 4 };
            subject3.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X3" });
            subject3.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X4" });
            subject3.Terms.Add(new Term() { Factor = 2, VarType = VariableType.Original, Vector = "X5" });
            subject3.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X6" });


            simplex.Subjects = new System.Collections.Generic.List<Subject>() { subject1, subject2, subject3 };

            /*
             * Regular Result
             * *************************
             ***         Solution
             ***         Optimal Value :16,000            ***
             *** Variable | Original   | X2 =      4,000 | ***
             *** Variable | Slack      | s2 =      8,000 | ***
             *** Variable | Original   | X5 =      2,000 | ***
             *************************
             * 
            */

            return simplex;
        }

        internal static SimplexModel CreateSimplexModel7()
        {
            SimplexModel simplex = new SimplexModel();

            //Maks 2x1 + 3x2 - 4x3 + 3x4 + x5 - 4x6 + 6X7
            simplex.GoalType = ObjectiveType.Maximum;
            simplex.ObjectiveFunction = new Subject() { Index = 0, RowLabel = "RO" };
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 2, VarType = VariableType.Original, Vector = "X1" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 3, VarType = VariableType.Original, Vector = "X2" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = -4, VarType = VariableType.Original, Vector = "X3" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 3, VarType = VariableType.Original, Vector = "X4" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X5" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = -4, VarType = VariableType.Original, Vector = "X6" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 6, VarType = VariableType.Original, Vector = "X7" });

            Subject subject1 = new Subject() { Index = 1, RowLabel = "R1", Equality = EquailtyType.Equals, RightHandValue = 3 };
            subject1.Terms.Add(new Term() { Factor = 3, VarType = VariableType.Original, Vector = "X2" });
            subject1.Terms.Add(new Term() { Factor = 3, VarType = VariableType.Original, Vector = "X3" });
            subject1.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X4" });
            subject1.Terms.Add(new Term() { Factor = -1, VarType = VariableType.Original, Vector = "X5" });
            subject1.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X6" });
            subject1.Terms.Add(new Term() { Factor = -5, VarType = VariableType.Original, Vector = "X7" });

            Subject subject2 = new Subject() { Index = 2, RowLabel = "R2", Equality = EquailtyType.Equals, RightHandValue = 4 };
            subject2.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X1" });
            subject2.Terms.Add(new Term() { Factor = 3, VarType = VariableType.Original, Vector = "X2" });
            subject2.Terms.Add(new Term() { Factor = -1, VarType = VariableType.Original, Vector = "X3" });
            subject2.Terms.Add(new Term() { Factor = -1, VarType = VariableType.Original, Vector = "X5" });
            subject2.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X6" });
            subject2.Terms.Add(new Term() { Factor = 3, VarType = VariableType.Original, Vector = "X7" });


            Subject subject3 = new Subject() { Index = 2, RowLabel = "R3", Equality = EquailtyType.Equals, RightHandValue = 5 };
            subject3.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X1" });
            subject3.Terms.Add(new Term() { Factor = 2, VarType = VariableType.Original, Vector = "X2" });
            subject3.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X4" });
            subject3.Terms.Add(new Term() { Factor = 2, VarType = VariableType.Original, Vector = "X5" });
            subject3.Terms.Add(new Term() { Factor = -2, VarType = VariableType.Original, Vector = "X7" });


            simplex.Subjects = new System.Collections.Generic.List<Subject>() { subject1, subject2, subject3 };

            /*
             *
            *************************
            ***         Solution
            ***         Optimal Value :30,907            ***
            *** Variable | Original   | X3 =      1,000 | *** 3/3
            *** Variable | Original   | X7 =      1,662 | *** 5/3
            *** Variable | Original   | X4 =      8,312 | ***25/3
            *************************
            */

            return simplex;
        }

        internal static SimplexModel CreateSimplexModel8()
        {
            SimplexModel simplex = new SimplexModel();
            /*
             * http://myweb.usf.edu/~molla/2015_spring_math588/example4.pdf
             * Minimize z = −19x1 − 13x2 − 12x3 − 17x4
             * subject to
             *              3x1 +2x2 +x3 +2x4 = 225,
             *              x1 +x2 +x3 +x4 = 117,
             *              4x1 +3x2 +3x3 +4x4 = 420
             *              x1, x2, x3, x4 ≥ 0.
            */

            simplex.GoalType = ObjectiveType.Minumum;
            simplex.ObjectiveFunction = new Subject() { Index = 0, RowLabel = "RO" };
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = -19, VarType = VariableType.Original, Vector = "X1" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = -13, VarType = VariableType.Original, Vector = "X2" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = -12, VarType = VariableType.Original, Vector = "X3" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = -17, VarType = VariableType.Original, Vector = "X4" });

            Subject subject1 = new Subject() { Index = 1, RowLabel = "R1", Equality = EquailtyType.Equals, RightHandValue = 225 };
            subject1.Terms.Add(new Term() { Factor = 3, VarType = VariableType.Original, Vector = "X1" });
            subject1.Terms.Add(new Term() { Factor = 2, VarType = VariableType.Original, Vector = "X2" });
            subject1.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X3" });
            subject1.Terms.Add(new Term() { Factor = 2, VarType = VariableType.Original, Vector = "X4" });

            Subject subject2 = new Subject() { Index = 2, RowLabel = "R2", Equality = EquailtyType.Equals, RightHandValue = 117 };
            subject2.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X1" });
            subject2.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X2" });
            subject2.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X3" });
            subject2.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X4" });


            Subject subject3 = new Subject() { Index = 2, RowLabel = "R3", Equality = EquailtyType.Equals, RightHandValue = 420 };
            subject3.Terms.Add(new Term() { Factor = 4, VarType = VariableType.Original, Vector = "X1" });
            subject3.Terms.Add(new Term() { Factor = 3, VarType = VariableType.Original, Vector = "X2" });
            subject3.Terms.Add(new Term() { Factor = 3, VarType = VariableType.Original, Vector = "X3" });
            subject3.Terms.Add(new Term() { Factor = 4, VarType = VariableType.Original, Vector = "X4" });

            simplex.Subjects = new System.Collections.Generic.List<Subject>() { subject1, subject2, subject3 };

            /*
             * http://myweb.usf.edu/~molla/2015_spring_math588/example4.pdf
            *************************
            ***         Solution
            ***         Optimal Value :214,451            ***
            *** Variable | Original   | X1 =     39,000 | ***
            *** Variable | Original   | X3 =     48,000 | ***
            *** Variable | Original   | X4 =     30,000 | ***
            *************************
             */

            return simplex;
        }

        internal static SimplexModel CreateSimplexModel9()
        {
            SimplexModel simplex = new SimplexModel();
            /*
             * https://www.utdallas.edu/~chandra/documents/lp/revsimpl.pdf
             *  min 40x1 + 36x2
             *  x1 ≤ 8
             *  x2 ≤ 10
             *  5x1 + 3x2 ≥ 45
             *  x1 ≥ 0; x2 ≥ 0
            */

            simplex.GoalType = ObjectiveType.Minumum;
            simplex.ObjectiveFunction = new Subject() { Index = 0, RowLabel = "RO" };
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 40, VarType = VariableType.Original, Vector = "X1" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 36, VarType = VariableType.Original, Vector = "X2" });

            Subject subject1 = new Subject() { Index = 1, RowLabel = "R1", Equality = EquailtyType.LessEquals, RightHandValue = 8 };
            subject1.Terms.Add(new Term() { Factor = 3, VarType = VariableType.Original, Vector = "X1" });

            Subject subject2 = new Subject() { Index = 2, RowLabel = "R2", Equality = EquailtyType.LessEquals, RightHandValue = 10 };
            subject2.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X2" });


            Subject subject3 = new Subject() { Index = 2, RowLabel = "R3", Equality = EquailtyType.GreaterEquals, RightHandValue = 45 };
            subject3.Terms.Add(new Term() { Factor = 5, VarType = VariableType.Original, Vector = "X1" });
            subject3.Terms.Add(new Term() { Factor = 3, VarType = VariableType.Original, Vector = "X2" });

            simplex.Subjects = new System.Collections.Generic.List<Subject>() { subject1, subject2, subject3 };


            return simplex;
        }

        internal static SimplexModel CreateSimplexModel10()
        {
            SimplexModel simplex = new SimplexModel();
            /*
             * https://www.utdallas.edu/~chandra/documents/lp/revsimpl.pdf
                max (z =) x1 + 3x3
                s.t. x1 + 3x2 − x3 + 2x4 = 5
                x1 − 3x2 + 5x3 − 4x4 = −1
                x1 , x2 , x3 , x4  0
            */

            simplex.GoalType = ObjectiveType.Maximum;
            simplex.ObjectiveFunction = new Subject() { Index = 0, RowLabel = "RO" };
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X1" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 3, VarType = VariableType.Original, Vector = "X3" });

            Subject subject1 = new Subject() { Index = 1, RowLabel = "R1", Equality = EquailtyType.Equals, RightHandValue = 5 };
            subject1.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X1" });
            subject1.Terms.Add(new Term() { Factor = 3, VarType = VariableType.Original, Vector = "X2" });
            subject1.Terms.Add(new Term() { Factor = -1, VarType = VariableType.Original, Vector = "X3" });
            subject1.Terms.Add(new Term() { Factor = 2, VarType = VariableType.Original, Vector = "X4" });

            Subject subject2 = new Subject() { Index = 2, RowLabel = "R3", Equality = EquailtyType.GreaterEquals, RightHandValue = -1 };
            subject2.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X1" });
            subject2.Terms.Add(new Term() { Factor = -3, VarType = VariableType.Original, Vector = "X2" });
            subject2.Terms.Add(new Term() { Factor = 5, VarType = VariableType.Original, Vector = "X3" });
            subject2.Terms.Add(new Term() { Factor = -4, VarType = VariableType.Original, Vector = "X4" });

            simplex.Subjects = new System.Collections.Generic.List<Subject>() { subject1, subject2 };


            return simplex;
        }

        internal static SimplexModel CreateSimplexModel11()
        {
            SimplexModel simplex = new SimplexModel();
            /*
             * https://www.utdallas.edu/~chandra/documents/lp/revsimpl.pdf
                max (z =) x1 + 3x3
                s.t. x1 + 3x2 − x3 + 2x4 = 5
                x1 − 3x2 + 5x3 − 4x4 = −1
                x1 , x2 , x3 , x4  0
            */

            simplex.GoalType = ObjectiveType.Minumum;
            simplex.ObjectiveFunction = new Subject() { Index = 0, RowLabel = "RO" };
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = -1, VarType = VariableType.Original, Vector = "X1" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = -2, VarType = VariableType.Original, Vector = "X2" });
            simplex.ObjectiveFunction.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X3" });

            Subject subject1 = new Subject() { Index = 1, RowLabel = "R1", Equality = EquailtyType.LessEquals, RightHandValue = 4 };
            subject1.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X1" });
            subject1.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X2" });
            subject1.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X3" });

            Subject subject2 = new Subject() { Index = 2, RowLabel = "R3", Equality = EquailtyType.LessEquals, RightHandValue = 6 };
            subject2.Terms.Add(new Term() { Factor = -1, VarType = VariableType.Original, Vector = "X1" });
            subject2.Terms.Add(new Term() { Factor = 2, VarType = VariableType.Original, Vector = "X2" });
            subject2.Terms.Add(new Term() { Factor = -2, VarType = VariableType.Original, Vector = "X3" });

            Subject subject3 = new Subject() { Index = 2, RowLabel = "R3", Equality = EquailtyType.LessEquals, RightHandValue = 5 };
            subject3.Terms.Add(new Term() { Factor = 2, VarType = VariableType.Original, Vector = "X1" });
            subject3.Terms.Add(new Term() { Factor = 1, VarType = VariableType.Original, Vector = "X2" });

            simplex.Subjects = new System.Collections.Generic.List<Subject>() { subject1, subject2, subject3 };


            return simplex;
        }
    }
}
