namespace Scriber.Packaging.Documentation
{
    public abstract class DocumentationElement
    {
        public string? Name { get; set; }
        public string? Description { get; set; }

        public abstract string ToMarkdown();
    }
}
