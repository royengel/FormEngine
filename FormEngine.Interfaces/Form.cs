﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormEngine.Interfaces
{
    public class Form : FieldProperties
    {
        public string formTitle;
        public PageSize pageSize;
        public List<Report> reports;

        public IEnumerable<IValues> TestValues()
        {
            return new List<IValues>() { new TestValues(this) };
        }
    }
}
