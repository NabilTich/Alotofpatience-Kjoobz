using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CubeLogic))]
[RequireComponent(typeof(ColorTable))]

public class Cube : MonoBehaviour
{
    [NonSerialized] public CubeLogic cubeLogic;
    ColorTable colorTable;
    [Header("Cube Rotation")]
    [SerializeField] [Tooltip("Dit gebruik je om te bepalen hoe makkelijk de kubus ronddraait")] float sensitivity = 0.5f;
    [Header("Cube Variables")]
    [SerializeField] int cubewidth = 3;
    //[SerializeField] float emptynessRadius = 0.1f;
    [SerializeField] [Tooltip("Hoe ver de tiles van elkaar af staan")] float tiledistance = 1f;
    [SerializeField] [Tooltip("Als dit aan staat draait de kubus in een frame")] bool instantMoves = false;
    [SerializeField] bool toggleBlocks = false;
    [SerializeField] bool rotationCover = true;
    [SerializeField] [Tooltip("Als Instant Moves uit staat bepaalt dit hoe snel de kubus draait")] float rotationSpeed = 1f;
    [Header("Tile Variables")]
    [SerializeField] [Tooltip("Stop hier een quad in met een Tile.cs script, staat gewoon in assets als het goed is")] GameObject tile;
    [SerializeField] float tileSize = 0.85f;
    [Header("InstantMoves Afterimage")]
    [SerializeField] [Tooltip("Stop hier een blokje met grootte 1x1x1 in met een transparant materiaal, staat als het goed is ook in je assets")] GameObject afterImage;
    [SerializeField] float afterimageSize = 0.999f;
    [Header("Block Variables")]
    [SerializeField] GameObject cubeblock;
    [SerializeField] float blockSize = 0.499f;
    // internal variables
    private Quaternion[] tilerotations;
    private Vector3[] rowsteps;
    private Vector3[] collumnsteps;
    private Vector3[] startingpoints;
    private GameObject rotator;
    private GameObject cubeHolder;
    public Queue<Rotation> moves;
    private Tile[,] allTiles;
    private float T, t;
    //administration
    MeshRenderer afterImage_ms;
    Vector3 vecDelta = Vector3.zero;
    Vector3 vecPrevPos = Vector3.zero;
    private bool makemove = true;
    private bool methodlocked = true;
    private Tile prevHitTile;
    void Start()
    {
        if(cubewidth <= 0) { cubewidth = 1; }
        cubeLogic = GetComponent<CubeLogic>();
        colorTable = GetComponent<ColorTable>();
        cubeLogic.InitializeCube(this.cubewidth);
        rotator = new GameObject("rotator");
        rotator.transform.parent = transform;
        T = (cubewidth * tiledistance) / 2;
        t = T - tiledistance / 2;
        if (toggleBlocks)
        {
            allTiles = new Tile[cubeLogic.triplemove + cubeLogic.square, cubewidth];// TODO aparte array voor blocks
            //SpawnCubeBlocks();
        }
        else
            allTiles = new Tile[cubeLogic.triplemove, cubewidth];
        moves = new Queue<Rotation>();
        cubeHolder = new GameObject("CubeHolder");
        transform.parent = cubeHolder.transform;
        Camera.main.GetComponent<CameraScript>().SetParent(transform);
        afterImage = Instantiate(afterImage);
        afterImage.transform.parent = transform;
        afterImage_ms = afterImage.GetComponent<MeshRenderer>();

        Quaternion front =  Quaternion.Euler(new Vector3(0f  ,180f,0f));
        Quaternion back =   Quaternion.Euler(new Vector3(0f  ,0f  ,0f));
        Quaternion left =   Quaternion.Euler(new Vector3(0f  ,270f,0f));
        Quaternion right =  Quaternion.Euler(new Vector3(0f  ,90f ,0f));
        Quaternion top =    Quaternion.Euler(new Vector3(90f ,90f ,0f));
        Quaternion bottom = Quaternion.Euler(new Vector3(270f,90f ,0f));
        Quaternion[] tilerotations = { front, back, left, right, top, bottom };
        this.tilerotations = tilerotations;
        Vector3 startposfront =  new Vector3( t, t, T);
        Vector3 startposback =   new Vector3( t, t,-T);
        Vector3 startposleft =   new Vector3( T, t, t);
        Vector3 startposright =  new Vector3(-T, t, t);
        Vector3 startpostop =    new Vector3( t, T, t);
        Vector3 startposbottom = new Vector3( t,-T, t);
        Vector3[] startingpoints = { startposfront, startposback, startposleft, startposright, startpostop, startposbottom };
        this.startingpoints = startingpoints;

        Vector3 firstaxisrow =  new Vector3(-tiledistance, 0f           , 0f           );
        Vector3 secondaxisrow = new Vector3(0f           , -tiledistance, 0f           );
        Vector3 thirdaxisrow =  new Vector3(0f           , 0f           , -tiledistance);
        Vector3[] rowsteps = { firstaxisrow, secondaxisrow, thirdaxisrow };
        this.rowsteps = rowsteps;
        Vector3 firstaxiscollumn =  new Vector3(0f           , -tiledistance, 0f           );
        Vector3 secondaxiscollumn = new Vector3(0f           , 0f           , -tiledistance);
        Vector3 thirdaxiscollumn =  new Vector3(-tiledistance, 0f           , 0f           );
        Vector3[] collumnsteps = { firstaxiscollumn, secondaxiscollumn, thirdaxiscollumn };
        this.collumnsteps = collumnsteps;

        SpawnTiles();
        Graphical_UpdateEverything();
    }
    public void ButtonPress() // voor testing
    {
        StartCoroutine(EnqueueMove());
    }
    IEnumerator EnqueueMove() // voor testing
    {
        while (true)
        {
            moves.Enqueue(Vector3IntToCubemove(new Vector3Int(1, 0, 0)));
            moves.Enqueue(Vector3IntToCubemove(new Vector3Int(0, 1, 0)));
            moves.Enqueue(Vector3IntToCubemove(new Vector3Int(0, 0, 1)));
            yield return new WaitForSeconds(0.01f);
        }
    }
    void Update()
    {
        if (makemove && moves.Count > 0)
        {
            makemove = false;
            MakeMove(moves.Dequeue());
        }
        else
        {
            Graphical_DisableAfterImage();
        }
        if (Input.GetMouseButton(1))
        {
            vecDelta = Input.mousePosition - vecPrevPos;
            //if (Vector3.Dot(cubeRotator.transform.up, Vector3.up) >= 0f)
                cubeHolder.transform.Rotate(cubeHolder.transform.up, Vector3.Dot(vecDelta, Camera.main.transform.right) * sensitivity, Space.World);
            //else
                //cubeRotator.transform.Rotate(cubeRotator.transform.up, Vector3.Dot(testvecDelta, Camera.main.transform.right) * sensitivity, Space.World);
            cubeHolder.transform.Rotate(Camera.main.transform.right, Vector3.Dot(vecDelta, Camera.main.transform.up) * sensitivity, Space.World);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            methodlocked = false;
            Ray ray = GetMouseRay();
            RaycastHit hit;
            Tile hitTile;
            if (Physics.Raycast(ray, out hit) && (hitTile = hit.transform.GetComponent<Tile>()) != null)
            {

                prevHitTile = hitTile;
            }
        }
        else if (Input.GetMouseButton(0) && !methodlocked)
        {
            Ray ray = GetMouseRay();
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Tile hitTile = hit.transform.GetComponent<Tile>();
                if (hitTile == null) return;
                if (!(hitTile.x == prevHitTile.x && hitTile.y == prevHitTile.y))
                {
                    moves.Enqueue(cubeLogic.MoveBasedOnTiles(prevHitTile.x, prevHitTile.y, hitTile.x, hitTile.y));
                    methodlocked = true;
                }
            }
        }
        SetCubeHolder();
        vecPrevPos = Input.mousePosition;
    }

    private void MakeMove(Rotation rotation)
    {
        Tile[] selection = new Tile[0];
        selection = SelectLayer(rotation.startpos, rotation.index, selection);
        if (rotation.index == 0)
            selection = SelectFace(rotation.startpos, selection);
        if (rotation.index == cubeLogic.cubewidth_)
            selection = SelectFace(rotation.startpos, selection, cubeLogic.cubewidth);

        if (instantMoves)
        {
            cubeLogic.PerformRotation(rotation);
            Graphical_updateSelection(selection);
            Graphical_AfterImage(rotation);
            makemove = true;
        }
        else
        {
            cubeLogic.PerformRotation(rotation);
            StartCoroutine(Graphical_FakeRotation_Tiles(rotation, selection));
        }
    }

    private void SetParentsOfArray(Component[] gameObjects, Transform parent)
    {
        for (int i = 0; i < gameObjects.Length; i++)
            gameObjects[i].transform.parent = parent;
    }
    //-----------------------------------------------------------miscellaneous--------------------------------------------------------------------------------------------------------------
    private Ray GetMouseRay()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }
    private Rotation Vector3IntToCubemove(Vector3Int entry)
    {
        if (entry.x != 0)
            return new Rotation(cubeLogic.doublemove, Math.Abs(entry.x) - 1, Math.Sign(entry.x));
        if (entry.y != 0)
            return new Rotation(0, Math.Abs(entry.y) - 1, Math.Sign(entry.y));
        return new Rotation(cubeLogic.move, Math.Abs(entry.z) - 1, Math.Sign(entry.z));
    }
    private void SetCubeHolder()
    {
        transform.parent = null;
        cubeHolder.transform.LookAt(Camera.main.transform);
        transform.parent = cubeHolder.transform;
    }
    //----------------------------------------------------------------graphical Methods---------------------------------------------------------------------------------------------------
    private void Graphical_DisableAfterImage()
    {
        afterImage_ms.enabled = false;
    }
    private void Graphical_RotationCover()
    {

    }
    private void Graphical_AfterImage(Rotation rotation)
    {
        Vector3Int vector = cubeLogic.vectors[rotation.startpos / cubeLogic.move];
        afterImage.transform.localPosition = vector - vector*rotation.index;
        afterImage.transform.localScale = (Vector3Int.one * cubewidth) - vector*2;
        afterImage.transform.localRotation = Quaternion.Euler(vector * UnityEngine.Random.Range(69, 420));
        afterImage_ms.enabled = true;

    }
    IEnumerator Graphical_FakeRotation_Tiles(Rotation rotation, Tile[] selection, float rot_amount = 1f, int degrees = 90, float time = 0.001f)
    {
        // store tranform info for recovery
        Vector3[] tempposs = new Vector3[selection.Length];
        Quaternion[] temprots = new Quaternion[selection.Length];
        for (int i = 0; i < selection.Length; i++)
        {
            tempposs[i] = selection[i].transform.localPosition;
            temprots[i] = selection[i].transform.localRotation;
        }
        // set parent as rotator
        SetParentsOfArray(selection, rotator.transform);
        //rotate
        for (int i = 1; i <= degrees / rotationSpeed; i++)
        {
            rotator.transform.Rotate(cubeLogic.vectors[rotation.startpos / cubeLogic.move] * rotation.sign, rot_amount * rotationSpeed);
            yield return new WaitForSeconds(Mathf.Epsilon);
        }
        rotator.transform.rotation = transform.rotation;
        // reset parents
        SetParentsOfArray(selection, transform);
        // reset transforms
        for (int i = 0; i < selection.Length; i++)
        {
            selection[i].transform.localPosition = tempposs[i];
            selection[i].transform.localRotation = temprots[i];
        }
        Graphical_updateSelection(selection);
        makemove = true;
    }
    //----------------------------------------------------------------graphical update methods---------------------------------------------------------------------------------------------------
    private void Graphical_updateSelection(Tile[] selectiontiles)
    {
        for (int i = 0; i < selectiontiles.Length; i++) 
            selectiontiles[i].GetComponent<Renderer>().material = colorTable.ToMaterial(cubeLogic.cubestate[selectiontiles[i].x, selectiontiles[i].y]);        
    }
    private void Graphical_UpdateEverything()
    {
        for (int x = 0; x < cubeLogic.triplemove; x++)
            for (int y = 0; y < cubewidth; y++)
                allTiles[x, y].GetComponent<Renderer>().material = colorTable.ToMaterial(cubeLogic.cubestate[x, y]);

    }
    //----------------------------------------------------------------------------------cube generation in unity----------------------------------------------------------------------------------
    private void SpawnTiles()
    {
        for(int a = 0; a < 3; a++)
            for(int b = 0; b < 2; b++)
            {
                SpawnTileFace(
                    startingpoints[a * 2 + b],
                    rowsteps[a],
                    collumnsteps[a],
                    tilerotations[a * 2 + b],
                    a * 2 + b
                    );
            }
        
    }
