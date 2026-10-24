using GuildMasterIsekai.Model;
using GuildMasterIsekai.Systems;
using UnityEngine;

namespace GuildMasterIsekai.ModelView
{
    [DefaultExecutionOrder(-100)]
    public class GameSaveData : MonoBehaviour
    {
        [SerializeField] private bool overrideGuild;
        [SerializeField] private Guild guild;
        [SerializeField] private bool overrideHall;
        [SerializeField] private GuildHall hall;

        private void Awake()
        {
            if (overrideGuild) Game.SaveMngr.Set(guild);
            if (overrideHall) Game.SaveMngr.Set(hall);
        }

        public void Start()
        {
            guild = Game.SaveMngr.Get<Guild>();
            hall = Game.SaveMngr.Get<GuildHall>();
        }
    }
}