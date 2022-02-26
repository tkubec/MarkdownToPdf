using Orionsoft.MarkdownToPdfLib;

namespace Tests.Examples
{
    /// <summary>
    /// Hello World example
    /// </summary>
    public static class HelloWorld
    {
        public static void Run()
        {
            var pdf = new MarkdownToPdf();

            pdf
             .Add("## Hello World\r\n\r\nHello!")
             .Save("hello.pdf");
        }
    }
}