/*    private void SpawnCubeBlocks()
    {
        Vector3 corner = new Vector3(t, t, t);
        Vector3 placement;
        for (int y = 0; y < cubewidth; y++)
        {   
            for (int x = 0; x < cubewidth; x++)
            {   
                for (int z = 0; z < cubewidth; z++)
                {
                    placement = corner + new Vector3(-tiledistance * x, -tiledistance * y, -tiledistance * z);
                    //if (Mathf.Sqrt(placement.x * placement.x + placement.y * placement.y + placement.z * placement.z) < emptynessRadius)  
                        //continue; 
                    GameObject block = Instantiate(cubeblock);
                    block.transform.parent = transform;
                    block.transform.localPosition = placement;
                    block.transform.localScale = new Vector3(blockSize, blockSize, blockSize);
                    int startpos = cubeLogic.triplemove + z*cubewidth;
                    allTiles[startpos + x, y] = block; // TODO seperate block array
                }
            }
        }
    }*/
    private void SpawnTileFace(Vector3 startingpoint, Vector3 rowstep, Vector3 collumnstep, Quaternion rotation, int tileindex)
    {
        Vector3 tilesize = new Vector3(this.tileSize, this.tileSize, this.tileSize);
        for (int collumn = 0; collumn < cubewidth; collumn++)
            for (int row = 0; row < cubewidth; row++)
            {
                Tile newtile = Instantiate(tile.GetComponent<Tile>(), transform);
                newtile.transform.localPosition = startingpoint + collumn * collumnstep + row * rowstep;
                newtile.transform.localRotation = rotation;
                newtile.transform.localScale = tilesize;
                newtile.SetTile(tileindex * cubewidth + row, collumn);
                allTiles[tileindex * cubewidth + row, collumn] = newtile;
            }

    }
    //----------------------------------------------------------------selector methods---------------------------------------------------------------------------------------------------------
    private Tile[] SelectLayer(int startpos, int index, Tile[] other)
    {
        int counter = 0;
        Tile[] selection = IncreaseSize(cubeLogic.doublemove, out counter, other);
        int offset2 = (startpos + cubeLogic.move) % cubeLogic.triplemove;
        int offset1 = startpos;
        for (int i = 0; i < cubeLogic.move; i++)
        {
            selection[counter] = allTiles[offset1 + i, index]; counter++;
        }
        for (int i = 0; i < cubewidth; i++)
        {
            selection[counter] = allTiles[offset2 + index, i]; counter++;
            selection[counter] = allTiles[offset2 + index + cubewidth, i]; counter++;
        }
        return selection;
    }
    private Tile[] SelectFace(int startpos, Tile[] other, int extraoffset = 0)
    {
        int counter = 0;
        Tile[] selection = IncreaseSize(cubeLogic.square, out counter, other);
        int offset = (startpos + cubeLogic.doublemove + extraoffset) % cubeLogic.triplemove;
        for (int x = 0; x < cubewidth; x++)
            for (int y = 0; y < cubewidth; y++)
            {
                selection[counter] = allTiles[offset + x, y]; counter++;
            }
        return selection;
    }
    private Tile[] IncreaseSize( int addedsize, out int counter, Tile[] other)
    {
        Tile[] selection;
        selection = new Tile[other.Length + addedsize];
        counter = other.Length;
        for (int i = 0; i < other.Length; i++)
            selection[i] = other[i];
        return selection;
    }
}