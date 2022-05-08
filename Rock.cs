using Godot;
using System;

public class Rock : RigidBody2D
{
    private bool hasHit = false;
    private AudioStreamPlayer hitSound;

    public override void _Ready()
    {
        Area2D rockArea = GetNode<Area2D>("RockArea");
        rockArea.Connect("area_entered", this, "_OnAppleEnter");
        hitSound = GetNode<AudioStreamPlayer>("HitAppleSound");
    }

    public override void _PhysicsProcess(float delta)
    {
        if (Position.y >= 171)
        {
            hitTheGround();
        }
    }
    
    private void _OnAppleEnter(Area2D area)
    {
        if (hasHit) return;
        
        if (area.IsInGroup("apple"))
        {
            Apple apple = area as Apple;
            apple.getHit();
            hasHit = true;
            hitSound.Play();
        }
    }

    private void hitTheGround()
    {
        if (this.Mode == Godot.RigidBody2D.ModeEnum.Static) return;
        GameMain gameMain = GetParent() as GameMain;
        gameMain.changeTurn();
        this.Mode = Godot.RigidBody2D.ModeEnum.Static;
        hasHit = false;
    }
}
