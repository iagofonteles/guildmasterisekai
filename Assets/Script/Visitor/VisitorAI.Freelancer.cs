using GuildMasterIsekai.Model;
using GuildMasterIsekai.System;
using GuildMasterIsekai.Visitor.Spot;
using UnityEngine;
using UnityEngine.AI;

namespace GuildMasterIsekai.Visitor
{
    public class FreelancerVisitorAI : BaseVisitorAI
    {
        [SerializeField] private Adventurer adventurer;

        private readonly Guild _guild;
        private readonly GuildHall _hall;
        private QuestRequest pendingRequest;

        public FreelancerVisitorAI(Adventurer adventurer, NavMeshAgent agent, HallSpots spots)
            : base(agent, spots)
        {
            this.adventurer = adventurer;
            _guild = Game.Save.Get<Guild>();
            _hall = Game.Save.Get<GuildHall>();
            _hall.Freelancers.Add(adventurer);

            CheckActions();
        }

        public override void Dispose()
        {
            ClearRequest();
            _hall.Freelancers.Remove(adventurer);
            base.Dispose();
        }

        private void CheckActions()
        {
            var spot = _hall.QuestBoard.Count > 0 ? Spots.Board.GetFreeSpot() : null;

            if (spot)
            {
                WalkTo(spot, GetQuest);
                return;
            }

            spot ??= Spots.Table.GetFreeSpot();

            if (spot)
            {
                WalkTo(spot, Rest);
                return;
            }

            Leave();
        }

        private void GetQuest()
        {
            Wait(10, () =>
            {
                if (Spots.Counter.IsFull)
                {
                    Leave();
                    return;
                }

                WaitInQueue(Spots.Counter, WaitReply);
            });
        }

        private void WaitReply()
        {
            if (_hall.QuestBoard.Count == 0) { CheckActions(); return; }
            var quest = _hall.QuestBoard[Random.Range(0, _hall.QuestBoard.Count)];
            var request = new QuestRequest(adventurer, quest);
            pendingRequest = request;
            request.OnReplied += Reply;
            _hall.QuestRequests.Add(request);
            _hall.QuestBoard.OnChanged += CancelIfRemoved;

            Wait(20, () => request.Reply(null));

        }

        private void CancelIfRemoved(Drafts.ListEvent action, int index, Quest quest)
        {
            if (pendingRequest != null && !_hall.QuestBoard.Contains(pendingRequest.Quest))
                Reply(pendingRequest, null);
        }

        private void ClearRequest()
        {
            if (pendingRequest == null) return;
            var request = pendingRequest;
            pendingRequest = null;
            request.OnReplied -= Reply;
            _hall.QuestBoard.OnChanged -= CancelIfRemoved;
            _hall.QuestRequests.Remove(request);
        }

        public void Reply(QuestRequest request, Quest quest)
        {
            if (request != pendingRequest) return;
            ClearRequest();

            if (quest != null && quest == request.Quest && _hall.QuestBoard.Contains(quest))
            {
                CompleteQuest(quest);
                return;
            }

            var spot = Spots.Table.GetFreeSpot();

            if (spot != null)
            {
                WalkTo(spot, Rest);
                return;
            }

            Leave();
        }

        private void Rest() => Wait(12, CheckActions);

        private void CompleteQuest(Quest quest)
        {
            _hall.QuestBoard.Remove(quest);
            _guild.Gold.Value += quest.Reward.Gold;
            Leave();
        }
    }
}
