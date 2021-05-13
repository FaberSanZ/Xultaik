using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Common
{
    public static class Constants
    {
        public static string ShadersFile => Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\Assets\Shaders\"));
        public static string ModelsFile => Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\Assets\Models\"));
        public static string ImagesFile => Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\Assets\Images\"));


    }
}
