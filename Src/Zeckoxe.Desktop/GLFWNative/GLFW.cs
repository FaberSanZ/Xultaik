// Copyright(c) 2019-2020 Faber Leonardo.All Rights Reserved.

/*=============================================================================
	GLFW.cs
=============================================================================*/


using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace Zeckoxe.Desktop.GLFWNative
{
    internal static unsafe partial class GLFW
    {
		internal const int GLFW_VERSION_MAJOR = 3;
		internal const int GLFW_VERSION_MINOR = 3;
		internal const int GLFW_VERSION_REVISION = 0;
		internal const int GLFW_TRUE = 1;
		internal const int GLFW_FALSE = 0;
		internal const int GLFW_RELEASE = 0;
		internal const int GLFW_PRESS = 1;
		internal const int GLFW_REPEAT = 2;
		internal const int GLFW_HAT_CENTERED = 0;
		internal const int GLFW_HAT_UP = 1;
		internal const int GLFW_HAT_RIGHT = 2;
		internal const int GLFW_HAT_DOWN = 4;
		internal const int GLFW_HAT_LEFT = 8;
		internal const int GLFW_KEY_UNKNOWN = -1;
		internal const int GLFW_KEY_SPACE = 32;
		internal const int GLFW_KEY_0 = 48;
		internal const int GLFW_KEY_1 = 49;
		internal const int GLFW_KEY_2 = 50;
		internal const int GLFW_KEY_3 = 51;
		internal const int GLFW_KEY_4 = 52;
		internal const int GLFW_KEY_5 = 53;
		internal const int GLFW_KEY_6 = 54;
		internal const int GLFW_KEY_7 = 55;
		internal const int GLFW_KEY_8 = 56;
		internal const int GLFW_KEY_9 = 57;
		internal const int GLFW_KEY_A = 65;
		internal const int GLFW_KEY_B = 66;
		internal const int GLFW_KEY_C = 67;
		internal const int GLFW_KEY_D = 68;
		internal const int GLFW_KEY_E = 69;
		internal const int GLFW_KEY_F = 70;
		internal const int GLFW_KEY_G = 71;
		internal const int GLFW_KEY_H = 72;
		internal const int GLFW_KEY_I = 73;
		internal const int GLFW_KEY_J = 74;
		internal const int GLFW_KEY_K = 75;
		internal const int GLFW_KEY_L = 76;
		internal const int GLFW_KEY_M = 77;
		internal const int GLFW_KEY_N = 78;
		internal const int GLFW_KEY_O = 79;
		internal const int GLFW_KEY_P = 80;
		internal const int GLFW_KEY_Q = 81;
		internal const int GLFW_KEY_R = 82;
		internal const int GLFW_KEY_S = 83;
		internal const int GLFW_KEY_T = 84;
		internal const int GLFW_KEY_U = 85;
		internal const int GLFW_KEY_V = 86;
		internal const int GLFW_KEY_W = 87;
		internal const int GLFW_KEY_X = 88;
		internal const int GLFW_KEY_Y = 89;
		internal const int GLFW_KEY_Z = 90;
		internal const int GLFW_KEY_ESCAPE = 256;
		internal const int GLFW_KEY_ENTER = 257;
		internal const int GLFW_KEY_TAB = 258;
		internal const int GLFW_KEY_BACKSPACE = 259;
		internal const int GLFW_KEY_INSERT = 260;
		internal const int GLFW_KEY_DELETE = 261;
		internal const int GLFW_KEY_RIGHT = 262;
		internal const int GLFW_KEY_LEFT = 263;
		internal const int GLFW_KEY_DOWN = 264;
		internal const int GLFW_KEY_UP = 265;
		internal const int GLFW_KEY_PAGE_UP = 266;
		internal const int GLFW_KEY_PAGE_DOWN = 267;
		internal const int GLFW_KEY_HOME = 268;
		internal const int GLFW_KEY_END = 269;
		internal const int GLFW_KEY_CAPS_LOCK = 280;
		internal const int GLFW_KEY_SCROLL_LOCK = 281;
		internal const int GLFW_KEY_NUM_LOCK = 282;
		internal const int GLFW_KEY_PRINT_SCREEN = 283;
		internal const int GLFW_KEY_PAUSE = 284;
		internal const int GLFW_KEY_F1 = 290;
		internal const int GLFW_KEY_F2 = 291;
		internal const int GLFW_KEY_F3 = 292;
		internal const int GLFW_KEY_F4 = 293;
		internal const int GLFW_KEY_F5 = 294;
		internal const int GLFW_KEY_F6 = 295;
		internal const int GLFW_KEY_F7 = 296;
		internal const int GLFW_KEY_F8 = 297;
		internal const int GLFW_KEY_F9 = 298;
		internal const int GLFW_KEY_F10 = 299;
		internal const int GLFW_KEY_F11 = 300;
		internal const int GLFW_KEY_F12 = 301;
		internal const int GLFW_KEY_F13 = 302;
		internal const int GLFW_KEY_F14 = 303;
		internal const int GLFW_KEY_F15 = 304;
		internal const int GLFW_KEY_F16 = 305;
		internal const int GLFW_KEY_F17 = 306;
		internal const int GLFW_KEY_F18 = 307;
		internal const int GLFW_KEY_F19 = 308;
		internal const int GLFW_KEY_F20 = 309;
		internal const int GLFW_KEY_F21 = 310;
		internal const int GLFW_KEY_F22 = 311;
		internal const int GLFW_KEY_F23 = 312;
		internal const int GLFW_KEY_F24 = 313;
		internal const int GLFW_KEY_F25 = 314;
		internal const int GLFW_KEY_KP_0 = 320;
		internal const int GLFW_KEY_KP_1 = 321;
		internal const int GLFW_KEY_KP_2 = 322;
		internal const int GLFW_KEY_KP_3 = 323;
		internal const int GLFW_KEY_KP_4 = 324;
		internal const int GLFW_KEY_KP_5 = 325;
		internal const int GLFW_KEY_KP_6 = 326;
		internal const int GLFW_KEY_KP_7 = 327;
		internal const int GLFW_KEY_KP_8 = 328;
		internal const int GLFW_KEY_KP_9 = 329;
		internal const int GLFW_KEY_KP_DECIMAL = 330;
		internal const int GLFW_KEY_KP_DIVIDE = 331;
		internal const int GLFW_KEY_KP_MULTIPLY = 332;
		internal const int GLFW_KEY_KP_SUBTRACT = 333;
		internal const int GLFW_KEY_KP_ADD = 334;
		internal const int GLFW_KEY_KP_ENTER = 335;
		internal const int GLFW_KEY_KP_EQUAL = 336;
		internal const int GLFW_KEY_LEFT_SHIFT = 340;
		internal const int GLFW_KEY_LEFT_CONTROL = 341;
		internal const int GLFW_KEY_LEFT_ALT = 342;
		internal const int GLFW_KEY_LEFT_SUPER = 343;
		internal const int GLFW_KEY_RIGHT_SHIFT = 344;
		internal const int GLFW_KEY_RIGHT_CONTROL = 345;
		internal const int GLFW_KEY_RIGHT_ALT = 346;
		internal const int GLFW_KEY_RIGHT_SUPER = 347;
		internal const int GLFW_KEY_MENU = 348;
		internal const int GLFW_KEY_LAST = GLFW_KEY_MENU;
		internal const int GLFW_MOD_SHIFT = 0x0001;
		internal const int GLFW_MOD_CONTROL = 0x0002;
		internal const int GLFW_MOD_ALT = 0x0004;
		internal const int GLFW_MOD_SUPER = 0x0008;
		internal const int GLFW_MOD_CAPS_LOCK = 0x0010;
		internal const int GLFW_MOD_NUM_LOCK = 0x0020;
		internal const int GLFW_MOUSE_BUTTON_1 = 0;
		internal const int GLFW_MOUSE_BUTTON_2 = 1;
		internal const int GLFW_MOUSE_BUTTON_3 = 2;
		internal const int GLFW_MOUSE_BUTTON_4 = 3;
		internal const int GLFW_MOUSE_BUTTON_5 = 4;
		internal const int GLFW_MOUSE_BUTTON_6 = 5;
		internal const int GLFW_MOUSE_BUTTON_7 = 6;
		internal const int GLFW_MOUSE_BUTTON_8 = 7;
		internal const int GLFW_MOUSE_BUTTON_LAST = GLFW_MOUSE_BUTTON_8;
		internal const int GLFW_MOUSE_BUTTON_LEFT = GLFW_MOUSE_BUTTON_1;
		internal const int GLFW_MOUSE_BUTTON_RIGHT = GLFW_MOUSE_BUTTON_2;
		internal const int GLFW_MOUSE_BUTTON_MIDDLE = GLFW_MOUSE_BUTTON_3;
		internal const int GLFW_JOYSTICK_1 = 0;
		internal const int GLFW_JOYSTICK_2 = 1;
		internal const int GLFW_JOYSTICK_3 = 2;
		internal const int GLFW_JOYSTICK_4 = 3;
		internal const int GLFW_JOYSTICK_5 = 4;
		internal const int GLFW_JOYSTICK_6 = 5;
		internal const int GLFW_JOYSTICK_7 = 6;
		internal const int GLFW_JOYSTICK_8 = 7;
		internal const int GLFW_JOYSTICK_9 = 8;
		internal const int GLFW_JOYSTICK_10 = 9;
		internal const int GLFW_JOYSTICK_11 = 10;
		internal const int GLFW_JOYSTICK_12 = 11;
		internal const int GLFW_JOYSTICK_13 = 12;
		internal const int GLFW_JOYSTICK_14 = 13;
		internal const int GLFW_JOYSTICK_15 = 14;
		internal const int GLFW_JOYSTICK_16 = 15;
		internal const int GLFW_JOYSTICK_LAST = GLFW_JOYSTICK_16;
		internal const int GLFW_GAMEPAD_BUTTON_A = 0;
		internal const int GLFW_GAMEPAD_BUTTON_B = 1;
		internal const int GLFW_GAMEPAD_BUTTON_X = 2;
		internal const int GLFW_GAMEPAD_BUTTON_Y = 3;
		internal const int GLFW_GAMEPAD_BUTTON_LEFT_BUMPER = 4;
		internal const int GLFW_GAMEPAD_BUTTON_RIGHT_BUMPER = 5;
		internal const int GLFW_GAMEPAD_BUTTON_BACK = 6;
		internal const int GLFW_GAMEPAD_BUTTON_START = 7;
		internal const int GLFW_GAMEPAD_BUTTON_GUIDE = 8;
		internal const int GLFW_GAMEPAD_BUTTON_LEFT_THUMB = 9;
		internal const int GLFW_GAMEPAD_BUTTON_RIGHT_THUMB = 10;
		internal const int GLFW_GAMEPAD_BUTTON_DPAD_UP = 11;
		internal const int GLFW_GAMEPAD_BUTTON_DPAD_RIGHT = 12;
		internal const int GLFW_GAMEPAD_BUTTON_DPAD_DOWN = 13;
		internal const int GLFW_GAMEPAD_BUTTON_DPAD_LEFT = 14;
		internal const int GLFW_GAMEPAD_BUTTON_LAST = GLFW_GAMEPAD_BUTTON_DPAD_LEFT;
		internal const int GLFW_GAMEPAD_BUTTON_CROSS = GLFW_GAMEPAD_BUTTON_A;
		internal const int GLFW_GAMEPAD_BUTTON_CIRCLE = GLFW_GAMEPAD_BUTTON_B;
		internal const int GLFW_GAMEPAD_BUTTON_SQUARE = GLFW_GAMEPAD_BUTTON_X;
		internal const int GLFW_GAMEPAD_BUTTON_TRIANGLE = GLFW_GAMEPAD_BUTTON_Y;
		internal const int GLFW_GAMEPAD_AXIS_LEFT_X = 0;
		internal const int GLFW_GAMEPAD_AXIS_LEFT_Y = 1;
		internal const int GLFW_GAMEPAD_AXIS_RIGHT_X = 2;
		internal const int GLFW_GAMEPAD_AXIS_RIGHT_Y = 3;
		internal const int GLFW_GAMEPAD_AXIS_LEFT_TRIGGER = 4;
		internal const int GLFW_GAMEPAD_AXIS_RIGHT_TRIGGER = 5;
		internal const int GLFW_GAMEPAD_AXIS_LAST = GLFW_GAMEPAD_AXIS_RIGHT_TRIGGER;
		internal const int GLFW_NO_ERROR = 0;
		internal const int GLFW_NOT_INITIALIZED = 0x00010001;
		internal const int GLFW_NO_CURRENT_CONTEXT = 0x00010002;
		internal const int GLFW_INVALID_ENUM = 0x00010003;
		internal const int GLFW_INVALID_VALUE = 0x00010004;
		internal const int GLFW_OUT_OF_MEMORY = 0x00010005;
		internal const int GLFW_API_UNAVAILABLE = 0x00010006;
		internal const int GLFW_VERSION_UNAVAILABLE = 0x00010007;
		internal const int GLFW_PLATFORM_ERROR = 0x00010008;
		internal const int GLFW_FORMAT_UNAVAILABLE = 0x00010009;
		internal const int GLFW_NO_WINDOW_CONTEXT = 0x0001000A;
		internal const int GLFW_FOCUSED = 0x00020001;
		internal const int GLFW_ICONIFIED = 0x00020002;
		internal const int GLFW_RESIZABLE = 0x00020003;
		internal const int GLFW_VISIBLE = 0x00020004;
		internal const int GLFW_DECORATED = 0x00020005;
		internal const int GLFW_AUTO_ICONIFY = 0x00020006;
		internal const int GLFW_FLOATING = 0x00020007;
		internal const int GLFW_MAXIMIZED = 0x00020008;
		internal const int GLFW_CENTER_CURSOR = 0x00020009;
		internal const int GLFW_TRANSPARENT_FRAMEBUFFER = 0x0002000A;
		internal const int GLFW_HOVERED = 0x0002000B;
		internal const int GLFW_FOCUS_ON_SHOW = 0x0002000C;
		internal const int GLFW_RED_BITS = 0x00021001;
		internal const int GLFW_GREEN_BITS = 0x00021002;
		internal const int GLFW_BLUE_BITS = 0x00021003;
		internal const int GLFW_ALPHA_BITS = 0x00021004;
		internal const int GLFW_DEPTH_BITS = 0x00021005;
		internal const int GLFW_STENCIL_BITS = 0x00021006;
		internal const int GLFW_ACCUM_RED_BITS = 0x00021007;
		internal const int GLFW_ACCUM_GREEN_BITS = 0x00021008;
		internal const int GLFW_ACCUM_BLUE_BITS = 0x00021009;
		internal const int GLFW_ACCUM_ALPHA_BITS = 0x0002100A;
		internal const int GLFW_AUX_BUFFERS = 0x0002100B;
		internal const int GLFW_STEREO = 0x0002100C;
		internal const int GLFW_SAMPLES = 0x0002100D;
		internal const int GLFW_SRGB_CAPABLE = 0x0002100E;
		internal const int GLFW_REFRESH_RATE = 0x0002100F;
		internal const int GLFW_DOUBLEBUFFER = 0x00021010;
		internal const int GLFW_CLIENT_API = 0x00022001;
		internal const int GLFW_CONTEXT_VERSION_MAJOR = 0x00022002;
		internal const int GLFW_CONTEXT_VERSION_MINOR = 0x00022003;
		internal const int GLFW_CONTEXT_REVISION = 0x00022004;
		internal const int GLFW_CONTEXT_ROBUSTNESS = 0x00022005;
		internal const int GLFW_OPENGL_FORWARD_COMPAT = 0x00022006;
		internal const int GLFW_OPENGL_DEBUG_CONTEXT = 0x00022007;
		internal const int GLFW_OPENGL_PROFILE = 0x00022008;
		internal const int GLFW_CONTEXT_RELEASE_BEHAVIOR = 0x00022009;
		internal const int GLFW_CONTEXT_NO_ERROR = 0x0002200A;
		internal const int GLFW_CONTEXT_CREATION_API = 0x0002200B;
		internal const int GLFW_SCALE_TO_MONITOR = 0x0002200C;
		internal const int GLFW_COCOA_RETINA_FRAMEBUFFER = 0x00023001;
		internal const int GLFW_COCOA_FRAME_NAME = 0x00023002;
		internal const int GLFW_COCOA_GRAPHICS_SWITCHING = 0x00023003;
		internal const int GLFW_X11_CLASS_NAME = 0x00024001;
		internal const int GLFW_X11_INSTANCE_NAME = 0x00024002;
		internal const int GLFW_NO_API = 0;
		internal const int GLFW_OPENGL_API = 0x00030001;
		internal const int GLFW_OPENGL_ES_API = 0x00030002;
		internal const int GLFW_NO_ROBUSTNESS = 0;
		internal const int GLFW_NO_RESET_NOTIFICATION = 0x00031001;
		internal const int GLFW_LOSE_CONTEXT_ON_RESET = 0x00031002;
		internal const int GLFW_OPENGL_ANY_PROFILE = 0;
		internal const int GLFW_OPENGL_CORE_PROFILE = 0x00032001;
		internal const int GLFW_OPENGL_COMPAT_PROFILE = 0x00032002;
		internal const int GLFW_CURSOR = 0x00033001;
		internal const int GLFW_STICKY_KEYS = 0x00033002;
		internal const int GLFW_STICKY_MOUSE_BUTTONS = 0x00033003;
		internal const int GLFW_LOCK_KEY_MODS = 0x00033004;
		internal const int GLFW_RAW_MOUSE_MOTION = 0x00033005;
		internal const int GLFW_CURSOR_NORMAL = 0x00034001;
		internal const int GLFW_CURSOR_HIDDEN = 0x00034002;
		internal const int GLFW_CURSOR_DISABLED = 0x00034003;
		internal const int GLFW_ANY_RELEASE_BEHAVIOR = 0;
		internal const int GLFW_RELEASE_BEHAVIOR_FLUSH = 0x00035001;
		internal const int GLFW_RELEASE_BEHAVIOR_NONE = 0x00035002;
		internal const int GLFW_NATIVE_CONTEXT_API = 0x00036001;
		internal const int GLFW_EGL_CONTEXT_API = 0x00036002;
		internal const int GLFW_OSMESA_CONTEXT_API = 0x00036003;
		internal const int GLFW_ARROW_CURSOR = 0x00036001;
		internal const int GLFW_IBEAM_CURSOR = 0x00036002;
		internal const int GLFW_CROSSHAIR_CURSOR = 0x00036003;
		internal const int GLFW_HAND_CURSOR = 0x00036004;
		internal const int GLFW_HRESIZE_CURSOR = 0x00036005;
		internal const int GLFW_VRESIZE_CURSOR = 0x00036006;
		internal const int GLFW_CONNECTED = 0x00040001;
		internal const int GLFW_DISCONNECTED = 0x00040002;
		internal const int GLFW_JOYSTICK_HAT_BUTTONS = 0x00050001;
		internal const int GLFW_COCOA_CHDIR_RESOURCES = 0x00051001;
		internal const int GLFW_COCOA_MENUBAR = 0x00051002;
		internal const int GLFW_DONT_CARE = -1;



		private delegate void glfwInit();
		private static readonly glfwInit glfwInit_ = GLFWLoader.GetStaticProc<glfwInit>("glfwInit");
		internal static void GlfwInit() => glfwInit_();



		internal delegate void glfwInitHint(int hint, int value);
		private static readonly glfwInitHint glfwInitHint_ = GLFWLoader.GetStaticProc<glfwInitHint>("glfwWindowHint");
		internal static void GlfwInitHint(int hint, int value) => glfwInitHint_(hint, value);




		internal delegate IntPtr glfwCreateWindow(int width, int height, string title, IntPtr monitor, IntPtr share);
		private static readonly glfwCreateWindow glfwCreateWindow_ = GLFWLoader.GetStaticProc<glfwCreateWindow>("glfwCreateWindow");
		internal static IntPtr GlfwCreateWindow(int width, int height, string title, IntPtr monitor, IntPtr share) => glfwCreateWindow_(width, height, title, monitor, share);




		internal delegate int glfwWindowShouldClose(IntPtr window);
		private static readonly glfwWindowShouldClose glfwWindowShouldClose_ = GLFWLoader.GetStaticProc<glfwWindowShouldClose>("glfwWindowShouldClose");
		internal static int GlfwWindowShouldClose(IntPtr window) => glfwWindowShouldClose_(window);




		internal delegate void glfwPollEvents();
		private static readonly glfwPollEvents glfwPollEvents_ = GLFWLoader.GetStaticProc<glfwPollEvents>("glfwPollEvents");
		internal static void GlfwPollEvents() => glfwPollEvents_();


		internal delegate void glfwShowWindow(IntPtr window);
		private static readonly glfwShowWindow glfwShowWindow_ = GLFWLoader.GetStaticProc<glfwShowWindow>("glfwShowWindow");
		internal static void ShowWindow(IntPtr window) => glfwShowWindow_(window);


		internal delegate IntPtr glfwGetWin32Window(IntPtr window);
		private static readonly glfwGetWin32Window glfwGetWin32Window_ = GLFWLoader.GetStaticProc<glfwGetWin32Window>("glfwGetWin32Window");
		internal static IntPtr GlfwGetWin32Window(IntPtr window) => glfwGetWin32Window_(window);





		public delegate void glfwSetWindowTitle(IntPtr window, byte* title);
		private static readonly glfwSetWindowTitle glfwSetWindowTitle_ = GLFWLoader.GetStaticProc<glfwSetWindowTitle>("glfwSetWindowTitle");
		internal static void GlfwSetWindowTitle(IntPtr window, byte* title) => glfwSetWindowTitle_(window, title);

	}
}

	

