using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace CustomTextImporter
{
    [ContentTypeWriter()]
    public class CustomTextWriter : ContentTypeWriter<CustomText>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(CustomTextReader).AssemblyQualifiedName;
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(CustomText).AssemblyQualifiedName;
        }

        protected override void Write(ContentWriter output, CustomText value)
        {
            output.Write(value.SourceCode);
        }
    }
}
