using Godot;
using System;

public partial class AudioController : Node
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void PlaySound(string audioPath)
	{
		AudioStream audio = GD.Load(audioPath) as AudioStream;

		AudioStreamPlayer player = new AudioStreamPlayer();
		AddChild(player);

		player.Stream = audio;
        player.Finished += () => DeleteOnFinish(player);
		player.Play();
	}

    private void DeleteOnFinish(AudioStreamPlayer player)
    {
		player.QueueFree();
    }
}
