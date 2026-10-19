using Drafts.DataView;
using GuildMasterIsekai.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuildMasterIsekai.ModelView
{
    public class CostumerView : DataView<Costomer>
    {
        [SerializeField] private TMP_Text id;
        [SerializeField] private TMP_Text displayName;
        [SerializeField] private Image icon;

        protected override void Subscribe()
        {
            base.Subscribe();
            id.TrySetText(Data.Id);
            displayName.TrySetText(Data.DisplayName);
            icon.TrySetSprite(Data.Icon);
        }
    }
}