using System;
using System.Collections.Generic;
using System.Collections;
namespace PuzzleState
{
    public class State
    {
        public int[] Arr { get; private set; }
        public int NumRows { get; }
        private int mEmptyTileIndex;
        public int GetEmptyTileIndex() { return mEmptyTileIndex; }
        public State(int rows)
        {
            NumRows = rows;
            Arr = new int[NumRows * NumRows];
            for (int i = 0; i < Arr.Length; i++)
            {
                Arr[i] = i;
            }
            //Empty tile defult index is 8
            mEmptyTileIndex = Arr.Length - 1;
        }
        public State(State other)
        {
            NumRows = other.NumRows;
            mEmptyTileIndex = other.mEmptyTileIndex;
            Arr = new int[NumRows * NumRows];
            other.Arr.CopyTo(Arr, 0);
        }

        //check equality between current state and goal state
        public static bool Equals(State a, State b)
        {
            for(int i=0; i< a.Arr.Length; i++)
            {
                if (a.Arr[i] != b.Arr[i]) return false;
            }
            return true;
        }
        //this function returns the index of the empty tile for any given state 
        public int FindEmptyTileIndex()
        {
            for(int i=0; i< Arr.Length;i++)
            {
                //if value of i in array = 8
                if (Arr[i] == Arr.Length-1) return i;
            }
            return Arr.Length;
        }
        //swapping the empty tile with a near by tile
        public void SwapWithEmpty(int index)
        {
            int temp = Arr[index];
            Arr[index] = Arr[mEmptyTileIndex];
            Arr[mEmptyTileIndex] = temp;
            mEmptyTileIndex = index;
        }
        //This function get the Manhattan distance which use for the f=g+h formula in A star 
        public int GetManhattanCost()
        {
            int ManhattanCost = 0;
            for(int i=0; i< Arr.Length; i++)
            {
                int v = Arr[i];
                if(v == Arr.Length-1) { continue; }
                int gx = v % NumRows;
                int gy = v/NumRows;
                int x = i%NumRows;
                int y = i/NumRows;
                //the formula below is use for calculate the distance between current state and goal state
                ManhattanCost += Math.Abs(x - gx) + Math.Abs(y - gy);
            }
            return ManhattanCost;
        }
    }
    public class PuzzleMap
    {
        private Dictionary<int, List<int>> mEdges = new Dictionary<int, List<int>>();
        private List<int> getneighbours(int id) { return mEdges[id]; }
        public List<Node<State>> GetNeighbours(PuzzleNode p)
        {
            List<Node<State>> neighbours = new List<Node<State>>();
            int zero = p.Value.GetEmptyTileIndex();
            List<int> intArra = getneighbours(zero);
            for(int i=0; i< intArra.Count; i++)
            {
                PuzzleNode state = new PuzzleNode(
                    this, new State(p.Value));
                state.Value.SwapWithEmpty(intArra[i]);
                neighbours.Add(state);
            }
            return neighbours;
        }
        
        private void CreateGraph(int numRows)
        {
            for(int i=0; i< numRows; i++)
            {
                for(int j=0; j< numRows; j++)
                {
                    int index = i * numRows + j;
                    List<int> list = new List<int>();
                    //4 if below is use for checking that if a tile have a neighbour nearby at each direction(left, right, up, down)
                    //this if is to check if a tile have a neighbour from  "up" direction
                    if(i-1 >=0)
                    {
                        list.Add((i-1) * numRows + j);
                    }
                    //this if is to check if a tile have a neighbour from "down" direction
                    if(i+1 < numRows)
                    {
                        list.Add((i+1) * numRows + j);    
                    }
                    //this if is to check if a tile have a neighbour from "left" direction
                    if(j-1 >=0)
                    {
                        list.Add(i*numRows + j-1);
                    }
                    //this if is to check if a tile have a neighbour from "right" direction
                    if(j+1 < numRows)
                    {
                        list.Add(i*numRows + j+1);
                    }
                    mEdges[index] = list;
                }
            }
        }
    }
    abstract public class Node<T>
    {
        public T Value { get; private set; }
        public Node(T value) { Value = value; }
        abstract public List<Node<T>> GetNeighbours();
    }
    public class PuzzleNode : Node<State>
    {
        private PuzzleMap map;
        public PuzzleNode(State value) : base(value)
        {

        }
        public PuzzleNode(PuzzleMap map, State value) : base(value)
        {
            this.map = map;
        }

        public override List<Node<State>> GetNeighbours()
        {
            return map.GetNeighbours(this);
        }
    }
}


