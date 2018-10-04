using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Simplex.Model;
using Simplex.Enums;
using Simplex.Helper;

namespace Simplex.Analysis
{
    public static class RevisedSimplexExtension
    {
        internal static void GenerateBasisMatrices(this RevisedSimplexModel model)
        {

            if (model.IsTwoPhase)
                //only artficial variables            
                GeneratePhaseMatrices(model);

            //all of the basic variables
            GeneratePrimalMatrices(model);
        }


        private static void GeneratePrimalMatrices(RevisedSimplexModel model)
        {
            /*
             *1-amaç fonksiyonda sadece temel değişkenler kalacak şekilde daralt
            */
            List<Term> tmp_SubjectArtificalTerms = null;
            int tmp_rowIndex = 0;
            int tmp_colIndex = 0;
            int rowCount = model.Subjects.Count;
            int columnCount = model.PhaseObjectiveFunction.Terms.Count;
            int tmp_basicCount = model.ObjectiveFunction.Terms.Where(term => term.isBasic == true).Count<Term>();

            Matrix tmp_BasisMatrix = new Matrix(tmp_basicCount, tmp_basicCount);
            Matrix tmp_NonBasisMatrix = new Matrix(model.Subjects.Count, model.ObjectiveFunction.Terms.Count);
            Matrix tmp_BasisRightHandMatrix = new Matrix(model.Subjects.Count, 2);
            Matrix tmp_BasisObjectiveMatrix = new Matrix(1, tmp_basicCount);
            Matrix tmp_BasisNonObjectiveMatrix = new Matrix(1, model.ObjectiveFunction.Terms.Count);

            tmp_colIndex = 0;
            for (int j = 0; j < model.ObjectiveFunction.Terms.Count; j++)
            {
                //tmp_types[j] = model.ObjectiveFunction.Terms[j].VarType;
                tmp_BasisNonObjectiveMatrix[0, j] = model.ObjectiveFunction.Terms[j].Factor;
                if (model.ObjectiveFunction.Terms[j].isBasic)
                {
                    tmp_BasisObjectiveMatrix[0, tmp_colIndex] = 0;
                    tmp_colIndex++;
                }
            }

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    tmp_NonBasisMatrix[i, j] = model.Subjects[i].Terms[j].Factor;
                }
                tmp_BasisRightHandMatrix[i, 0] = model.Subjects[i].RightHandValue;
                tmp_BasisRightHandMatrix[i, 1] = -1;
            }

            tmp_rowIndex = 0;
            for (int i = 0; i < rowCount; i++)
            {
                //reset temrory column index
                tmp_colIndex = 0;
                //get list of basic term from current(i) subject.
                //tmp_SubjectArtificalTerms = model.Subjects[i].Terms.Where(term => term.VarType == VariableType.Artificial).ToList();
                tmp_SubjectArtificalTerms = model.Subjects[i].Terms.Where(term => term.isBasic).ToList();
                //we assume this subject row does not contain member of unit matrix artificial term
                //if (tmp_SubjectArtificalTerms.Any(term => term.VarType == VariableType.Artificial && term.Factor == 1))
                if (tmp_SubjectArtificalTerms.Any(term => term.isBasic && term.Factor == 1))
                {
                    tmp_BasisRightHandMatrix[tmp_rowIndex, 1] = -1;
                    for (int j = 0; j < model.ObjectiveFunction.Terms.Count; j++)
                    {
                        //we add absolutely column element to the nonbasis matrix.
                        tmp_NonBasisMatrix[tmp_rowIndex, j] = model.Subjects[i].Terms[j].Factor;
                        //we add selectively column element to the basis matrix .
                        //if (model.Subjects[i].Terms[j].VarType == VariableType.Artificial)
                        if (model.ObjectiveFunction.Terms[j].isBasic)// && model.Subjects[i].Terms[j].Factor==1)
                        {
                            tmp_BasisMatrix[tmp_rowIndex, tmp_colIndex] = tmp_NonBasisMatrix[tmp_rowIndex, j];
                            //basic variable flag
                            if (tmp_BasisMatrix[tmp_rowIndex, tmp_colIndex] == 1 && tmp_rowIndex == tmp_colIndex)
                            {
                                tmp_BasisRightHandMatrix[tmp_rowIndex, 1] = j;
                            }
                            tmp_colIndex++;
                        }
                    }
                    tmp_BasisRightHandMatrix[tmp_rowIndex, 0] = model.Subjects[i].RightHandValue;
                    model.ObjectiveCost -= model.Subjects[i].RightHandValue;
                    tmp_rowIndex++;
                }
            }

