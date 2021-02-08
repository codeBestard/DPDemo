using System;
using System.Collections.Generic;

namespace DP.Common.Configurations
{
    public abstract class BaseConfig
    {
        private static Dictionary<string, string> _Values = new Dictionary<string, string>();

        protected static string Get(string variable)
        {
            if (_Values.ContainsKey(variable))
            {
                return _Values[variable];
            }

            var value = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Machine);
            if (string.IsNullOrEmpty(value))
            {
                value = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Process);
            }
            _Values[variable] = value;
            return _Values[variable];
        }

    }
}
