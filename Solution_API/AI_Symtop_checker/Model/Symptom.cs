using System;
using System.Collections.Generic;

namespace AI_Symtop_checker.Model
{
    public class Symptom
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> RelatedConditions { get; set; }
    }
}