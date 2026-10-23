using UnityEngine.UIElements;

namespace TileGrid.UI
{
    public interface IViewElement
    {
        object Data { get; set; }
        void Refresh();
        void SetSelected(object selected);
        VisualElement Element { get; }
    }

    public abstract class ViewElement<T> : VisualElement, IViewElement
    {
        private T _data;

        public T Data
        {
            get => _data;
            set {
                _data = value;
                Refresh();
            }
        }

        object IViewElement.Data { get => Data; set => Data = value is T d ? d : default; }

        protected abstract void Subscribe();
        protected abstract void Unsubscribe();
        public abstract void Refresh();
        public abstract void SetSelected(object selected);

        VisualElement IViewElement.Element => this;
    }
}