using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Random = System.Random;
using Debug = UnityEngine.Debug;
#if UNITY_EDITOR
using UnityEditor;
#endif

using static Celestial.Levels.Util;

namespace Celestial.Levels
{
    public class LevelGenerator : MonoBehaviour
    {
#region Serialized_Settings
        [SerializeField]
        int mainPathLength = 4;
        [SerializeField]
        int seed = 666;
        //TODO: need a tileset of some sort.
        [SerializeField]
        TileSet tileSet;
#endregion

#region Runtime_Generation
        List<GridTile> tiles;
        TileFlags roomsNotGenerated;

        Vector3Int bossRoomOffset;

        Random rng;
        Random connectionRng;
#endregion

        //Helper:
        static readonly Cardinals[] singleCardinals = {Cardinals.North, Cardinals.East, Cardinals.South, Cardinals.West};

        public void GenerateLevelComplete() 
        {
            DestroyInstances();
            InitializeGeneration();
            GenerateLevelLayout();
            PlaceLevel();
            //PlaceDebugLevel();
        }

        //Destroys active tileInstances before regenerating the level.
        private void DestroyInstances()
        {
            if(tiles != null)
                foreach (var tile in tiles)
                    if(tile != null && tile.instance)
                        DestroyImmediate(tile.instance);
        }

        ///<summary>Initialize parameters for the actual generation of the level.</summary>
        private void InitializeGeneration()
        {
            rng = new Random(seed);
            connectionRng = new Random(seed);
            //path length in X and Z direction.
            int xPathLength = rng.Next(1, mainPathLength - 1);
            int zPathLength = mainPathLength - xPathLength;
            //offset in x and z direction randomized to get more diverse level layouts.
            bossRoomOffset.x = rng.Next(2) > 0 ? -xPathLength: xPathLength;
            bossRoomOffset.z = rng.Next(2) > 0 ? -zPathLength: zPathLength;

            //The first tile.
            tiles = new List<GridTile>();
            GridTile startTile = new GridTile()
            {
                gridPosition = Vector3Int.zero,
                flags = TileFlags.Entrance,
                instance = null
            };
            //the last tile in the floor.
            GridTile lastTile = new GridTile()
            {
                gridPosition = bossRoomOffset,
                flags = TileFlags.BossRoom,
                instance = null
            };
            //Add both to the level
            tiles.Add(startTile);
            tiles.Add(lastTile);

            //set up the set of special rooms to be generated: //only 1 of each possible with this.
            roomsNotGenerated = TileFlags.Shop | TileFlags.Special;
        }

        ///<summary>Generates the level layout based on the intialized tiles list.</summary>
        private void GenerateLevelLayout()
        {
            //temp stat
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Vector3Int stepsTaken = Vector3Int.zero;
            Vector3Int currentPosition = Vector3Int.zero;
            //Step 1: Make a path between the start and the end.
            for(int i = 0; i < mainPathLength-1; i++)
            {
                //whether the generator can step in the given direction.
                bool canStepX = stepsTaken.x < Mathf.Abs(bossRoomOffset.x);
                bool canStepZ = stepsTaken.z < Mathf.Abs(bossRoomOffset.z);

                //if any direction is fine, randomize, otherwise go in whichever is available.
                bool walksInX = canStepX && canStepZ && rng.Next(2)>0 || canStepX && !canStepZ;
                TakeStep(walksInX, ref stepsTaken, ref currentPosition);
            }
            //INBETWEEN: make sure the boss room is connected!
            {
                GridTile bossRoom = tiles[1];
                GridTile lastPath = tiles[tiles.Count-1];
                bossRoom.AddConnection(new TileConnection(lastPath));
                lastPath.AddConnection(new TileConnection(bossRoom));
            }
            //Step 2: branch off from the main path.
            int lastMainPathTile = tiles.Count-1;
            for(int i = 0; i <= lastMainPathTile; i++)
            {
                GridTile currentTile = tiles[i];
                if(currentTile.flags.HasFlag(TileFlags.BossRoom)) //dont add rooms onto the boss room.
                    continue;
                //2.1: check if we should start adding rooms here
                if(rng.Next(2)>0) //50%chance to add a room onto the current one.
                    AddBranchTile(currentTile);
            }
            //Step 3: mark required walls! (rooms next to each other without connection)
            foreach(GridTile gridTile in tiles)
            {
                gridTile.RequiresWalls = Cardinals.None; //Reset the requiresWalls to none.
                foreach(Cardinals cardinal in singleCardinals)
                {
                    Vector3Int otherPos = gridTile.gridPosition + cardinal.GetDirection();
                    //if the tile exists, but we dont have a connection to it
                    if(tiles.Exists(x => x.gridPosition == otherPos) && !gridTile.HasConnectionInDirection(cardinal))
                        gridTile.RequiresWalls |= cardinal;
                    
                }
            }
            stopwatch.Stop();
            Debug.Log($"Finished generating the layout in {stopwatch.ElapsedMilliseconds}ms");
        }

