using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace DotNetTestLibrary
{
    public static class TestClassLibrary
    {
        [DllExport("LogMessage", CallingConvention = CallingConvention.StdCall)]
        public static bool LogMessage([MarshalAs(UnmanagedType.LPWStr)] string message)
        {
            try
            {
                InternalLogString(message);
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    InternalLogString($"Message {message} failed because of: {Environment.NewLine}{ex}");
                }
                catch
                {
                    // silent crash
                }
                return false;
            }
        }

        [DllExport("CleanFile", CallingConvention = CallingConvention.StdCall)]
        public static bool CleanFile([MarshalAs(UnmanagedType.LPWStr)] string fullPath)
        {
            try
            {
                File.Delete(fullPath);
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    InternalLogString($"Deleting file at path {fullPath} failed because of: {Environment.NewLine}{ex}");
                }
                catch
                {
                    // silent crash
                }
                return false;
            }
        }

        [DllExport("AppendLine", CallingConvention = CallingConvention.StdCall)]
        public static bool AppendLine([MarshalAs(UnmanagedType.LPWStr)] string fullPath, [MarshalAs(UnmanagedType.LPWStr)] string line)
        {
            try
            {
                File.AppendAllText(fullPath, $"{line}{Environment.NewLine}", Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    InternalLogString($"Appending line {line} at path {fullPath} failed because of: {Environment.NewLine}{ex}");
                }
                catch
                {
                    // silent crash
                }
                return false;
            }
        }

        [DllExport("ReadLine", CallingConvention = CallingConvention.StdCall)]
        public static int ReadLine([MarshalAs(UnmanagedType.LPWStr)] string fullPath, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder text, int lineNumber)
        {
            try
            {
                if (lineNumber < 0)
                    return -1;
                var readLines = File.ReadAllLines(fullPath, Encoding.UTF8);
                if (readLines == null)
                    return -2;
                if (readLines.Length < lineNumber)
                    return -3;

                text.Clear();
                text.Append(readLines[lineNumber]);
                return 0;
            }
            catch (Exception ex)
            {
                try
                {
                    InternalLogString($"Reading line {lineNumber} at path {fullPath} failed because of: {Environment.NewLine}{ex}");
                }
                catch
                {
                    // silent crash
                }
                return -4;
            }
        }

        [DllExport("GetFileLinesCount", CallingConvention = CallingConvention.StdCall)]
        public static int GetFileLinesCount([MarshalAs(UnmanagedType.LPWStr)] string fullPath)
        {
            try
            {
                var readLines = File.ReadAllLines(fullPath, Encoding.UTF8);
                if (readLines == null)
                    return -1;
                return readLines.Length;
            }
            catch (Exception ex)
            {
                try
                {
                    InternalLogString($"Reading line count at path {fullPath} failed because of: {Environment.NewLine}{ex}");
                }
                catch
                {
                    // silent crash
                }
                return -2;
            }
        }

        [DllExport("GetFileMaximumLineLength", CallingConvention = CallingConvention.StdCall)]
        public static int GetFileMaximumLineLength([MarshalAs(UnmanagedType.LPWStr)] string fullPath)
        {
            try
            {
                var readLines = File.ReadAllLines(fullPath, Encoding.UTF8);
                if (readLines == null)
                    return -1;

                var maxLineCount = -1;
                foreach (var line in readLines)
                    if (maxLineCount < line.Length)
                        maxLineCount = line.Length + 1;

                return maxLineCount;
            }
            catch (Exception ex)
            {
                try
                {
                    InternalLogString($"Reading line maximum line length at path {fullPath} failed because of: {Environment.NewLine}{ex}");
                }
                catch
                {
                    // silent crash
                }
                return -2;
            }
        }

        [DllExport("ReadAllText", CallingConvention = CallingConvention.StdCall)]
        public static bool ReadAllText([MarshalAs(UnmanagedType.LPWStr)] string fullPath, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder text)
        {
            try
            {
                var readText = File.ReadAllText(fullPath, Encoding.UTF8);

                text.Clear();
                text.Append(readText);
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    InternalLogString($"Reading text at path {fullPath} failed because of: {Environment.NewLine}{ex}");
                }
                catch
                {
                    // silent crash
                }
                return false;
            }
        }

        private static void InternalLogString(string message)
        {
            var tempFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var fileName = $"Messages{DateTime.Now.ToString("ddMMyyyy")}.log";
            var fullPath = Path.Combine(tempFolder, fileName);

            File.AppendAllText(fullPath, $"{DateTime.Now.ToShortTimeString()}: {message}{Environment.NewLine}", Encoding.UTF8);
        }

        [DllExport("ReplaceString", CallingConvention = CallingConvention.StdCall)]
        public static int ReplaceString([In, Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder str,
        [MarshalAs(UnmanagedType.LPWStr)]string a, [MarshalAs(UnmanagedType.LPWStr)]string b)
        {
            str.Replace(a, b);

            if (str.ToString().Contains(a)) return 1;
            else return 0;
        }
    }
}