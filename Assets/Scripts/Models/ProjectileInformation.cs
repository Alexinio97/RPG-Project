using RPG.Attributes;
using UnityEngine;

namespace RPG.Models
{
    public class ProjectileInformation
    {
        public Transform RightHand { get; set; }
        public Transform LeftHand { get; set; }
        public Health Target { get; set; }
        public GameObject Instigator { get; set; }
        public float CalculatedDamage { get; set; }
    }
}
