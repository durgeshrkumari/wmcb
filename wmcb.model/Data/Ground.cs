﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wmcb.model.Data
{
    public class Ground
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Boolean Active { get; set; }
        public string PermitLink { get; set; }
    }
}
