using System;
using System.Linq;
using GoCommando;
using GoCommando.Api;
using GoCommando.Attributes;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;


namespace Solution_Formatter {

[Banner(@"Roslyn Solution Formatter
 
Copyright (c) 2014 Allan Ritchie
")]
    public class Program : ICommando {

        [NamedArgument("solution", "f")]
        [PositionalArgument]
        [Description("Specifies the Microsoft Visual Studio solution file you wish to format")]
        [Example("Test.sln")]
        public string SolutionFileName { get; set; }


        static void Main(string[] args) {
            Go.Run<Program>(args);
        }


        public void Run() {
            var workspace = MSBuildWorkspace.Create();

            Console.WriteLine("Opening solution: {0}", this.SolutionFileName);
            var loadSolution = workspace.OpenSolutionAsync(this.SolutionFileName).Result;
            var solution = loadSolution;
            var documents = solution.Projects.SelectMany(x => x.Documents).ToList();

            Console.WriteLine("Loaded Successfully.  {0} documents across {1} projects to format", solution.Projects.Count(), documents.Count);

            var count = 1;
            foreach (var doc in documents) {
                Console.WriteLine();
                Console.Write("Formatting {0} of {1} - {2}", count, documents.Count, doc.FilePath);
                var newDoc = Formatter.FormatAsync(doc).Result;
                solution = newDoc.Project.Solution; // immutable - original document is not touched and a new solution is created
                count++;
            }
            if (!workspace.TryApplyChanges(solution))
                Console.WriteLine("Failed to apply changes");

            Console.WriteLine("Solution {0} Format Complete", this.SolutionFileName);
            Console.WriteLine("Press <Enter> to exit.");
            Console.ReadLine();
        }
    }
}
