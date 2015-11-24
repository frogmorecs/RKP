using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace common
{
    public class CommandLineParser
    {
        public static T ParseCommandLine<T>(string[] args) where T: new()
        {
            var job = new T();

            var properties = GetPropertiesWithParameterAttribute(typeof(T)).ToList();
            var required = properties.Where(prop => GetParameterAttribute(prop).Required).Select(prop => prop.Name).ToList();

            for (int i = 0; i < args.Length; i++)
            {
                var currentArgument = args[i];

                foreach (var propertyInfo in properties)
                {
                    var parameterAttribute = GetParameterAttribute(propertyInfo);
                    var flag = parameterAttribute.Parameter;

                    if (currentArgument.StartsWith(flag))
                    {
                        if (required.Contains(propertyInfo.Name))
                        {
                            required.Remove(propertyInfo.Name);
                        }

                        if (propertyInfo.PropertyType == typeof (string))
                        {
                            var value = currentArgument.Length > 2 ? currentArgument.Substring(2) : args[++i];
                            if (!parameterAttribute.AllowSpaces && value.Any(c => new[] {'\x09', '\x0B', '\x0C', ' '}.Contains(c)))
                            {
                                throw new ArgumentException("Spaces aren't allowed in printer names.");
                            }

                            job.GetType().GetProperty(propertyInfo.Name).SetValue(job, value, null);

                            break;
                        }
                        if (propertyInfo.PropertyType == typeof (bool))
                        {
                            job.GetType().GetProperty(propertyInfo.Name).SetValue(job, true, null);

                            break;
                        }
                    }
                }
            }

            if (required.Any())
            {
                throw new ArgumentException("Missing option: " + required.First());
            }

            return job;
        }

        private static ParameterAttribute GetParameterAttribute(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttributes(false).OfType<ParameterAttribute>().First();
        }

        private static IEnumerable<PropertyInfo> GetPropertiesWithParameterAttribute(Type type)
        {
            return type.GetProperties().Where(HasParameterAttribute);
        }

        private static bool HasParameterAttribute(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttributes(false).OfType<ParameterAttribute>().Any();
        }
    }
}