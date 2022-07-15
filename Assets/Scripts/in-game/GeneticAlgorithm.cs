using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
public class GeneticAlgorithm
{
    public float max_score_offset = 0.01f; 
    public Population population;
    public Individual fittest;
    public Individual secondFittest;
    public int generationCount = 0;

    Position start;
    Position end;

    public void Run(int size, int width, int height, Position start, Position end, float targetScore)
    {
        this.start = start;
        this.end = end;
        population = new Population(size, width, height, start, end, targetScore);
        population.CalculateScore();
        fittest = new Individual(population.GetFittest());
        secondFittest = new Individual(population.GetSecondFittest());
        var rand = new Random(Guid.NewGuid().GetHashCode());
        while (Mathf.Abs(population.fittest - targetScore) > max_score_offset)
        {
            generationCount += 1;
            if (generationCount % 100 == 0)
                max_score_offset += 0.01f;
            Selection();
            Crossover();
            if (rand.Next(2) > 0) 
            {
                Mutation();
            }
            fittest.RemoveSharedWalls();
            secondFittest.RemoveSharedWalls();
            AddFittestOffspring();
            population.CalculateScore();
        } 
        Debug.Log("Generation: " + generationCount);
    }
    void Selection()
    {
        fittest = new Individual(population.GetFittest());
        secondFittest = new Individual(population.GetSecondFittest());
    }
    void Crossover()
    {
        var rand = new Random(Guid.NewGuid().GetHashCode());
        int crossOverPointX1 = rand.Next(population.individuals[0].width);
        int crossOverPointY1 = rand.Next(population.individuals[0].height);
        int crossOverPointX2 = rand.Next(crossOverPointX1, population.individuals[0].width);
        int crossOverPointY2 = rand.Next(crossOverPointY1, population.individuals[0].height);
        for (int i = crossOverPointY1; i <= crossOverPointY2; i++)
        {
            for (int j = crossOverPointX1; j < crossOverPointX2; j++)
            {
                WallState temp = fittest.maze[i, j];
                fittest.maze[i, j] = secondFittest.maze[i, j];
                secondFittest.maze[i, j] = temp;
            }
        }
    }
    void Mutation()
    {
        var rand = new Random(Guid.NewGuid().GetHashCode());
        int mutationPointX = rand.Next(population.individuals[0].width);
        int mutationPointY = rand.Next(population.individuals[0].height);

        fittest.RandomWall(mutationPointY, mutationPointX);

        mutationPointX = rand.Next(population.individuals[0].width);
        mutationPointY = rand.Next(population.individuals[0].height);

        secondFittest.RandomWall(mutationPointY, mutationPointX);
    }
    void AddFittestOffspring()
    {
        fittest.CalcScore(start, end);
        secondFittest.CalcScore(start, end);
        int leastFittestIndex = population.GetLeastFittestIndex();
        population.individuals[leastFittestIndex] = GetFittestOffspring();
    }
    public Individual GetFittestOffspring()
    {
        if (Mathf.Abs(fittest.score - population.targetScore) < Mathf.Abs(secondFittest.score - population.targetScore))
            return fittest;
        return secondFittest;
    }
}

public class Individual
{
    public float score = 0;
    public WallState[,] maze;
    public int width;
    public int height;

