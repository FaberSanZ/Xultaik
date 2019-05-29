using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    public class GraphicsAdapter
    {
        public Adapter Adapter { get; set; }

        public string Description { get; }

        private GraphicsAdapter(string description, Adapter adapter)
        {
            Description = description;

            Adapter = adapter;
        }


        public static List<GraphicsAdapter> EnumerateGraphicsAdapter()
        {
            List<GraphicsAdapter> Adapters = new List<GraphicsAdapter>();


            using (Factory1 factory = new Factory1())
                foreach (var adapter in factory.Adapters)
                    Adapters.Add(new GraphicsAdapter(adapter.Description.Description, adapter));

            
            return Adapters;
        }
    }
}
