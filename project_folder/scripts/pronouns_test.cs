using Godot;
using System;

public partial class pronouns_test : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnPronounListItemSelected(int n) {
		Label speech_node = (Label)GetNode("speech");
		speech_node.Text = "Boss dialogue examples:\n\n";
		/*
		string[,] variants = new string[9,5] {
			{"","","","",""},
			{"","","","",""},
			{"","","","",""},
			{"","","","",""},
			{"","","","",""},
			{"","","","",""},
			{"","","","",""},
			{"","","","",""},
			{"","","","",""}
		};*/

		/*
		Pronoun forms template: they, them, their, theirs, themself

		___ escaped containment. Somembody go after ___, ___ equipment is still secure but ___ on ___ way.
		___ got ___ in a whole heap of trouble now. How stupid can ___ possibly be? The fault is entirely ___.

		Yivo pronouns: shkle, shkler, shkler, shklis, shklimself
		*/
		
		switch (n) {
			case 0: //he/him
				speech_node.Text += "He has escaped containment. Somebody go after him, his equipment is still secure but he is on his way.\n\nHe has got himself in a whole heap of trouble now. How stupid can he possibly be? The fault is entirely his."; break;

			case 1: //she/her
				speech_node.Text += "She has escaped containment. Somebody go after her, her equipment is still secure but she is on her way.\n\nShe has got herself in a whole heap of trouble now. How stupid can she possibly be? The fault is entirely hers."; break;

			case 2: //they/them
				speech_node.Text += "They have escaped containment. Somebody go after them, their equipment is still secure but they are on their way.\n\nThey have got themself in a whole heap of trouble now. How stupid can they possibly be? The fault is entirely theirs."; break;

			case 3: //it/its
				speech_node.Text += "It has escaped containment. Somembody go after it, it's equipment is still secure but it is on it's way.\n\nIt has got itself in a whole heap of trouble now. How stupid can it possibly be? The fault is entirely its."; break;

			case 4: //ve/ver
				speech_node.Text += "Ve has escaped containment. Somembody go after ver, vis equipment is still secure but ve is on ver way.\n\nVe has got verself in a whole heap of trouble now. How stupid can ve possibly be? The fault is entirely vis."; break;

			case 5: //xe/xem
				speech_node.Text += "Xe has escaped containment. Somembody go after xem, xyr equipment is still secure but xe is on xyr way.\n\nXe has got xemself in a whole heap of trouble now. How stupid can xe possibly be? The fault is entirely xyrs."; break;

			case 6: //ey/em
				speech_node.Text += "Ey have escaped containment. Somembody go after em, eir equipment is still secure but ey are on eir way.\n\nEy have got eirself in a whole heap of trouble now. How stupid can ey possibly be? The fault is entirely eirs."; break;

			case 7: //He/Him
				speech_node.Text += "He has escaped containment. Somebody go after Him, His equipment is still secure but He is on His way.\n\nHe has got Himself in a whole heap of trouble now. How stupid can He possibly be? The fault is entirely His."; break;

			case 8: //shkle/shkler
				speech_node.Text += "Shkle has escaped containment. Somembody go after shkler, shkler equipment is still secure but shkle is on shkler way.\n\nShkle got shklimself in a whole heap of trouble now. How stupid can shkle possibly be? The fault is entirely shklis."; break;

			default: speech_node.Text += "unknown case"; break;
		}
	}
}
