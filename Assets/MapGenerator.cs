using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //set seed to generate the same map again
    public string seed;
    public bool useRandomSeed;

    //set percentage of wall covering
    //[Range(40,70)]
    public int noisePercent;

    //set how many iterations of CA are to be done
    public int CAIterations;

    //width and height of the map
    public int height;
    public int width;

    //the map itself
    int[,] map;

    //generate a map at the start of the game
    void Start(){
        GenerateMap();
    }

    //generate map with given size
    void GenerateMap(){
        map = new int[width,height];
        //filling the map array with random 0's and 1's
        FillMapRandom();
        for(int i = 0; i < CAIterations; i++){
            //using Cellular Automata Methode on the map
            SmoothMap();
        }
        ProcessMap();
        int borderSize = 2;
        int[,] borderedMap = new int[width+borderSize*2,height+borderSize*2];
        for(int x = 0; x < borderedMap.GetLength(0); x++){
            for(int y = 0; y < borderedMap.GetLength(1); y++){
                if(x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize){
                    borderedMap[x,y] = map[x-borderSize, y-borderSize];
                }else{
                    borderedMap[x,y] = 1;
                }
            }
        }
        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(borderedMap, 1);
    }

    //fill map with random 0's and 1's depending on noisePercent
    void FillMapRandom(){
        if(useRandomSeed){
            seed = System.DateTime.Now.Ticks.ToString();
        }
        System.Random randomNumber = new System.Random(seed.GetHashCode());
        for(int x = 0; x < width; x++){
            for(int y = 0; y < height; y++){
                if(x == 0 || x == width - 1 || y == 0 || y == height - 1 ){
                    map[x,y] = 1;
                }else{
                    map[x,y] = (randomNumber.Next(0,100) < noisePercent)? 1: 0;
                }
            }
        }
    }

    //use CA to smooth the map
    void SmoothMap(){
        for(int x = 0; x < width ; x++){
            for(int y = 0; y < height; y++){
                int surroundingWallTiles = GetSurroundingWallTiles(x,y);
                if(surroundingWallTiles > 4){
                    map[x,y] = 1;
                }else if(surroundingWallTiles < 4){
                    map[x,y] = 0;
                }
            }
        }
    }

    //function to count how many 1's surround the selected tile
    int GetSurroundingWallTiles(int posX, int posY){
        int count = 0;
        for(int x = posX -1; x <= posX +1; x++ ){
            for(int y = posY -1; y <= posY +1; y++){
                if(x >= 0 && x < width && y >= 0 && y < height){
                    if(x != posX || y != posY){
                        count += map[x,y];
                    }
                }else{
                    count++;
                }
            }
        }
        return count;
    }

    //a struct that holds one coordinate of a point int the map
    struct Coord{
        public int tileX;
        public int tileY;

        public Coord(int x, int y){
            tileX = x;
            tileY = y;
        }
    }

    //
    List<Coord> GetRegionTiles(int startX, int startY){
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[width,height];
        int tileType = map[startX,startY];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX,startY] = 1;
        while(queue.Count > 0){
            Coord tile = queue.Dequeue();
            tiles.Add(tile);
            for(int x = tile.tileX -1; x <= tile.tileX +1; x++){
                for(int y = tile.tileY -1; y <= tile.tileY +1; y++){
                    if(IsInMapRange(x,y) && (x == tile.tileX || y == tile.tileY)){
                        if(mapFlags[x,y] == 0 && map[x,y] == tileType){
                            mapFlags[x,y] = 1;
                            queue.Enqueue(new Coord(x,y));
                        }
                    }
                }
            }
        }
        return tiles;
    }

    //function to calculate the area of an Room
    List<List<Coord>> GetRegions(int tileType){
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[width, height];
        for(int x = 0; x < width ; x++){
            for(int y = 0; y < height; y++){
                if(mapFlags[x,y] == 0 && map[x,y] == tileType){
                    List<Coord> newRegion = GetRegionTiles(x,y);
                    regions.Add(newRegion);
                    foreach(Coord tile in newRegion){
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }
        return regions;
    }

    //function to delete small Rooms or small walls if they are created 
    void ProcessMap(){
        List<List<Coord>> wallRegions = GetRegions(1);
        int wallTresholdSize = 50;
        foreach(List<Coord> wallRegion in wallRegions){
            if(wallRegion.Count < wallTresholdSize){
                foreach(Coord tile in wallRegion){
                    map[tile.tileX, tile.tileY] = 0;
                }
            }
        }
        List<List<Coord>> roomRegions = GetRegions(0);
        int roomTresholdSize = 50;
        List<Room> roomsLeft = new List<Room>();
        foreach(List<Coord> roomRegion in roomRegions){
            if(roomRegion.Count < roomTresholdSize){
                foreach(Coord tile in roomRegion){
                    map[tile.tileX, tile.tileY] = 1;
                }
            }else{
                roomsLeft.Add(new Room(roomRegion, map));
            }
        }
        //calculating which Room is the biggest so it can be flagged as the MainRoom
        roomsLeft.Sort();
        roomsLeft[0].isMainRoom = true;
        roomsLeft[0].isAccesibleFormMainRoom = true;
        ConnectClosestRooms(roomsLeft);
    }

    //checls if a coordinate is still on the map
    bool IsInMapRange(int x, int y){
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    /*funtion that claculates the two closest Room from all Rooms. It creates two lists of Rooms. One list contains all Rooms that are in some way connected two the MainRoom, the other list contains Rooms that are not connected two the MainRoom. Then it calculates the shortest pathway that would connect those two lists of connected Rooms.
    */
    void ConnectClosestRooms(List<Room> allRooms, bool forceAccessFromMainRoom = false){
        int bestDistance = 0;
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;
        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();
        if(forceAccessFromMainRoom){
            foreach(Room room in allRooms){
                if(room.isAccesibleFormMainRoom){
                    roomListB.Add(room);
                }else{
                    roomListA.Add(room);
                }
            }
        }else{
            roomListA = allRooms;
            roomListB = allRooms;
        }
        foreach(Room roomA in roomListA){
            if(!forceAccessFromMainRoom){
                possibleConnectionFound = false;
                if(roomA.connectedRooms.Count > 0){
                    continue;
                }
            }
            possibleConnectionFound = false;
            foreach(Room roomB in roomListB){
                if(roomA == roomB || roomA.IsConnected(roomB)){
                    continue;
                }               
                for(int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++){
                    for(int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++){
                        Coord tileA = roomA.edgeTiles[tileIndexA];
                        Coord tileB = roomB.edgeTiles[tileIndexB];
                        int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));
                        if(distanceBetweenRooms < bestDistance || !possibleConnectionFound){
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }
            if(possibleConnectionFound && !forceAccessFromMainRoom){
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }
        if(possibleConnectionFound && forceAccessFromMainRoom){
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(allRooms, true);
        }
        if(!forceAccessFromMainRoom){
            ConnectClosestRooms(allRooms, true);
        }
    }

    //function that connects two Rooms and creates the necessary path between them
    void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB){
        Room.ConnectRooms(roomA, roomB);
        List<Coord> line = GetLine(tileA, tileB);
        foreach(Coord c in line){
            //setting the width of the created path, here it is 2
            DrawCircle(c,2);
        }
    }

    //transforming the walls that are block the way between two Rooms 
    void DrawCircle(Coord c, int r){
        for(int x = -r; x <= r; x++){
            for(int y = -r; y <=r; y++){
                if(x*x + y*y <= r*r){
                    int realX = c.tileX + x;
                    int realY = c.tileY + y;
                    if(IsInMapRange(realX, realY)){
                        map[realX,realY] = 0;
                    }
                }
            }
        }
    }

    /*function calculating the shortest path from one point in the map array to another in the map array. This is used to connect isolated Rooms with the shortest possible way. This works by calculating a straight line between the two points an then changing only one coordinate like x until the differnce to y is great enough so the created path will not go through 2 coordinates at the same time.
    */
    List<Coord> GetLine(Coord start, Coord end){
        List<Coord> line = new List<Coord>();
        int x = start.tileX;
        int y = start.tileY;
        int dx = end.tileX - start.tileX;
        int dy = end.tileY - start.tileY;
        int step = Math.Sign(dx);
        int gradientStep = Math.Sign(dy);
        int longest = Math.Abs(dx);
        int shortest = Math.Abs(dy);
        bool inverted = false;
        if(longest < shortest){
            inverted = true;
            longest = Math.Abs(dy);
            shortest = Math.Abs(dx);
            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }
        int gradientAccumulation = longest / 2;
        for(int i = 0; i < longest; i++){
            line.Add(new Coord(x,y));
            if(inverted){
                y+= step;
            }else{
                x += step;
            }
            gradientAccumulation += shortest;
            if(gradientAccumulation >= longest){
                if(inverted){
                    x += gradientStep;
                }else{
                    y += gradientStep;
                }
                gradientAccumulation -= longest;
            }
        }
        return line;
    }

    //helper function transforming a given Coord into an Vector3
    Vector3 CoordToWorldPoint(Coord tile){
        return new Vector3(-width/2 +.5f + tile.tileX, 2, -height/2 + .5f + tile.tileY);
    }

    //a Room contains all coordiantes Coord of an closed area
    class Room : IComparable<Room>{
        public List<Coord> tiles;
        public List<Coord> edgeTiles;
        public List<Room> connectedRooms;
        //the MainRoom is the Room with the most coordinates
        public bool isMainRoom;
        //bool value to see if a Room is reachable from the MainRoom
        public bool isAccesibleFormMainRoom;
        public int roomSize;

        public Room(){}

        public Room(List<Coord> roomTiles, int[,] map){
            tiles = roomTiles;
            roomSize = tiles.Count;
            connectedRooms = new List<Room>();
            edgeTiles = new List<Coord>();

            foreach(Coord tile in tiles){
                for(int x = tile.tileX -1; x <= tile.tileX +1; x++){
                    for(int y = tile.tileY -1; y <= tile.tileY +1; y++){
                        if(x == tile.tileX || y == tile.tileY){
                            if(map[x,y] == 1){
                                edgeTiles.Add(tile);
                            }
                        }
                    }
                }
            }
        }

        //setting the bool if a Room is accessible from the MainRoom for the Room itself and all Rooms connected to this Room to true
        public void SetAccessFromMainRoom(){
            if(!isAccesibleFormMainRoom){
                isAccesibleFormMainRoom = true;
                foreach(Room conncetedRoom in connectedRooms){
                    conncetedRoom.SetAccessFromMainRoom();
                }
            }
        }

        //function comparing the size of two Rooms/ the number of Coords the Room consists of
        public int CompareTo(Room otherRoom){
            return otherRoom.roomSize.CompareTo(roomSize);
        }

        //adding the Rooms to eachothers list of connected Rooms and if one is connected to the MainRoom the bool value
        //isAccesibleFromMainRoom of the other will be set to true
        public static void ConnectRooms(Room roomA, Room roomB){
            if(roomA.isAccesibleFormMainRoom){
                roomB.SetAccessFromMainRoom();
            }else if(roomB.isAccesibleFormMainRoom){
                roomA.SetAccessFromMainRoom();
            }
            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }

        //checks if the given Room is connected to the looked at Room
        public bool IsConnected(Room otherRoom){
            return connectedRooms.Contains(otherRoom);
        }
    }

}
