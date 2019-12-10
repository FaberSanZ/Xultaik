using System;

namespace _02_Hello_Triangle
{
    class Program
    {
        static void Main(string[] args)
        {

            var data = Zeckoxe.Image.DDSLoader.LoadFromFile("Test.dds");
            Console.WriteLine("-----------------TextureData-----------");

            Console.WriteLine(data.Format);
            Console.WriteLine(data.Width);
            Console.WriteLine(data.Height);
            Console.WriteLine(data.Depth);
            Console.WriteLine(data.MipMaps);
            Console.WriteLine(data.Size);
            Console.WriteLine(data.IsCubeMap);
            Console.WriteLine("-----------------DataInBytes-----------");
            foreach (var item in data.Data)
                Console.Write("-{0}",item);
            Console.WriteLine("-----------------DataInBytes-----------");



            using (var App = new Game())
                App.Run();
        }
    }
}
