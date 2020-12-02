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
    [SerializeField] float sensitivity = 0.5f;
    [Header("Cube Variables")]
    [SerializeField] int cubewidth = 3;
    //[SerializeField] float emptynessRadius = 0.1f;
    [SerializeField] float tiledistance = 1f;
    [SerializeField] bool instantMoves = false;
    [SerializeField] bool toggleBlocks = true;
    [SerializeField] float rotationSpeed = 1f;
    [Header("Tile Variables")]
    [SerializeField] GameObject tile;
    [SerializeField] float tileSize = 0.85f;
    [Header("InstantMoves Afterimage")]
    [SerializeField] GameObject afterImage;
    [SerializeField] float afterimageSize;
    [Header("Block Variables")]
    [SerializeField] GameObject cubeblock;
    [SerializeField] float blockSize = 0.499f;
    public Vector3Int[] vectors;
    // internal variables
    private GameObject rotator;
    private GameObject cubeHolder;
    public Queue<CubeMove> moves;
    private GameObject[,] allTiles;
    private float T, t;
    //administration
    MeshRenderer afterImage_ms;
    Vector3 vecDelta = Vector3.zero;
    Vector3 vecPrevPos = Vector3.zero;
    private bool makemove = true;
    private bool methodlocked = true;
    private GameObject hitobj1;
    private GameObject hitobj2;
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
            allTiles = new GameObject[cubeLogic.triplemove + cubeLogic.square, cubewidth];// TODO aparte array voor blocks
            SpawnCubeBlocks();
        }
        else
            allTiles = new GameObject[cubeLogic.triplemove, cubewidth];
        moves = new Queue<CubeMove>();
        SpawnTiles();
        Graphical_UpdateEverything();
        cubeHolder = new GameObject("CubeHolder");
        transform.parent = cubeHolder.transform;
        Camera.main.GetComponent<CameraScript>().SetParent(transform);
        afterImage = Instantiate(afterImage);
        afterImage.transform.parent = transform;
        afterImage_ms = afterImage.GetComponent<MeshRenderer>();
        Vector3Int vec1 = new Vector3Int(0, 1, 0);
        Vector3Int vec2 = new Vector3Int(0, 0, 1);
        Vector3Int vec3 = new Vector3Int(1, 0, 0);
        Vector3Int[] Vectors = { vec1, vec2, vec3};
        vectors = Vectors;
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
            if (Physics.Raycast(ray, out hit))
            {
                hitobj1 = hit.transform.gameObject;
            }
        }
        else if (Input.GetMouseButton(0) && !methodlocked)
        {
            Ray ray = GetMouseRay();
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if ((!hit.transform.gameObject.Equals(hitobj1)) && hit.transform.GetComponent<Tile>() != null)
                {
                    hitobj2 = hit.transform.gameObject;
                    string[] namestrings1;
                    string[] namestrings2;
                    namestrings1 = hitobj1.name.Split();
                    namestrings2 = hitobj2.name.Split();

                    AddMoveBasedOnTwoTiles(int.Parse(namestrings1[0]), int.Parse(namestrings1[1]), int.Parse(namestrings2[0]), int.Parse(namestrings2[1]));
                    methodlocked = true;
                }
            }
        }
        SetCubeHolder();
        vecPrevPos = Input.mousePosition;
    }

    private void AddMoveBasedOnTwoTiles(int tile1x, int tile1y, int tile2x, int tile2y)
    { // ****** dit is een lelijk algoritme maar nou geen zin om het te verbeteren ********
        //info on section in matrix
        int tile1axis = Section(tile1x, cubeLogic.move);
        int t1ax = tile1axis;
        int tile2axis = Section(tile2x, cubeLogic.move);
        int tile1tileindex = Section(tile1x, cubewidth);
        int tile2tileindex = Section(tile2x, cubewidth);

        // section in matrix used for numbers in algorithm
        int posnegface = tile1tileindex % 2;
        int posnegface2 = tile2tileindex % 2;
        // moving all positions to face 0 as info has been extracted already
        tile1x = tile1x % cubewidth;
        tile2x = tile2x % cubewidth;
        int tileaxisdifference = 0;
        // acquire difference in tileaxis
        while (tile2axis != tile1axis)
        {
            tileaxisdifference++;
            tile1axis = (tile1axis + 1) % 3;
        }
        // new format for tile2
        int[] tile2 = { 0, tile2y, tile2x, 0, 0 };
        // if first click in uneven face, flip either x or y based on tileaxis difference
        if (posnegface == 1)
            tile2[tileaxisdifference] = cubeLogic.cubewidth_ - tile2[tileaxisdifference];

        int temptile2x = tile2x;
        // diagonal mirror, topleft to bottomright
        if (tileaxisdifference != 0)
        {
            tile2[2] = tile2y;
            tile2[1] = temptile2x;
        }

        // flip tile 2 in x or y if tilaxisdifference is > 0
        tile2[3 - tileaxisdifference] = cubeLogic.cubewidth_ - tile2[3 - tileaxisdifference];
        if (Math.Abs(posnegface - posnegface2) == 1)
            tile2[3 - tileaxisdifference] = cubeLogic.cubewidth_ - tile2[3 - tileaxisdifference];
        // relocate tile2 to its position in new (imaginairy) matrix
        posnegface2 = (posnegface2 * 2) - 1;
        tile2[3 - tileaxisdifference] = tile2[3 - tileaxisdifference] + cubewidth * posnegface2;

        int index;
        int sign;
        int startpos;


        int diffx = tile1x - tile2[2];
        int diffy = tile2[1] - tile1y;
        if (Math.Abs(diffx) > Math.Abs(diffy))
        {
            posnegface = -((posnegface * 2) - 1);
            sign = Math.Sign(diffx) * posnegface;
            index = tile1y;
            startpos = t1ax * cubeLogic.move;
        }
        else
        {
            posnegface = -((posnegface * 2) - 1);
            sign = Math.Sign(diffy) * posnegface;
            index = tile1x;
            startpos = (t1ax * cubeLogic.move + cubeLogic.doublemove) % cubeLogic.triplemove;
        }                                                
        moves.Enqueue(new CubeMove(startpos, index, sign));
                                                         
    }

 

    private void MakeMove(CubeMove cubeMove)
    {
        GameObject[] selection = new GameObject[0];
        selection = SelectLayer(cubeMove.startpos, cubeMove.index, selection);
        if (cubeMove.index == 0)
            selection = SelectFace(cubeMove.startpos, selection);
        if (cubeMove.index == cubeLogic.cubewidth_)
            selection = SelectFace(cubeMove.startpos, selection, cubeLogic.cubewidth);

        if (instantMoves)
        {
            cubeLogic.PerformRotation(cubeMove);
            Graphical_updateSelection(selection);
            Graphical_AfterImage(cubeMove);
            makemove = true;
        }
        else
        {
            cubeLogic.PerformRotation(cubeMove);
            StartCoroutine(Graphical_FakeRotation(cubeMove, selection));
        }
    }
    private void SetParentsOfGameObjectArray(GameObject[] gameObjects, Transform parent)
    {
        for (int i = 0; i < gameObjects.Length; i++)
            gameObjects[i].transform.parent = parent;
    }
    //-----------------------------------------------------------miscellaneous--------------------------------------------------------------------------------------------------------------
    private Ray GetMouseRay()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }
    private CubeMove Vector3IntToCubemove(Vector3Int entry)
    {
        if (entry.x != 0)
            return new CubeMove(cubeLogic.doublemove, Math.Abs(entry.x) - 1, Math.Sign(entry.x));
        if (entry.y != 0)
            return new CubeMove(0, Math.Abs(entry.y) - 1, Math.Sign(entry.y));
        return new CubeMove(cubeLogic.move, Math.Abs(entry.z) - 1, Math.Sign(entry.z));
    }
    private void SetCubeHolder()
    {
        transform.parent = null;
        cubeHolder.transform.LookAt(Camera.main.transform);
        transform.parent = cubeHolder.transform;
    }
    private int Section(int a, int b)
    {
        return (a - (a % b)) / b;
    }
    //--------------------------------------------------- graphical :  timing based mathods-------------------------------------------------------------------------------------------------
    private void Graphical_FinishedFakeRotation(GameObject[] selection)
    {
        Graphical_updateSelection(selection);
        makemove = true;
    }
    private void Graphical_DisableAfterImage()
    {
        afterImage_ms.enabled = false;
    }
    //----------------------------------------------------------------graphical Methods---------------------------------------------------------------------------------------------------
    private void Graphical_AfterImage(CubeMove cubeMove)
    {
        Vector3Int vector = vectors[cubeMove.startpos / cubeLogic.move];
        afterImage.transform.localPosition = vector - vector*cubeMove.index;
        afterImage.transform.localScale = (Vector3Int.one * cubewidth) - vector*2;
        afterImage.transform.localRotation = Quaternion.Euler(vector * UnityEngine.Random.Range(69, 420));
        afterImage_ms.enabled = true;

    }
    IEnumerator Graphical_FakeRotation(CubeMove cubeMove, GameObject[] selection, float rot_amount = 1f, int degrees = 90, float time = 0.001f)
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
        SetParentsOfGameObjectArray(selection, rotator.transform);
        //rotate
        for (int i = 1; i <= degrees / rotationSpeed; i++)
        {
            rotator.transform.Rotate(vectors[cubeMove.startpos / cubeLogic.move] * cubeMove.sign, rot_amount * rotationSpeed);
            yield return new WaitForSeconds(Mathf.Epsilon);
        }
        rotator.transform.rotation = transform.rotation;
        // reset parents
        SetParentsOfGameObjectArray(selection, transform);
        // reset transforms
        for (int i = 0; i < selection.Length; i++)
        {
            selection[i].transform.localPosition = tempposs[i];
            selection[i].transform.localRotation = temprots[i];
        }
        Graphical_FinishedFakeRotation(selection);
    }
    //----------------------------------------------------------------graphical update methods---------------------------------------------------------------------------------------------------
    private void Graphical_updateSelection(GameObject[] selectiontiles)
    {
        for (int i = 0; i < selectiontiles.Length; i++)
        {
            string[] strings = selectiontiles[i].name.Split();
            selectiontiles[i].GetComponent<Renderer>().material = colorTable.ToMaterial(cubeLogic.cubestate[int.Parse(strings[0]), int.Parse(strings[1])]);
        }
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
        
        int tileind = 0;
        // front tile
        SpawnTileFace(
            new Vector3(t, t, T),
            new Vector3(-tiledistance, 0f, 0f),
            new Vector3(0f, -tiledistance, 0f),
            new Vector3(0f, 180f, 0f),
            tileind++
            );
        // back tiles
        SpawnTileFace(
            new Vector3(t, t, -T),
            new Vector3(-tiledistance, 0f, 0f),
            new Vector3(0f, -tiledistance, 0f),
            new Vector3(0f, 0f, 0f),
            tileind++
            );
        // left tiles
        SpawnTileFace(
            new Vector3(T, t, t),
            new Vector3(0f, -tiledistance, 0f),
            new Vector3(0f, 0f, -tiledistance),
            new Vector3(0f, 270f, 0f),
            tileind++
            );
        // right tiles
        SpawnTileFace(
            new Vector3(-T, t, t),
            new Vector3(0f, -tiledistance, 0f),
            new Vector3(0f, 0f, -tiledistance),
            new Vector3(0f, 90f, 0f),
            tileind++
            );
        // up tiles
        SpawnTileFace(
            new Vector3(t, T, t),
            new Vector3(0f, 0f, -tiledistance),
            new Vector3(-tiledistance, 0f, 0f),
            new Vector3(90f, 90f, 0f),
            tileind++
            );
        // down tiles
        SpawnTileFace(
            new Vector3(t, -T, t),
            new Vector3(0f, 0f, -tiledistance),
            new Vector3(-tiledistance, 0f, 0f),
            new Vector3(270f, 90f, 0f),
            tileind++
            );
    }
    private void SpawnCubeBlocks()
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
    }
    private void SpawnTileFace(Vector3 startingpoint, Vector3 rowstep, Vector3 collumnstep, Vector3 quaternion_rotation, int tileindex)
    {
        Vector3 tilesize = new Vector3(this.tileSize, this.tileSize, this.tileSize);
        Quaternion rotation = Quaternion.Euler(quaternion_rotation);
        for (int collumn = 0; collumn < cubewidth; collumn++)
            for (int row = 0; row < cubewidth; row++)
            {
                GameObject newtile = Instantiate(tile);
                newtile.transform.parent = transform;
                newtile.name = (row + cubewidth * tileindex).ToString() + " " + collumn.ToString();
                newtile.transform.localPosition = startingpoint + collumn * collumnstep + row * rowstep;
                newtile.transform.localRotation = rotation;
                newtile.transform.localScale = tilesize;
                newtile.gameObject.AddComponent<Tile>();
                allTiles[tileindex * cubewidth + row, collumn] = newtile;
            }

    }
    //----------------------------------------------------------------selector methods---------------------------------------------------------------------------------------------------------
    private GameObject[] SelectLayer(int startpos, int index, GameObject[] other)
    {
        int counter = 0;
        GameObject[] selection = IncreaseSize(other, cubeLogic.doublemove, out counter);
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
    private GameObject[] SelectFace(int startpos, GameObject[] other, int extraoffset = 0)
    {
        int counter = 0;
        GameObject[] selection = IncreaseSize(other, cubeLogic.square, out counter);
        int offset = (startpos + cubeLogic.doublemove + extraoffset) % cubeLogic.triplemove;
        for (int x = 0; x < cubewidth; x++)
            for (int y = 0; y < cubewidth; y++)
            {
                selection[counter] = allTiles[offset + x, y]; counter++;
            }
        return selection;
    }
    private GameObject[] IncreaseSize(GameObject[] other, int addedsize, out int counter)
    {
        GameObject[] selection;
        selection = new GameObject[other.Length + addedsize];
        counter = other.Length;
        for (int i = 0; i < other.Length; i++)
            selection[i] = other[i];
        return selection;
    }
}