using System;
using UnityEngine;

namespace GuildMasterIsekai.Visitor
{
    [CreateAssetMenu(menuName = "Guild Master/VisitorAIConfig")]
    public class VisitorAIConfig : ScriptableObject
    {
        [SerializeField] private CostomerConf freelancer;
        [SerializeField] private CostomerConf costomer;

        public CostomerConf Costomer => costomer;

        [Serializable]
        public class CostomerConf
        {
            public float sitTime = 15;
        }
    }
}