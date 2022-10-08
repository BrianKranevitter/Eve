using Game.Enemies;

using UnityEngine;

namespace Game.Level.Triggers
{
    public sealed class MakeEnemiesAwareOfPlayerLocationPlayerTriggerAction : PlayerTriggerAction
    {
        [SerializeField, Tooltip("Enemies to make aware of player location.")]
        private Enemy[] enemies;

        public override void OnTriggerEnter()
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy == null)
                    continue;
                enemy.MakeAwareOfPlayerLocation();
            }
        }
    }
}