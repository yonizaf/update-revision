using System;
//using System.Collections.Generic;
using System.IO;
//using System.Linq;
//using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace UpdateRevision
{
	class Program
	{
		static void Main(string[] args)
		{
			string revision = "0";
			string fileContent = "";
			bool   revert = false;
			string inputFile = "AssemblyInfo.cs";
			Console.WriteLine("Computing Arguments...");
			for (int key = 0; key < args.Length; ++key)
			{

				if (args[key] == "--revert")
				{
					revert = true;
					Console.WriteLine("Revert mode: Yes.");
				}
				else if (key > 0 && args[key - 1] == "-f")
				{
					inputFile = args[key];
				}
			}
			Console.WriteLine("Input file: " + inputFile + "\ncurrent dir: "+Directory.GetCurrentDirectory());

			if (!revert)//no need for revision on revert
			{
				try
				{
					Console.WriteLine("Collecting revision...");
					//collect revision from svn
					Process svnVersion = new Process();
					svnVersion.StartInfo.FileName = "svnversion.exe";
					svnVersion.StartInfo.Arguments = "-n";
					svnVersion.StartInfo.RedirectStandardOutput = true;
					svnVersion.StartInfo.UseShellExecute = false;
					svnVersion.Start();
					revision = svnVersion.StandardOutput.ReadToEnd();
					svnVersion.WaitForExit(10000);
					if (!svnVersion.HasExited)
					{
						svnVersion.Kill();
					}
				}
				catch 
				{
					Console.WriteLine("Error collecting revision.");
				}

				if (revision == "" || revision == "exported")
				{
					revision = "0";
					Console.WriteLine("Revision set to 0");
				}
				else
				{
					revision = Regex.Replace(revision, "([0-9]+:)?([0-9]+)([MS]*)", "$2");
					Console.WriteLine("Computed revision: " + revision);
				}
			}
			else
			{
				revision = "*";
				Console.WriteLine("Revert mode. Revision set to *");
			}

			Console.WriteLine("");//empty line

			if (File.Exists(inputFile))
			{
				Console.WriteLine("Input file exists. opening...");
				/*StreamReader streamReader = File.OpenText(inputFile);
				fileContent = streamReader.ReadToEnd();
				streamReader.Close();*/
				fileContent = File.ReadAllText(inputFile);

				Console.WriteLine("Opened. Fixing version...");
				Regex AsmVer = new Regex("(AssemblyVersion\\(\"[0-9]+\\.[0-9]+\\.[0-9]+\\.)([0-9]+|\\*)(\"\\))");
				string fixedFile = AsmVer.Replace(fileContent, "${1}" + revision + "${3}");

				Console.WriteLine("Writing back to file...");
				File.WriteAllText(inputFile, fixedFile);
			}
			else
			{
				Console.WriteLine("\nError: Input file does not exist. Exiting...\n");
			}
			//string str = "[assembly: AssemblyVersion(\"1.1.7.*\")]";
			//AsmVer.Replace(str,"${1}"+""+"${3}");
		}
	}
}
