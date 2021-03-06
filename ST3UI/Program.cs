using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

Application.SetHighDpiMode(HighDpiMode.SystemAware);
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);

var form = new Form();
Game game = new Game();
App app = new App();

form.WindowState = FormWindowState.Maximized;
form.FormBorderStyle = FormBorderStyle.FixedToolWindow;
form.Text = "Super Tic Tac Toe Artificial Inteligence";
Cursor.Hide();

PictureBox pb = new PictureBox();
pb.Dock = DockStyle.Fill;
form.Controls.Add(pb);

// graphical information
Bitmap bmp = null;
Graphics g = null;
int frame = 0;
int animationendframe = -1,
    animationlenth = 0;;
Action<int> animationexecution = null;
float wid = 0, hei = 0, 
    majorgridwidth = 10f, 
    minorgridwidth = 5f, 
    majorsize = 0, 
    minorsize = 0, 
    gridstart = 0, 
    padding = 0, 
    minorpadding = 0;
List<Action> staticobjects = new List<Action>();

// test game varibles
bool[] hadwinner = new bool[9];

// cursor position
int ccol = 0;
int crow = 0;
int ci = 0;
int cj = 0;

Timer tm = new Timer();
tm.Interval = 25;

form.Load += delegate
{
    bmp = new Bitmap(pb.Width, pb.Height);
    g = Graphics.FromImage(bmp);
    wid = bmp.Width;
    hei = bmp.Height;
    majorsize = .3f * hei - 2 * majorgridwidth;
    minorsize = .3f * majorsize - 2 * minorgridwidth;
    gridstart = (wid - hei) / 2f;
    padding = .05f * hei;
    minorpadding = .05f * majorsize;
    tm.Start();
};

tm.Tick += delegate
{
    frame++;
    g.Clear(Color.White);

    testallwinners();

    #region Start Grid Animation

    float initstate = (frame > 40 ? 40 : frame) / 40f;

    animategrid(gridstart, 0, padding, majorsize, majorgridwidth, initstate, Brushes.Black);
    
    for (int i = 0; i < 3; i++)
    {
        for (int j = 0; j < 3; j++)
        {
            Brush brush = Brushes.Black;
            if (!game.IsActive(i, j))
                brush = new SolidBrush(Color.FromArgb(128, 128, 128));
            animategrid(padding + gridstart + i * (majorsize + majorgridwidth / 2),
                padding + j * (majorsize + majorgridwidth / 2),
                minorpadding, minorsize, minorgridwidth,
                initstate, brush);
        }
    }

    if (frame < 40) //initialization fase
    {
        pb.Image = bmp;
        return;
    }
    else if (frame == 40) //start game
    {
        startgame();
    }
    
    #endregion

    #region Pieces Draw

    for (int coll = 0; coll < 3; coll++)
        for (int row = 0; row < 3; row++)
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    var pieceloc = gridstartlocation(coll, row, i, j);
                    var piece = game.State[coll, row, i, j];
                    float opacity = 1f;
                    if (!game.IsActive(coll, row))
                        opacity = .5f;
                    if (piece == Piece.X)
                        drawX(pieceloc.X, pieceloc.Y, minorsize - 4f - minorgridwidth, opacity);
                    else if (piece == Piece.O)
                        drawO(pieceloc.X, pieceloc.Y, minorsize - 4f - minorgridwidth, opacity);
                }

    if (ccol != -1 && crow != -1)
    {
        var piece = game.State[ccol, crow, ci, cj];
        if (piece == Piece.None)
        {
            var cursorstartloc = gridstartlocation(ccol, crow, ci, cj);
            if (game.IsActive(ccol, crow))
            {
                if (game.CurrentPiece == Piece.X)
                    drawX(cursorstartloc.X, cursorstartloc.Y, minorsize - 4f - minorgridwidth, .5f);
                else if (game.CurrentPiece == Piece.O)
                    drawO(cursorstartloc.X, cursorstartloc.Y, minorsize - 4f - minorgridwidth, .5f);
            }
        }
    }

    #endregion

    #region Animations and Static Objects

    foreach (var stobj in staticobjects)
        stobj();

    if (animationexecution != null)
        animationexecution(frame - animationendframe + animationlenth);

    #endregion

    #region Cursor Draw

    float cursoranimationload = (frame % 40) / 40f;
    float cursoropacity = 1f;
    if (!game.IsActive(ccol, crow))
    {
        cursoranimationload = 1f;
        cursoropacity = .5f;
    }
    if (game.CurrentPiece == Piece.X)
        drawX(Cursor.Position.X - pb.PointToScreen(pb.Location).X, 
            Cursor.Position.Y - pb.PointToScreen(pb.Location).Y, 20f, 
            cursoropacity, cursoranimationload);
    else if (game.CurrentPiece == Piece.O)
        drawO(Cursor.Position.X - pb.PointToScreen(pb.Location).X, 
            Cursor.Position.Y - pb.PointToScreen(pb.Location).Y, 20f, 
            cursoropacity, cursoranimationload);
    
    #endregion

    pb.Image = bmp;
};

