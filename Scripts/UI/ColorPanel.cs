using Godot;
using System;

public partial class ColorPanel : PanelContainer
{
	[Export]
	private Timer _animTimer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
		Scale = Vector2.Zero;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_animTimer != null && _animTimer.TimeLeft > 0)
		{
			double t = 1 - (_animTimer.TimeLeft / _animTimer.WaitTime);
			float scale = (float)Mathf.Lerp(0, 1, Mathf.SmoothStep(0, 1, t));
			Scale = new Vector2 (scale, scale);
		}
	}

	public void Activate()
	{
		Visible = true;
		_animTimer.Start();
	}
}
