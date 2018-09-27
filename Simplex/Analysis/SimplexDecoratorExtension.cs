using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Simplex.Enums;
using Simplex.Model;

namespace Simplex.Analysis
{
    internal static  class SimplexDecoratorExtension
    {
        internal static TestMessage CheckBFS(this SimplexModelDecorator model)
        {
            TestMessage retval = new TestMessage() { Exception = null, Message = string.Empty };

            //first, update rhs values for equality direction control
            UpdateNegativeRHSValues(model);

            foreach (Subject constarint in model.Subjects)
            {
                if (constarint.Equality != EquailtyType.LessEquals)
                {
                    retval.Exception = new ArithmeticException();
                    retval.Message += "Constraint has Right-Hand Side Value " + constarint.RightHandValue.ToString() + " is " + constarint.Equality.ToString() + " different from " + EquailtyType.LessEquals.ToString() + "\n";
                }
            }
            if (retval.Exception == null)
                retval.Message = "Success";

            return retval;
        }

        internal static void UpdateNegativeRHSValues(this SimplexModelDecorator model)
        {
            //1) Check and update Right Hand Side- RHS value for positive 
            foreach (Subject constarint in model.Subjects)
            {
                if (constarint.RightHandValue < 0)
                {
                    constarint.RightHandValue *= -1;
                    foreach (Term term in constarint.Terms)
                    {
                        term.Factor *= -1;
                    }

                    if (constarint.Equality == EquailtyType.GreaterEquals)
                        constarint.Equality = EquailtyType.LessEquals;
                    else if (constarint.Equality == EquailtyType.LessEquals)
                        constarint.Equality = EquailtyType.GreaterEquals;
                    //nothing to do for equal, equal is equal
                }
            }
        }

        internal static void CreatePhaseOneObjective(this SimplexModelDecorator model, bool regularSimplex)
        {
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
                    if (regularSimplex)
                    {
                        foreach (Term item in tmp_willaddedterms)
                        {
                            tmp_term = null;
                            tmp_term = model.PhaseObjectiveFunction.Terms.Find(term => term.Vector.Equals(item.Vector));
                            if (tmp_term != null)
                                tmp_term.Factor += item.Factor;
                            else
                                model.PhaseObjectiveFunction.Terms.Add(new Term() { Factor = item.Factor, VarType = item.VarType, Vector = item.Vector });
                        }
                    }
                    if (tmp_willaddedterms.Count > 0)
                    {
                        if (regularSimplex)
                            model.PhaseObjectiveFunction.RightHandValue += constraint.RightHandValue;
                        else
                            model.PhaseObjectiveFunction.RightHandValue -= constraint.RightHandValue;
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
    }
}
