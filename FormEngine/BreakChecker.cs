using System;
using System.Collections.Generic;
using FormEngine.Interfaces;

namespace FormEngine
{
    public class BreakChecker
    {
        IValues previousValues = null;
        IValues currentValues = null;

        public bool IsBreak(List<string> breakValueNames)
        {
            if (breakValueNames == null)
                return true;
            if (breakValueNames.Count == 0)
                return true;
            if (previousValues == null)
                return true;
            if (currentValues == null)
                return true;
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
    }
}