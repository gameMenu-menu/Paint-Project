using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GridPart
{
    public List<Pixel> pixels = new List<Pixel>();

    public Color color;

    public int middleRow, middleColumn;


    public void AddPixel(int row, int column, Color col)
    {

        Pixel pixel = new Pixel();

        pixel.Init(row, column, col);

        pixels.Add(pixel);

    }

    public void CalculatePosition(int[] indexes)
    {
        middleRow = indexes[0];
        middleColumn = indexes[1];
    }
    
}
public struct Pixel
{
    public int row, column;
    public Color color;

    public void Init(int _row, int _column, Color _colur)
    {
        row = _row;
        column = _column;
        color = _colur;
    }
}
