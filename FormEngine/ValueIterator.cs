using System;
using System.Collections.Generic;
using FormEngine.Interfaces;

namespace FormEngine
{
    public class ValueIterator
    {
        IValues previousValues = null;
        IValues currentValues = null;

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
        public bool IsBreak(List<string> breakValueNames, bool headerBreak)
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

            if (breakValueNames == null)
                return false;
            if (breakValueNames.Count == 0)
                return false;
            foreach (string valueName in breakValueNames)
                if (previousValues.Get(valueName) != currentValues.Get(valueName))
                    return true;
            return false;
        }

        public void IterateTo(IValues values)
        {
            previousValues = currentValues;
            currentValues = values;
        }

        public IValues PreviousValues()
        {
            return previousValues;
        }
    }
}