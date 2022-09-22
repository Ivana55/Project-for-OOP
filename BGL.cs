using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Pmfst_GameSDK
{
    /// <summary>
    /// -
    /// </summary>
    public partial class BGL : Form
    {
        /* ------------------- */
        #region Environment Variables

        List<Func<int>> GreenFlagScripts = new List<Func<int>>();

        /// <summary>
        /// Uvjet izvršavanja igre. Ako je <c>START == true</c> igra će se izvršavati.
        /// </summary>
        /// <example><c>START</c> se često koristi za beskonačnu petlju. Primjer metode/skripte:
        /// <code>
        /// private int MojaMetoda()
        /// {
        ///     while(START)
        ///     {
        ///       //ovdje ide kod
        ///     }
        ///     return 0;
        /// }</code>
        /// </example>
        public static bool START = true;

        //sprites
        /// <summary>
        /// Broj likova.
        /// </summary>
        public static int spriteCount = 0, soundCount = 0;

        /// <summary>
        /// Lista svih likova.
        /// </summary>
        //public static List<Sprite> allSprites = new List<Sprite>();
        public static SpriteList<Sprite> allSprites = new SpriteList<Sprite>();

        //sensing
        int mouseX, mouseY;
        Sensing sensing = new Sensing();

        //background
        List<string> backgroundImages = new List<string>();
        int backgroundImageIndex = 0;
        string ISPIS = "";

        SoundPlayer[] sounds = new SoundPlayer[1000];
        TextReader[] readFiles = new StreamReader[1000];
        TextWriter[] writeFiles = new StreamWriter[1000];
        bool showSync = false;
        int loopcount;
        DateTime dt = new DateTime();
        String time;
        double lastTime, thisTime, diff;

        #endregion
        /* ------------------- */
        #region Events

        private void Draw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            try
            {                
                foreach (Sprite sprite in allSprites)
                {                    
                    if (sprite != null)
                        if (sprite.Show == true)
                        {
                           g.DrawImage(sprite.CurrentCostume, new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Height));
                        }
                    if (allSprites.Change)
                        break;
                }
                if (allSprites.Change)
                    allSprites.Change = false;
            }
            catch
            {
                //ako se doda sprite dok crta onda se mijenja allSprites
                //MessageBox.Show("Greška!");
            }
        }

        private void startTimer(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
            Init();
        }

        private void updateFrameRate(object sender, EventArgs e)
        {
            updateSyncRate();
        }

        /// <summary>
        /// Crta tekst po pozornici.
        /// </summary>
        /// <param name="sender">-</param>
        /// <param name="e">-</param>
        public void DrawTextOnScreen(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var brush = new SolidBrush(Color.WhiteSmoke);
            string text = ISPIS;

            SizeF stringSize = new SizeF();
            Font stringFont = new Font("Arial", 14);
            stringSize = e.Graphics.MeasureString(text, stringFont);

            using (Font font1 = stringFont)
            {
                RectangleF rectF1 = new RectangleF(0, 0, stringSize.Width, stringSize.Height);
                e.Graphics.FillRectangle(brush, Rectangle.Round(rectF1));
                e.Graphics.DrawString(text, font1, Brushes.Black, rectF1);
            }
        }

        private void mouseClicked(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;            
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = false;
            sensing.MouseDown = false;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;

            //sensing.MouseX = e.X;
            //sensing.MouseY = e.Y;
            //Sensing.Mouse.x = e.X;
            //Sensing.Mouse.y = e.Y;
            sensing.Mouse.X = e.X;
            sensing.Mouse.Y = e.Y;

        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            sensing.Key = e.KeyCode.ToString();
            sensing.KeyPressedTest = true;
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            sensing.Key = "";
            sensing.KeyPressedTest = false;
        }

        private void Update(object sender, EventArgs e)
        {
            if (sensing.KeyPressed(Keys.Escape))
            {
                START = false;
            }

            if (START)
            {
                this.Refresh();
            }
        }

        #endregion
        /* ------------------- */
        #region Start of Game Methods

        //my
        #region my

        //private void StartScriptAndWait(Func<int> scriptName)
        //{
        //    Task t = Task.Factory.StartNew(scriptName);
        //    t.Wait();
        //}

        //private void StartScript(Func<int> scriptName)
        //{
        //    Task t;
        //    t = Task.Factory.StartNew(scriptName);
        //}

        private int AnimateBackground(int intervalMS)
        {
            while (START)
            {
                setBackgroundPicture(backgroundImages[backgroundImageIndex]);
                Game.WaitMS(intervalMS);
                backgroundImageIndex++;
                if (backgroundImageIndex == 3)
                    backgroundImageIndex = 0;
            }
            return 0;
        }

        private void KlikNaZastavicu()
        {
            foreach (Func<int> f in GreenFlagScripts)
            {
                Task.Factory.StartNew(f);
            }
        }

        #endregion

        /// <summary>
        /// BGL
        /// </summary>
        public BGL()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pričekaj (pauza) u sekundama.
        /// </summary>
        /// <example>Pričekaj pola sekunde: <code>Wait(0.5);</code></example>
        /// <param name="sekunde">Realan broj.</param>
        public void Wait(double sekunde)
        {
            int ms = (int)(sekunde * 1000);
            Thread.Sleep(ms);
        }

        //private int SlucajanBroj(int min, int max)
        //{
        //    Random r = new Random();
        //    int br = r.Next(min, max + 1);
        //    return br;
        //}

        /// <summary>
        /// -
        /// </summary>
        public void Init()
        {
            if (dt == null) time = dt.TimeOfDay.ToString();
            loopcount++;
            //Load resources and level here
            this.Paint += new PaintEventHandler(DrawTextOnScreen);
            SetupGame();
        }

        /// <summary>
        /// -
        /// </summary>
        /// <param name="val">-</param>
        public void showSyncRate(bool val)
        {
            showSync = val;
            if (val == true) syncRate.Show();
            if (val == false) syncRate.Hide();
        }

        /// <summary>
        /// -
        /// </summary>
        public void updateSyncRate()
        {
            if (showSync == true)
            {
                thisTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                diff = thisTime - lastTime;
                lastTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                double fr = (1000 / diff) / 1000;

                int fr2 = Convert.ToInt32(fr);

                syncRate.Text = fr2.ToString();
            }

        }

        //stage
        #region Stage

        /// <summary>
        /// Postavi naslov pozornice.
        /// </summary>
        /// <param name="title">tekst koji će se ispisati na vrhu (naslovnoj traci).</param>
        public void SetStageTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Postavi boju pozadine.
        /// </summary>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        public void setBackgroundColor(int r, int g, int b)
        {
            this.BackColor = Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Postavi boju pozornice. <c>Color</c> je ugrađeni tip.
        /// </summary>
        /// <param name="color"></param>
        public void setBackgroundColor(Color color)
        {
            this.BackColor = color;
        }

        /// <summary>
        /// Postavi sliku pozornice.
        /// </summary>
        /// <param name="backgroundImage">Naziv (putanja) slike.</param>
        public void setBackgroundPicture(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        /// <summary>
        /// Izgled slike.
        /// </summary>
        /// <param name="layout">none, tile, stretch, center, zoom</param>
        public void setPictureLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        #endregion

        //sound
        #region sound methods

        /// <summary>
        /// Učitaj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        /// <param name="file">-</param>
        public void loadSound(int soundNum, string file)
        {
            soundCount++;
            sounds[soundNum] = new SoundPlayer(file);
        }

        /// <summary>
        /// Sviraj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        public void playSound(int soundNum)
        {
            sounds[soundNum].Play();
        }

        /// <summary>
        /// loopSound
        /// </summary>
        /// <param name="soundNum">-</param>
        public void loopSound(int soundNum)
        {
            sounds[soundNum].PlayLooping();
        }

        /// <summary>
        /// Zaustavi zvuk.
        /// </summary>
        /// <param name="soundNum">broj</param>
        public void stopSound(int soundNum)
        {
            sounds[soundNum].Stop();
        }

        #endregion

        //file
        #region file methods

        /// <summary>
        /// Otvori datoteku za čitanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToRead(string fileName, int fileNum)
        {
            readFiles[fileNum] = new StreamReader(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToRead(int fileNum)
        {
            readFiles[fileNum].Close();
        }

        /// <summary>
        /// Otvori datoteku za pisanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToWrite(string fileName, int fileNum)
        {
            writeFiles[fileNum] = new StreamWriter(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToWrite(int fileNum)
        {
            writeFiles[fileNum].Close();
        }

        /// <summary>
        /// Zapiši liniju u datoteku.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <param name="line">linija</param>
        public void writeLine(int fileNum, string line)
        {
            writeFiles[fileNum].WriteLine(line);
        }

        /// <summary>
        /// Pročitaj liniju iz datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća pročitanu liniju</returns>
        public string readLine(int fileNum)
        {
            return readFiles[fileNum].ReadLine();
        }

        /// <summary>
        /// Čita sadržaj datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća sadržaj</returns>
        public string readFile(int fileNum)
        {
            return readFiles[fileNum].ReadToEnd();
        }

        #endregion

        //mouse & keys
        #region mouse methods

        /// <summary>
        /// Sakrij strelicu miša.
        /// </summary>
        public void hideMouse()
        {
            Cursor.Hide();
        }

        /// <summary>
        /// Pokaži strelicu miša.
        /// </summary>
        public void showMouse()
        {
            Cursor.Show();
        }

        /// <summary>
        /// Provjerava je li miš pritisnut.
        /// </summary>
        /// <returns>true/false</returns>
        public bool isMousePressed()
        {
            //return sensing.MouseDown;
            return sensing.MouseDown;
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">naziv tipke</param>
        /// <returns></returns>
        public bool isKeyPressed(string key)
        {
            if (sensing.Key == key)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">tipka</param>
        /// <returns>true/false</returns>
        public bool isKeyPressed(Keys key)
        {
            if (sensing.Key == key.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion
        /* ------------------- */

        /* ------------ GAME CODE START ------------ */

        /* Game variables */
        int x = 500;
        int y = 85;
        int brojac = 45;
        bool rakbool;
        bool meduzabool;

        // deklaracija objekata
        Sprite ribar;
        Udica udica;
        RibaMala ribaS;
        RibaSrednja ribaM;
        RibaVelika ribaL;
        Rak rak;
        Meduza meduza;
        Sprite vrijeme;

        /* Initialization */
        
        private void SetupGame()
        {
            //1. setup stage
            SetStageTitle("PMF");
            //setBackgroundColor(Color.WhiteSmoke);       
            setBackgroundPicture("backgrounds\\sea.png");
            //none, tile, stretch, center, zoom
            setPictureLayout("stretch");
            

            Random r = new Random();

            //2. add sprites
            ribar = new Sprite("sprites\\man.png", 530, 90);
            ribar.SetSize(20);
            Game.AddSprite(ribar);

            udica = new Udica("sprites\\hook2.png",500,85);
            udica.RotationStyle = "RightLeft";
            udica.SetSize(8);
            Game.AddSprite(udica);

            ribaS = new RibaMala("sprites\\fish5.gif",GameOptions.LeftEdge-80,r.Next(200, GameOptions.DownEdge - 40));
            ribaS.SetSize(40);
            Game.AddSprite(ribaS);

            ribaM = new RibaSrednja("sprites\\fish4.gif", GameOptions.RightEdge+80, r.Next(200, GameOptions.DownEdge - 60));
            ribaM.SetSize(60);
            Game.AddSprite(ribaM);

            ribaL = new RibaVelika("sprites\\fish.gif", GameOptions.LeftEdge-80, r.Next(200, GameOptions.DownEdge - 90));
            ribaL.SetSize(90);
            Game.AddSprite(ribaL);

            rak = new Rak("sprites\\crab.png", -100,-100);
            rak.SetSize(20);
            rak.SetVisible(false);
            
            Game.AddSprite(rak);

            vrijeme = new Sprite("sprites\\time.png", -100,-100);
            vrijeme.SetVisible(false);
            vrijeme.SetSize(15);
            Game.AddSprite(vrijeme);

            meduza = new Meduza("sprites\\jellyfish.gif", -100,-100);
            meduza.SetSize(25);
            meduza.SetVisible(false);
            Game.AddSprite(meduza);


            //dodati event handlers ovdje
            _touch += Riba_kretanje;
            _touchlik += SpriteUdica;
            //napomena: prije metoda u kojima se pozivaju
            testEvent += BGL_testEvent;
            //ovaj primjer pozivamo: testEvent.Invoke(1234);
            //tada se poziva izvršavanje metode BGL_testEvent(1234)

            Form1 frm = new Form1();
            frm.ShowDialog();

            //3. scripts that start
            //Game.StartScript(Metoda);
            Game.StartScript(VelikaRibaKretanje);
            Game.StartScript(SrednjaRibaKretanje);
            Game.StartScript(MalaRibaKretanje);
            Game.StartScript(Udica);
            Game.StartScript(Vrijeme);
        }

        //možemo slati i parametre kod poziva događaja
        public delegate void testHandlerDelegat(int broj);
        public event testHandlerDelegat testEvent;

        private void BGL_testEvent(int broj)
        {
            MessageBox.Show("Poslali ste broj: " + broj);
        }

        /* Event handlers - metode*/
        public delegate void MoveHandler(MorskaCuda riba);
        public static event MoveHandler _touch;

        public delegate void MoveHandler2(Sprite lik);
        public static event MoveHandler2 _touchlik;

        /* Scripts */
        

        private int Metoda()
        {
            while (START) //ili neki drugi uvjet
            {
                Wait(0.1);
            }
            return 0;
        }

        private void Crtanje(int pocetnix,int pocetniy,int krajx,int krajy)
        {
            Pen myPen = new Pen(Color.Black);
            myPen.Width = (int)2.5;
            Graphics formGraphics = this.CreateGraphics();
            formGraphics.DrawLine(myPen,pocetnix,pocetniy,krajx,krajy);
        }

        private int Udica()
        {
            while (START)
            {
                udica.PointToMouse(sensing.Mouse);
                if (sensing.MouseDown)
                {
                    while (udica.TouchingEdge()==false)
                    {
                        udica.MoveSteps(10);
                        Wait(0.01);

                        Crtanje(530, 90, udica.X + 28, udica.Y + 28);
                        
                        string rub;
                        if (udica.TouchingEdge(out rub))
                        {
                            while (udica.TouchingSprite(ribar) == false)
                            {
                                udica.MoveSteps(-20);
                                Crtanje(530, 90, udica.X+28, udica.Y+28);
                                Wait(0.02);
                            }
                            udica.GotoXY(x, y);
                            break;
                            
                        }

                        if (udica.TouchingSprite(ribaS))
                        {
                            RibiceUdica(ribaS);
                            _touch.Invoke(ribaS);
                            break;
                        }

                        if (udica.TouchingSprite(ribaM))
                        {
                            RibiceUdica(ribaM);
                            _touch.Invoke(ribaM);
                            break;
                        }

                        if (udica.TouchingSprite(ribaL))
                        {
                            RibiceUdica(ribaL);
                            _touch.Invoke(ribaL);
                            break;
                        }

                        if (udica.TouchingSprite(rak))
                        {
                            _touchlik.Invoke(rak);
                            break;
                        }


                        if (udica.TouchingSprite(vrijeme))
                        {
                            _touchlik.Invoke(vrijeme);
                            break;
                        }

                        if (udica.TouchingSprite(meduza))
                        {
                            _touchlik.Invoke(meduza);
                            break;
                        }
                    }
                }
            }
            return 0;
        }

        private void RibiceUdica(MorskaCuda novi)
        {
            udica.Bodovi += novi.Bodovi;
            while (udica.TouchingSprite(ribar)==false)
            {
                udica.MoveSteps(-20);
                novi.Goto_Sprite(udica);
                Crtanje(530,90,udica.X+28,udica.Y+28);
                Wait(0.02);
            }
            udica.GotoXY(x,y);
            novi.SetVisible(false);
        }

        private void SpriteUdica(Sprite novi)
        {
            while (udica.TouchingSprite(ribar) == false)
            {
                udica.MoveSteps(-20);
                novi.Goto_Sprite(udica);
                Crtanje(530, 90, udica.X+28, udica.Y+28);
                Wait(0.02);
            }
            udica.GotoXY(x,y);

            if (novi.Equals(meduza))
            {
                meduzabool = false;
                udica.Bodovi += meduza.Bodovi;
            }
            if(novi.Equals(rak))
            {
                rakbool = false;
                udica.Bodovi += rak.Bodovi;
            }
            if (novi.Equals(vrijeme))
            {
                brojac += 30;
            }
            novi.SetVisible(false);
            novi.GotoXY(-100, -100);
        }

        private int MalaRibaKretanje()
        {
            ribaS.SetVisible(true);

            while (START)
            {
                ribaS.MoveSteps(ribaS.Speed);
                ribaS.SetHeading(90);
                Wait(0.2);
                string rub;
                if (ribaS.TouchingEdge(out rub))
                {
                    if (rub=="right")
                    {
                        ribaS.SetVisible(false);
                        ribaS.X -= 4;
                        _touch.Invoke(ribaS);
                        break;
                    }
                }
            }
            return 0;
        }

        private void Riba_kretanje(MorskaCuda riba)
        {
            riba.SetVisible(false);
            Random r = new Random();

            Wait(0.2);

            if (riba.Equals(ribaL))
            {
                ribaL.GotoXY(GameOptions.LeftEdge - 40, r.Next(200, GameOptions.DownEdge - 90));
                Game.StartScript(VelikaRibaKretanje);
            }

            if (riba.Equals(ribaM))
            {
                ribaM.GotoXY(GameOptions.RightEdge + 40, r.Next(200, GameOptions.DownEdge - 60));
                Game.StartScript(SrednjaRibaKretanje);
            }

            if (riba.Equals(ribaS))
            {
                ribaS.GotoXY(GameOptions.LeftEdge - 40, r.Next(200, GameOptions.DownEdge - 40));
                Game.StartScript(MalaRibaKretanje);
            }
        }

        private int SrednjaRibaKretanje()
        {
            ribaM.SetVisible(true);

            while (START)
            {
                ribaM.MoveSteps(ribaM.Speed);
                ribaM.SetHeading(270);
                Wait(0.2);
                string rub;
                if (ribaM.TouchingEdge(out rub))
                {
                    if (rub == "left")
                    {
                        ribaM.SetVisible(false);
                        ribaM.X += 4;
                        _touch.Invoke(ribaM);
                        break;
                    }
                }
            }
            return 0;
        }
        

        private int VelikaRibaKretanje()
        {
            ribaL.SetVisible(true);

            while (START)
            {
                ribaL.MoveSteps(ribaL.Speed);
                ribaL.SetHeading(90);
                Wait(0.2);
                string rub;
                if (ribaL.TouchingEdge(out rub))
                {
                    if (rub == "right")
                    {
                        ribaL.SetVisible(false);
                        ribaL.X -= 4;
                        _touch.Invoke(ribaL);
                        break;
                    }
                }
            }
            return 0;
        }
        
        private int RakKretanje()
        {
            rak.SetVisible(true);
            rak.GotoXY(0, GameOptions.DownEdge - 70);
            while (rakbool)
            {
                rak.MoveSteps(5);
                string rub;

                if (rak.TouchingEdge(out rub))
                {
                    if (rub == "right")
                    {
                        rak.SetDirection(270);
                    }
                    if (rub == "left")
                    {
                        rak.SetDirection(90);
                    }
                }
                Wait(0.01);
            }
            return 0;
        }

        private int Meduza()
        {
            Random r = new Random();
            meduza.SetVisible(true);
            while (meduzabool)
            {
                meduza.GotoXY(r.Next(0, GameOptions.RightEdge + 60), r.Next(200, GameOptions.DownEdge - 100));
                Wait(1);
            }
            Wait(0.2);
            return 0;

        }

        private int Vrijeme()
        {
            Random r = new Random();
            while (brojac>0)
            {
                brojac--;
                ISPIS="Vrijeme: "+brojac+" Bodovi: "+udica.Bodovi;
                Wait(1);

                if (brojac==0)
                {
                    START = false;
                    Kraj frm2 = new Kraj();
                    frm2.Brojac = udica.Bodovi;
                    frm2.ShowDialog();
                }

                if (brojac == 35 || brojac==15)
                {
                    rakbool = false;
                    rak.SetVisible(false);
                    rak.GotoXY(-100,-100);
                    Game.StartScript(RakKretanje);
                    rakbool = true;
                }

                if (brojac==20)
                {
                    vrijeme.SetVisible(true);
                    vrijeme.GotoXY(r.Next(0, GameOptions.RightEdge + 60), r.Next(230, GameOptions.DownEdge - 60));
                }

                if (brojac%10==0)
                {
                    meduzabool = false;
                    meduza.SetVisible(false);
                    meduza.GotoXY(-100, -100);
                    Game.StartScript(Meduza);
                    meduzabool = true;
                }
            }
            return 0;
        }

        /* ------------ GAME CODE END ------------ */
    }
}
