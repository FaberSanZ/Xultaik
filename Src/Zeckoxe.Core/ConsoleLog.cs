// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ

/*=============================================================================
	ConsoleLog.cs
=============================================================================*/


using System;

namespace Zeckoxe.Core
{
    public class ConsoleLog : ILog
    {
        public void Info(string message)
        {
            WriteColored(ConsoleColor.Green, "[INFO]");
            Console.WriteLine(" " + message);
        }

        public void Warn(string message)
        {
            WriteColored(ConsoleColor.Yellow, "[WARN]");
            Console.WriteLine(" " + message);
        }

        public void Error(string type, string message)
        {
            WriteColored(ConsoleColor.Red, $"[ERROR] [{type}]");
            Console.WriteLine(" " + message);
        }

        internal void WriteColored(ConsoleColor color, string message)
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = currentColor;
        }
    }
}
