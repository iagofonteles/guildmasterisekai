using System.IO;
using Drafts.SaveData;
using System;
using UnityEngine;

namespace GuildMasterIsekai
{
    [Serializable]
    public class Assignment : IBinarySave
    {
        public enum TaskType { Quest, Training, Scout }

        [SerializeField] private TaskType taskType;
        [SerializeField] private uint taskId;
        [SerializeField] private long departTime;
        [SerializeField] private float duration;

        private object _task;

        public object Task => _task ??= GetTask();

        private object GetTask()
        {
            var hall = Game.Save.Get<GuildHall>();

            return taskType switch
            {
                TaskType.Quest => throw new NotImplementedException(),
                TaskType.Training => throw new NotImplementedException(),
                TaskType.Scout => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };
        }

        public Assignment(Quest quest)
        {
            taskId = quest.Id;
            taskType = TaskType.Quest;
        }

        public Assignment() { }

        public void Save(BinaryWriter writer)
        {
            writer.Write((int)taskType);
            writer.Write(taskId);
            writer.Write(departTime);
            writer.Write(duration);
        }

        public void Load(BinaryReader reader)
        {
            taskType = (TaskType)reader.ReadInt32();
            taskId = reader.ReadUInt32();
            departTime = reader.ReadInt64();
            duration = reader.ReadSingle();
            _task = null;
        }
    }
}