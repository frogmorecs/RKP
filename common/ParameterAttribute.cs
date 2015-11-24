using System;

namespace common
{
    class ParameterAttribute : Attribute
    {
        public string Parameter { get; set; }

        public bool Required { get; set; }

        public bool AllowSpaces { get; set; } = true;

        public ParameterAttribute(string parameter)
        {
            Parameter = parameter;
        }

        public ParameterAttribute(string parameter, bool required)
        {
            Parameter = parameter;
            Required = required;
        }
    }
}