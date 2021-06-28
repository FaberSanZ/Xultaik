// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


using System;
using System.Text;

namespace Vultaik
{
    public static class ConsoleLog
    {
        static ConsoleLog()
        {
            Data = new();
        }
        public static StringBuilder Data { get; set; } 

        public static void Info(string type, string message, bool node = false)
        {
            string s_node = "";
            if (node)
                s_node += "\n";

            WriteColored(ConsoleColor.Green, $"[INFO] [{type}]");
            Console.WriteLine(" " + message + s_node);
            Data.Append($"[INFO] [{type}] " + message + s_node);
        }

        public static void InfoNode(string type, string message, bool endNode = false)
        {
            var sub_node_count = type.Length / 2;
            string s = "  ";
            for (int i = 0; i < sub_node_count; i++)
                s += " ";

            string s_node = "";
            if (endNode)
                s_node += "\n";

            WriteColored(ConsoleColor.Green, s + "└──");
            Console.WriteLine(" " + message + s_node);

            Data.Append($"{s} + {type + message}" + s_node);
        }

        public static void Warn(string type, string message)
        {
            WriteColored(ConsoleColor.Yellow, $"[WARN] [{type}]");
            Console.WriteLine(" " + message + "\n \n");
        }

        public static void Error(string type, string message)
        {
            WriteColored(ConsoleColor.Red, $"[ERROR] [{type}]");
            Console.WriteLine(" " + message + "\n \n");

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
