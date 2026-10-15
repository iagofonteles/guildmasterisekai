using GuildMasterIsekai.Visitor.Spot;
using UnityEngine;
using UnityEngine.AI;

namespace GuildMasterIsekai.Visitor
{
    public interface IVisitor
    {
        GameObject Prefab => Resources.Load<GameObject>("Visitor/Default");
        IVisitorAI GetAI(NavMeshAgent agent, HallSpots spots);
    }
}