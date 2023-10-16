using System.Runtime.InteropServices;
using CefSharp;
using CefSharp.WinForms;
using System.Diagnostics;

namespace LWallPaper;
public partial class LPaper : Form
{
    private NotifyIcon trayIcon;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam, SendMessageTimeoutFlags fuFlags, uint uTimeout, out IntPtr lpdwResult);

    [DllImport("user32.dll")]
    public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, IntPtr windowTitle);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    public enum SendMessageTimeoutFlags : uint
    {
        SMTO_NORMAL = 0x0,
        SMTO_BLOCK = 0x1,
        SMTO_ABORTIFHUNG = 0x2,
        SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
        SMTO_ERRORONEXIT = 0x20
    }

    private static IntPtr workerw;

    public LPaper()
    {
        InitializeComponent();
        setSystemTray();
        setForm(createBrowser());

        Load += new EventHandler(Form1_Load);
    }

    private void setForm(ChromiumWebBrowser browser)
    {
        FormBorderStyle = FormBorderStyle.None;
        WindowState = FormWindowState.Maximized;
        TopMost = false;
        Controls.Add(browser);
    }

    private ChromiumWebBrowser createBrowser()
    {
        CefSettings settings = new CefSettings();
        Cef.Initialize(settings);

        string exePath = AppDomain.CurrentDomain.BaseDirectory;
        string url = Path.Combine(exePath, "index.html");
        var browser = new ChromiumWebBrowser(exePath);

        browser.LoadUrl(url);

        return browser;
    }

    private void setSystemTray()
    {
        trayIcon = new NotifyIcon();

        // Set the icon for the tray icon
        trayIcon.Icon = new Icon("Resources/icon.ico");

        // Set the text for the tray icon
        trayIcon.Text = "LWallPaper";

        // Add a context menu strip to the tray icon
        ContextMenuStrip trayMenu = new ContextMenuStrip();
        trayMenu.Items.Add("Exit", null, OnExit);
        trayIcon.ContextMenuStrip = trayMenu;

        // Show the tray icon
        trayIcon.Visible = true;
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        IntPtr progman = FindWindow("Progman", null);
        IntPtr result = IntPtr.Zero;
        workerw = IntPtr.Zero;

        SendMessageTimeout(progman, 0x052C, new IntPtr(0), IntPtr.Zero, SendMessageTimeoutFlags.SMTO_NORMAL, 1000, out result);

        EnumWindows(new EnumWindowsProc((tophandle, topparamhandle) =>
        {
            IntPtr shellDefViewHandle = FindWindowEx(tophandle, IntPtr.Zero, "SHELLDLL_DefView", IntPtr.Zero);

            if (shellDefViewHandle != IntPtr.Zero)
            {
                // Gets the WorkerW Window after the current one.
                workerw = FindWindowEx(IntPtr.Zero, tophandle, "WorkerW", IntPtr.Zero);
            }

            return true;
        }), IntPtr.Zero);

        SetParent(this.Handle, workerw);

        // Get all screens
        Screen[] screens = Screen.AllScreens;

        // Choose the primary screen
        Screen primaryScreen = Screen.PrimaryScreen;

        // Get the working area (considering taskbar space) of the primary screen
        Rectangle workingArea = primaryScreen.WorkingArea;

        // We adjust for this by offsetting our window position with the smallest left bound among all screens.
        int offset = screens.Min(screen => screen.Bounds.Left);

        // Set the position and size of the window to fit the primary screen, considering offset
        SetWindowPos(this.Handle, IntPtr.Zero, workingArea.Left + Math.Abs(offset), workingArea.Top, workingArea.Width, workingArea.Height, 0);
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        Cef.Shutdown();
    }

    private void OnExit(object sender, EventArgs e)
    {
        //  shut down the browser
        Cef.Shutdown();
        // Get a handle for the Progman window
        IntPtr progman = FindWindow("Progman", null);

        // Send a message to Progman to refresh the desktop
        SendMessageTimeout(progman, 0x052C, new IntPtr(0), IntPtr.Zero, SendMessageTimeoutFlags.SMTO_NORMAL, 1000, out _);

        // really bad solution, but is done to avoid weirdness in in the desktop background
        Process[] explorer = Process.GetProcessesByName("explorer");

        foreach (Process process in explorer)
            process.Kill();

        Application.Exit();
    }
}

