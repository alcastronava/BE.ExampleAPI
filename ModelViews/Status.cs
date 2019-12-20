using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BE.ExampleAPI.ModelViews
{
    public class Status
    {
        public string Language { get; set; }

        public string Country { get; set; }

        public int Translated { get; set; }

        public int InReview { get; set; }
    }
}
