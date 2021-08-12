using System;
using System.Collections.Generic;
using System.Text;

namespace XnLogger.Model
{
    public class LogLevel
    {
        public string Name { get; set; }
        public int Value { get; set; }

        public LogLevel(string name, int value)
        {
            Name = name;
            Value = value;
        }

    }
}
