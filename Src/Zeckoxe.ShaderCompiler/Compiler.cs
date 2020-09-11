using System;
using System.IO;




namespace Zeckoxe.ShaderCompiler
{

    internal static class Convert
    {


        internal static ShaderCompiler.Stage ToStage(int stage)
        {
            switch (stage)
            {
                case 0:
                    return ShaderCompiler.Stage.Vertex;

                case 1:
                    return ShaderCompiler.Stage.Fragment;

                case 2:
                    return ShaderCompiler.Stage.Compute;

                case 3:
                    return ShaderCompiler.Stage.Geometry;

                case 4:
                    return ShaderCompiler.Stage.TessControl;

                case 5:
                    return ShaderCompiler.Stage.TessEvaluation;

                case 6:
                    return ShaderCompiler.Stage.Raygen;

                case 7:
                    return ShaderCompiler.Stage.Anyhit;

                case 8:
                    return ShaderCompiler.Stage.Closesthit;

                case 9:
                    return ShaderCompiler.Stage.Miss;

                case 10:
                    return ShaderCompiler.Stage.Intersection;

                case 11:
                    return ShaderCompiler.Stage.Callable;

                case 12:
                    return ShaderCompiler.Stage.Task;

                case 13:
                    return ShaderCompiler.Stage.Mesh;

                default:
                    throw new ArgumentOutOfRangeException(nameof(stage));
            }

        }
    }





    public class Compiler
    {

        public static byte[] LoadFromFile(string path, int stage)
        {
            return new Compiler().LoadSPIR_V_ShaderGLSL(path, Convert.ToStage(stage));
        }



        internal byte[] LoadSPIR_V_ShaderGLSL(string path, ShaderCompiler.Stage stage)
        {
            CompileOptions options = new CompileOptions()
            {
                Language = InputLanguage.GLSL,
                Optimization = OptimizationLevel.None,
            };

            CompileResult result = new ShaderCompiler().Compile(File.ReadAllText(path), stage, options, "testShader", "main");


            return result.GetBytes();

        }

        internal byte[] LoadSPIR_V_ShaderHLSL(string path, ShaderCompiler.Stage stage)
        {
            CompileOptions options = new CompileOptions()
            {
                Language = InputLanguage.HLSL,
                //Optimization = CompileOptions.OptimizationLevel.None,
            };

            CompileResult result = new ShaderCompiler().Compile(File.ReadAllText(path), stage, options, "testShader", "main");


            return result.GetBytes();

        }
    }
}
