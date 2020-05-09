using Microsoft.Xna.Framework.Content;

namespace CustomTextImporter
{
    public class CustomTextReader : ContentTypeReader<CustomText>
    {
        protected override CustomText Read(ContentReader input, CustomText existingInstance)
        {
            return new CustomText(input.ReadString());
        }
    }

}