pb.MouseDown += async (sen, ev) =>
{
    if (ev.Button == MouseButtons.Left)
    {
        if (!game.IsActive(ccol, crow))
            return;
        var piece = game.State[ccol, crow, ci, cj];
        if (piece != Piece.None)
            return;
        if (game.CurrentPlayer is HumanPlayer player)
        {
            var cursorstartloc = gridstartlocation(ccol, crow, ci, cj);
            var playedpiece = game.CurrentPiece;
            // draw piece animation
            setanimation(f =>
            {
                // hide symbol on instantaneos drawing
                g.FillRectangle(Brushes.White, cursorstartloc.X, cursorstartloc.Y, 
                    minorsize - 4f - minorgridwidth,
                    minorsize - 4f - minorgridwidth);
                if (playedpiece == Piece.X)
                    drawX(cursorstartloc.X, cursorstartloc.Y, minorsize - 4f - minorgridwidth, 1f, f / 20f);
                else 
                    drawO(cursorstartloc.X, cursorstartloc.Y, minorsize - 4f - minorgridwidth, 1f, f / 20f);
            }, 20);
            await player.RegisterPlay(ccol, crow, ci, cj);
        }
    }
};

pb.MouseMove += (sen, ev) =>
{
    calculatecursorgridposition(
        ev.Location.X - gridstart - padding,
        ev.Location.Y - padding);
};

void animategrid(
    float startx, float starty, 
    float padding, float size, float barwidth, 
    float animationload, Brush color)
{
    float totalsize = 3 * size + 2 * padding;
    animatebar(
        startx + padding + size - barwidth / 2, starty + totalsize / 2,
        barwidth, 0, 
        barwidth, 3 * size,
        animationload, color);
    
    animatebar(
        startx + padding + 2 * size - barwidth / 2, starty + totalsize / 2,
        barwidth, 0, 
        barwidth, 3 * size,
        animationload, color);
    
    animatebar(
        startx + totalsize / 2, starty + padding + size - barwidth / 2,
        0, barwidth, 
        3 * size, barwidth,
        animationload, color);
    
    animatebar(
        startx + totalsize / 2, starty + padding + 2 * size - barwidth / 2,
        0, barwidth, 
        3 * size, barwidth,
        animationload, color);   
}

void animatebar(
    float startx, float starty,
    float startwid, float starthei,
    float maxwid, float maxhei,
    float animationload, Brush color)
{
    float wid = (maxwid - startwid) * animationload + startwid;
    float hei = (maxhei - starthei) * animationload + starthei;
    g.FillRectangle(color, startx - wid / 2, starty - hei / 2, wid, hei);
}

void calculatecursorgridposition(float x, float y)
{
    int tempccol = -1, tempci = -1;
    while (x > 0)
    {
        tempccol++;
        x -= majorsize - majorgridwidth / 2;
        if (x <= 0)
        {
            x += majorsize - majorgridwidth / 2;
            while (x > 0)
            {
                tempci++;
                x -= minorsize - minorgridwidth / 2;
            }
        }
    }
    ccol = tempccol;
    // correcting calculation considering padding
    ci = tempci == 3 ? tempci - 1 : tempci;

    int tempcrow = -1, tempcj = -1;
    while (y > 0)
    {
        tempcrow++;
        y -= majorsize - majorgridwidth / 2;
        if (y <= 0)
        {
            y += majorsize - majorgridwidth / 2;
            while (y > 0)
            {
                tempcj++;
                y -= minorsize - minorgridwidth / 2;
            }
        }
    }
    crow = tempcrow;
    // correcting calculation considering padding
    cj = tempcj == 3 ? tempcj - 1 : tempcj;

    if (ccol > 2)
    {
        ccol = -1;
        ci = -1;
    }
    if (crow > 2)
    {
        crow = -1;
        cj = -1;
    }
}

