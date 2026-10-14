using System;
using Drafts.Database;
using UnityEngine;

namespace GuildMasterIsekai
{
    public abstract class DatabaseItem : ScriptableObject, IDatabaseItemInternal
    {
        [SerializeField] protected Sprite icon;
        
        [NonSerialized] protected uint _id;
        [NonSerialized] protected locstr _displayName;
        [NonSerialized] protected string _description;

        public virtual int Index { get; private set; }
        public virtual object Id => _id ??= $"{GetType().Name} {name}".ToLower();
        public virtual string DisplayName => _displayName ??= name;
        public virtual string Description => _description ??= Id + "-desc";
        public virtual Sprite Icon => icon;
        
        public virtual bool Discovered { get; set; }

        void IDatabaseItemInternal.SetIndex(int i) => Index = i;
    }
    
    public class Database :Drafts.Database.DatabaseSO<> { }
}