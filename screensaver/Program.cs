using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Svg.Skia;
using SkiaSharp;

internal static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        string arg = args.FirstOrDefault()?.ToLowerInvariant() ?? "/s";
        if (arg.StartsWith("/c")) { MessageBox.Show("DVD Corner Screensaver\\nThe logo bounces around and changes color on every wall hit.", "DVD Screensaver"); return; }
        if (arg.StartsWith("/p")) return;
        ApplicationConfiguration.Initialize();
        Application.Run(new SaverForm());
    }
}

sealed class SaverForm : Form
{
    readonly Bitmap logo;
    readonly System.Windows.Forms.Timer timer = new() { Interval = 16 };
    readonly Random rng = new();
    float x = 90, y = 90, vx = 4.2f, vy = 3.1f;
    Color tint = Color.White;
    bool firstMove = true;
    Point initialMouse;

    public SaverForm()
    {
        FormBorderStyle = FormBorderStyle.None;
        WindowState = FormWindowState.Maximized;
        TopMost = true;
        BackColor = Color.Black;
        DoubleBuffered = true;
        Cursor.Hide();
        initialMouse = Cursor.Position;
        using var stream = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "dvd.svg"));
        var svg = new SKSvg();
        svg.Load(stream);
        using SKBitmap sk = svg.Picture!.ToBitmap(SKColors.Transparent, 1f, 1f, SKColorType.Rgba8888, SKAlphaType.Premul, null);
        using var ms = new MemoryStream();
        using var skImage = SKImage.FromBitmap(sk);
        skImage.Encode(SKEncodedImageFormat.Png, 100).SaveTo(ms);
        ms.Position = 0;
        logo = new Bitmap(ms);
        timer.Tick += (_, _) => Step();
        timer.Start();
        KeyDown += (_, _) => Close();
        MouseDown += (_, _) => Close();
        MouseMove += (_, _) => { if (firstMove) { firstMove = false; initialMouse = Cursor.Position; } else if (Math.Abs(Cursor.Position.X - initialMouse.X) + Math.Abs(Cursor.Position.Y - initialMouse.Y) > 8) Close(); };
    }

    void Step()
    {
        float w = Math.Min(ClientSize.Width * .42f, 440);
        float h = w * logo.Height / logo.Width;
        x += vx; y += vy;
        bool hit = false;
        if (x <= 0 || x + w >= ClientSize.Width) { vx = -vx; x = Math.Clamp(x, 0, Math.Max(0, ClientSize.Width - w)); hit = true; }
        if (y <= 0 || y + h >= ClientSize.Height) { vy = -vy; y = Math.Clamp(y, 0, Math.Max(0, ClientSize.Height - h)); hit = true; }
        if (hit) tint = Color.FromArgb(rng.Next(70,256), rng.Next(70,256), rng.Next(70,256));
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        float w = Math.Min(ClientSize.Width * .42f, 440);
        float h = w * logo.Height / logo.Width;
        using var attributes = new ImageAttributes();
        var matrix = new ColorMatrix(new float[][]
        {
            new float[] { tint.R/255f,0,0,0,0 }, new float[] { 0,tint.G/255f,0,0,0 },
            new float[] { 0,0,tint.B/255f,0,0 }, new float[] { 0,0,0,1,0 }, new float[] { 0,0,0,0,1 }
        });
        attributes.SetColorMatrix(matrix);
        e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        e.Graphics.DrawImage(logo, new Rectangle((int)x,(int)y,(int)w,(int)h), 0,0,logo.Width,logo.Height,GraphicsUnit.Pixel,attributes);
    }
}
