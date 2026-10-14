using Drafts;
using UnityEngine;
using UnityEngine.AI;

namespace GuildMasterIsekai
{
    public class FreelancerVisitorAI : BaseVisitorAI
    {
        [SerializeField] private Adventurer adventurer;

        private readonly Guild _guild;
        private readonly GuildHall _hall;

        public FreelancerVisitorAI(Adventurer adventurer, NavMeshAgent agent, HallSpots spots)
            : base(agent, spots)
        {
            this.adventurer = adventurer;
            _guild = Game.Save.Get<Guild>();
            _hall = Game.Save.Get<GuildHall>();
            _hall.Freelancers.Add(adventurer);

            CheckActions();
        }

        protected override void Destroy()
        {
            base.Destroy();
            _hall.Freelancers.Remove(adventurer);
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
            var quest = _hall.QuestBoard.RandomOrDefault();
            var request = new QuestRequest(adventurer, quest);
            request.OnReplied += Reply;
            _hall.QuestRequests.Add(request);
            _hall.QuestBoard.OnRemoved += CancelIfRemoved;

            Wait(20, () => request.Reply(null));

            void CancelIfRemoved(Quest q)
            {
                if (q == quest) Reply(request, null);
                _hall.QuestBoard.OnRemoved -= CancelIfRemoved;
            }
        }

        public void Reply(QuestRequest request, Quest quest)
        {
            _hall.QuestRequests.Remove(request);

            if (quest == request.Quest)
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