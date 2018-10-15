using System;
using System.Collections.Generic;
using System.Text;
using Simplex.Enums;

namespace Simplex.Helper
{

    public static class ColumnSelectorFactory
    {
        internal static IColumnSelector GetSelector(ObjectiveType objective)
        {
            Type tmp_selectorType = _SelectorList[objective];
            return Activator.CreateInstance(tmp_selectorType) as IColumnSelector;
        }
        private static Dictionary<ObjectiveType, Type> _SelectorList = FillSelectorTypes();
        private static Dictionary<ObjectiveType, Type> FillSelectorTypes()
        {
            Dictionary<ObjectiveType, Type> _builderList = new Dictionary<ObjectiveType, Type>();
            _builderList.Add(ObjectiveType.Maximum, typeof(ColumnMinValueSelector));
            _builderList.Add(ObjectiveType.Minumum, typeof(ColumnMaxValueSelector));
            return _builderList;
        }
    }

    internal interface IColumnSelector
    {
        int GetSelectedIndex(Matrix matrix, int rowIndex, List<int> ExclusionList);

    }

    internal class ColumnMaxValueSelector : IColumnSelector
    {
        public int GetSelectedIndex(Matrix matrix, int rowIndex, List<int> ExclusionList)
        {
            //return matrix.GetPositiveMaxValueByRow(rowIndex);
            int tmp_index = -1;
            double tmp_maxValue = 0;
            double tmp_currentValue = 0;
            for (int i = 0; i < matrix.ColumnCount; i++)
            {
                if (ExclusionList.Contains(i))
                    continue;

                tmp_currentValue = matrix.StorageArray[rowIndex * matrix.ColumnCount + i];
                if (tmp_currentValue > tmp_maxValue)
                {
                    tmp_maxValue = tmp_currentValue;
                    tmp_index = i;
                }
            }
            return tmp_index;
        }


    }

    internal class ColumnMinValueSelector : IColumnSelector
    {
        public int GetSelectedIndex(Matrix matrix, int rowIndex, List<int> ExclusionList)
        {
            //return matrix.GetNegativeMinValueByRow(rowIndex);

            int tmp_index = -1;
            double tmp_minValue = 0;
            double tmp_currentValue = 0;
            for (int i = 0; i < matrix.ColumnCount; i++)
            {
                if (ExclusionList.Contains(i))
                    continue;
                tmp_currentValue = matrix.StorageArray[rowIndex * matrix.ColumnCount + i];
                if (tmp_currentValue < tmp_minValue)
                {
                    tmp_minValue = tmp_currentValue;
                    tmp_index = i;
                }
            }
            return tmp_index;

        }
    }
}