        ///<summary>Adds a branch tile to the current one. </summary>
        private void AddBranchTile(GridTile currentTile)
        {
            //2.2: check for a direction add a room in.
            List<Vector3Int> directions = new List<Vector3Int>(); //list of available directions that have not been occupied yet.
            //this may not be nice looking or good code, but it works and is reasonably fast.
            if(!currentTile.HasConnectionAtLocation(currentTile.gridPosition + Vector3Int.right)) 
                directions.Add(Vector3Int.right);
            if(!currentTile.HasConnectionAtLocation(currentTile.gridPosition + Vector3Int.left))
                directions.Add(Vector3Int.left);
            if(!currentTile.HasConnectionAtLocation(currentTile.gridPosition + Vector3IntForward))
                directions.Add(Vector3IntForward);
            if(!currentTile.HasConnectionAtLocation(currentTile.gridPosition + Vector3IntBack))
                directions.Add(Vector3IntBack);
            //Is there a free direction we can add something to?
            if(directions.Count > 0)
            {
                //Generate the direction we want.
                Vector3Int direction = directions[rng.Next(directions.Count)];
                GridTile newTile = new GridTile()
                {
                    gridPosition = currentTile.gridPosition + direction,
                    //TODO: this does not guarantee a shop to exist in the level!
                    flags = (roomsNotGenerated.HasFlag(TileFlags.Shop) && rng.Next(2)>0)? TileFlags.Shop : TileFlags.Standard,
                    instance = null
                };
                newTile.AddConnection(new TileConnection(currentTile));
                currentTile.AddConnection(new TileConnection(newTile));
                //add the newly generated tile to the list.
                tiles.Add(newTile);
                //2.3: check for other neighbours that already exist => chance to connect them all together.
                AttemptConnections(newTile);
            }
        }

        private void AttemptConnections(GridTile tile)
        {
            AttemptConnectionInDirection(tile, Vector3Int.right);
            AttemptConnectionInDirection(tile, Vector3Int.left);
            AttemptConnectionInDirection(tile, Vector3IntForward);
            AttemptConnectionInDirection(tile, Vector3IntBack);
        }

        private void AttemptConnectionInDirection(GridTile tile, Vector3Int direction)
        {
            Vector3Int position = tile.gridPosition + direction;
            GridTile otherTile = tiles.Find(x => x.gridPosition == position);
            //other must exist, and not be connected already.
            if(otherTile != null && !tile.HasConnectionTo(otherTile) && connectionRng.Next(2) > 0)
            {
                tile.AddConnection(new TileConnection(otherTile));
                otherTile.AddConnection(new TileConnection(tile));
            }
        }

