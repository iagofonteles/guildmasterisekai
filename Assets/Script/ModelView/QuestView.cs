using Drafts.DataView;
using GuildMasterIsekai.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuildMasterIsekai.ModelView
{
    public class QuestView : DataView<Quest>
    {
        [SerializeField] private TMP_Text id;
        [SerializeField] private TMP_Text displayName;
        [SerializeField] private TMP_Text description;
        [SerializeField] private TMP_Text rank;
        [SerializeField] private Image icon;
        [SerializeField] private StatsView requisite;
        [SerializeField] private TMP_Text fee;
        //[SerializeField] Reward reward;

        protected override void Subscribe()
        {
            id.TrySetText(Data.Id);
            displayName.TrySetText(Data.DisplayName);
            description.TrySetText(Data.Description);
            rank.TrySetText(Data.Rank.ToString());
            icon.TrySetSprite(Data.Icon);
            requisite.TrySetData(Data.Requisite);
            fee.TrySetText(Data.Fee);
            //reward = reward;
        }
    }
}