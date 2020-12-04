using System.Collections;
using System.Collections.Generic;
using UnityEngine;// kan uiteindelijk verwijderd worden

public class CubeSolver : MonoBehaviour// kan uiteindelijk verwijderd worden
{
    private Cube cube;

    public CubeSolver(Cube cube)
    {
        this.cube = cube;
    }

    public void Scramble()
    {
        for (int i = 0; i < 20; i++)
        {
            int layer = Random.Range(1, cube.cubeLogic.cubewidth + 1);
            if (Random.Range(0, 2) == 1) { layer *= -1; }
            switch (Random.Range(0, 2))
            {
                case 0: cube.moves.Enqueue(cube.cubeLogic.VectorToRotation(new Vector3Int(layer, 0, 0))); ; break;
                case 1: cube.moves.Enqueue(cube.cubeLogic.VectorToRotation(new Vector3Int(0, layer, 0))); ; break;
                case 2: cube.moves.Enqueue(cube.cubeLogic.VectorToRotation(new Vector3Int(0, 0, layer))); ; break;
            }

        }
    }

    public IEnumerator doMove(Vector3Int v)
    {
        cube.moves.Enqueue(cube.cubeLogic.VectorToRotation(v));
        while (cube.moves.Count > 0)
        {
            yield return new WaitForSeconds(0.1f); // wait for 1 second.
        }
    }

    public IEnumerator SolveCorner(int a, int b, int c)
    {
        //Solve corner with colors a, b, c in bottom layer.
        //Zet hoek in goede positie om naar beneden te gaan
        if (findCorner(a, b, c) > 3)
        {
            while (findCorner(a, b, c) != 5)
            {
                yield return doMove(new Vector3Int(0, 0, 2));
            }
        }
        else
        {
            //Haal verkeerd hoekje uit onderste laag naar bovenste laag of laat goed staan
            int aantal = 0;
            while (findCorner(a, b, c) != 2)
            {
                aantal += 1;
                yield return doMove(new Vector3Int(0, 0, 1));
            }

            //Haal hoekje naar boven
            //D' R' D B
            yield return doMove(new Vector3Int(0, 2, 0));
            yield return doMove(new Vector3Int(2, 0, 0));
            yield return doMove(new Vector3Int(0, -2, 0));
            yield return doMove(new Vector3Int(0, 0, -2));

            //Draai witte kant terug
            for (int i = 0; i < aantal; i++)
            {
                yield return doMove(new Vector3Int(0, 0, -1));
            }
        }

        //Zet hoekje naar beneden goed
        if (findCorner(a, b, c) == 5 && cube.cubeLogic.cubestate[1 + cube.cubeLogic.cubewidth * 4, 1] == a)
        {
            //D' R' D
            yield return doMove(new Vector3Int(0, 2, 0));
            yield return doMove(new Vector3Int(2, 0, 0));
            yield return doMove(new Vector3Int(0, -2, 0));
        }
        else if (findCorner(a, b, c) == 5 && cube.cubeLogic.cubestate[0 + cube.cubeLogic.cubewidth * 3, 1] == a)
        {
            //R B R'
            yield return doMove(new Vector3Int(-2, 0, 0));
            yield return doMove(new Vector3Int(0, 0, -2));
            yield return doMove(new Vector3Int(2, 0, 0));
        }
        else if (findCorner(a, b, c) == 5 && cube.cubeLogic.cubestate[1 + cube.cubeLogic.cubewidth * 1, 0] == a)
        {
            //R D B2 D' R'
            yield return doMove(new Vector3Int(-2, 0, 0));
            yield return doMove(new Vector3Int(0, -2, 0));
            yield return doMove(new Vector3Int(0, 0, -2));
            yield return doMove(new Vector3Int(0, 0, -2));
            yield return doMove(new Vector3Int(0, 2, 0));
            yield return doMove(new Vector3Int(2, 0, 0));
        }
    }

    public IEnumerator Solve()
    {
        //Wacht tot scramble klaar is
        while (cube.moves.Count > 0)
        {
            yield return new WaitForSeconds(1f); // wait for 1 second.
        }

        yield return SolveCorner(0, 4, 2);
        yield return doMove(new Vector3Int(0, 0, 1));
        yield return SolveCorner(0, 2, 5);
        yield return doMove(new Vector3Int(0, 0, 1));
        yield return SolveCorner(0, 3, 5);
        yield return doMove(new Vector3Int(0, 0, 1));
        yield return SolveCorner(0, 3, 4);

        yield return doMove(new Vector3Int(1, 0, 0));
        yield return doMove(new Vector3Int(2, 0, 0));


        yield return new WaitForSeconds(1f);
    }

    public int findCorner(int a, int b, int c)
    {
        //Find the position of a cornerpiece with colors a, b, c.
        //Wie verzint hier een formule voor die altijd werkt?
        int[,,] corners = {
            { { 0, 0, 1 }, { 5, 0, 0 }, { 2, 1, 0 } },
            { { 0, 1, 1 }, { 3, 1, 0 }, { 5, 0, 1 } },
            { { 0, 1, 0 }, { 3, 0, 0 }, { 4, 0, 1 } },
            { { 0, 0, 0 }, { 4, 0, 0 }, { 2, 0, 0 } },

            { { 1, 0, 0 }, { 4, 1, 0 }, { 2, 0, 1 } },
            { { 1, 1, 0 }, { 3, 0, 1 }, { 4, 1, 1 } },
            { { 1, 0, 1 }, { 5, 1, 0 }, { 2, 1, 1 } },
            { { 1, 1, 1 }, { 3, 1, 1 }, { 5, 1, 1 } },
        };

        for (int i = 0; i < corners.GetLength(0); i++)
        {
            bool thisCorner = true;
            for (int j = 0; j < corners.GetLength(1); j++)
            {
                int color = cube.cubeLogic.cubestate[corners[i, j, 1] + cube.cubeLogic.cubewidth * corners[i, j, 0], corners[i, j, 2]];
                if (color != a && color != b && color != c)
                {
                    thisCorner = false;
                    break;
                }
            }
            if (thisCorner) return i;
        }
        return -1;
    }
}