using Drafts.DataView;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuildMasterIsekai
{
    public class CostumerView : DataView<Costumer>
    {
        [SerializeField] TextMeshProUGUI id;
        [SerializeField] TextMeshProUGUI displayName;
        [SerializeField] Image icon;

        protected override void Subscribe()
        {
            base.Subscribe();
            id.TrySetText(Data.Guid);
            displayName.TrySetText(Data.DisplayName);
            icon.TrySetSprite(Data.Icon);
        }
    }
}