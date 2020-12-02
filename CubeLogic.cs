using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CubeLogic : MonoBehaviour
{
    [System.NonSerialized] public int[,] cubestate;
    [System.NonSerialized] public int cubewidth;
    [System.NonSerialized] public int maplength;
    [System.NonSerialized] public int maplength_;
    [System.NonSerialized] public int square;
    [System.NonSerialized] public int move;
    [System.NonSerialized] public int doublemove;
    [System.NonSerialized] public int triplemove;
    [System.NonSerialized] public int cubewidth_;
    public void InitializeCube(int cubewidth)
    {
        this.cubewidth = cubewidth;
        maplength = cubewidth * 7;
        maplength_ = maplength - 1;
        square = cubewidth * cubewidth;
        move = cubewidth * 2;
        doublemove = cubewidth * 4;
        triplemove = cubewidth * 6;
        cubewidth_ = cubewidth - 1;
        cubestate = new int[maplength, cubewidth];
        for (int i = 0; i < 6; i++)
            for (int x = 0; x < cubewidth; x++)
                for (int y = 0; y < cubewidth; y++)
                    cubestate[x + cubewidth * i, y] = i;

    }
    public void InitializeCube(int[,] cubestate)
    {
        this.cubewidth = cubestate.GetLength(1);
        maplength = cubewidth * 7;
        maplength_ = maplength - 1;
        square = cubewidth * cubewidth;
        move = cubewidth * 2;
        doublemove = cubewidth * 4;
        triplemove = cubewidth * 6;
        cubewidth_ = cubewidth - 1;
        cubestate = new int[maplength, cubewidth];
        for (int i = 0; i < 6; i++)
            for (int x = 0; x < cubewidth; x++)
                for (int y = 0; y < cubewidth; y++)
                    this.cubestate[x + cubewidth * i, y] = cubestate[x + cubewidth * i, y];
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
    public void PerformRotation(CubeMove cubeMove)
    {
        StripRotation(cubeMove.index, cubeMove.sign, cubeMove.startpos);

        if (cubeMove.index != cubewidth_ && cubeMove.index != 0)
            return;

        if (cubeMove.index == cubewidth_)
            cubeMove.index = cubeMove.index + 1;

        if (cubeMove.sign == -1)
            RotateFaceCounterClockwise((cubeMove.startpos + doublemove + cubeMove.index) % triplemove);
        else if (cubeMove.sign == 1)
            RotateFaceClockwise((cubeMove.startpos + doublemove + cubeMove.index) % triplemove);
    }
    public void PerformRotation(Vector3Int move)
    {
        PerformRotation(Vector3IntToCubemove(move));
    }
    public CubeMove Vector3IntToCubemove(Vector3Int entry)
    {
        if (entry.x != 0)
            return new CubeMove(doublemove, Math.Abs(entry.x) - 1, Math.Sign(entry.x)); 
        if (entry.y != 0)
            return new CubeMove(0, Math.Abs(entry.y) - 1, Math.Sign(entry.y));
        return new CubeMove(move, Math.Abs(entry.z) - 1, Math.Sign(entry.z));
    }
}