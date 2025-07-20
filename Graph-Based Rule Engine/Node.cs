using System;
using System.Collections.Generic;

//Let 's model a rule engine where:
//Nodes represent concepts or conditions.
//Some nodes are children of multiple parents.
//When you "activate" a node, its rule is executed, and it may trigger its children.

namespace IsometricFarmGenerator
{

    public enum NodeID
    {
        ID_Default,

        ID_Management_Added_Hedgerow,

        ID_Indicator_Measure,
        ID_Supporting_Ecosystem,
        ID_Key_Attribute,
        ID_Sustainability_Score,


    };
    public class Node
    {


        public NodeID ID { get; }
        public string Name { get; }
        public List<Node> Parents { get; } = new List<Node>();
        public List<Node> Children { get; } = new List<Node>();
        public Action Rule { get; set; }
        public double Score { get; set; } // Add this
        public int ActivationCount { get; private set; } = 0;

        public Node(NodeID id, string name, Action rule, double score = 0)
        {
            ID = id;
            Name = name;
            Rule = rule;
            Score = score;
        }

        public void AddChild(Node child)
        {
            Children.Add(child);
            child.Parents.Add(this);
        }

        public void Activate(HashSet<Node> path = null)
        {
            if (path == null)
                path = new HashSet<Node>();

            if (path.Contains(this))
                return; // prevent cycles

            ActivationCount++;
            path.Add(this);

            Rule?.Invoke();

            foreach (var child in Children)
            {
                child.Activate(new HashSet<Node>(path));
            }
        }




































        //public NodeID ID { get; }
        //public string Name { get; }
        //public List<Node> Parents { get; } = new List<Node>();
        //public List<Node> Children { get; } = new List<Node>();
        //public Action Rule { get; }
        //public int ActivationCount { get; private set; } = 0;

        //public Node(NodeID id, string name, Action rule)
        //{
        //    ID = id;
        //    Name = name;
        //    Rule = rule;
        //}

        //public void AddChild(Node child)
        //{
        //    Children.Add(child);
        //    child.Parents.Add(this);
        //}

        //public void Activate(HashSet<Node> path = null)
        //{
        //    if (path == null)
        //        path = new HashSet<Node>();

        //    if (path.Contains(this))
        //        return; // prevent cycles only

        //    ActivationCount++;
        //    path.Add(this);

        //    Rule?.Invoke();

        //    foreach (var child in Children)
        //    {
        //        child.Activate(new HashSet<Node>(path)); // pass a copy to each branch
        //    }
        //}






































        //The node checks if it was already visited using the visited HashSet (prevents re-activation and infinite loops).
        //Executes its Rule.
        //Then recursively calls Activate() on its Children.
        //public void Activate(HashSet<string> visited = null)
        //{
        //    if (visited == null) visited = new HashSet<string>();
        //    if (visited.Contains(Name)) return;

        //    visited.Add(Name);
        //    //print("\NActivating: {Name}");
        //    Rule?.Invoke();

        //    foreach (var child in Children)
        //    {
        //        child.Activate(visited);
        //    }
        //}
    }
}