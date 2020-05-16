﻿using CommandLine;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GModMountManager
{
    internal static class Program
    {
        public static Options Arguments = new Options();
        public static Main mainWindow;
        public static bool FirstRun = false;
        public static IniData config = Config.Load();

        public class Options
        {
            [Option('c', "console", Required = false, HelpText = "Enable console")]
            public bool ConsoleEnabled { get; set; }

            [Option("ignoresslerrors", Required = false, HelpText = "Ignore SSL Errors")]
            public bool IgnoreSSLErrors { get; set; }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            Logger.Debug("Current Date and Time: {0} (UTC: {1})", DateTime.Now, DateTime.UtcNow);
            var assembly = System.Reflection.Assembly.GetEntryAssembly().GetName();
            Logger.Debug("{0} v{1} ({2}) with args: {3}", assembly.Name, assembly.Version, assembly.ProcessorArchitecture, string.Join(" ", args));
            Parser.Default.ParseArguments<Options>(args).WithParsed(o => Arguments = o).WithNotParsed(o => Logger.Error("Unable to parse arguments: {0}", o.First().Tag));
            Logger.Trace("Parsed arguments: {}", Arguments.ToJSON());
            if (Arguments.ConsoleEnabled) ExternalConsole.InitConsole();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mainWindow = new Main();
            Application.Run(mainWindow);
            Logger.Debug("Ended");
            OnProcessExit(false, new EventArgs());
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            Logger.Log("Exiting...");
            Logger.Trace(config.ToJSON());
            Config.Save(config);
            ExternalConsole.Dispose();
            Application.Exit();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}