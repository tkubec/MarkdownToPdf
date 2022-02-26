namespace Tests
{
    internal static class Program
    {
        private static void Main()
        {
            RunAllExamples();
        }

        private static void RunAllExamples()
        {
            Examples.HelloWorld.Run();
            Examples.BasicStyling.Run();
            Examples.CustomStyles.Run();
            Examples.AdvancedStyling.Run();
            Examples.Tables.Run();
            Examples.Sections.Run();
            Examples.Events.Run();
            Examples.Toc.Run();
            Examples.Highlighting.Run();
            Examples.Features.Run();
            Examples.Attributes.Run();
            Examples.FullBook.Run();
        }
    }
}