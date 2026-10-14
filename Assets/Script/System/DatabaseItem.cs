using Drafts;
using Drafts.Database;
using Drafts.Localization;
using UnityEngine;
using UnityEngine.Localization;

namespace GuildMasterIsekai
{
    public abstract class DatabaseEntrySo : ScriptableObject, IDatabaseItemInternal
    {
        [SerializeField, ReadOnly] private uint id;
        [SerializeField, Loc] private LocalizedString displayName;
        [SerializeField, Loc] private LocalizedString description;
        [SerializeField] protected Sprite icon;

        object IDatabaseItem.Id => id;
        public uint Id => id;
        public virtual int Index { get; private set; }
        public virtual string DisplayName => displayName.GetString(this);
        public virtual string Description => description.GetString(this);
        public virtual Sprite Icon => icon;
        public virtual bool Discovered => true;

        void IDatabaseItemInternal.SetIndex(int i) => Index = i;

        [ContextMenu("Reroll Id")]
        private void RerollId()
        {
            do id = Game.Id.Generate();
            while (Game.Database.Find(id) != null);
        }
    }
}