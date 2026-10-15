using System;
using System.IO;
using Drafts;
using Drafts.SaveData;
using UnityEngine;

namespace GuildMasterIsekai.Model
{
    [Serializable]
    public class GuildHall : IBinarySave
    {
        [SerializeField] private ObservableList<Quest> questBoard = new();
        [SerializeField] private ObservableList<Costumer> costumers = new();
        [SerializeField] private ObservableList<Adventurer> freelancers = new();
        [SerializeField] private ObservableList<QuestRequest> requests = new();
        [SerializeField] private ObservableList<ProblemRequest> problems = new();

        public ObservableList<Quest> QuestBoard => questBoard;
        public ObservableList<Costumer> Costumers => costumers;
        public ObservableList<Adventurer> Freelancers => freelancers;
        public ObservableList<QuestRequest> QuestRequests => requests;
        public ObservableList<ProblemRequest> ProblemRequests => problems;

        public void Save(BinaryWriter writer)
        {
            writer.WriteSaveCollection(questBoard);
            writer.WriteSaveCollection(costumers);
            writer.WriteSaveCollection(freelancers);
            writer.WriteSaveCollection(requests);
            writer.WriteSaveCollection(problems);
        }

        public void Load(BinaryReader reader)
        {
            questBoard = new(reader.ReadSaveCollection<Quest>());
            costumers = new(reader.ReadSaveCollection<Costumer>());
            freelancers = new(reader.ReadSaveCollection<Adventurer>());
            requests = new(reader.ReadSaveCollection<QuestRequest>());
            problems = new(reader.ReadSaveCollection<ProblemRequest>());
        }
    }
}