// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	Primitive.cs
=============================================================================*/


using System;
using System.Numerics;
using Vultaik.Physics;



namespace Vultaik.GLTF
{

	public class Material
	{
		public enum Workflow { PhysicalyBaseRendering = 1, SpecularGlossinnes };
		public string Name;
		public Workflow workflow;
		public Int32 baseColorTexture;
		public Int32 metallicRoughnessTexture;
		public Int32 normalTexture;
		public Int32 occlusionTexture;
		public Int32 emissiveTexture;

		public Vector4 baseColorFactor;
		public Vector4 emissiveFactor;
		//public AttachmentType availableAttachments;
		//public AttachmentType availableAttachments1;

		//public AlphaMode alphaMode;
		public float alphaCutoff;
		public float metallicFactor;
		public float roughnessFactor;

		public bool metallicRoughness = true;
		public bool specularGlossiness = false;

		public Material(Int32 _baseColorTexture = -1, Int32 _metallicRoughnessTexture = -1,
			Int32 _normalTexture = -1, Int32 _occlusionTexture = -1)
		{
			workflow = Workflow.PhysicalyBaseRendering;
			baseColorTexture = _baseColorTexture;
			metallicRoughnessTexture = _metallicRoughnessTexture;
			normalTexture = _normalTexture;
			occlusionTexture = _occlusionTexture;
			emissiveTexture = -1;

			//alphaMode = AlphaMode.Opaque;
			alphaCutoff = 1f;
			metallicFactor = 1f;
			roughnessFactor = 1;
			baseColorFactor = new Vector4(1);
			emissiveFactor = new Vector4(1);

			metallicRoughness = true;
			specularGlossiness = false;

		}
	}


	public class Primitive
    {
        public Primitive()
        {
        }

        public Primitive(int vertexCount, int indexCount, int firstVertex = 0, int firstIndex = 0)
        {
            VertexCount = vertexCount;
            IndexCount = indexCount;
            FirstVertex = firstVertex;
            FirstIndex = firstIndex;
        }

        public BoundingBox BoundingBox;

        public string Name { get; set; }
        public int FirstIndex { get; set; }
        public int FirstVertex { get; set; }
        public int VertexCount { get; set; }
        public int IndexCount { get; set; }
        public int Material { get; set; }





        public void Draw(CommandBuffer cmd, int instanceCount = 1, int firstInstance = 0)
        {
            //cmd.DrawIndexed(IndexCount, instanceCount, IndexBase, VertexBase, firstInstance);
        }
    }
}
