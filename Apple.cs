using Godot;
using System;

public class Apple : Area2D
{
    [Export]
    private bool isHit = false;
    [Export]
    private bool isOnGround = false;
    private float acc = 20;

    public override void _Process(float delta)
    {
        if (isOnGround)
        {
            return;
        }

        if (isHit)
        {
            Position += new Vector2(0, acc) * delta;
            acc += 50*delta;
        }
    }

    public void getHit()
    {
        isHit = true;
    }

    public void setOnGround(bool a)
    {
        isOnGround = a;
    }

    public bool getOnGround()
    {
        return isOnGround;
    }
}
