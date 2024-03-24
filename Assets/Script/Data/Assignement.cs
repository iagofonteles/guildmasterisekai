using Drafts;
using System;

namespace GuildMasterIsekai {
	[Serializable]
	public class Assignement {
		public enum TaskType { Quest, Training, Scout }

		TaskType taskType;
		string taskGuid;
		object _task;

		public object Task => _task ??= GetTask();

		private object GetTask() {
			var guild = Game.Save.Get<Guild>();

			return taskType switch {
				TaskType.Quest => guild.QuestBoard.BinaryFind(taskGuid),
				TaskType.Training => throw new NotImplementedException(),
				TaskType.Scout => throw new NotImplementedException(),
				_ => throw new NotImplementedException(),
			};
		}

		public Assignement(Quest quest) {
			taskGuid = Guid.NewGuid().ToString();
		}
	}
}
