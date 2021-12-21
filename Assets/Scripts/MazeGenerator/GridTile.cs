using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Celestial.Levels
{
    public class GridTile : IEnumerable<TileConnection>
    {
        public Vector3Int gridPosition;
        public TileFlags flags;
        public GameObject instance;
        //just for convenience
        private List<TileConnection> connections = new List<TileConnection>();

        private Cardinals connectionCardinals;
        
        public int ConnectionCount => connections.Count;
    
        public Cardinals RequiresWalls {get; set;} = Cardinals.Undefined;
        public Cardinals ConnectionCardinals
        {
            get => connectionCardinals;
            set => connectionCardinals = value;
        }

        public void AddConnection(TileConnection con)
        {
            connections.Add(con);
            Vector3Int dir = con.other.gridPosition - this.gridPosition;
            if(dir.x > 0)
                connectionCardinals |= Cardinals.East;
            else if(dir.x < 0)
                connectionCardinals |= Cardinals.West;
            else if(dir.z > 0)
                connectionCardinals |= Cardinals.North;
            else if(dir.z < 0)
                connectionCardinals |= Cardinals.South;
        }

        public bool HasConnectionTo(GridTile other)
            => connections.Exists(x => x.other == other);
        public bool HasConnectionAtLocation(Vector3Int position)
            => connections.Exists(x => x.other.gridPosition == position);
        
        public bool HasConnectionInDirection(Cardinals cardinal)
            => connectionCardinals.HasFlag(cardinal);

        public IEnumerator<TileConnection> GetEnumerator()
        {
            return connections.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return connections.GetEnumerator();
        }
    }

    public class TileConnection
    {
        public GridTile other;
        public PathStyle pathStyle;
        public bool open; //Whether there is a connection

        //ctor
        public TileConnection(){}
        public TileConnection(GridTile tile)
        {
            other = tile;
        }
    }

    [System.Flags]
    public enum TileFlags
    {
        Undefined,
        Entrance = 1 << 1, 
        BossRoom = 1 << 2, 
        Standard = 1 << 3,
        Shop = 1 << 4,
        Special = 1 << 5
    }

    public enum PathStyle //specific to the tiles. preferably only have centeronly for now.
    {
        CenterOnly, LeftOnly, RightOnly, BothSides, FullWidth
    }   
}