using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    //grid containg all necessary nodes to create a smoother looking world
    public SquareGrid squareGrid;
    //list containg all vertices of the grid
    List<Vector3> vertices;
    //list containg the triangles that are caluclated based on the state of the vertices
    List<int> triangles;
    public MeshFilter walls;
    public MeshFilter dungeon;

    //those three datastructures are used to define how the 3D walls are to be built 
    Dictionary<int, List<Triangle>> triangleDictonary = new Dictionary<int, List<Triangle>>();
    List<List<int>> outlines = new List<List<int>>();
    HashSet<int> checkedVertices = new HashSet<int>();
    
    //generating a mesh by calulating and creating the necessary Triangles
    public void GenerateMesh(int[,] map, float squareSize){
        outlines.Clear();
        checkedVertices.Clear();
        triangleDictonary.Clear();
        Destroy(walls.GetComponent<MeshCollider>());
        squareGrid = new SquareGrid(map, squareSize);
        vertices = new List<Vector3>();
        triangles = new List<int>();

        for (int x = 0; x < squareGrid.squares.GetLength(0); x ++) {
			for (int y = 0; y < squareGrid.squares.GetLength(1); y ++) {
                TriangulateSquare(squareGrid.squares[x,y]);
            }
        }
        Mesh mesh = new Mesh();
        dungeon.mesh = mesh;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        int tileAmount = 10;
        Vector2[] uvs = new Vector2[vertices.Count];
        for (int i =0; i < vertices.Count; i ++) {
            float percentX = Mathf.InverseLerp(-map.GetLength(0)/2*squareSize,map.GetLength(0)/2*squareSize,vertices[i].x) * tileAmount;
            float percentY = Mathf.InverseLerp(-map.GetLength(0)/2*squareSize,map.GetLength(0)/2*squareSize,vertices[i].z) * tileAmount;
            uvs[i] = new Vector2(percentX,percentY);
        }
        mesh.uv = uvs;
        CreateWallMesh();
    }

    //caluclating all the differnt combinations possible for each combination of active ControlNodes
    void TriangulateSquare(Square square){
        switch(square.configuration){
            case 0: 
                break;

            //1 point cases
            case 1:
                MeshFromPoints(square.centerLeft, square.centerBottom, square.bottomLeft);
                break;
            case 2:
                MeshFromPoints(square.bottomRight, square.centerBottom, square.centerRight);
                break;
            case 4:
                MeshFromPoints(square.topRight, square.centerRight, square.centerTop);
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerLeft);
                break;
            
            //2 points cases
            case 3:
                MeshFromPoints(square.centerRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 5:
                MeshFromPoints(square.centerTop, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft, square.centerLeft);
                break;
            case 6:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.centerBottom);
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerBottom, square.bottomLeft);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerLeft);
                break;

            //3 points cases
            case 7:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;
            
            //4 point case
            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                checkedVertices.Add(square.topLeft.vertexIndex);
                checkedVertices.Add(square.topRight.vertexIndex);
                checkedVertices.Add(square.bottomRight.vertexIndex);
                checkedVertices.Add(square.bottomLeft.vertexIndex);
                break;
        }
    }

    //creating the Triangles calculated from the binary number of the active ControlNodes
    void MeshFromPoints(params Node[] points){
        AssignVerticies(points);
        if(points.Length >= 3){
            CreateTriangle(points[0], points[1], points[2]);
        }
        if(points.Length >= 4){
            CreateTriangle(points[0], points[2], points[3]);
        }
        if(points.Length >= 5){
            CreateTriangle(points[0], points[3], points[4]);
        }
        if(points.Length >= 6){
            CreateTriangle(points[0], points[4], points[5]);
        }
    }

    //assinging the vertices of a list of Nodes
    void AssignVerticies(Node[] points){
        for(int i = 0; i < points.Length; i++){
            if(points[i].vertexIndex == -1){
                points[i].vertexIndex = vertices.Count;
                vertices.Add(points[i].position);
            }
        }
    }


    void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle){
        if(triangleDictonary.ContainsKey(vertexIndexKey)){
            triangleDictonary[vertexIndexKey].Add(triangle);
        }else{
            List<Triangle> triangleList = new List<Triangle>();
            triangleList.Add(triangle);
            triangleDictonary.Add(vertexIndexKey, triangleList);
        }
    }

    //creating a Triangle out of three positons
    void CreateTriangle(Node a, Node b, Node c){
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);

        Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
        AddTriangleToDictionary(triangle.vertexIndexA, triangle);
        AddTriangleToDictionary(triangle.vertexIndexB, triangle);
        AddTriangleToDictionary(triangle.vertexIndexC, triangle);

    }

    //structure that defines a triangle based on three nodes
    struct Triangle{
        public int vertexIndexA;
        public int vertexIndexB;
        public int vertexIndexC;
        int[] vertices;
        public Triangle(int a, int b, int c){
            vertexIndexA = a;
            vertexIndexB = b;
            vertexIndexC = c;
            vertices = new int[3];
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
        }

        public int this[int i]{
            get{
                return vertices[i];
            }
        }

        public bool Contains(int vertexIndex){
            return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
        }
    }

    //function that checks if the edge is towards other walls or the walkable ground
    bool IsOutlineEdge(int vertexA, int vertexB){
        List<Triangle> trianglesContainingVertexA = triangleDictonary[vertexA];
        int sharedTriangleCount = 0;
        for(int i = 0; i < trianglesContainingVertexA.Count; i++){
            if(trianglesContainingVertexA[i].Contains(vertexB)){
                sharedTriangleCount++;
                if(sharedTriangleCount > 1){
                    break;
                }
            }
        }
        return sharedTriangleCount == 1;
    }

    int GetConnectedOutlineVertex(int vertexIndex){
        List<Triangle> trianglesContainingVertex = triangleDictonary[vertexIndex];
        for(int i = 0; i < trianglesContainingVertex.Count; i++){
            Triangle triangle = trianglesContainingVertex[i];
            for(int j = 0; j < 3; j++){
                int vertexB = triangle[j];
                if(vertexB != vertexIndex && !checkedVertices.Contains(vertexB)){
                    if(IsOutlineEdge(vertexIndex, vertexB)){
                        return vertexB;
                    }
                }
            }
        }
        return -1;
    }

    void CalculateMeshOutlines(){
        for(int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++){
            if(!checkedVertices.Contains(vertexIndex)){
                int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
                if(newOutlineVertex != -1){
                    checkedVertices.Add(vertexIndex);
                    List<int> newOutline = new List<int>();
                    newOutline.Add(vertexIndex);
                    outlines.Add(newOutline);
                    FollowOutline(newOutlineVertex, outlines.Count-1);
                    outlines[outlines.Count-1].Add(vertexIndex);
                }
            }
        }
    }

    void FollowOutline(int vertexIndex, int outlineIndex){
        outlines[outlineIndex].Add(vertexIndex);
        checkedVertices.Add(vertexIndex);
        int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);
        if(nextVertexIndex != -1){
            FollowOutline(nextVertexIndex, outlineIndex);
        }
    }

    void CreateWallMesh(){
        CalculateMeshOutlines();
        List<Vector3> wallVertices = new List<Vector3>();
        List<int> wallTriangles = new List<int>();
        Mesh wallMesh = new Mesh();
        float wallHeight = 5;
        foreach(List<int> outline in outlines){
            for(int i = 0; i < outline.Count-1; i++){
                int startIndex = wallVertices.Count;
                wallVertices.Add(vertices[outline[i]]);     //left
                wallVertices.Add(vertices[outline[i+1]]);   //right
                wallVertices.Add(vertices[outline[i]]- Vector3.up * wallHeight);       //bottomLeft
                wallVertices.Add(vertices[outline[i+1]]- Vector3.up * wallHeight);     //bottomRight
                wallTriangles.Add(startIndex);
                wallTriangles.Add(startIndex + 2);
                wallTriangles.Add(startIndex + 3);
                wallTriangles.Add(startIndex + 3);
                wallTriangles.Add(startIndex + 1);
                wallTriangles.Add(startIndex);
            }
        }
        wallMesh.vertices = wallVertices.ToArray();
        wallMesh.triangles = wallTriangles.ToArray();
        walls.mesh = wallMesh;
        MeshCollider wallCollider = walls.gameObject.AddComponent<MeshCollider>();
        wallCollider.sharedMesh = wallMesh;
    }

    //one node of a square
    public class Node{
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 _pos){
            position = _pos;
        }
    }

    //these nodes a the vertices of a square
    public class ControlNode : Node{
        public bool active;
        public Node above;
        public Node right;

        public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos){
            active = _active;
            above = new Node(position + Vector3.forward * squareSize/2f);
            right = new Node(position + Vector3.right * squareSize/2f);
        }
    }
    
    //a square consists of 4 ControlNodes and 4 Nodes, vertices and Nodes a numberd clockwise
    public class Square{
        public ControlNode topLeft;
        public ControlNode topRight;
        public ControlNode bottomLeft;
        public ControlNode bottomRight;
        public Node centerTop;
        public Node centerBottom;
        public Node centerRight;
        public Node centerLeft;

        //a value to calculate which Nodes of a square a "on"
        public int configuration;

        public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft){
            topLeft = _topLeft;
            topRight = _topRight;
            bottomRight = _bottomRight;
            bottomLeft = _bottomLeft;
            centerTop = topLeft.right;
            centerRight = bottomRight.above;
            centerBottom = bottomLeft.right;
            centerLeft = bottomLeft.above;

            //ControlNodes are numberd as a binary number 0000, if topLeft is active then the number would be 1000 and so on for the 
            //other Control Nodes
            if(topLeft.active){
                configuration += 8;
            }
            if(topRight.active){
                configuration += 4;
            }
            if(bottomRight.active){
                configuration += 2;
            }
            if(bottomLeft.active){
                configuration += 1;
            }
        }
    }

    //creating a grid from the map containg the ControlNodes and Nodes 
    public class SquareGrid{
        public Square[,] squares;

        public SquareGrid(int[,] map, float squareSize){
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;
            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];
            for(int x = 0; x < nodeCountX; x++){
                for(int y = 0; y < nodeCountY; y++){
                    Vector3 pos = new Vector3(-mapWidth/2 + x * squareSize + squareSize/2, 0, -mapHeight/2 + y * squareSize + squareSize/2);
                    controlNodes[x,y]= new ControlNode(pos, map[x,y] == 1, squareSize);
                }
            }
            squares = new Square[nodeCountX -1, nodeCountY -1];
            for(int x = 0; x < nodeCountX -1; x++){
                for(int y = 0; y < nodeCountY -1; y++){
                    squares[x,y] = new Square(controlNodes[x,y+1], controlNodes[x+1,y+1], controlNodes[x+1,y], controlNodes[x,y]);
                }
            }
        }
    }
}
