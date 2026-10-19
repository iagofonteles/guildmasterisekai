using GuildMasterIsekai.Model;
using GuildMasterIsekai.Systems;
using GuildMasterIsekai.Visitor.Spot;
using UnityEngine;
using UnityEngine.AI;

namespace GuildMasterIsekai.Visitor
{
    public class CostomerVisitorAI : BaseVisitorAI
    {
        private static VisitorAIConfig.CostomerConf _config;
        private readonly Costomer _costomer;
        private readonly Guild guild;
        private readonly GuildHall hall;
        private ProblemRequest pendingRequest;

        public CostomerVisitorAI(Costomer costomer, NavMeshAgent agent, HallSpots spots)
            : base(agent, spots)
        {
            this._costomer = costomer;
            guild = Game.SaveMngr.Get<Guild>();
            hall = Game.SaveMngr.Get<GuildHall>();
            hall.Costomers.Add(costomer);

            CheckQueue();
        }

        private static VisitorAIConfig.CostomerConf Config =>
            _config ??= Resources.Load<VisitorAIConfig>(nameof(VisitorAIConfig)).Costomer;

        private void CheckQueue()
        {
            if (Spots.Office.IsFull) Leave();
            else WaitInQueue(Spots.Office, WaitReply);
        }

        private void WaitReply()
        {
            var problem = guild.Generator.NewProblem(_costomer);
            var request = new ProblemRequest(_costomer, problem);
            pendingRequest = request;
            request.OnReplied += Reply;
            hall.ProblemRequests.Add(request);

            Wait(Config.sitTime, () => request.Reply(-1));
        }

        public void Reply(ProblemRequest request, int reply)
        {
            if (request != pendingRequest) return;
            ClearRequest();
            if (reply >= 0 && reply < request.Problem.Outcomes.Length)
                foreach (var quest in request.Problem.Outcomes[reply])
                    hall.QuestBoard.Add(quest);

            if (Agent) Leave();
        }

        private void ClearRequest()
        {
            if (pendingRequest == null) return;
            var request = pendingRequest;
            pendingRequest = null;
            request.OnReplied -= Reply;
            hall.ProblemRequests.Remove(request);
        }

        public override void Dispose()
        {
            ClearRequest();
            hall.Costomers.Remove(_costomer);
            base.Dispose();
        }
    }
}