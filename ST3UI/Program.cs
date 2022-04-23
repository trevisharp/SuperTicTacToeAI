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

Bitmap bmp = null;
Graphics g = null;
int frame = 0;

Timer tm = new Timer();
tm.Interval = 25;

form.Load += async delegate
{
    bmp = new Bitmap(pb.Width, pb.Height);
    g = Graphics.FromImage(bmp);
    tm.Start();
    await game.Play();
};

tm.Tick += delegate
{
    frame++;
    g.Clear(Color.White);

    float wid = bmp.Width,
          hei = bmp.Height;
    
    float majorgridwidth = 10f;
    float minorgridwidth = 5f;
    float majorsize = .3f * hei - 2 * majorgridwidth;
    float minorsize = .3f * majorsize - 2 * minorgridwidth;
    float gridstart = (wid - hei) / 2f;
    float padding = .05f * hei;
    float minorpadding = .05f * majorsize;

    #region Start Grid Animation

    float initstate = (frame > 40 ? 40 : frame) / 40f;

    animategrid(gridstart, 0, padding, majorsize, majorgridwidth, initstate, Brushes.Black);
    
    for (int i = 0; i < 3; i++)
    {
        for (int j = 0; j < 3; j++)
        {
            animategrid(gridstart + padding + i * majorsize,
                padding + j * majorsize,
                minorpadding, minorsize, minorgridwidth,
                initstate, Brushes.Black);
        }
    }

    if (frame < 40) //initialization fase
    {
        pb.Image = bmp;
        return;
    }
    
    #endregion

    g.DrawString(game.State[0, 0, 0, 0].ToString(), form.Font, Brushes.Black, PointF.Empty);

    // Cursor.Position

    pb.Image = bmp;
};

pb.MouseDown += async (sen, ev) =>
{
    await player.RegisterPlay(0, 0, 0, 0);
};

pb.MouseMove += (sen, ev) =>
{

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

Application.Run(form);