using System.Collections;
using System.Collections.Specialized;
using Drafts.DataView;
using UnityEngine.Events;

namespace GuildMasterIsekai.ModelView
{
    // Retains the count callback used by GameCanvas with the current DataView package.
    public class GameCollectionView : CollectionView
    {
        public UnityEvent<int> onCountChanged = new();

        protected override void Subscribe()
        {
            base.Subscribe();
            NotifyCount();
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            if (!IsDestroying) onCountChanged.Invoke(0);
        }

        protected override void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.CollectionChanged(sender, e);
            NotifyCount();
        }

        private void NotifyCount()
        {
            var count = 0;
            if (Data is ICollection collection) count = collection.Count;
            else if (Data != null)
                foreach (var item in Data) count++;
            onCountChanged.Invoke(count);
        }
    }
}
