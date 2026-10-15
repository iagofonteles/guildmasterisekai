using System;
using System.IO;
using Drafts.SaveData;

namespace GuildMasterIsekai.Model
{
    [Serializable]
    public class ProblemRequest : IBinarySave
    {
        public ProblemRequest(Costomer costomer, Problem problem)
        {
            Costomer = costomer;
            Problem = problem;
        }

        public ProblemRequest() { }
        public Costomer Costomer { get; private set; }
        public Problem Problem { get; private set; }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Costomer != null);
            Costomer?.Save(writer);
            writer.Write(Problem != null);
            Problem?.Save(writer);
        }

        public void Load(BinaryReader reader)
        {
            Costomer = reader.ReadBoolean() ? new Costomer() : null;
            Costomer?.Load(reader);
            Problem = reader.ReadBoolean() ? new Problem() : null;
            Problem?.Load(reader);
        }

        public event Action<ProblemRequest, int> OnReplied;
        public void Reply(int reply) => OnReplied?.Invoke(this, reply);
    }
}