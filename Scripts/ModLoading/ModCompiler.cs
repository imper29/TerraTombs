using System.Diagnostics;
using System.IO;

namespace ModLoading
{
    /// <summary>
    /// This entire class is temporary until we get a more functional mod compiler. I just needed a simple compiler to be able to properly test the mod loader.
    /// </summary>
    public static class ModCompiler
    {
        public static void CompileMod(DirectoryInfo modRootDirectory)
        {
            //Begin a command to compile a mod.
            string command = "csc -out:" + modRootDirectory.FullName + "\\" + modRootDirectory.Name + ".dll -t:library";

            //Add references to the libs.
            int modRootDirSize = modRootDirectory.FullName.Length + 1;
            FileInfo[] libFiles = modRootDirectory.GetFiles("*.dll", SearchOption.AllDirectories);
            for (int i = 0; i < libFiles.Length; i++)
                if (libFiles[i].FullName != modRootDirectory.FullName + "\\" + modRootDirectory.Name + ".dll")
                    command += " -r:\"" + libFiles[i].FullName.Substring(modRootDirSize) + "\"";

            //Add the files to actually compile into a mod.
            command += " *.cs";

            //Execute the command in a command prompt terminal.
            ExecuteCommand(command, modRootDirectory);
        }

        private static void ExecuteCommand(string command, DirectoryInfo workingDirectory)
        {
            ProcessStartInfo info = new ProcessStartInfo("cmd.exe", "/K" + command);
            info.UseShellExecute = true;
            info.CreateNoWindow = false;
            info.WorkingDirectory = workingDirectory.FullName;
            Process.Start(info);
        }
    }
}
