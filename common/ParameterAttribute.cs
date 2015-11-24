using System;

namespace common
{
    class ParameterAttribute : Attribute
    {
        public string Parameter { get; set; }

        public bool Required { get; set; }

        public bool AllowSpaces { get; set; } = true;

        public bool Default { get; set; }

        public ParameterAttribute(string parameter = null)
        {
            Parameter = parameter;
        }
    }
}