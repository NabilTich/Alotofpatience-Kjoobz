using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CubeLogic : MonoBehaviour
{
    [NonSerialized] public Vector3Int[] vectors;
    [NonSerialized] public int[,] cubestate;
    [NonSerialized] public int cubewidth;
    [NonSerialized] public int maplength;
    [NonSerialized] public int maplength_;
    [NonSerialized] public int square;
    [NonSerialized] public int move;
    [NonSerialized] public int doublemove;
    [NonSerialized] public int triplemove;
    [NonSerialized] public int cubewidth_;
    public void InitializeCube(int cubewidth)
    {
        GeneralInitialization(cubewidth);
        for (int i = 0; i < 6; i++)
            for (int x = 0; x < cubewidth; x++)
                for (int y = 0; y < cubewidth; y++)
                    cubestate[x + cubewidth * i, y] = i;

    }
    public void InitializeCube(int[,] cubestate)
    {
        GeneralInitialization(cubestate.GetLength(1));
        for (int i = 0; i < 6; i++)
            for (int x = 0; x < cubewidth; x++)
                for (int y = 0; y < cubewidth; y++)
                    this.cubestate[x + cubewidth * i, y] = cubestate[x + cubewidth * i, y];
    }
    private void GeneralInitialization(int cubewidth)
    {
        Vector3Int vec1 = new Vector3Int(0, 1, 0);
        Vector3Int vec2 = new Vector3Int(0, 0, 1);
        Vector3Int vec3 = new Vector3Int(1, 0, 0);
        Vector3Int[] Vectors = { vec1, vec2, vec3 };
        vectors = Vectors;
        this.cubewidth = cubewidth;
        maplength = cubewidth * 7;
        maplength_ = maplength - 1;
        square = cubewidth * cubewidth;
        move = cubewidth * 2;
        doublemove = cubewidth * 4;
        triplemove = cubewidth * 6;
        cubewidth_ = cubewidth - 1;
        cubestate = new int[maplength, cubewidth];
    }
    private void RotateFaceClockwise(int xtopleft)
    {
        for (int i = 0; i < cubewidth; i++)
            TranslateStrip(xtopleft, i, maplength_ - i, 0, 1, 0, 0, 1);
        RetrieveFace(xtopleft);
    }
    private void RotateFaceCounterClockwise(int xtopleft)
    {
        for (int i = 0; i < cubewidth; i++)
            TranslateStrip(xtopleft, i, triplemove + i, cubewidth_, 1, 0, 0, -1);
        RetrieveFace(xtopleft);
    }
    private void RotateFace180(int xtopleft)
    {
        for (int i = 0; i < cubewidth; i++)
            TranslateStrip(xtopleft, i, cubewidth - i, maplength_, 1, 0, -1, 0);
        RetrieveFace(xtopleft);
    }
    private void RetrieveFace(int xtopleft)
    {
        for (int i = 0; i < cubewidth; i++)
            TranslateStrip(triplemove, i, xtopleft, i, 1, 0, 1, 0);
    }
    private void TranslateStrip(int x, int y, int xend, int yend, int xstep, int ystep, int xendstep, int yendstep)
    {
        for (int i = 0; i < cubewidth; i++)
            Overwrite(x + i * xstep, y + i * ystep, xend + i * xendstep, yend + i * yendstep);
    }
    private void TranslateStrip(StripArgs start, StripArgs end)
    {
        for (int i = 0; i < start.length; i++)
            Overwrite(start.x + i * start.xstep, start.y + i * start.ystep, end.x + i * end.xstep, end.y + i * end.ystep);
    }
    private void Overwrite(int xfrom, int yfrom, int xto, int yto)
    {
        cubestate[xto, yto] = cubestate[xfrom, yfrom];
    }
    private void StripRotation(int index, int sign, int startpos)
    {
        StripArgs leftargs = new StripArgs(startpos, index, 1, 0, cubewidth);
        StripArgs rightargs = new StripArgs((startpos + move + index) % triplemove, cubewidth_, 0, -1, cubewidth);
        StripArgs storage = new StripArgs(triplemove, index, 1, 0, cubewidth);
        StripArgs[] OOP = { storage, leftargs, rightargs.ShoveAndInvert(), leftargs.ShoveAndInvert(), rightargs };

        // produces two orders of operations based on the sign, to relocate all the strips
        int count = 0; if (sign <= 0) count = 5;
        for (int i = 0; i < 5; i++)
            TranslateStrip(OOP[(count += sign) % 5], OOP[(count + -1 * sign) % 5]);

    }
    public void PerformRotation(Rotation rotation)
    {
        StripRotation(rotation.index, rotation.sign, rotation.startpos);

        if (rotation.index != cubewidth_ && rotation.index != 0)
            return;

        if (rotation.index == cubewidth_)
            rotation.index += 1;

        if (rotation.sign == -1)
            RotateFaceCounterClockwise((rotation.startpos + doublemove + rotation.index) % triplemove);
        else if (rotation.sign == 1)
            RotateFaceClockwise((rotation.startpos + doublemove + rotation.index) % triplemove);
    }
    public void PerformRotation(Vector3Int move)
    {
        PerformRotation(VectorToRotation(move));
    }
    public Rotation VectorToRotation(Vector3Int entry)
    {
        if (entry.x != 0)
            return new Rotation(doublemove, Math.Abs(entry.x) - 1, Math.Sign(entry.x)); 
        if (entry.y != 0)
            return new Rotation(0, Math.Abs(entry.y) - 1, Math.Sign(entry.y));
        if (entry.z != 0)
            return new Rotation(move, Math.Abs(entry.z) - 1, Math.Sign(entry.z));
        else
            return new Rotation(UnityEngine.Random.Range(0, 3)*move, UnityEngine.Random.Range(0, cubewidth), Sign(UnityEngine.Random.Range(-1,1)));
    }
    private int Sign(int input)
    {
        if (input >= 0)
            return 1;
        else
            return -1;
    }
    private int Section(int a, int b)
    {
        return (a - (a % b)) / b;
    }
    public Rotation MoveBasedOnTiles(int tile1x, int tile1y, int tile2x, int tile2y)
    { // ****** dit is een lelijk algoritme maar nou geen zin om het te verbeteren ********
        //info on section in matrix
        int tile1axis = Section(tile1x, move);
        int t1ax = tile1axis;
        int tile2axis = Section(tile2x, move);
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
            tile2[tileaxisdifference] = cubewidth_ - tile2[tileaxisdifference];

        int temptile2x = tile2x;
        // diagonal mirror, topleft to bottomright
        if (tileaxisdifference != 0)
        {
            tile2[2] = tile2y;
            tile2[1] = temptile2x;
        }

        // flip tile 2 in x or y if tilaxisdifference is > 0
        tile2[3 - tileaxisdifference] = cubewidth_ - tile2[3 - tileaxisdifference];
        if (Math.Abs(posnegface - posnegface2) == 1)
            tile2[3 - tileaxisdifference] = cubewidth_ - tile2[3 - tileaxisdifference];
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
            startpos = t1ax * move;
        }
        else
        {
            posnegface = -((posnegface * 2) - 1);
            sign = Math.Sign(diffy) * posnegface;
            index = tile1x;
            startpos = (t1ax * move + doublemove) % triplemove;
        }
        return new Rotation(startpos, index, sign);
    }
    public Vector3Int RandomVector()
    {
        return vectors[UnityEngine.Random.Range(0, 3)] * UnityEngine.Random.Range(-cubewidth, cubewidth + 1);
    }

}