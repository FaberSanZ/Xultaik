// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


using System;

namespace Zeckoxe.Core
{
    public static class ConsoleLog
    {
        public static void Info(string type, string message)
        {
            WriteColored(ConsoleColor.Green, $"[INFO] [{type}]");
            Console.WriteLine(" " + message + "\n \n \n");
        }

        public static void Warn(string type, string message)
        {
            WriteColored(ConsoleColor.Yellow, $"[WARN] [{type}]");
            Console.WriteLine(" " + message + "\n \n \n");
        }

        public static void Error(string type, string message)
        {
            WriteColored(ConsoleColor.Red, $"[ERROR] [{type}]");
            Console.WriteLine(" " + message + "\n \n \n");

        }

        internal static void WriteColored(ConsoleColor color, string message)
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = currentColor;
        }
    }
}
