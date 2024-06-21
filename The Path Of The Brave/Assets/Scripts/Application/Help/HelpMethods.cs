using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace MyApplication
{
    public static class HelpMethods
    {
        public static List<Vector3> ToPos(this List<GraphNode> list)
        {
            var n = new List<Vector3>();

            foreach (var item in list)
            {
                n.Add((Vector3)item.position);
            }

            return n;
        }

        public static List<GraphNode> ToGraphNode(this List<MyCharacterController> list)
        {
            var n = new List<GraphNode>();

            foreach (var item in list)
            {
                n.Add(item.MapNode);
            }

            return n;
        }
    }
}