        ///<summary>Take a step in either X or Z, directly towards the boss room.</summary>
        private void TakeStep(bool inXDirection, ref Vector3Int steps, ref Vector3Int position)
        {
            Vector3Int oldPosition = position;
            Vector3Int lateral = inXDirection? Vector3Int.right : Vector3IntForward;
            steps += lateral;
            //the actual world direction we are going in.
            Vector3Int direction = inXDirection? (position.x < bossRoomOffset.x? lateral: -lateral)
                                                : (position.z < bossRoomOffset.z? lateral: -lateral);
            position += direction;
            //Now create a gridtile at the position.
            GridTile tile = new GridTile()
            {
                gridPosition = position,
                flags = TileFlags.Standard, //for now, every tile is a normal tile, idc.
                instance = null
            };
            //add the connections.
            GridTile oldTile = tiles.Find(x => x.gridPosition == oldPosition);
            tile.AddConnection(new TileConnection(oldTile));
            oldTile.AddConnection(new TileConnection(tile));
            //add the tile to the list of all tiles.
            tiles.Add(tile);
        }
    
        ///<summary>Place the parts of the level in the scene.</summary>
        private void PlaceLevel()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<Tile> oneEntrance = tileSet.GetTilesWith(x=>x.entranceCount == 1);
            List<Tile> straights = tileSet.GetTilesWith(x=>x.entranceCount == 2 && x.entrancesOnSides == (Cardinals.West|Cardinals.East) 
                                                                                || x.entrancesOnSides == (Cardinals.North|Cardinals.South));
            //logically all tiles with two entrances that arent included in the straights are corners.
            List<Tile> corners = tileSet.GetTilesWith(x=>x.entranceCount == 2 && straights.IndexOf(x) < 0); // index == -1 means that the tile does not exist within the list.
            List<Tile> threeEntrances = tileSet.GetTilesWith(x=>x.entranceCount==3); // T-crossings
            List<Tile> fourEntrances = tileSet.GetTilesWith(x=>x.entranceCount==4); // +

            foreach(GridTile gridTile in tiles)
            {
                TileFlags flags = gridTile.flags;
                int maxEntrances = 4;
                foreach(var cardinal in singleCardinals) //unfortunately we cant use Bitoperations in this subset of c#
                    if(gridTile.RequiresWalls.HasFlag(cardinal))
                        maxEntrances--;

                if(flags.HasFlag(TileFlags.BossRoom))
                {
                    //Boss rooms always only have one entrance, so we can immediately get a room to spawn.
                    //SpawnInstanceTile(gridTile, tileSet.GetBossTile(rng)); //!!!GameObject is implicitly cast to Tile!!!
                }
                else if(flags.HasFlag(TileFlags.Entrance))//Entrance can vary a lot.
                {
                    //1. get possible tiles for the entrance.
                    List<Tile> possibleEntrances = tileSet.GetEntranceTiles().FindAll(x => x.entranceCount >= gridTile.ConnectionCount && x.entranceCount <= maxEntrances);
                    //gridTile.ConnectionCardinals
                    Tile spawnTile = possibleEntrances[0]; //-----------rng.Next(possibleEntrances.Count)
                    SpawnInstanceTile(gridTile, spawnTile);
                }
                //else if(flags.HasFlag(TileFlags.Shop))
                //{
                    //TODO:
                //}
                else //Standard tiles have a lot of things to take care of!
                {
                    Tile tileToSpawn = null;
                    //room style is how many "entrances" the room has. this can be more than the required amount of connection
                    //but has to be less or equal to the max amount of entrances, as to not conflict with required walls.
                    int roomStyle = rng.Next(gridTile.ConnectionCount, maxEntrances); //+1 since its exclusive max?
                    switch(roomStyle)
                    {
                        case 1: 
                            tileToSpawn = oneEntrance[rng.Next(oneEntrance.Count)];
                            break;
                        case 2: //2 entrances is ambiguous on its own, we need to differentiate between the types.
                            tileToSpawn = gridTile.ConnectionCardinals.DescribesStraight()? straights[rng.Next(straights.Count)]
                                                                                        :   corners[rng.Next(corners.Count)];
                            break;
                        case 3:
                            tileToSpawn = threeEntrances[rng.Next(threeEntrances.Count)];
                            break;
                        case 4: 
                            tileToSpawn = fourEntrances[rng.Next(fourEntrances.Count)];
                            break;           
                    }
                    SpawnInstanceTile(gridTile, tileToSpawn);
                }
                //Tile has been spawned.
            }
            stopwatch.Stop();
            Debug.Log($"Placed tiles in {stopwatch.ElapsedMilliseconds}ms.");
        }

