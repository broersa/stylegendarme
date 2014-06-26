/// <summary>
/// Watch out! when updating stylecop package also update stylecoprules dll from original msi distribution
/// </summary>
namespace StyleGendarme
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using StyleCop;
    using System.IO;
    using NDesk.Options;

    public class Program
    {
        static void Main(string[] args)
        {
            string inpath = null;
            string outxml = null;
            bool help = false;
            bool verbose = false;
            var p = new OptionSet () {
   	            { "inpath=",    v => inpath = v },
   	            { "outxml=",    v => outxml = v },
   	            { "v|verbose",  v => verbose = v != null },
   	            { "h|?|help",   v => help = v != null },
            };
            p.Parse (args);
            if (inpath == null || outxml == null || help)
            {
                Console.WriteLine("StyleGendarme.exe --inpath <path> --outxml <file>");
            }
            else
            {
                StyleCopConsole console = new StyleCopConsole(null, true, outxml, null, true);
                Configuration configuration = new Configuration(new string[] { "DEBUG" });
                List<CodeProject> projects = new List<CodeProject>();
                CodeProject project = new CodeProject(inpath.GetHashCode(), inpath, configuration);

                // Add each source file to this project.
                foreach (string sourceFilePath in Directory.GetFiles(inpath, "*.cs", SearchOption.AllDirectories))
                {
                    console.Core.Environment.AddSourceCode(project, sourceFilePath, null);
                }
                projects.Add(project);
                console.OutputGenerated += new EventHandler<OutputEventArgs>(OnOutputGenerated);
                console.ViolationEncountered += new EventHandler<ViolationEventArgs>(OnViolationEncountered);
                console.Start(projects, true);
            }
        }

        static protected void OnOutputGenerated(object sender, OutputEventArgs e)
        {
            Console.WriteLine(e.Output);
        }

        static protected void OnViolationEncountered(object sender, ViolationEventArgs e)
        {
            Console.WriteLine("violation: " + e.Violation.Message);
        }
        
    }
}
