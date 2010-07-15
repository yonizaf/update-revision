﻿using System;
//using System.Collections.Generic;
using System.IO;
//using System.Linq;
//using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Forms;


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
			string currentDir = Directory.GetCurrentDirectory();
			string RepoDir = Path.GetDirectoryName(Application.ExecutablePath);

			Console.WriteLine("UpdateRevision version " + Application.ProductVersion);
			Console.WriteLine("current dir: " + currentDir + "\n");

			Console.WriteLine("Computing Arguments...");
			foreach (string arg in args)
			{
				if (arg == "--revert")
				{// revert mode will change revision to *
					revert = true;
				}
				else if (Regex.IsMatch(arg,"-f=",RegexOptions.IgnoreCase))
				{//filename for input/output
					inputFile = Regex.Replace(arg,"(-f=)(.*)","$2",RegexOptions.IgnoreCase);
				}
				else if (Regex.IsMatch(arg,"-rd=",RegexOptions.IgnoreCase))
				{//repository directory
					RepoDir = Regex.Replace(arg, "(-rd=)(.*)", "$2", RegexOptions.IgnoreCase);
					if (RepoDir == "" || !Directory.Exists(RepoDir))
					{
						Console.WriteLine("Repository directory does not exist. using default.");
						RepoDir = Path.GetDirectoryName(Application.ExecutablePath);
					}
				}
			}
			Console.WriteLine("Input file: " + inputFile + "\nrepository dir: " + Path.GetFullPath(RepoDir) + "\n" + (revert ? "Mode: revert." : "Mode: normal")+"\n");

			if (!revert)//no need for revision on revert
			{
				Directory.SetCurrentDirectory(RepoDir);
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
				Directory.SetCurrentDirectory(currentDir);

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
			Directory.SetCurrentDirectory(currentDir);
		}
	}
}