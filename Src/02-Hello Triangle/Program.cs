using System;

namespace _02_Hello_Triangle
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-----------------TextureData-----------");

            //Zeckoxe.Image.DDSLoader dDSLoader = new Zeckoxe.Image.DDSLoader("desertcube1024.dds");
            //var data = dDSLoader.TextureData;


            var data = Zeckoxe.Image.IMGLoader.LoadFromFile("UVCheckerMap08-512.png");
            //var data = Zeckoxe.Image.DDSLoader.LoadFromFile("desertcube1024.dds");

            Console.WriteLine("Format = {0}", data.Format);
            Console.WriteLine("Width = {0}", data.Width);
            Console.WriteLine("Height = {0}", data.Height);
            Console.WriteLine("Depth = {0}", data.Depth);
            Console.WriteLine("MipMaps = {0}", data.MipMaps);
            Console.WriteLine("Size = {0}", data.Size);
            Console.WriteLine("IsCubeMap = {0}", data.IsCubeMap);
            Console.WriteLine("-----------------DataInBytes-----------");
            foreach (byte item in data.Data.Span)
                Console.Write("-{0}", item);
            Console.WriteLine("-----------------DataInBytes-----------");



            using (var App = new Game())
                App.Run();
        }
    }
}
