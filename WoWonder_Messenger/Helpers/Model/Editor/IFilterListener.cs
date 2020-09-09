using WoWonder.NiceArt.Models;

namespace PixelPhoto.Helpers.Model.Editor
{
    public interface IFilterListener
    {
        void OnFilterSelected(PhotoFilter photoFilter);
    }
}