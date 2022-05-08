using Godot;
using System;

public class GameMain : Node2D
{
    [Export]
    public bool p1Turn = false;

    [Export]
    private NodePath rockPath;

    [Export]
    private NodePath p1ShootPosPath;

    [Export]
    private NodePath p2ShootPosPath;

    [Export]
    private NodePath girlSpritePath;

    [Export]
    private NodePath boySpritePath;

    [Export]
    private NodePath applesNodePath;

    [Export]
    private NodePath rockAmountLabelPath;

    [Export]
    private float RotateSpeed = 90;

    [Export(PropertyHint.Range, "-360, 360, 0.2")]
    private float p1MinAngNum;

    [Export(PropertyHint.Range, "-360, 360, 0.2")]
    private float p1MaxAngNum;

    [Export(PropertyHint.Range, "-360, 360, 0.2")]
    private float p2MinAngNum;

    [Export(PropertyHint.Range, "-360, 360, 0.2")]
    private float p2MaxAngNum;

    [Export]
    private int maxRockAmount = 12;
    [Export]
    private int appleAmount = 5;

    [Export]
    private float rockThrowForce = 190;

    [Export]
    private NodePath scoreLeftLabelPath;
    [Export]
    private NodePath scoreRightLabelPath;

    private Rock rock;
    private Position2D p1ShootPos;
    private Position2D p2ShootPos;
    private Sprite girl;
    private Sprite boy;
    private float curRotation = 0;
    private bool rotClockwise;
    private Label rockAmountLabel;
    private Label scoreLeftLabel;
    private Label scoreRightLabel;
    private AudioStreamPlayer throwSound;

    private const float spawnRadius = 27;

    public override void _Ready()
    {
        PackedScene apple = ResourceLoader.Load<PackedScene>("res://Apple.tscn");
        Node2D apples = GetNode<Node2D>("Apples");

        throwSound = GetNode<AudioStreamPlayer>("ThrowSound");

        RandomNumberGenerator myRand = new RandomNumberGenerator();
        myRand.Randomize();

        for (int i = 0; i < apples.GetChildCount(); i++)
        {
            Position2D curNode = apples.GetChild<Position2D>(i);

            for (int n = 0; n < appleAmount; n++)
            {
                Apple myApple = apple.Instance<Apple>();
                myApple.Position = new Vector2(
                    myRand.RandfRange(-spawnRadius, spawnRadius),
                    myRand.RandfRange(-spawnRadius, spawnRadius)
                );
                curNode.AddChild(myApple);
            }
        }

        rock = GetNode<Rock>(rockPath);

        rockAmountLabel = GetNode<Label>(rockAmountLabelPath);
        rockAmountLabel.Text = String.Format("Rocks: {0}", maxRockAmount);

        scoreLeftLabel = GetNode<Label>(scoreLeftLabelPath);
        scoreRightLabel = GetNode<Label>(scoreRightLabelPath);

        p1ShootPos = GetNode<Position2D>(p1ShootPosPath);
        p2ShootPos = GetNode<Position2D>(p2ShootPosPath);

        girl = GetNode<Sprite>(girlSpritePath);
        boy = GetNode<Sprite>(boySpritePath);

        rotClockwise = false;

        if (p1Turn)
        {
            curRotation = p1MaxAngNum;
            p1ShootPos.GetNode<Sprite>("P1Arrow").Visible = true;
            return;
        }
        p2ShootPos.GetNode<Sprite>("P2Arrow").Visible = true;
        curRotation = p2MaxAngNum;
    }

    public override void _Process(float delta)
    {
        if (maxRockAmount <= 0)
        {
            girl.Frame = 0;
            boy.Frame = 2;
            return;
        }

        if (rock.Mode == Godot.RigidBody2D.ModeEnum.Static)
        {
            curRotation += rotClockwise? RotateSpeed*delta : -RotateSpeed*delta;

            Position2D curShootPos;
            float maxAng;
            float minAng;

            if (p1Turn)
            {
                curShootPos = p1ShootPos;
                maxAng = p1MaxAngNum;
                minAng = p1MinAngNum;
                girl.Frame = 1;
            } else {
                curShootPos = p2ShootPos;
                maxAng = p2MaxAngNum;
                minAng = p2MinAngNum;
                boy.Frame = 3;
            }

            if (rotClockwise)
            {
                if (curRotation > maxAng)
                {
                    curRotation = maxAng;
                    rotClockwise = !rotClockwise;
                }
            } else {
                if (curRotation < minAng)
                {
                    curRotation = minAng;
                    rotClockwise = !rotClockwise;
                }
            }

            curShootPos.RotationDegrees = curRotation;
        } else {
            if (p1Turn) girl.Frame = 0;
            else boy.Frame = 2;
        }

        if (Input.IsActionJustPressed("ui_accept"))
        {
            throwRock();
        }
    }

    private void throwRock()
    {
        if (rock.Mode == Godot.RigidBody2D.ModeEnum.Rigid) return;

        rock.Mode = Godot.RigidBody2D.ModeEnum.Rigid;

        float xVel = Mathf.Cos(Mathf.Deg2Rad(curRotation-90));
        float yVel = Mathf.Sin(Mathf.Deg2Rad(curRotation-90));
        xVel *= rockThrowForce;
        yVel *= rockThrowForce;

        if (p1Turn)
        {
            rock.Position = p1ShootPos.Position;
            rock.LinearVelocity = new Vector2(xVel, yVel);
            removeRock(1);
            throwSound.Play();
            return;
        }
        rock.Position = p2ShootPos.Position;
        rock.LinearVelocity = new Vector2(xVel, yVel);
        throwSound.Play();
        removeRock(1);
    }

    public void changeTurn()
    {
        if (maxRockAmount <= 0)
        {
            p1ShootPos.GetNode<Sprite>("P1Arrow").Visible = false;
            p2ShootPos.GetNode<Sprite>("P2Arrow").Visible = false;
            Timer endTimer = GetNode<Timer>("EndDelayTimer");
            endTimer.Connect("timeout", this, "gameEnd");
            endTimer.Start();
            return;
        }
        p1Turn = !p1Turn;
        if (p1Turn)
        {
            curRotation = rotClockwise? p1MinAngNum : p1MaxAngNum;
            p1ShootPos.GetNode<Sprite>("P1Arrow").Visible = true;
            p2ShootPos.GetNode<Sprite>("P2Arrow").Visible = false;
            return;
        }
        curRotation = rotClockwise? p2MinAngNum : p2MaxAngNum;
        p1ShootPos.GetNode<Sprite>("P1Arrow").Visible = false;
        p2ShootPos.GetNode<Sprite>("P2Arrow").Visible = true;
    }

    private void removeRock(int a)
    {
        maxRockAmount -= a;
        rockAmountLabel.Text = String.Format("Rocks: {0}", maxRockAmount);
    }

    public void addScore(int a, bool player1)
    {
        if (player1)
        {
            scoreLeftLabel.Text = (scoreLeftLabel.Text.ToInt() + a).ToString();
            return;
        }
        scoreRightLabel.Text = (scoreRightLabel.Text.ToInt() + a).ToString();
    }

    private void gameEnd()
    {
        GetParent().GetNode<GameGlobal>("GameGlobal").changeToEnd(scoreLeftLabel.Text.ToInt(), scoreRightLabel.Text.ToInt());
    }
}