            //if model does not have two phase solution

            model.BasisMatrix = tmp_BasisMatrix;
            model.NonBasisMatrix = tmp_NonBasisMatrix;
            model.BasisObjectiveMatrix = tmp_BasisObjectiveMatrix;
            model.BasisRightHandMatrix = tmp_BasisRightHandMatrix;
            model.BasisNonObjectiveMatrix = tmp_BasisNonObjectiveMatrix;
        }

        private static void GeneratePhaseMatrices(RevisedSimplexModel model)
        {
            /*
             *  -eğer iki aşamalı ise amaç fonksiyonu sadece yapay değişkenler cinsinden yazılır  ve sadece onlar temel değişkendir.
             */
            List<Term> tmp_SubjectArtificalTerms = null;
            int tmp_rowIndex = 0;
            int tmp_colIndex = 0;
            int rowCount = model.Subjects.Count;
            int columnCount = model.PhaseObjectiveFunction.Terms.Count;
            int tmp_basicCount = model.PhaseObjectiveFunction.Terms.Where(term => term.VarType == VariableType.Artificial).Count<Term>();

            Matrix tmp_PhaseOneBasisMatrix = null;
            Matrix tmp_PhaseOneNonBasisMatrix = null;
            Matrix tmp_PhaseOneBasisRightHandMatrix = null;
            Matrix tmp_PhaseOneBasisObjectiveMatrix = null;
            Matrix tmp_PhaseOneNonBasisObjectiveMatrix = null;

            tmp_rowIndex = 0;
            tmp_colIndex = 0;

            tmp_PhaseOneBasisObjectiveMatrix = new Matrix(1, tmp_basicCount);
            tmp_PhaseOneNonBasisObjectiveMatrix = new Matrix(1, model.PhaseObjectiveFunction.Terms.Count);
            tmp_PhaseOneBasisRightHandMatrix = new Matrix(tmp_basicCount, 2);
            tmp_PhaseOneBasisMatrix = new Matrix(tmp_basicCount, tmp_basicCount);
            tmp_PhaseOneNonBasisMatrix = new Matrix(tmp_basicCount, model.PhaseObjectiveFunction.Terms.Count);


            for (int j = 0; j < model.PhaseObjectiveFunction.Terms.Count; j++)
            {
                tmp_PhaseOneNonBasisObjectiveMatrix[0, j] = model.PhaseObjectiveFunction.Terms[j].Factor;
                if (model.PhaseObjectiveFunction.Terms[j].VarType == VariableType.Artificial)
                {
                    tmp_PhaseOneBasisObjectiveMatrix[0, tmp_colIndex] =  model.PhaseObjectiveFunction.Terms[j].Factor;
                    tmp_colIndex++;
                }
            }

            model.ObjectiveCost =  model.PhaseObjectiveFunction.RightHandValue;

            //reset the variables
            tmp_SubjectArtificalTerms = null;
            tmp_rowIndex = 0;
            tmp_colIndex = 0;
            for (int i = 0; i < rowCount; i++)
            {
                //reset temrory column index
                tmp_colIndex = 0;
                //get list of basic term from current(i) subject.
                tmp_SubjectArtificalTerms = model.Subjects[i].Terms.Where(term => term.VarType == VariableType.Artificial).ToList();
                //we assume this subject row does not contain member of unit matrix artificial term
                if (tmp_SubjectArtificalTerms.Any(term => term.VarType == VariableType.Artificial && term.Factor == 1))
                {
                    tmp_PhaseOneBasisRightHandMatrix[tmp_rowIndex, 1] = -1;
                    for (int j = 0; j < model.PhaseObjectiveFunction.Terms.Count; j++)
                    {
                        //we add absolutely column element to the nonbasis matrix.
                        tmp_PhaseOneNonBasisMatrix[tmp_rowIndex, j] = model.Subjects[i].Terms[j].Factor;
                        //we add selectively column element to the basis matrix .
                        if (model.Subjects[i].Terms[j].VarType == VariableType.Artificial)
                        {
                            tmp_PhaseOneBasisMatrix[tmp_rowIndex, tmp_colIndex] = tmp_PhaseOneNonBasisMatrix[tmp_rowIndex, j];
                            //basic variable flag
                            if (tmp_PhaseOneBasisMatrix[tmp_rowIndex, tmp_colIndex] == 1 && tmp_rowIndex == tmp_colIndex)
                            {
                                tmp_PhaseOneBasisRightHandMatrix[tmp_rowIndex, 1] = j;
                            }
                            tmp_colIndex++;
                        }
                    }
                    tmp_PhaseOneBasisRightHandMatrix[tmp_rowIndex, 0] = model.Subjects[i].RightHandValue;
                    tmp_rowIndex++;
                }
            }

            model.PhaseOneBasisMatrix = tmp_PhaseOneBasisMatrix;
            model.PhaseOneBasisObjectiveMatrix = tmp_PhaseOneBasisObjectiveMatrix;
            model.PhaseOneNonBasisMatrix = tmp_PhaseOneNonBasisMatrix;
            model.PhaseOneBasisRightHandMatrix = tmp_PhaseOneBasisRightHandMatrix;
            model.PhaseNonOneBasisObjectiveMatrix = tmp_PhaseOneNonBasisObjectiveMatrix;
        }

        internal static void CreatePhaseOneObjective(this RevisedSimplexModel model)
        {
            if (!model.IsTwoPhase)
                return;

            //Steps
            //#.Modify the constraints so that the RHS of each constraint is nonnegative (This requires that each constraint with a negative RHS be multiplied by - 1.Remember that if you multiply an inequality by any negative number, the direction of the inequality is reversed!). After modification, identify each constraint as a ≤, ≥ or = constraint.
            //#.Convert each inequality constraint to standard form(If constraint i is a ≤ constraint, we add a slack variable si; and if constraint i is a ≥ constraint, we subtract an excess variable ei).

            //Steps
            //1.Add an artificial variable ai to the constraints identified as ≥ or = constraints at the end of Step 1.Also add the sign restriction ai ≥ 0.
            //2.In the phase I, ignore the original LP’s objective function, instead solve an LP whose objective function is minimizing w = ai(sum of all the artificial variables).The act of solving the Phase I LP will force the artificial variables to be zero.
            //3.Since each artificial variable will be in the starting basis, all artificial variables must be eliminated from row 0 before beginning the simplex. Now solve the transformed problem by the simplex.

            //1) add all of artificial variables in orginal objective function to the phaseonefunction
            //Get the list of artificial variables in orginal objective function
            List<Term> tmp_artificialterms = model.ObjectiveFunction.Terms.Where(term => term.VarType == VariableType.Artificial).ToList();
            foreach (Term item in tmp_artificialterms)
            {
                model.PhaseObjectiveFunction.Terms.Add(new Term() { Factor = 1, VarType = item.VarType, Vector = item.Vector });
            }

            ////2) change signt the factor value of term in new objective fonction terms and add positive balance variable ("w")
            foreach (Term term in model.PhaseObjectiveFunction.Terms)
            {
                term.Factor *= -1;
            }
            //3) add balance type variable ("w") to the new objective function
            //model.PhaseObjectiveFunction.Terms.Insert(0, new Term() { Factor = 1, VarType = VariableType.Balance, Vector = "w", Index = 0 });

            //Let us define new objective function as negative (-w)
            model.PhaseObjectiveFunction.RightHandValue = 0;

            //4) find all artificial variable that has factor value is equal to 0 in cosntarint terms and put the artificial variable value in the new objective function;
            //   all artificial variables must be eliminated from row 0 before we can solve Phase I
            Term tmp_term = null;
            foreach (Subject constraint in model.Subjects)
            {
                if (constraint.Terms.Any(term => term.VarType == VariableType.Artificial && term.Factor == 1))
                {
                    //Find 
                    List<Term> tmp_willaddedterms = constraint.Terms.Where(term => term.Factor != 0).ToList();
                    foreach (Term item in tmp_willaddedterms)
                    {
                        tmp_term = null;
                        tmp_term = model.PhaseObjectiveFunction.Terms.Find(term => term.Vector.Equals(item.Vector));
                        if (tmp_term != null)
                            tmp_term.Factor += item.Factor;
                        else
                            model.PhaseObjectiveFunction.Terms.Add(new Term() { Factor = item.Factor, VarType = item.VarType, Vector = item.Vector });
                    }
                    if (tmp_willaddedterms.Count > 0)
                    {
                        model.PhaseObjectiveFunction.RightHandValue += constraint.RightHandValue;
                    }
                }
            }

            //5)Add other variable that has zero factor value from objective function
            foreach (Term item in model.ObjectiveFunction.Terms)
            {
                if (item.VarType == VariableType.Balance)
                    continue;
                if (!model.PhaseObjectiveFunction.IsVectorContained(item.Vector))
                    model.PhaseObjectiveFunction.Terms.Add(new Term() { Vector = item.Vector, Factor = 0, VarType = item.VarType });
            }

            //6) sort the terms of new objective 
            TermComparer tc = new TermComparer();
            model.PhaseObjectiveFunction.Terms.Sort(tc);
        }

        internal static void PhaseOnePrintMatrix(this StandartSimplexModel model)
        {
            model.PrintMatrix();

            string tmp_sign = string.Empty;
            System.Diagnostics.Debug.WriteLine("Goal :" + model.GoalType.ToString());
            System.Diagnostics.Debug.Write("New Objective Function - " + model.GoalType.ToString() + ": ");
            foreach (Term item in model.PhaseObjectiveFunction.Terms)
            {
                tmp_sign = string.Empty;
                if (Math.Sign(item.Factor) > -1)
                    tmp_sign = "+";
                System.Diagnostics.Debug.Write(tmp_sign + item.Factor + "*" + item.Vector + " ");
            }
            System.Diagnostics.Debug.Write(" = ");
            System.Diagnostics.Debug.WriteLine(model.PhaseObjectiveFunction.RightHandValue);
            System.Diagnostics.Debug.WriteLine("*********************************");
        }

        internal static void TruncatePhaseResult(this RevisedSimplexModel model, Solution solution)
        {
            //transfer the phaseoneobjective function factors
            //Crete temprory matrix for new objective  operation 
            //http://myweb.usf.edu/~molla/2015_spring_math588/example4.pdf
            Matrix tmp_OldCoEfficientMatrix = new Matrix(1, model.PhaseOneBasisRightHandMatrix.RowCount);
            Matrix tmp_NewCoEfficientMatrix = null;
            Matrix tmp_NewObjectiveCostMatrix = null;

            for (int i = 0; i < model.PhaseOneBasisRightHandMatrix.RowCount; i++)
            {
                //for (-z) multiply (-1)
                tmp_OldCoEfficientMatrix[0, i] = (-1) * model.BasisNonObjectiveMatrix[0, (int)model.PhaseOneBasisRightHandMatrix[i, 1]];
                //tmp_OldCoEfficientMatrix[0, i] = model.BasisNonObjectiveMatrix[0, (int)model.PhaseOneBasisRightHandMatrix[i, 1]];
            }

            tmp_NewCoEfficientMatrix = tmp_OldCoEfficientMatrix * model.PhaseOneBasisMatrix ;
            tmp_NewObjectiveCostMatrix = tmp_NewCoEfficientMatrix * model.PhaseOneBasisRightHandMatrix.GetCol(0);
            model.ObjectiveCost = tmp_NewObjectiveCostMatrix[0, 0];

            model.BasisMatrix = model.PhaseOneBasisMatrix;
            model.BasisRightHandMatrix = model.PhaseOneBasisRightHandMatrix;
            model.BasisObjectiveMatrix = tmp_NewCoEfficientMatrix;

            //for (int i = 0; i < model.BasisRightHandMatrix.RowCount; i++)
            //{
            //    model.BasisRightHandMatrix[i,1]= - 1;
            //}


            model.NonBasisMatrix = model.PhaseOneNonBasisMatrix;
            model.BasisNonObjectiveMatrix = model.PhaseNonOneBasisObjectiveMatrix;

        }
   
    }
}
