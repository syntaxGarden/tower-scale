using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class multiple_keys : Node2D
{
	private List<char> input_list = new List<char>();
	private List<double> time = new List<double>();
	private readonly char[] ALPHANUM = new char[36] {
		'a','b','c','d','e','f','g','h','i','j','k','l','m',
		'n','o','p','q','r','s','t','u','v','w','x','y','z',
		'0','1','2','3','4','5','6','7','8','9'
		}; //readonly is heap allocated (I think) version of const

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	private double current_mean, lowest = 1, highest = 0;
	public override void _Process(double delta)
	{
		double start = Time.GetUnixTimeFromSystem();
		foreach (char a in ALPHANUM) { if (Input.IsActionPressed(a.ToString())) { input_list.Add(a); } }
		double end = Time.GetUnixTimeFromSystem();
		
		time.Add(end - start);
		Label name = (Label)GetNode("name"); 
		name.Text = "C# output. " + time.Count();
		current_mean = time.Average();
		if (time.Count > 1000) {
			time.RemoveAt(0);
			if (lowest > current_mean) {
				Label node = (Label)GetNode("lowest");
				node.Text = "Lowest mean time: " + current_mean;
				lowest = current_mean;
			} else if (highest < current_mean) {
				Label node = (Label)GetNode("highest");
				node.Text = "Highest mean time: " + current_mean;
				highest = current_mean;
			}
		}

		Label output_node = (Label)GetNode("output");
		output_node.Text = String.Join(" , ", input_list);
		input_list.Clear();
	}
}