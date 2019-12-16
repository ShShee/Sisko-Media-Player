using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Windows.Media;
namespace Sisko
{
   
    public partial class Siskoo : Form
    {
        public Point MouseLocation;
        MediaPlayer MyPlayer = new MediaPlayer();
        System.Windows.Controls.MediaElement sound = new System.Windows.Controls.MediaElement();
        bool command = false, save = false, sound_status = true,repeat_mode_status=false,shuffle_mode_status=false,isfullscreen=false,movie_mode=false,music_mode=false,movie_playing=false;
        string[] files,Performers;
        List<int>shuffle=new List<int>();
        List<string> path=new List<string>();
        String title,album;
        Random r = new Random();
        int m = 0, s = 0, show = 0, play_now = -1;
        int component = 0;
        Point Prev_Location = MousePosition;
        Timer t = new Timer();
        Timer showtime = new Timer();
        Timer hide_playbar = new Timer();
        public Siskoo()
        {
            InitializeComponent();
            listBox1.Hide();
            this.Icon = Sisko.Properties.Resources.icons8_playlist_96;
            //Timer bai hat
            t.Tick += T_Tick;
            showtime.Tick += Showtime_Tick;
            hide_playbar.Tick += Hide_playbar_Tick;
            //
            //Timer thong tin
            t.Interval = 1000;
            showtime.Interval = 1000;
            hide_playbar.Interval = 10;
            //
        }
        private void Sisko_Load(object sender, EventArgs e)
        {
            //Them de su dung MediaElement
            elementHost1.Dock = DockStyle.Fill;
            elementHost1.Child = sound;
        }

        #region Hide PlayBar
        private void Hide_playbar(Panel pnl)
        {
            Rectangle r = pnl.RectangleToScreen(pnl.ClientRectangle);
            if (r.Contains(MousePosition))
            {
                if (!pnl.Visible)
                    pnl.Visible = true;
            }
            else
            {
                if (pnl.Visible)
                    pnl.Visible = false;
            }
        }
        private void Hide_playbar_Tick(object sender, EventArgs e)
        {
            Hide_playbar(panel4);
           if(MousePosition!=Prev_Location)
            {
                panel2.Visible = true;
                Prev_Location = MousePosition;
                hide_playbar.Interval = 3000;
            }
           else { panel2.Visible = false; hide_playbar.Interval = 10; }
        }
        #endregion

        #region Information Timer
        private void Showtime_Tick(object sender, EventArgs e)
        {
            //Thoi gian chay thong tin
            switch(show.ToString())
            {
                case "0":
                songname.Text = title + '-';
                break;
                case "4":
                    foreach (string Perform in Performers)
                        songname.Text = Perform + '\n';
                    break;
                case "8":
                    songname.Text = album + '\n';
                    break;
            }
            if (show < 11)
            {
                show++;
            }
            else { show = 0; }
            //

        }
        #endregion
        
        #region Play Timer
        private void T_Tick(object sender, EventArgs e)
        {
            //Thoi gian bai hat
            Prev_Location = MousePosition;
            trackBar2.Value = m * 60 + s;
            now.Text = Convert.ToString(m) + ":" + Convert.ToString(s);
            //Xu li cac mode 
            if (trackBar2.Value == trackBar2.Maximum)
            {
                t.Enabled = false;
                if(shuffle_mode_status==true)
                {
                        forward_btn.PerformClick();
                }
               else if(listBox1.SelectedIndex<listBox1.Items.Count-1 && repeat_mode_status==false && shuffle_mode_status==false)
                 forward_btn.PerformClick();
                else if(listBox1.SelectedIndex == listBox1.Items.Count - 1 && repeat_mode_status == false && shuffle_mode_status == false && movie_mode==false)
                {
                  DialogResult reusult =MessageBox.Show("Play again", "Resume", MessageBoxButtons.YesNo, MessageBoxIcon.Question,MessageBoxDefaultButton.Button1);
                    switch (reusult)
                    {
                        case DialogResult.Yes:
                            now.Text = Convert.ToString(0) + ":" + Convert.ToString(0);
                            trackBar2.Value = 0;
                            forward_btn.PerformClick();
                            break;
                        case DialogResult.No:
                            now.Text = Convert.ToString(0) + ":" + Convert.ToString(0);
                            trackBar2.Value = 0;
                            forward_btn.PerformClick();
                            play_btn.PerformClick();
                            break;
                        default:
                            break;
                    }
                }
              else if(repeat_mode_status==true)
                    forward_btn.PerformClick();
               else if(movie_mode==true)
                     play_btn.Enabled=false;
            }
            //
            //Gia tăng
            s++;
            if (s == 60)
            {
                m++;
                s = 0;
            }
            //
        }
        #endregion

