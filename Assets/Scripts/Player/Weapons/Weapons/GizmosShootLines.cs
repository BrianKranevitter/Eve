using System.Collections.Generic;

using UnityEngine;

namespace Game.Player.Weapons
{
#if UNITY_EDITOR
    public sealed class GizmosShootLines
    {
        private List<(Vector3, Vector3, Color, float)> gizmos = new List<(Vector3, Vector3, Color, float)>();

        public void Add(Transform start, Vector3 direction, float maximumDistance, Color color)
            => gizmos.Add((start.position, start.position + (direction * maximumDistance), color, Time.time + 1f));

        public void Add(Transform start, Vector3 end, Color color)
           => gizmos.Add((start.position, end, color, Time.time + 1f));

        public void OnDrawGizmos()
        {
            for (int i = gizmos.Count - 1; i > 0; i--)
            {
                (Vector3, Vector3, Color, float) info = gizmos[i];
                Gizmos.color = info.Item3;
                Gizmos.DrawLine(info.Item1, info.Item2);
                if (info.Item4 < Time.time)
                    gizmos.RemoveAt(i);
            }
        }
    }
#endif
}
