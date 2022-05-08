using Godot;
using System;

public class EndScene : Node2D
{
    [Export]
    private NodePath scoresLabelPath;
    [Export]
    private NodePath spPath;

    private Label scoreLabel;
    private Sprite sp;

    private Texture noWinImage = ResourceLoader.Load("res://assets/images/no_win.png") as Texture;
    private Texture p1WinImage = ResourceLoader.Load("res://assets/images/p1_win.png") as Texture;
    private Texture p2WinImage = ResourceLoader.Load("res://assets/images/p2_win.png") as Texture;
    private Texture tieWin = ResourceLoader.Load("res://assets/images/tie_win.png") as Texture;

    public override void _Ready()
    {
        scoreLabel = GetNode<Label>(scoresLabelPath);
        sp = GetNode<Sprite>(spPath);

        GameGlobal globalRefer = GetParent().GetNode<GameGlobal>("GameGlobal");
        setScores(globalRefer.curP1Scores, globalRefer.curP2Scores);
    }

    public override void _Process(float delta)
    {
       if (Input.IsActionJustPressed("ui_accept")) changeToGame();
    }

    public void setScores(int score1, int score2)
    {
        scoreLabel.Text = String.Format("Left Player Scores: {0} \nRight Player Scores: {1}", score1, score2);

        if (score1 <= 0 && score2 <= 0)
        {
            sp.Texture = noWinImage;
            return;
        }

        if (score1 > score2)
        {
            sp.Texture = p1WinImage;
            return;
        }

        if (score1 < score2)
        {
            sp.Texture = p2WinImage;
            return;
        }

        sp.Texture = tieWin;
    }

    private void changeToGame()
    {
        GetParent().GetNode<GameGlobal>("GameGlobal").changeToGame();
    }
}