void drawX(float x, float y, float size, float opacity = 1f, float animationload = 1f)
{
    var opacityfactor = (int)(255 * (1f - opacity));
    var color = Color.FromArgb(255, opacityfactor, opacityfactor);
    var pen = new Pen(color, 5f);
    g.DrawLine(pen, x, y, 
        (x + size) * animationload + (1f - animationload) * (x),
        (y + size) * animationload + (1f - animationload) * (y));
    g.DrawLine(pen, x + size, y, 
        (x + size) * (1f - animationload) + animationload * (x),
        (y + size) * animationload + (1f - animationload) * (y));
}

void drawO(float x, float y, float size, float opacity = 1f, float animationload = 1f)
{
    var opacityfactor = (int)(255 * (1f - opacity));
    var color = new SolidBrush(Color.FromArgb(opacityfactor, opacityfactor, 255));
    
    g.FillPie(color, x, y, size, size, 0f, 360f * animationload);
    g.FillEllipse(Brushes.White, x + 5f, y + 5f, size - 10f, size - 10f);
}

PointF gridstartlocation(int coll, int row, int i, int j)
    => new PointF(
        gridstart + padding + minorpadding + 
        coll * (majorsize + majorgridwidth / 2) 
        + i * (minorsize + minorgridwidth / 2),
        padding + minorpadding + 
        row * (majorsize + majorgridwidth / 2) 
        + j * (minorsize + minorgridwidth / 2)
    );

void setanimation(Action<int> drawlogic, int len)
{
    animationlenth = len;
    animationendframe = frame + len;
    animationexecution = (f) =>
    {
        drawlogic(f);
        if (f == len)
        {
            animationendframe = -1;
            animationlenth = -1;
            animationexecution = null;
        }
    };
}

bool testwinner(int col, int row)
{
    var winner = game.VerifyMinorWinner(col, row);
    if (winner != Piece.None)
    {
        var majorlocation = gridstartlocation(col, row, 0, 0);
        staticobjects.Add(() =>
        {
            // hide objects
            g.FillRectangle(Brushes.White, 
                majorlocation.X - minorpadding, 
                majorlocation.Y - minorpadding, 
                majorsize- 2 * minorpadding, 
                majorsize- 2 * minorpadding);
            if (winner == Piece.X)
                drawX(majorlocation.X - minorpadding,
                    majorlocation.Y - minorpadding,
                    majorsize - 2 * minorpadding, 1f, 1f);
            else 
                drawO(majorlocation.X - minorpadding,
                    majorlocation.Y - minorpadding,
                    majorsize - 2 * minorpadding, 1f, 1f);
        });
        setanimation(f =>
        {
            g.FillRectangle(Brushes.White, 
                majorlocation.X - minorpadding, 
                majorlocation.Y - minorpadding, 
                majorsize- 2 * minorpadding, 
                majorsize- 2 * minorpadding);
            if (winner == Piece.X)
            drawX(majorlocation.X - minorpadding,
                majorlocation.Y - minorpadding,
                majorsize - 2 * minorpadding, 1f, f / 20f);
            else 
            drawO(majorlocation.X - minorpadding,
                majorlocation.Y - minorpadding,
                majorsize - 2 * minorpadding, 1f, f / 20f);
        }, 20);
        return true;
    }
    return false;
}

void testallwinners()
{
    for (int col = 0; col < 3; col++)
    {
        for (int row = 0; row < 3; row++)
        {
            int index = 3 * row + col;
            if (!hadwinner[index])
                hadwinner[index] = testwinner(row, col);
        }
    }
}

async void startgame()
{
    game.XPlayer = app.PlayerX;
    game.OPlayer = app.PlayerO;
    Piece winner = await game.Play();

    tm.Stop();
    g.Clear(Color.White);

    if (winner == Piece.X)
    {
        drawX(gridstart, 0, hei, 1f, 1f);
    }
    else if (winner == Piece.O)
    {
        drawO(gridstart, 0, hei, 1f, 1f);
    }

    pb.Image = bmp;
    Cursor.Show();
}

Application.Run(form);