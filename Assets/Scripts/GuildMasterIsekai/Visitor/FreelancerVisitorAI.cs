using Drafts;
using GuildMasterIsekai.Model;
using GuildMasterIsekai.Systems;
using GuildMasterIsekai.Visitor.Spot;
using UnityEngine;
using UnityEngine.AI;

namespace GuildMasterIsekai.Visitor
{
    public class FreelancerVisitorAI : BaseVisitorAI
    {
        private readonly Guild _guild;
        private readonly GuildHall _hall;
        private readonly Adventurer _adventurer;
        private QuestRequest _pendingRequest;

        public FreelancerVisitorAI(Adventurer adventurer, NavMeshAgent agent, HallSpots spots)
            : base(agent, spots)
        {
            _adventurer = adventurer;
            _guild = Game.SaveMngr.Get<Guild>();
            _hall = Game.SaveMngr.Get<GuildHall>();
            _hall.Freelancers.Add(adventurer);

            CheckActions();
        }

        public override void Dispose()
        {
            ClearRequest();
            _hall.Freelancers.Remove(_adventurer);
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
            if (_hall.QuestBoard.Count == 0)
            {
                CheckActions();
                return;
            }

            var quest = _hall.QuestBoard[Random.Range(0, _hall.QuestBoard.Count)];
            var request = new QuestRequest(_adventurer, quest);
            _pendingRequest = request;
            request.OnReplied += Reply;
            _hall.QuestRequests.Add(request);
            _hall.QuestBoard.OnChanged += CancelIfRemoved;

            Wait(20, () => request.Reply(null));
        }

        private void CancelIfRemoved(ListEvent action, int index, Quest quest)
        {
            if (_pendingRequest != null && !_hall.QuestBoard.Contains(_pendingRequest.Quest))
                Reply(_pendingRequest, null);
        }

        private void ClearRequest()
        {
            if (_pendingRequest == null) return;
            var request = _pendingRequest;
            _pendingRequest = null;
            request.OnReplied -= Reply;
            _hall.QuestBoard.OnChanged -= CancelIfRemoved;
            _hall.QuestRequests.Remove(request);
        }

        public void Reply(QuestRequest request, Quest quest)
        {
            if (request != _pendingRequest) return;
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