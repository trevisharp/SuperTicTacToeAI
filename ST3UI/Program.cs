using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

Application.SetHighDpiMode(HighDpiMode.SystemAware);
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);

var form = new Form();

Game game = new Game();

var player = new HumanPlayer();
game.XPlayer = player;

form.WindowState = FormWindowState.Maximized;
form.FormBorderStyle = FormBorderStyle.FixedToolWindow;
form.Text = "Super Tic Tac Toe Artificial Inteligence";
// Cursor.Hide();

PictureBox pb = new PictureBox();
pb.Dock = DockStyle.Fill;
form.Controls.Add(pb);

// graphical information
Bitmap bmp = null;
Graphics g = null;
int frame = 0;
float wid = 0, hei = 0, 
    majorgridwidth = 10f, 
    minorgridwidth = 5f, 
    majorsize = 0, 
    minorsize = 0, 
    gridstart = 0, 
    padding = 0, 
    minorpadding = 0;

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

    #region Start Grid Animation

    float initstate = (frame > 40 ? 40 : frame) / 40f;

    animategrid(gridstart, 0, padding, majorsize, majorgridwidth, initstate, Brushes.Black);
    
    for (int i = 0; i < 3; i++)
    {
        for (int j = 0; j < 3; j++)
        {
            animategrid(padding + gridstart + i * (majorsize + majorgridwidth / 2),
                padding + j * (majorsize + majorgridwidth / 2),
                minorpadding, minorsize, minorgridwidth,
                initstate, Brushes.Black);
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

    
    for (int coll = 0; coll < 3; coll++)
        for (int row = 0; row < 3; row++)
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    var pieceloc = gridstartlocation(coll, row, i, j);
                    var piece = game.State[coll, row, i, j];
                    if (piece == Piece.X)
                        drawX(pieceloc.X, pieceloc.Y);
                    else if (piece == Piece.O)
                        drawO(pieceloc.X, pieceloc.Y);
                }

    if (ccol != -1 && crow != -1)
    {

        var cursorstartloc = gridstartlocation(ccol, crow, ci, cj);
        if (game.CurrentPiece == Piece.X)
            drawX(cursorstartloc.X, cursorstartloc.Y, .5f);
        else if (game.CurrentPiece == Piece.O)
            drawO(cursorstartloc.X, cursorstartloc.Y, .5f);

    }


    // Cursor.Position

    pb.Image = bmp;
};

pb.MouseDown += async (sen, ev) =>
{
    if (ev.Button == MouseButtons.Left)
    {
        if (game.CurrentPlayer is HumanPlayer player)
            await player.RegisterPlay(ccol, crow, ci, cj);
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
    float animationpercent, Brush color)
{
    float totalsize = 3 * size + 2 * padding;
    animatebar(
        startx + padding + size - barwidth / 2, starty + totalsize / 2,
        barwidth, 0, 
        barwidth, 3 * size,
        animationpercent, color);
    
    animatebar(
        startx + padding + 2 * size - barwidth / 2, starty + totalsize / 2,
        barwidth, 0, 
        barwidth, 3 * size,
        animationpercent, color);
    
    animatebar(
        startx + totalsize / 2, starty + padding + size - barwidth / 2,
        0, barwidth, 
        3 * size, barwidth,
        animationpercent, color);
    
    animatebar(
        startx + totalsize / 2, starty + padding + 2 * size - barwidth / 2,
        0, barwidth, 
        3 * size, barwidth,
        animationpercent, color);   
}

void animatebar(
    float startx, float starty,
    float startwid, float starthei,
    float maxwid, float maxhei,
    float animationpercent, Brush color)
{
    float wid = (maxwid - startwid) * animationpercent + startwid;
    float hei = (maxhei - starthei) * animationpercent + starthei;
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

void drawX(float x, float y, float opacity = 1f)
{
    var opacityfactor = (int)(255 * (1f - opacity));
    var color = new SolidBrush(Color.FromArgb(255, opacityfactor, opacityfactor));
    g.FillRectangle(color, x + 2f, y + 2f, minorsize - 4f, minorsize - 4f);
}

void drawO(float x, float y, float opacity = 1f)
{
    var opacityfactor = (int)(255 * (1f - opacity));
    var color = new SolidBrush(Color.FromArgb(opacityfactor, opacityfactor, 255));
    g.FillRectangle(Brushes.Blue, x + 2f, y + 2f, minorsize - 4f, minorsize - 4f);
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

async void startgame()
{
    game.XPlayer = new HumanPlayer();
    game.OPlayer = new HumanPlayer();
    Piece winner = await game.Play();
}

Application.Run(form);