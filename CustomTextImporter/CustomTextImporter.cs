using Microsoft.Xna.Framework.Content.Pipeline;

namespace CustomTextImporter
{
    [ContentImporter(".xml", ".txt", ".htm", ".html", DisplayName = "Custom Text Importer")]
    class CustomTextImporter : ContentImporter<CustomText>
    {
        public override CustomText Import(string filename, ContentImporterContext context)
        {
            string sourceCode = System.IO.File.ReadAllText(filename);
            return new CustomText(sourceCode);
        }
    }
}
