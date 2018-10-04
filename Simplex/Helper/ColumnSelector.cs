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
        int GetSelectedIndex(Matrix matrix, int rowIndex);

    }

    internal class ColumnMaxValueSelector : IColumnSelector
    {
        public int GetSelectedIndex(Matrix matrix, int rowIndex)
        {
            return matrix.GetPositiveMaxValueByRow(rowIndex);
        }
    }

    internal class ColumnMinValueSelector : IColumnSelector
    {
        public int GetSelectedIndex(Matrix matrix, int rowIndex)
        {
            return matrix.GetPositiveMinValueByRow(rowIndex);
        }
    }
}