        #region Play Media
        #region Play Music
        private void playmusicToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog opFD = new OpenFileDialog();
            opFD.Filter = "Mp3 files(*.mp3)|*.mp3";
            opFD.Multiselect = true;
            if (opFD.ShowDialog() == DialogResult.OK)
            {
                files = opFD.SafeFileNames;
                foreach (string filename in opFD.FileNames)
                {
                    path.Add(filename);
                }
                for (int i = 0; i < files.Length; i++)
                {
                    listBox1.Items.Add(files[i]);
                }
                shuffle = BuildShuffledIndexArray(listBox1.Items.Count);
                playmusicToolStripMenuItem.BackgroundImage=null;
                playVideoToolStripMenuItem.BackgroundImage=Sisko.Properties.Resources.Violeta_SomosCanarias__2_;
                backward_btn.Show();
                forward_btn.Show();
                shuffle_btn.Show();
                repeat_btn.Show();
                play_btn.Enabled=true;
                music_mode = true;
                command = false;
                save = false;
                hide_playbar.Stop();
                panel2.Visible = true;
                panel4.Visible = true;
                listBox1.Show();
                pictureBox1.Show();
                if (movie_mode == true)
                { m = 0; s = 0; movie_mode = false; play_btn.PerformClick(); }
                pictureBox1.BringToFront();
                
            }
        }
        #endregion

