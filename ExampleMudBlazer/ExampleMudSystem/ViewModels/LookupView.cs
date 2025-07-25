﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleMudSystem.ViewModels
{
    public class LookupView
    {
        public int LookupID { get; set; }

        public int CategoryID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool RemoveFromViewFlag { get; set; }
    }
}
