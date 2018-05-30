using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierTriangle {
    int degree = -1;
    private PointData[][] grid;
    private int[] factorals;
    public BezierTriangle(PointData[][] grid)
    {
        this.grid = grid;
        SetupDegree();
        SetupFactorals();
    }

    void SetupDegree()
    {
        int rowLength = this.grid[0].Length;
        if (rowLength < 2) throw new ArgumentOutOfRangeException("The base of triangle must have at least 2 points");
        for (int i = 1; i < this.grid.Length; i++)
        {
            if (rowLength -1 != this.grid[i].Length)
            {
                throw new ArgumentOutOfRangeException("The triangle must lose 1 point every row");
            }
            rowLength = this.grid[i].Length;
        }
        degree = this.grid[0].Length -  1;
    }

    void SetupFactorals()
    {
        int index = 0;
        factorals = new int[(degree + 1)*(degree + 2) / 2];
        for (int i = 0; i < (degree + 1); i++)
        {
            for (int j = 0; j < (degree + 1) - i; j++)
            {
                int iFactoral = (degree - i) - j;
                int jFactoral = j;
                int kFactoral = i;

                factorals[index] = CalculateFactoral(iFactoral, jFactoral, kFactoral, degree);
                index++;
            }
        }
    }

    public PointData GetPoint(float u, float v, float w)
    {
        int index = 0;
        PointData result = new PointData();
        for (int i = 0; i < (degree + 1); i++)
        {
            for (int j = 0; j < (degree + 1) - i; j++)
            {
                int iFactoral = (degree - i) - j;
                int jFactoral = j;
                int kFactoral = i;

                result += grid[i][j] * (factorals[index] * Mathf.Pow(u, iFactoral) * Mathf.Pow(v, jFactoral) * Mathf.Pow(w, kFactoral));
                index++;
            }
        }

        return result;
    }

    static int CalculateFactoral(int i, int j, int k, int n)
    {
        return FactorialLoop(n) / (FactorialLoop(i) * FactorialLoop(j) * FactorialLoop(k));
    }

    static int FactorialLoop(int number)
    {
        if (number < 0)
            throw new ArgumentException("Factoral needs to be positive number or 0!");
        if (number == 0 || number == 1)
            return 1;
        else
            return number * FactorialLoop(number - 1);
    }
    // n = degree;
    // i + j + k = n;
    // u, v, w = Barycentric coordinates
    // u + v + w = 1
    // contorl points per degree (n + 1)(n + 2) / 2
    // planar x(u, v, w) => UX100 + VX010 + WX001
    // quadratic x(u, v, w) => U^2X200 + V^2X020 + W^2X002 + 2uvX110 + 2vwX011 + 2Wux101
    // cubic U^3X300 + V^3X030 + W^3X003 + 6uvwX222

}
