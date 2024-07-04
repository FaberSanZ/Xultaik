// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



namespace Xultaik.Desktop;

/// <summary>
/// Initializes a new instance of the <see cref="FrameEventArgs"/> struct.
/// </summary>
/// <param name="elapsedTicks">The elapsed ticks since the previous Update call.</param>
/// <param name="elapsedSeconds">The elapsed time since the previous Update call, in seconds.</param>
/// <param name="totalTicks">Total ticks since the start of the program.</param>
/// <param name="totalSeconds">Total time since the start of the program, in seconds.</param>
/// <param name="frameCount">Number of frames elapsed.</param>
/// <param name="framesPerSecond">Current framerate.</param>
public readonly struct FrameEventArgs(ulong elapsedTicks, double elapsedSeconds, ulong totalTicks, double totalSeconds, ulong frameCount, uint framesPerSecond)
{
    /// <summary>
    /// Gets elapsed ticks since the previous Update call.
    /// </summary>
    public ulong ElapsedTicks => elapsedTicks;

    /// <summary>
    /// Gets elapsed time since the previous Update call, in seconds.
    /// </summary>
    public double ElapsedSeconds => elapsedSeconds;

    /// <summary>
    /// Gets total time since the start of the program.
    /// </summary>
    public ulong TotalTicks => totalTicks;

    /// <summary>
    /// Gets total time in seconds since the start of the program.
    /// </summary>
    public double TotalSeconds => totalSeconds;

    /// <summary>
    /// Gets total number of updates since start of the program.
    /// </summary>
    public ulong FrameCount => frameCount;

    /// <summary>
    /// Gets the current framerate.
    /// </summary>
    public uint FramesPerSecond => framesPerSecond;
}
