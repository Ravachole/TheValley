using Godot;
using System;

public partial class BehaviorTreeUI : Control
{
    private GraphEdit _graphEdit;

    public override void _Ready()
    {
        _graphEdit = GetNode<GraphEdit>("GraphEdit");
        if (_graphEdit == null)
        {
            GD.PrintErr("GraphEdit not found. Ensure it's present in the scene tree.");
            return;
        }
        
        CreateBehaviorTree();
    }

	private void CreateBehaviorTree()
	{
		GD.Print("Creating behavior tree...");

		// Create nodes
		GraphNode hungerSeq = CreateGraphNode("Hunger Sequence");
		GraphNode thirstSeq = CreateGraphNode("Thirst Sequence");
		GD.Print("Created nodes: " + hungerSeq.Name + ", " + thirstSeq.Name);

		// Add nodes to the GraphEdit
		_graphEdit.AddChild(hungerSeq);
		_graphEdit.AddChild(thirstSeq);
		GD.Print("Added nodes to GraphEdit.");

		// Set positions
		hungerSeq.SetPosition(new Vector2(100, 100));
		thirstSeq.SetPosition(new Vector2(600, 100));
		GD.Print("Set positions for nodes");

		// Connect nodes
		ConnectBehaviorNodes(hungerSeq, thirstSeq);
		GD.Print("Connected nodes.");
	}



    // private void CreateBehaviorTree()
    // {
    //     // Create nodes
    //     GraphNode hungerSeq = CreateGraphNode("Hunger Sequence");
    //     GraphNode thirstSeq = CreateGraphNode("Thirst Sequence");
    //     GraphNode tiredSeq = CreateGraphNode("Tired Sequence");
    //     GraphNode idleSeq = CreateGraphNode("Idle Sequence");
    //     GraphNode wanderSeq = CreateGraphNode("Wander Sequence");

    //     // Add nodes to the GraphEdit
    //     _graphEdit.AddChild(hungerSeq);
    //     _graphEdit.AddChild(thirstSeq);
    //     _graphEdit.AddChild(tiredSeq);
    //     _graphEdit.AddChild(idleSeq);
    //     _graphEdit.AddChild(wanderSeq);

    //     // Set positions
    //     hungerSeq.SetPosition(new Vector2(100, 100));
    //     thirstSeq.SetPosition(new Vector2(300, 100));
    //     tiredSeq.SetPosition(new Vector2(500, 100));
    //     idleSeq.SetPosition(new Vector2(700, 100));
    //     wanderSeq.SetPosition(new Vector2(900, 100));

    //     // Connect nodes
    //     ConnectBehaviorNodes(hungerSeq, thirstSeq);
    //     ConnectBehaviorNodes(thirstSeq, tiredSeq);
    //     ConnectBehaviorNodes(tiredSeq, idleSeq);
    //     ConnectBehaviorNodes(idleSeq, wanderSeq);
    // }

	private GraphNode CreateGraphNode(string name)
	{
		GraphNode node = new GraphNode();
		node.Name = name;
		node.Title = name;

		// Create an input slot (on the left, index 0) and an output slot (on the right, index 1)
		node.SetSlot(0, true, 1, new Color(0, 1, 0), true, 1, new Color(1, 0, 0)); // Input slot
		node.SetSlot(1, false, 1, new Color(0, 1, 0), false, 1, new Color(1, 0, 0)); // Output slot

		return node;
	}

	private void ConnectBehaviorNodes(GraphNode nodeFrom, GraphNode nodeTo)
	{
		GD.Print($"Connecting {nodeFrom.Name} to {nodeTo.Name}");

		// Assuming you want to connect output port 1 of nodeFrom to input port 0 of nodeTo
		var outputPortIndex = 1;
		var inputPortIndex = 0;

		// Check if ports exist by trying to get their positions
		if (nodeFrom.GetOutputPortCount() != 0 && nodeTo.GetInputPortCount() != 0)
		{
			_graphEdit.ConnectNode(nodeFrom.Name, outputPortIndex, nodeTo.Name, inputPortIndex);
		}
		else
		{
			GD.PrintErr("One of the nodes does not have the required ports.");
		}
	}

}
