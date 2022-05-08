using Godot;
using System;

public class GameGlobal : Node
{
    private PackedScene endScenePack;
    private PackedScene gameScenePack;

    public int curP1Scores = 0;
    public int curP2Scores = 0;

    public override void _Ready()
    {
        endScenePack = ResourceLoader.Load<PackedScene>("res://EndScene.tscn");
        gameScenePack = ResourceLoader.Load<PackedScene>("res://GameMain.tscn");
    }

    public void changeToEnd(int score1, int score2)
    {
        curP1Scores = score1;
        curP2Scores = score2;
        GetParent().GetNode<GameMain>("GameMain").QueueFree();
        EndScene endScene = endScenePack.Instance<EndScene>();
        GetParent().AddChild(endScene);
    }

    public void changeToGame()
    {
        GetParent().GetNode<EndScene>("EndScene").QueueFree();
        GameMain gameScene = gameScenePack.Instance<GameMain>();
        GetParent().AddChild(gameScene);
    }
}
