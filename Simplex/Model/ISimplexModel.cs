using System;
using System.Collections.Generic;
using System.Text;
using Simplex.Enums;

namespace Simplex.Model
{
    public interface ISimplexModel
    {
        ObjectiveType GoalType { get; set; }

        Subject ObjectiveFunction { get; set; }
        List<Subject> Subjects { get; set; }
    }
}
