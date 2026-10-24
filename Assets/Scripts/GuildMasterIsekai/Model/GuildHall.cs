using System;
using System.IO;
using Drafts;
using Drafts.SaveData;
using GuildMasterIsekai.Systems;
using UnityEngine;

namespace GuildMasterIsekai.Model
{
    [Serializable]
    public class GuildHall : IBinarySave
    {
        [SerializeField] private Watcher<bool> openStatus = new();
        [SerializeField] private ObservableList<Quest> questBoard = new();
        [SerializeField] private ObservableList<Costomer> costomers = new();
        [SerializeField] private ObservableList<Adventurer> freelancers = new();
        [SerializeField] private ObservableList<QuestRequest> requests = new();
        [SerializeField] private ObservableList<ProblemRequest> problems = new();

        public Watcher<bool> OpenStatus => openStatus;
        public ObservableList<Quest> QuestBoard => questBoard;
        public ObservableList<Costomer> Costomers => costomers;
        public ObservableList<Adventurer> Freelancers => freelancers;
        public ObservableList<QuestRequest> QuestRequests => requests;
        public ObservableList<ProblemRequest> ProblemRequests => problems;

        public void Save(BinaryWriter writer)
        {
            writer.WriteSaveCollection(questBoard);
            writer.WriteSaveCollection(costomers);
            writer.WriteSaveCollection(freelancers);
            writer.WriteSaveCollection(requests);
            writer.WriteSaveCollection(problems);
        }

        public void Load(BinaryReader reader)
        {
            questBoard = new(reader.ReadSaveCollection<Quest>());
            costomers = new(reader.ReadSaveCollection<Costomer>());
            freelancers = new(reader.ReadSaveCollection<Adventurer>());
            requests = new(reader.ReadSaveCollection<QuestRequest>());
            problems = new(reader.ReadSaveCollection<ProblemRequest>());
        }
    }
}