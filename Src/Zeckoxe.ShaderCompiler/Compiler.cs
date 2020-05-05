using System;
using System.IO;




namespace Zeckoxe.ShaderCompiler
{

    internal static class Convert
    {


        internal static ShaderCompiler.Stage ToStage(Stage stage)
        {
            switch (stage)
            {
                case Stage.Vertex:
                    return ShaderCompiler.Stage.Vertex;

                case Stage.Fragment:
                    return ShaderCompiler.Stage.Fragment;

                case Stage.Compute:
                    return ShaderCompiler.Stage.Compute;

                case Stage.Geometry:
                    return ShaderCompiler.Stage.Geometry;

                case Stage.TessControl:
                    return ShaderCompiler.Stage.TessControl;

                case Stage.TessEvaluation:
                    return ShaderCompiler.Stage.TessEvaluation;

                case Stage.Raygen:
                    return ShaderCompiler.Stage.Raygen;

                case Stage.Anyhit:
                    return ShaderCompiler.Stage.Anyhit;

                case Stage.Closesthit:
                    return ShaderCompiler.Stage.Closesthit;

                case Stage.Miss:
                    return ShaderCompiler.Stage.Miss;

                case Stage.Intersection:
                    return ShaderCompiler.Stage.Intersection;

                case Stage.Callable:
                    return ShaderCompiler.Stage.Callable;

                case Stage.Task:
                    return ShaderCompiler.Stage.Task;

                case Stage.Mesh:
                    return ShaderCompiler.Stage.Mesh;

                default:
                    throw new ArgumentOutOfRangeException(nameof(stage));
            }

        }
    }



    public enum Stage
    {
        Vertex,
        Fragment,
        Compute,
        Geometry,
        TessControl,
        TessEvaluation,
        Raygen,
        Anyhit,
        Closesthit,
        Miss,
        Intersection,
        Callable,
        Task,
        Mesh,
    }

    public class Compiler
    {

        public static byte[] LoadFromFile(string path, Stage stage)
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
