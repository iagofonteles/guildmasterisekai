using System.IO;
using Drafts.SaveData;
using Drafts;
using System;
using UnityEngine;

namespace GuildMasterIsekai {
	[Serializable]
	public class Assignement : IBinarySave {
		public enum TaskType { Quest, Training, Scout }

		[SerializeField] TaskType taskType;
		[SerializeField] string taskGuid;
		[SerializeField] long departTime;
		[SerializeField] float duration;

		object _task;

		public object Task => _task ??= GetTask();

		private object GetTask() {
			var hall = Game.Save.Get<GuildHall>();

			return taskType switch {
				TaskType.Quest => hall.QuestBoard.BinaryFind(taskGuid),
				TaskType.Training => throw new NotImplementedException(),
				TaskType.Scout => throw new NotImplementedException(),
				_ => throw new NotImplementedException(),
			};
		}

		public Assignement(Quest quest) {
			taskGuid = quest.Guid;
			taskType = TaskType.Quest;
		}

		public Assignement() { }

		public void Save(BinaryWriter writer) {
			writer.Write((int)taskType);
			writer.Write(taskGuid != null);
			if (taskGuid != null) writer.Write(taskGuid);
			writer.Write(departTime);
			writer.Write(duration);
		}

		public void Load(BinaryReader reader) {
			taskType = (TaskType)reader.ReadInt32();
			taskGuid = reader.ReadBoolean() ? reader.ReadString() : null;
			departTime = reader.ReadInt64();
			duration = reader.ReadSingle();
			_task = null;
		}
	}
}
