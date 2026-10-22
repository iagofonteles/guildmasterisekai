namespace TileGrid.UI
{
    public interface IViewElement
    {
        object Data { get; set; }
        void Refresh();
        void SetSelected(object selected);
    }
}