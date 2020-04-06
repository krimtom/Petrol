using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Petrol.Model
{
    public class PetrolObject
    {
        public double bensin95 { get; set; }
        public double? bensin95_discount { get; set; }
        public string company { get; set; }
        public double diesel { get; set; }
        public double? diesel_discount { get; set; }
        public Geo geo { get; set; }
        public string key { get; set; }
        public string name { get; set; }
    }
}