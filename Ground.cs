using Godot;
using System;

public class Ground : Area2D
{
    private AudioStreamPlayer scoreSound;

    public override void _Ready()
    {
        scoreSound = GetNode<AudioStreamPlayer>("ScoreSound");
        Connect("area_entered", this, "_OnAppleEnter");
    }

    private void _OnAppleEnter(Area2D area)
    {
        if (area.IsInGroup("apple"))
        {
            Apple fallenApple = area as Apple;

            if (fallenApple.getOnGround()) return;

            fallenApple.setOnGround(true);
            GetParent<GameMain>().addScore(1, fallenApple.GlobalPosition.x < 256);
            Position = new Vector2(Position.x, 204 + GD.Randf()*10);
            scoreSound.Play();
        }
    }
}
