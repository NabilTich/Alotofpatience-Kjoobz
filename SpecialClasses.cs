﻿public struct StripArgs
{
    public StripArgs(int x, int y, int xstep, int ystep, int length)
    {
        this.x = x;
        this.y = y;
        this.xstep = xstep;
        this.ystep = ystep;
        this.length = length;
    }
    public int length;
    public int x;
    public int y;
    public int xstep;
    public int ystep;
    public StripArgs Shove()
    {
        return new StripArgs(x + length, y, xstep, ystep, length);
    }
    public StripArgs ShoveAndInvert()
    {
        return new StripArgs(x + length + xstep * (length-1), y + ystep*(length -1), xstep *-1, ystep*-1, length);
    }
}
public struct Rotation
{
    public Rotation(int startpos, int index, int sign)
    {
        this.startpos = startpos;
        this.index = index;
        this.sign = sign;
    }
    public int sign;
    public int index;
    public int startpos;
}