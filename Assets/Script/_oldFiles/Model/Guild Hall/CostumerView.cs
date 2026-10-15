using Drafts.DataView;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuildMasterIsekai
{
    public class CostumerView : DataView<Costumer>
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