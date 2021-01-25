using System;
using System.Collections.Generic;
using FormEngine.Interfaces;

namespace FormEngine
{
    public class ValueIterator
    {
        dynamic previousValues = null;
        dynamic currentValues = null;

        public bool IsFirst()
        {
            if (previousValues == null)
                return true;
            return false;
        }
        public bool IsLast()
        {
            if (currentValues == null && previousValues != null)
                return true;
            return false;
        }
        public bool IsBreak(List<Func<dynamic, object>> breakValues, bool headerBreak)
        {
            if (previousValues == null)
                if (headerBreak)
                    return true;
                else
                    return false;

            if (currentValues == null)
                if (headerBreak)
                    return false;
                else
                    return true;

            if (breakValues == null)
                return false;
            if (breakValues.Count == 0)
                return false;
            foreach (Func<dynamic, object> value in breakValues)
                if (value(previousValues) != value(currentValues))
                    return true;
            return false;
        }

        public void IterateTo(dynamic values)
        {
            previousValues = currentValues;
            currentValues = values;
        }

        public dynamic PreviousValues()
        {
            return previousValues;
        }
    }
}