        ///<summary>Instantiate the spawnTile at the gridTile. Fill eventual holes with dead ends.</summary>
        private void SpawnInstanceTile(GridTile gridTile, Tile spawnTile)
        {
            Cardinals entranceCardinals = spawnTile.entrancesOnSides;
            for(int i = 0; i < 4; i++)
            {
                //check if the entrances on the tile are on the correct sides. only need to countercheck the required walls for a few edge-cases.
                if(entranceCardinals.HasFlag(gridTile.ConnectionCardinals) && !entranceCardinals.HasAnyFlag(gridTile.RequiresWalls))
                {
                    //spawn the tile.
                    GameObject spawnedInstance = Instantiate(spawnTile.tileMeshPrefab);
                    Transform trans = spawnedInstance.transform;
                    trans.position = ((Vector3)gridTile.gridPosition) * tileSet.TileSize;
                    trans.Rotate(0, i*90, 0, Space.World);//rotate it clock-wise to fit the current setting.
                    //Debug.Log($"Rotate by {i*90}degrees");
                    //set the instance.
                    gridTile.instance = spawnedInstance;
                    //stop the loop, go to the next gridTile
                    break;
                }
                //is not correct -> rotate
                entranceCardinals = entranceCardinals.RotateBy90ClockWise();
            }
            //tile has been spawned.
            //check for open entrances.
            Cardinals openEntrances = entranceCardinals ^ gridTile.ConnectionCardinals;
            Cardinals cardinal = Cardinals.North;
            while(openEntrances > 0)
            {
                if(openEntrances.HasFlag(cardinal))
                {
                    //We need to invert this, to get the direction from the dead-end to the room it will connect to.
                    Cardinals invertedCardinal = cardinal.Invert();
                    //spawn a random dead end at that location.
                    Vector3 origin = gridTile.instance.transform.position;
                    Tile deadTile = tileSet.GetDeadEnd(rng);
                    Cardinals tileEntrance = deadTile.entrancesOnSides;
                    
                    //since entrancesOnSides ALWAYS has to be a single value, there should be no issue here
                    float angle = deadTile.entrancesOnSides.AngleTo(invertedCardinal);
                    GameObject instance = Instantiate(deadTile.tileMeshPrefab);
                    Debug.Log($"{deadTile.entrancesOnSides}->{invertedCardinal}={angle}", instance);
                    Transform t = instance.transform;
                    t.position = origin + ((Vector3)cardinal.GetDirection() * tileSet.TileSize);
                    t.Rotate(0, angle, 0, Space.World);
                            
                    openEntrances -= cardinal;
                }
                cardinal = cardinal.Next(); //Advance North -> East -> South -> West.
            }
            
        }

        private void PlaceDebugLevel()
        {
            foreach(GridTile tile in tiles)
            {
                Transform t = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                t.position = tile.gridPosition * 2;
                foreach(TileConnection con in tile)
                {
                    t = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
                    t.position = (Vector3)(tile.gridPosition*2 + con.other.gridPosition*2) * 0.5f;
                    t.localScale = Vector3.one * 0.5f;
                }
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LevelGenerator))]
    public class LevelGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUILayout.Button("Generate Level"))
            {
                (target as LevelGenerator).GenerateLevelComplete();
            }
        }
    }
#endif
}