using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NathanielGame.Levels;

namespace NathanielGame
{
    public class PathFinder
    {
        #region Declarations
        private enum NodeStatus { Open, Closed };

        private readonly Dictionary<Vector2, NodeStatus> _nodeStatus =
            new Dictionary<Vector2, NodeStatus>();

        private const int CostStraight = 10;
        private const int CostDiagonal = 15;

        private readonly List<PathNode> _openList = new List<PathNode>();

        private readonly Dictionary<Vector2, float> _nodeCosts =
            new Dictionary<Vector2, float>();
        #endregion

        #region Helper Methods
        private void AddNodeToOpenList(PathNode node)
        {
            int index = 0;
            float cost = node.TotalCost;

            while ((_openList.Count() > index) &&
                (cost < _openList[index].TotalCost))
            {
                index++;
            }

            _openList.Insert(index, node);
            _nodeCosts[node.GridLocation] = node.TotalCost;
            _nodeStatus[node.GridLocation] = NodeStatus.Open;
        }

        private IEnumerable<PathNode> FindAdjacentNodes(
        PathNode currentNode,
        PathNode endNode)
        {
            var adjacentNodes = new List<PathNode>();

            var x = currentNode.GridX;
            var y = currentNode.GridY;

            bool upLeft = true;
            bool upRight = true;
            bool downLeft = true;
            bool downRight = true;
                       
            if ((x > 0) && (!TiledMap.IsCollisionTile(x - 1, y)))
            {
                adjacentNodes.Add(new PathNode(
                        currentNode,
                        endNode,
                        new Vector2(x - 1, y),
                        CostStraight + currentNode.DirectCost));
            }
            else
            {
                upLeft = false;
                downLeft = false;
            }

            if ((x < Level.CurrentMap.Width-1) && (!TiledMap.IsCollisionTile(x + 1, y)))
            {
                adjacentNodes.Add(new PathNode(
                        currentNode,
                        endNode,
                        new Vector2(x + 1, y),
                        CostStraight + currentNode.DirectCost));
            }
            else
            {
                upRight = false;
                downRight = false;
            }


            if ((y > 0) && (!TiledMap.IsCollisionTile(x, y - 1)))
            {
                adjacentNodes.Add(new PathNode(
                    currentNode,
                    endNode,
                    new Vector2(x, y - 1),
                    CostStraight + currentNode.DirectCost));
            }
            else
            {
                upLeft = false;
                upRight = false;
            }

            if ((y < Level.CurrentMap.Height-1) && (!TiledMap.IsCollisionTile(x, y + 1)))
            {
                adjacentNodes.Add(new PathNode(
                    currentNode,
                    endNode,
                    new Vector2(x, y + 1),
                    CostStraight + currentNode.DirectCost));
            }
            else
            {
                downLeft = false;
                downRight = false;
            }


            if ((upLeft) && (!TiledMap.IsCollisionTile(x - 1, y - 1)))
            {
                adjacentNodes.Add(new PathNode(
                    currentNode,
                    endNode,
                    new Vector2(x - 1, y - 1),
                    CostDiagonal + currentNode.DirectCost));
            }

            if ((upRight) && (!TiledMap.IsCollisionTile(x + 1, y - 1)))
            {
                adjacentNodes.Add(new PathNode(
                    currentNode,
                    endNode,
                    new Vector2(x + 1, y - 1),
                    CostDiagonal + currentNode.DirectCost));
            }

            if ((downLeft) && (!TiledMap.IsCollisionTile(x - 1, y + 1)))
            {
                adjacentNodes.Add(new PathNode(
                    currentNode,
                    endNode,
                    new Vector2(x - 1, y + 1),
                    CostDiagonal + currentNode.DirectCost));
            }

            if ((downRight) && (!TiledMap.IsCollisionTile(x + 1, y + 1)))
            {
                adjacentNodes.Add(new PathNode(
                    currentNode,
                    endNode,
                    new Vector2(x + 1, y + 1),
                    CostDiagonal + currentNode.DirectCost));
            }

            return adjacentNodes;
        }

        #endregion

        #region Public Methods
        public List<Vector2> FindPath(
            Vector2 startTile,
            Vector2 endTile)
        {
            if (TiledMap.IsCollisionTile(endTile) ||
                TiledMap.IsCollisionTile(startTile))
            {
                return null;
            }

            _openList.Clear();
            _nodeCosts.Clear();
            _nodeStatus.Clear();

            var endNode = new PathNode(null, null, endTile, 0);
            var startNode = new PathNode(null, endNode, startTile, 0);

            AddNodeToOpenList(startNode);

            while (_openList.Count > 0)
            {
                PathNode currentNode = _openList[_openList.Count - 1];

                if (currentNode.IsEqualToNode(endNode))
                {
                    var bestPath = new List<Vector2>();
                    while (currentNode != null)
                    {
                        bestPath.Insert(0, currentNode.GridLocation);
                        currentNode = currentNode.ParentNode;
                    }
                    return bestPath;
                }

                _openList.Remove(currentNode);
                _nodeCosts.Remove(currentNode.GridLocation);

                foreach (
                    PathNode possibleNode in
                    FindAdjacentNodes(currentNode, endNode))
                {
                    if (_nodeStatus.ContainsKey(possibleNode.GridLocation))
                    {
                        if (_nodeStatus[possibleNode.GridLocation] ==
                            NodeStatus.Closed)
                        {
                            continue;
                        }

                        if (
                            _nodeStatus[possibleNode.GridLocation] ==
                            NodeStatus.Open)
                        {
                            if (possibleNode.TotalCost >=
                                _nodeCosts[possibleNode.GridLocation])
                            {
                                continue;
                            }
                        }
                    }

                    AddNodeToOpenList(possibleNode);
                }

                _nodeStatus[currentNode.GridLocation] = NodeStatus.Closed;
            }

            return null;
        }
        #endregion
    }
}