        #region Play Video
        private void playVideoToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog opFD = new OpenFileDialog();
            opFD.Filter = "Mp4 files(*.mp4)|*.mp4";
            if (opFD.ShowDialog() == DialogResult.OK)
            {
                elementHost1.BringToFront();
                listBox1.Items.Clear();
                listBox1.Hide();
                pictureBox1.Hide();
                sound.UnloadedBehavior = 0;
                sound.LoadedBehavior = 0;
                sound.Source = new Uri(opFD.FileName);
                playVideoToolStripMenuItem.BackgroundImage=null;
                playmusicToolStripMenuItem.BackgroundImage=Sisko.Properties.Resources.Violeta_SomosCanarias__2_;
                backward_btn.Hide();
                forward_btn.Hide();
                shuffle_btn.Hide();
                repeat_btn.Hide();
                movie_mode = true;
                play_btn.Enabled=true;
                music_mode = false;
                var tfile = TagLib.File.Create(opFD.FileName);
                var duration = (int)tfile.Properties.Duration.TotalSeconds;
                trackBar2.Maximum = duration;
                time.Text = Convert.ToString(duration / 60) + ":" + Convert.ToString(duration % 60);
                m = 0; s = 0;
                showtime.Enabled = false;
                show = 0;
                songname.Text = "----------------------------------------------------------";
                hide_playbar.Start();
                t.Enabled = true;
                movie_playing = true;
                play_btn.BackgroundImage = Sisko.Properties.Resources.icons8_gradient_line_65;
                sound.Play();
            }
        }
        #endregion
        #endregion

        #region Play Mode: Shuffle - Repeat - Stop
        #region Shuffle
        private static List<int> BuildShuffledIndexArray(int size)
        {

            List<int> array = new List<int>(size-1);
            for (int i=0; i < size; i++)
            {
                array.Add(0);
            }
            Random rand = new Random();
            for (int currentIndex = size - 1; currentIndex > 0; currentIndex--)
            {
                int nextIndex = rand.Next(currentIndex + 1);
                Swap(array, currentIndex, nextIndex);
            }
            return array;
        }

        private static void Swap(List<int> array, int firstIndex, int secondIndex)
        {

            if (array[firstIndex] == 0)
            {
                array[firstIndex] = firstIndex;
            }
            if (array[secondIndex] == 0)
            {
                array[secondIndex] = secondIndex;
            }
            int temp = array[secondIndex];
            array[secondIndex] = array[firstIndex];
            array[firstIndex] = temp;
        }
        private void shuffle_btn_Click(object sender, EventArgs e)
        {
            if (shuffle_mode_status == false)
            {
                shuffle_btn.BackgroundImage = Sisko.Properties.Resources.icons8_shuffle_45;
                shuffle_mode_status = true;
            }
            else if (shuffle_mode_status == true)
            {
                shuffle_btn.BackgroundImage = Sisko.Properties.Resources.icons8_gradient_line_45;
                shuffle_mode_status = false;
            }
        }
        #endregion
        #region Repeat
        private void repeat_btn_Click(object sender, EventArgs e)
        {
            if(repeat_mode_status==false)
            {
                repeat_btn.BackgroundImage = Sisko.Properties.Resources.icons8_cute_clipart_45;
                repeat_mode_status = true;
            }
            else if(repeat_mode_status==true)
            {
                repeat_btn.BackgroundImage = Sisko.Properties.Resources.icons8_repeat_45;
                repeat_mode_status = false;
            }
        }
        #endregion
        #region Stop
        private void stop_btn_Click(object sender, EventArgs e)
        {
            command = true;
            save = false;
            if(movie_mode==true)
            movie_playing = true;
            m = 0;
            s = 0;
            trackBar2.Value = 0;
            now.Text = Convert.ToString(m) + ":" + Convert.ToString(s);
            sound.Position = TimeSpan.FromSeconds(0);
            play_btn.PerformClick();
        }
        #endregion
        #endregion

        #region Tool Strip : Play - PLayNext - Remove
        #region PlayNext
        private void playNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >0)
                play_now = listBox1.SelectedIndex - 1;
            else if (listBox1.SelectedIndex == 0)
                play_now = listBox1.Items.Count-1;
        }
        #endregion
        #region Remove
        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int del = listBox1.SelectedIndex;
            if (del != -1)
            {
                    listBox1.Items.Remove(listBox1.SelectedItem);
                path.RemoveAt(del);
                shuffle = BuildShuffledIndexArray(listBox1.Items.Count);
                if (del <= play_now) { play_now--; }
            }
        }
        #endregion

        #region PLay
        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            command = false; show = 0; songname.Text = title;
            now.Text = "0:00";
            m = 0;
            s = 0;
            save = false;
            play_btn.PerformClick();
        }
        #endregion
        #endregion

        #region PLayTimeTrackBar
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            m = trackBar2.Value / 60;
            s = trackBar2.Value % 60;
            if(trackBar2.Value==trackBar2.Maximum-1)
            {
                t.Enabled = false;
                command = false;
                play_btn.PerformClick();
            }
                sound.Position = TimeSpan.FromSeconds(trackBar2.Value);
        }
        #endregion

        #region Close-Minimize-Move-FullScreen Sisko
        #region Minimize
        private void button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        #endregion
        #region Full Screen
        private void full_btn_Click(object sender, EventArgs e)
        {
            if (isfullscreen == false)
            {
                this.TopMost = true;
                this.WindowState = FormWindowState.Maximized;
                isfullscreen = true;
            }
            else if (isfullscreen==true)
            {
                this.TopMost = false;
                this.WindowState = FormWindowState.Normal;
                isfullscreen = false;
            }
        }
        #endregion
        #region Close
        private void close_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
        #region Move
        private void panel4_MouseDown(object sender, MouseEventArgs e)
        {
            MouseLocation = new Point(-e.X, -e.Y);
        }
        private void panel4_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePose = Control.MousePosition;
                mousePose.Offset(MouseLocation.X, MouseLocation.Y);
                Location = mousePose;
            }
        }
        #endregion
        #endregion

        #region Process: Play - Pause
        private void play_btn_Click(object sender, EventArgs e)
        {   
            //Khi chua co nhac vao
            if(listBox1.SelectedIndex==-1 && shuffle_mode_status==false &&listBox1.Items.Count!=0)
            {
                listBox1.SelectedIndex = 0;
                play_now = 0;
            }
            if (listBox1.SelectedIndex == -1 && shuffle_mode_status == true && listBox1.Items.Count != 0)
            {
                listBox1.SelectedIndex = shuffle[0];
                play_now = shuffle[0];
            }
            if (listBox1.SelectedIndex != -1 && command == false && save==false && music_mode==true)
            {

                //Chay Hien thi thong tin nhac
                showtime.Enabled = true;
                //

                //Bat dau chay nhac
                sound.Source = new Uri(path[listBox1.SelectedIndex]);
                sound.UnloadedBehavior=0;
                sound.LoadedBehavior = 0;
                sound.Play();
                t.Enabled = true;
                //

                //Lay thong tin nhac
                var tfile = TagLib.File.Create(path[listBox1.SelectedIndex]);
                //

                //Thoi gian
                var duration = (int)tfile.Properties.Duration.TotalSeconds;
                trackBar2.Maximum = duration;
                time.Text = Convert.ToString(duration / 60) + ":" + Convert.ToString(duration % 60);
                //

                //Image
                if (tfile.Tag.Pictures.Length!=0)
                {
                    MemoryStream ms = new MemoryStream(tfile.Tag.Pictures[0].Data.Data);
                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                    pictureBox1.Image = image;
                }
                //

                //Performer
                Performers = tfile.Tag.Performers;
                //

                //Title
                title = tfile.Tag.Title;
                //

                //Album
                album = tfile.Tag.Album;
                //

                //Chuyen trang thai de dung lai khi can
                command = true;
                play_btn.BackgroundImage = Sisko.Properties.Resources.icons8_gradient_line_65;
                save = true;
                play_now = listBox1.SelectedIndex;
                // 

                }
            //

           //Nhac dang dung
           else if(command==true && music_mode==true)
            {

                //Chuyen ve trang thai de chay khi can
                play_btn.BackgroundImage = Sisko.Properties.Resources.icons8_play_65;
                sound.Pause();
                t.Enabled = false;
                showtime.Enabled = false;
                songname.Text = "Paused";
                command = false;
                //

            }  
            
            //Nhac tiep tuc chay
            else if(save==true && music_mode==true)
            {   //Chuyen ve trang thai de dung khi can
                showtime.Enabled = true;
                show = 0;
                songname.Text = title;
                sound.Play();
                t.Enabled = true;
                command = true;
                play_btn.BackgroundImage = Sisko.Properties.Resources.icons8_gradient_line_65;
                //
            }
            //
            //xu li khi dang o phim
            if(movie_mode==true && movie_playing==true)
            {
                play_btn.BackgroundImage = Sisko.Properties.Resources.icons8_play_65;
                t.Enabled = false;
                sound.Pause();
                movie_playing = false;

            }
            else if(movie_mode == true && movie_playing == false)
            {
                play_btn.BackgroundImage = Sisko.Properties.Resources.icons8_gradient_line_65;
                t.Enabled = true;
                sound.Play();
                movie_playing = true;
            }
        }
        #endregion

        #region Volume
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            //Thay doi am thanh
            if (sound_status == true)
                sound.Volume = (double)trackBar1.Value/100;
            //
        }
        private void voice_Click(object sender, EventArgs e)
        {
            //Chuyen am thanh sang mute
            if (sound_status == true)
            {
                voice.BackgroundImage = Sisko.Properties.Resources.icons8_mute_45;
                sound.Volume = 0;
                sound_status = false;
            }
            //

            //Chuyen am thanh sang voice
            else if (sound_status == false)
            {
                voice.BackgroundImage = Sisko.Properties.Resources.icons8_voice_45;
                sound_status = true;
                sound.Volume = (double)trackBar1.Value/100;
            }
            //

        }
        #endregion

        #region Change: Forward - BackWard - QuickPlay
        #region Forward
        private void forward_btn_Click(object sender, EventArgs e)
        {
            //Set lai lenh de chay bai khac
            command = false; m = 0;s = 0;save = false;show=0;songname.Text = title;
            if (play_now != -1) { listBox1.SelectedIndex = play_now; }
            //

            //Xu li tien bai
            if (listBox1.SelectedIndex<listBox1.Items.Count-1 && shuffle_mode_status==false && listBox1.SelectedIndex!=-1)
            {
                listBox1.SelectedIndex++;
                play_btn.PerformClick();
            }
            else if (listBox1.SelectedIndex == listBox1.Items.Count - 1 && shuffle_mode_status==false && listBox1.SelectedIndex != -1)
            {
                listBox1.SelectedIndex = 0;
                play_btn.PerformClick();
            }
            else if (shuffle_mode_status==true && component<listBox1.Items.Count && listBox1.SelectedIndex != -1)
            {
                component++;
                if (component == listBox1.Items.Count)
                {
                    component = 0;
                }
                listBox1.SelectedIndex = shuffle[component];
                play_btn.PerformClick();      
            }
            //

        }
        #endregion
        #region QuickPlay
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {  
            //Set lai de doi bai nhanh
            command = false;show = 0;songname.Text = title;
            now.Text = "0:00";
            m = 0;
            s = 0;
            save = false;
            play_btn.PerformClick();
            //

        }
        #endregion
        #region Backward
        private void backward_btn_Click(object sender, EventArgs e)
        {
            //Set lai lenh de choi bai khac
            command = false;m = 0;s = 0;save = false;show = 0;songname.Text = title;
            if (play_now != -1) { listBox1.SelectedIndex = play_now; }
            //

            //Xu li lui bai hat
            if (listBox1.SelectedIndex >0 && shuffle_mode_status==false && listBox1.SelectedIndex != -1)
            {
                listBox1.SelectedIndex--;
                play_btn.PerformClick();
            }
            else if(listBox1.SelectedIndex==0 && shuffle_mode_status==false && listBox1.SelectedIndex != -1)
            {
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                play_btn.PerformClick();
            }
            else if(shuffle_mode_status==true && component>=0 && listBox1.SelectedIndex != -1)
            {
                component--;
                if (component == -1)
                {
                    component = listBox1.Items.Count - 1;
                }
                listBox1.SelectedIndex = shuffle[component];
                play_btn.PerformClick();       
            }
            //

        }
        #endregion
        #endregion
    }
}
