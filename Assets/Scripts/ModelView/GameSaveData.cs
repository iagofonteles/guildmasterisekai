using System;
using Drafts.DataView;
using GuildMasterIsekai.Model;
using GuildMasterIsekai.Systems;
using UnityEngine;

namespace GuildMasterIsekai.ModelView
{
    public class GameSaveData : MonoBehaviour
    {
        [SerializeField] private Data data;

        private void Start()
        {
            var guild = Game.Save.Get<Guild>();
            var hall = Game.Save.Get<GuildHall>();

            GetComponent<DataView>().SetData(data switch
            {
                Data.Guild => guild,
                Data.GuildHall => hall,
                Data.GuildRank => guild.Rank,
                Data.Adventurers => guild.Adventurers,
                Data.Freelancers => hall.Freelancers,
                Data.Costumers => hall.Costomers,
                Data.QuestBoard => hall.QuestBoard,
                Data.QuestRequests => hall.QuestRequests,
                Data.ProblemRequests => hall.ProblemRequests,
                _ => throw new NotImplementedException()
            });
        }

        private enum Data
        {
            Guild,
            GuildHall,
            GuildRank,
            Adventurers,
            Freelancers,
            Costumers,
            QuestBoard,
            QuestRequests,
            ProblemRequests
        }
    }
}