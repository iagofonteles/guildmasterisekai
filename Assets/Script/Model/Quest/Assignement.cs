using Drafts;
using System;
using UnityEngine;

namespace GuildMasterIsekai {
	[Serializable]
	public class Assignement {
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
	}
}
