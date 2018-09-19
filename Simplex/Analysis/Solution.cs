/*
http://web.itu.edu.tr/topcuil/ya/END331.pdf
4.1 DP ÇÖZÜMLERİ: Bir DP çözüldüğü zaman aşağıdaki dört durumdan biri ile karşılaşılır:
1. DP’nin "bir tek en iyi çözümü" vardır.
2. DP’nin "alternatif (çok sayıda) en iyi çözümleri" vardır. Birden fazla (aslında!sonsuz!sayıda) en iyi çözüm bulunur.
3. DP "olurlu değildir (infeasible)". Hiç olurlu çözümü yoktur (Olurlu bölgede nokta yoktur).
4. DP "sınırlı değildir (unbounded)". Olurlu bölgedeki noktalar sonsuz büyüklükte amaç fonksiyon değeri vermektedir.
*/

using System;
using System.Collections.Generic;
using System.Text;
using Simplex.Enums;

namespace Simplex.Analysis
{
    [Serializable]
    public class Solution
    {
        public Solution()
        {
            Results = new List<ResultTerm>();
        }
        public List<ResultTerm> Results { get; set; }

        /// <summary>
        /// The optimal value of the objective function.
        /// </summary>
        public double[,] RightHandValues { get; set; }

        /// <summary>
        /// Set true if alternate solutions exist.
        /// </summary>
        public bool AlternateSolutionsExist { get; set; }

        /// <summary>
        /// The quality of the solution: optimal, infeasible, etc.
        /// </summary>
        public SolutionQuality Quality { get; set; }

        public double[] ObjectiveMatrix { get; set; }

    }

}