    public Individual(int width, int height, Position start)
    {
        this.width = width;
        this.height = height;
        maze = MazeGenerator.Generate(width, height, start, 10);      
        score = 0;
    }
    public Individual(Individual other)
    {
        width = other.width;
        height = other.height;
        maze = new WallState[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                maze[i, j] = other.maze[i, j];
            }
        }
    }
    public void RandomWall(int i, int j)
    {
        int rand = new Random(Guid.NewGuid().GetHashCode()).Next(9);
        maze[i, j] = (WallState)rand;
        if (i == 0)
            maze[i, j] |= WallState.UP;
        if (i == height - 1)
            maze[i, j] |= WallState.DOWN;
        if (j == 0)
            maze[i, j] |= WallState.LEFT;
        if (j == width - 1)
            maze[i, j] |= WallState.RIGHT;
    }
    public void RemoveSharedWalls()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width - 1; j++)
            {
                if (maze[i, j].HasFlag(WallState.RIGHT))
                    maze[i, j + 1] |= WallState.LEFT;
                if (maze[i, j + 1].HasFlag(WallState.LEFT))
                    maze[i, j] |= WallState.RIGHT;
            }
        }
        for (int i = 0; i < height - 1; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (maze[i, j].HasFlag(WallState.DOWN))
                    maze[i + 1, j] |= WallState.UP;
                if (maze[i + 1, j].HasFlag(WallState.UP))
                    maze[i, j] |= WallState.DOWN;
            }
        }   
    }
    public void CalcScore(Position startPosition, Position endPosition)
    {
        MazeGenerator.ResetUnvistedMaze(maze, width, height);
        List<Position> minPath = PathFinder.ApplyBFS(maze, width, height, startPosition, endPosition);
        int minPathStep;
        int nSuccessPaths = 0;
        int nFailPaths = 0;
        if (minPath != null)
        {
            minPathStep = minPath.Count - 1;
        }
        else
        {
            minPathStep = 0;
        }
        MazeGenerator.ResetUnvistedMaze(maze, width, height);
        PathFinder.CountPaths(maze, width, height, startPosition, endPosition, ref nSuccessPaths, ref nFailPaths);
        //(nSuccessPaths, nFailPaths) = PathFinder.CountPaths(maze, width, height, startPosition, endPosition);
        //score = (nFailPaths * minPathStep) / (float)((nFailPaths + nSuccessPaths)*(height*width));
        if (nSuccessPaths == 0)
            score = 0;
        else
            score = nFailPaths / (float)(2*(nFailPaths + nSuccessPaths)) + (minPathStep + width + height) / (float)(GameConstant.MAX_HEIGHT * GameConstant.MAX_WIDTH * 2); 
    }
}

public class Population
{
    public int popSize = 10;
    public int width;
    public int height;
    public Position start;
    public Position end;
    public float targetScore;
    public Individual[] individuals = new Individual[10];
    public float fittest = 0;

    public Population(int size, int width, int height, Position start, Position end, float targetScore)
    {
        popSize = size;
        this.width = width;
        this.height = height;
        this.targetScore = targetScore;
        this.start = start;
        this.end = end;
        for (int i = 0; i < individuals.Length; i++)
        {
            individuals[i] = new Individual(width, height, start);
        }
    }
    public Individual GetFittest()
    {
        float minOffset = int.MaxValue;
        int fittestIndex = -1;
        for (int i = 0; i < popSize; i++)
        {
            float scoreOffset = Mathf.Abs(targetScore - individuals[i].score);
            if (scoreOffset < minOffset)
            {
                minOffset = scoreOffset;
                fittestIndex = i;
            }
        }
        fittest = individuals[fittestIndex].score;
        return individuals[fittestIndex];
    }
    public Individual GetSecondFittest()
    {
        int maxFit1 = 0;
        int maxFit2 = 0;
        for (int i = 1; i < popSize; i++) 
        {
            if (Mathf.Abs(individuals[i].score - targetScore) < Mathf.Abs(individuals[maxFit1].score - targetScore)) 
            {
                maxFit2 = maxFit1;
                maxFit1 = i;
            } 
            else if (Mathf.Abs(individuals[i].score - targetScore) < Mathf.Abs(individuals[maxFit2].score - targetScore)) 
            {
                maxFit2 = i;
            }
        }
        return individuals[maxFit2];
    }
    public int GetLeastFittestIndex()
    {
        float maxOffset = int.MinValue;
        int leastFittestIndex = -1;
        for (int i = 0; i < popSize; i++)
        {
            float scoreOffset = Mathf.Abs(targetScore - individuals[i].score);
            if (scoreOffset > maxOffset)
            {
                maxOffset = scoreOffset;
                leastFittestIndex = i;
            }
        }
        return leastFittestIndex;
    }
    public void CalculateScore() 
    {
        for (int i = 0; i < popSize; i++) 
        {
            individuals[i].CalcScore(start, end);
        }
        GetFittest();
    }
}
