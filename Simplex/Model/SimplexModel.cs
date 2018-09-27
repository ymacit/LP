//***************************
//Sınıf Adı : SimplexModel 
//Dosya Adı : SimplexModel.cs 
//Tanım : amaç ve ksıtlardan oluşan modeli tanımlar ve standart hale getirir
//****************************

using System;
using System.Collections.Generic;
using System.Text;
using Simplex.Enums;

namespace Simplex.Model
{
    [Serializable]
    public class SimplexModel: ISimplexModel
    {
        Subject m_objective = new Subject();
        List<Subject> m_subjects = new List<Subject>();
        ObjectiveType m_ObjectiveType = ObjectiveType.Minumum;

        public ObjectiveType GoalType
        {
            get { return m_ObjectiveType; }
            set { m_ObjectiveType = value; }
        }

        public Subject ObjectiveFunction
        {
            get { return m_objective; }
            set { m_objective = value; }
        }

        public List<Subject> Subjects
        {
            get { return m_subjects; }
            set { m_subjects = value; }
        }
    }

    public struct TestMessage
    {
        public Exception Exception { get; set; }
        public string Message { get; set; }
    }
}
